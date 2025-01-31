using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageController : MonoBehaviour
{
    [SerializeField]
    private BackgroundBlockSpawner backgroundBlockSpawner; //��� ��� ����
    [SerializeField]
    private BackgroundBlockSpawner foregroundBlockSpawner; // ��� ��� ����
    [SerializeField]
    private DragBlockSpawner dragBlockSpwner; // �巡�� ��� ����
    [SerializeField]
    private BlockArrangeSystem blockArrangeSystem; // ��� ��ġ
    [SerializeField]
    private UIController uiController; //���ӿ��� �Ǿ����� UI Ȱ��ȭ

    public int CurrentScore { private set; get; } // ���� ����
    public int HighScore { private set; get; } //�ְ� ����

    private BackgroundBlock[] backgroundBlocks; // ������ ��� ��� ���� ����
    private int currentDragBlockCount; // ���� �����ִ� �巡�� ��� ����

    private readonly Vector2Int blockCount = new Vector2Int(10, 10); // ��� �ǿ� ��ġ�Ǵ� ��� ����
    private readonly Vector2 blockHalf = new Vector2(0.5f, 0.5f); // ��� �ϳ��� ���� ũ��
    private readonly int maxDragBlockount = 3; // �� ���� ������ �� �ִ� �巡�� ��� ����

    private List<BackgroundBlock> filledBlockList;

    private void Awake()
    {
        // ���� �ʱ�ȭ(���� ����, �ְ� ����)
        CurrentScore = 0;
        HighScore = PlayerPrefs.GetInt("HighScore");

        // ���� �ϼ��� ��ϵ��� �����ϱ� ���� �ӽ� �����ϴ� ����Ʈ
        filledBlockList = new List<BackgroundBlock>(); 

        //�� ������� ���Ǵ� ��� ����� ����
        backgroundBlockSpawner.SpawnBlocks(blockCount, blockHalf);

        // �巡�� ����� ��ġ�� �� ������ ����Ǵ� ��� ����� ����
        backgroundBlocks = new BackgroundBlock[blockCount.x * blockCount.y];
        backgroundBlocks = foregroundBlockSpawner.SpawnBlocks(blockCount, blockHalf);

        // ��� ��ġ �ý���
        blockArrangeSystem.Setup(blockCount, blockHalf, backgroundBlocks, this);

        // �巡�� ��� ����
        //SpawnDragBlocks();
        StartCoroutine(SpawnDragBlocks());
    }

    //private void SpawnDragBlocks()
    private IEnumerator SpawnDragBlocks()
    {
        // ���� �巡�� ����� ������ �ִ�(3)�� ����
        currentDragBlockCount = maxDragBlockount;
        //�巡�� ��� ����
        dragBlockSpwner.SpawnBlocks();

        // �巡�� ��ϵ��� �̵��� �Ϸ�� ������ ���
        yield return new WaitUntil(() => IsCompleteSpawnBlocks());
    }

    /// <summary>
    /// �巡�� ����� �����ϰ�, ���� �ִϸ��̼��� ����� ��
    /// ��� �巡�� ����� ���� �ִϸ��̼��� ����Ǿ����� �˻�
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
    /// ��� ��ġ ��ó��
    /// �巡�� ��� ����/����, �� �ϼ�, ���ӿ���, ����
    /// </summary>
    public void AfterBlockArraangement(DragBlock block)
    {
        // ��� ��ġ �Ϸ� �� �ൿ ó�� (��� ����, ���� ���, �巡�� ��� ���� )
        StartCoroutine(OnAfterBlockArrangement(block));
    }

    /// <summary>
    /// ��� ��ġ ��ó��
    /// �巡�� ��� ����, ����� ���� Ȯ��, �� �ϼ�, ���ӿ���, ����
    /// </summary>
    public IEnumerator OnAfterBlockArrangement(DragBlock block)
    {
        // ��ġ�� �Ϸ�� �巡�� ��� ����
        Destroy(block.gameObject);

        // �ϼ��� ���� �ִ��� �˻��ϰ�, �ϼ��� ���� ��ϵ��� ������ ����
        int filedLineCount = CheckFilledLine();

        // �ϼ��� ���� ������ 0��, �ϼ��� ���� ������ 2�� filledLineCount�� * 10��(10, 20, 40, 80..)
        int lineScore = filedLineCount == 0 ? 0 : (int)Mathf.Pow(2, filedLineCount - 1) * 10;
        // ���� ��� (��� ���� + ���� ����)
        CurrentScore += block.ChildBlocks.Length + lineScore;

        // ���� �ϼ��� ��ϵ��� ���� (�������� ��ġ�� ����� �������� ������������ ����)
        yield return StartCoroutine(DestroyFilledBlocks(block));

        // ��� ��ġ�� ���������� ���� �����ִ� �巡�� ����� ������ 1 ����
        currentDragBlockCount--;
        //���� ��ġ ������ �巡�� ����� ������ 0�̸� �巡�� ��� ����
        if(currentDragBlockCount == 0)
        {
            //SpawnDragBlocks();
            yield return StartCoroutine(SpawnDragBlocks());
        }

        // ���� �������� ����� ������ ���
        yield return new WaitForEndOfFrame();

        // ���� ������ �������� �˻�
        if(IsGameOver())
        {
            //Debug.Log("���ӿ���");

            // ���� ������ �ְ� �������� ������ ���� ������ �ְ� ������ ����
            if(CurrentScore > HighScore)
            {
                PlayerPrefs.SetInt("HighScore", CurrentScore);
            }

            // ���ӿ��� �Ǿ��� �� ����ϴ� Panel U�� Ȱ��ȭ�ϰ�, ��ũ����, ���� �� ����
            uiController.GameOver();
        }

        
    }

    /// <summary>
    /// �ϼ��� ���� �ִ��� �˻��ϰ�, �ϼ��� ���� ��ϵ���
    /// </summary>
    private int CheckFilledLine()
    {
        int filledLineCount = 0;

        filledBlockList.Clear();

        // ���� �� �˻�
        for(int y = 0; y <blockCount.y; ++y)
        {
            int fillBlockCount = 0;
            for(int x = 0; x<blockCount.x; ++x)
            {
                //�ش� ����� ä���� ������ fillBlockCount 1 ����
                if (backgroundBlocks[y * blockCount.x + x].BlockState == BlockState.Fill) fillBlockCount++;
            }

            // �ϼ��� ���� ������ �ش� ���� ��� ��� �����filledBlockList�� ����
            if(fillBlockCount == blockCount.x)
            {
                for(int x=0; x<blockCount.x; ++x)
                {
                    filledBlockList.Add(backgroundBlocks[y * blockCount.x + x]);
                }
                filledLineCount++;
            }
        }
        // ���� �� �˻�
        for (int x = 0; x < blockCount.x; ++x)
        {
            int fillBlockCount = 0;
            for (int y = 0; y < blockCount.y; ++y)
            {
                //�ش� ����� ä���� ������ fillBlockCount 1 ����
                if (backgroundBlocks[y * blockCount.x + x].BlockState == BlockState.Fill) fillBlockCount++;
            }

            // �ϼ��� ���� ������ �ش� ���� ��� ��� ����� filledBlockList�� ����
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
    /// ���� �ϼ��� ��� �������� ������������ ��� ����
    /// </summary>
    /// <returns></returns>
    private IEnumerator DestroyFilledBlocks(DragBlock block)
    {
        // �������� ��ġ�� ���(block)�� �Ÿ��� ����� ������ ����
        filledBlockList.Sort((a, b) => (a.transform.position - block.transform.position).sqrMagnitude.
        CompareTo((b.transform.position - block.transform.position).sqrMagnitude));
       
        // filledBlockList�� ����Ǿ� �ִ� ��� ����� ������� �ʱ�ȭ
        for(int i=0;i<filledBlockList.Count; i++)
        {
            filledBlockList[i].EmptyBlock();

            yield return new WaitForSeconds(0.01f);
        }

        filledBlockList.Clear();
    }

    /// <summary>
    /// ���ӿ��� �˻�
    /// �����ִ� �巡�� ����� �ִ���? ������ �ش� ����� ��ġ�� �� �ִ� ������ �ִ��� �˻�
    /// </summary>
    private bool IsGameOver()
    {
        int dragBlockCount = 0;

        // ��ġ ������ �巡�� ����� �������� ��
        for(int i = 0; i<dragBlockSpwner.BlockSpawnPoints.Length; ++i)
        {
            //dragBlockSpwaner.BlockSpawnPoints[i]�� �ڽ��� ������ (�ڽ� = �巡�� ���)
            if(dragBlockSpwner.BlockSpawnPoints[i].childCount != 0)
            {
                dragBlockCount++;

                // ����� ��ġ�� ���� ���������� �巡�� ����� ������ �������� �ڽ� ��ϵ��� ��ġ�� �� �ִ� ������ �ִ��� �˻�
                //IsPossibleArrangement()�� true�� ��ȯ�ϸ� ����� ��ġ�� �� �ִٴ� ������ IsGameOver()�� false ��ȯ�� ���� ����
                if(blockArrangeSystem.IsPossibleArrangement(dragBlockSpwner.BlockSpawnPoints[i].GetComponentInChildren<DragBlock>()))
                {
                    return false;
                }
            }
        }

        // dragBlockCount�� ���� �����ִ� �巡�� ����� �����ε� �ʿ� ��ġ�� �� �ִ� �巡�� ����� ������
        // if( IsPossibleArrangement() ) ���� true �� �޼ҵ带 ���������� ������
        // �� ��ȯ �ڵ尡 ����ǰ� dragBlockCount�� 0�� �ƴϸ� ���ӿ���
        
        return dragBlockCount != 0;
    }
}
