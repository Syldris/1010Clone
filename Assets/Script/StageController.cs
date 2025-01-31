using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageController : MonoBehaviour
{
    [SerializeField]
    private BackgroundBlockSpawner backgroundBlockSpawner; //배경 블록 생성
    [SerializeField]
    private BackgroundBlockSpawner foregroundBlockSpawner; // 배경 블록 생성
    [SerializeField]
    private DragBlockSpawner dragBlockSpwner; // 드래그 블록 생성
    [SerializeField]
    private BlockArrangeSystem blockArrangeSystem; // 블록 배치
    [SerializeField]
    private UIController uiController; //게임오버 되었을때 UI 활성화

    public int CurrentScore { private set; get; } // 현재 점수
    public int HighScore { private set; get; } //최고 점수

    private BackgroundBlock[] backgroundBlocks; // 생성한 배경 블록 정보 저장
    private int currentDragBlockCount; // 현재 남아있는 드래그 블록 개수

    private readonly Vector2Int blockCount = new Vector2Int(10, 10); // 블록 판에 배치되는 블록 개수
    private readonly Vector2 blockHalf = new Vector2(0.5f, 0.5f); // 블록 하나의 절반 크기
    private readonly int maxDragBlockount = 3; // 한 번에 생성할 수 있는 드래그 블록 개수

    private List<BackgroundBlock> filledBlockList;

    private void Awake()
    {
        // 점수 초기화(현재 점수, 최고 점수)
        CurrentScore = 0;
        HighScore = PlayerPrefs.GetInt("HighScore");

        // 줄이 완성된 블록들을 삭제하기 위해 임시 저장하는 리스트
        filledBlockList = new List<BackgroundBlock>(); 

        //뒷 배경으로 사용되는 배경 블록판 생성
        backgroundBlockSpawner.SpawnBlocks(blockCount, blockHalf);

        // 드래그 블록을 배치할 때 색상이 변경되는 배경 블록판 생성
        backgroundBlocks = new BackgroundBlock[blockCount.x * blockCount.y];
        backgroundBlocks = foregroundBlockSpawner.SpawnBlocks(blockCount, blockHalf);

        // 블록 배치 시스템
        blockArrangeSystem.Setup(blockCount, blockHalf, backgroundBlocks, this);

        // 드래그 블록 생성
        //SpawnDragBlocks();
        StartCoroutine(SpawnDragBlocks());
    }

    //private void SpawnDragBlocks()
    private IEnumerator SpawnDragBlocks()
    {
        // 현재 드래그 블록의 개수를 최대(3)로 설정
        currentDragBlockCount = maxDragBlockount;
        //드래그 블록 생성
        dragBlockSpwner.SpawnBlocks();

        // 드래그 블록들의 이동이 완료될 때까지 대기
        yield return new WaitUntil(() => IsCompleteSpawnBlocks());
    }

    /// <summary>
    /// 드래그 블록을 생성하고, 등장 애니메이션을 재생할 때
    /// 모든 드래그 블록의 등장 애니메이션이 종료되었는지 검사
    /// </summary>
    private bool IsCompleteSpawnBlocks()
    {
        int count = 0;
        for(int i = 0; i < dragBlockSpwner.BlockSpawnPoints.Length; ++i)
        { 
            if( dragBlockSpwner.BlockSpawnPoints[i].childCount != 0 &&
                dragBlockSpwner.BlockSpawnPoints[i].GetChild(0).localPosition == Vector3.zero)
            {
                count++;
            }
        }

        return count == dragBlockSpwner.BlockSpawnPoints.Length;
    }


    /// <summary>
    /// 블록 배치 후처리
    /// 드래그 블록 삭제/생성, 줄 완성, 게임오버, 점수
    /// </summary>
    public void AfterBlockArraangement(DragBlock block)
    {
        // 블록 배치 완료 후 행동 처리 (블록 삭제, 점수 계산, 드래그 블록 생성 )
        StartCoroutine(OnAfterBlockArrangement(block));
    }

    /// <summary>
    /// 블록 배치 후처리
    /// 드래그 블록 삭제, 재생성 여부 확인, 줄 완성, 게임오버, 점수
    /// </summary>
    public IEnumerator OnAfterBlockArrangement(DragBlock block)
    {
        // 배치가 완료된 드래그 블록 삭제
        Destroy(block.gameObject);

        // 완성된 줄이 있는지 검사하고, 완성된 줄의 블록들은 별도로 저장
        int filedLineCount = CheckFilledLine();

        // 완성된 줄이 없으면 0점, 완성된 줄이 있으면 2의 filledLineCount승 * 10점(10, 20, 40, 80..)
        int lineScore = filedLineCount == 0 ? 0 : (int)Mathf.Pow(2, filedLineCount - 1) * 10;
        // 점수 계산 (블록 점수 + 라인 점수)
        CurrentScore += block.ChildBlocks.Length + lineScore;

        // 줄이 완성된 블록들을 삭제 (마지막에 배치한 블록을 기준으로 퍼져나가듯이 삭제)
        yield return StartCoroutine(DestroyFilledBlocks(block));

        // 블록 배치에 성공했으니 현재 남아있는 드래그 블록의 개수를 1 감소
        currentDragBlockCount--;
        //현재 배치 가능한 드래그 블록의 개수가 0이면 드래그 블록 생성
        if(currentDragBlockCount == 0)
        {
            //SpawnDragBlocks();
            yield return StartCoroutine(SpawnDragBlocks());
        }

        // 현재 프레임이 종료될 때까지 대기
        yield return new WaitForEndOfFrame();

        // 게임 진행이 가능한지 검사
        if(IsGameOver())
        {
            //Debug.Log("게임오버");

            // 현재 점수가 최고 점수보다 높으면 현재 점수를 최고 점수로 갱신
            if(CurrentScore > HighScore)
            {
                PlayerPrefs.SetInt("HighScore", CurrentScore);
            }

            // 게임오버 되었을 때 출력하는 Panel U를 활성화하고, 스크린샷, 점수 등 갱신
            uiController.GameOver();
        }

        
    }

    /// <summary>
    /// 완성된 줄이 있는지 검사하고, 완성된 줄의 블록들을
    /// </summary>
    private int CheckFilledLine()
    {
        int filledLineCount = 0;

        filledBlockList.Clear();

        // 가로 줄 검사
        for(int y = 0; y <blockCount.y; ++y)
        {
            int fillBlockCount = 0;
            for(int x = 0; x<blockCount.x; ++x)
            {
                //해당 블록이 채워져 있으면 fillBlockCount 1 증가
                if (backgroundBlocks[y * blockCount.x + x].BlockState == BlockState.Fill) fillBlockCount++;
            }

            // 완성된 줄이 있으면 해당 줄의 모든 배경 블록을filledBlockList에 저장
            if(fillBlockCount == blockCount.x)
            {
                for(int x=0; x<blockCount.x; ++x)
                {
                    filledBlockList.Add(backgroundBlocks[y * blockCount.x + x]);
                }
                filledLineCount++;
            }
        }
        // 세로 줄 검사
        for (int x = 0; x < blockCount.x; ++x)
        {
            int fillBlockCount = 0;
            for (int y = 0; y < blockCount.y; ++y)
            {
                //해당 블록이 채워져 있으면 fillBlockCount 1 증가
                if (backgroundBlocks[y * blockCount.x + x].BlockState == BlockState.Fill) fillBlockCount++;
            }

            // 완성된 줄이 있으면 해당 줄의 모든 배경 블록을 filledBlockList에 저장
            if(fillBlockCount == blockCount.y)
            {
                for(int y = 0; y<blockCount.y; ++y)
                {
                    filledBlockList.Add(backgroundBlocks[y * blockCount.x + x]);
                }
                fillBlockCount++;
            }
        }

        return filledLineCount;
    }

    /// <summary>
    /// 줄을 완성한 블록 기준으로 퍼져나가듯이 블록 삭제
    /// </summary>
    /// <returns></returns>
    private IEnumerator DestroyFilledBlocks(DragBlock block)
    {
        // 마지막에 배치한 블록(block)과 거리가 가까운 순서로 정렬
        filledBlockList.Sort((a, b) => (a.transform.position - block.transform.position).sqrMagnitude.
        CompareTo((b.transform.position - block.transform.position).sqrMagnitude));
       
        // filledBlockList에 저장되어 있는 배경 블록을 순서대로 초기화
        for(int i=0;i<filledBlockList.Count; i++)
        {
            filledBlockList[i].EmptyBlock();

            yield return new WaitForSeconds(0.01f);
        }

        filledBlockList.Clear();
    }

    /// <summary>
    /// 게임오버 검사
    /// 남아있는 드래그 블록이 있는지? 있으면 해당 블록을 배치할 수 있는 공간이 있는지 검사
    /// </summary>
    private bool IsGameOver()
    {
        int dragBlockCount = 0;

        // 배치 가능한 드래그 블록이 남아있을 때
        for(int i = 0; i<dragBlockSpwner.BlockSpawnPoints.Length; ++i)
        {
            //dragBlockSpwaner.BlockSpawnPoints[i]에 자식이 있으면 (자식 = 드래그 블록)
            if(dragBlockSpwner.BlockSpawnPoints[i].childCount != 0)
            {
                dragBlockCount++;

                // 블록을 배치할 때와 마찬가지로 드래그 블록의 정보를 바탕으로 자식 블록들을 배치할 수 있는 공간이 있는지 검사
                //IsPossibleArrangement()가 true를 반환하면 블록을 배치할 수 있다는 뜻으로 IsGameOver()는 false 반환해 게임 진행
                if(blockArrangeSystem.IsPossibleArrangement(dragBlockSpwner.BlockSpawnPoints[i].GetComponentInChildren<DragBlock>()))
                {
                    return false;
                }
            }
        }

        // dragBlockCount는 현재 남아있는 드래그 블록의 개수인데 맵에 배치할 수 있는 드래그 블록이 있으면
        // if( IsPossibleArrangement() ) 에서 true 로 메소드를 빠져나가기 때문에
        // 이 반환 코드가 실행되고 dragBlockCount가 0이 아니면 게임오버
        
        return dragBlockCount != 0;
    }
}
