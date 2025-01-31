using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockArrangeSystem : MonoBehaviour
{
    private Vector2Int blockCount;
    private Vector2 blockHalf;
    private BackgroundBlock[] backgroundBlocks;
    private StageController stageController;

    public void Setup(Vector2Int blockCount, Vector2 blockHalf, BackgroundBlock[] backgroundBlocks, StageController stageController)
    {
        this.blockCount = blockCount;
        this.blockHalf = blockHalf;
        this.backgroundBlocks = backgroundBlocks;
        this.stageController = stageController;
    }
    /// <summary>
    /// 매개변수로 받아온 block을 배치할 수 있는지 검사하고,
    /// 배치가 가능하면 블록 배치, 줄 완성 검사, 점수 계산 처리
    /// </summary>


    public bool TryArrangementBlock(DragBlock block)
    {
        // 블록 배치가 가능한지 검사
        for (int i = 0; i < block.ChildBlocks.Length; ++i)
        {
            // 자식 블록의 월드 위치 (부모의 월드 좌표 + 자식의 지역 좌표)
            Vector3 position = block.transform.position + block.ChildBlocks[i];

            // 블록이 맵 내부에 위치하고 있는지?
            if (!IsBlockInsideMap(position)) return false;
            // 현재 위치에 이미 다른 블록이 배치되어 있는지?
            if (!IsOtherBlockInThisBlock(position)) return false;
        }

        // 블록 배치
        for (int i = 0; i < block.ChildBlocks.Length; ++i)
        {
            // 자식 블록의 월드 위치 (부모의 월드 좌표 + 자식의 지역 좌표)
            Vector3 position = block.transform.position + block.ChildBlocks[i];

            // 해당 위치에 있는 배경 블록의 색상을 변경하고, 채움(BloockState.Fill)으로 변경
            backgroundBlocks[PositionToIndex(position)].FillBlock(block.Color);
        }

        // 블록 배치 후처리
        StartCoroutine(stageController.OnAfterBlockArrangement(block));

        return true;
    }
    /// <summary>
    /// 매개변수로 받아온 위치(position)가 배경 블록판의 바깥인지 검사
    /// 블록판의 바깥이면 false, 안쪽이면 true 반환
    /// </summary>
    private bool IsBlockInsideMap(Vector2 position)
    {
        // x가 -4.5보다 작거나, 4.5보다 크거나, y가 -4.5보다 작거나, 4.5보다 크면 false 반환하고 그렇지 않으면 true 반
        if (position.x < -blockCount.x * 0.5f + blockHalf.x || position.x > blockCount.x * 0.5f - blockHalf.x ||
            position.y < -blockCount.x * 0.5f + blockHalf.y || position.y > blockCount.y * 0.5f - blockHalf.y)
        {
            return false;
        }
        return true;
    }
    /// <summary>
    /// 매개변수로 받아온 위치(poition) 정보를 바탕으로 맵에 배치된 블록의 순번(index)을 계산해서 반환
    /// </summary>
    private int PositionToIndex(Vector2 position)
    {
        float x = blockCount.x * 0.5f - blockHalf.x + position.x;
        float y = blockCount.y * 0.5f - blockHalf.y - position.y;

        return (int)(y * blockCount.x + x);
    }
    ///<summary>
    /// 현재 위치(position)에 있는 블록이 비어있는지 검사 후 결과 반환
    /// 블록이 비어있으면 true, 비어있지 않으면 false
    /// </summary>
    private bool IsOtherBlockInThisBlock(Vector2 position)
    {
        int index = PositionToIndex(position);

        if(backgroundBlocks[index].BlockState == BlockState.Fill)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 드래그 블록에 포함된 자식 블록들이(childBlocks) 현재 블록 판에 배치 가능한지 검사
    /// </summary>
    public bool IsPossibleArrangement(DragBlock block)
    {
        for(int y = 0; y< blockCount.y; ++y)
        {
            for(int x = 0; x<blockCount.x; ++x)
            {
                int count = 0;
                Vector3 position = new Vector3(-blockCount.x * 0.5f + blockHalf.x + x,
                                                 blockCount.y * 0.5f - blockHalf.y - y, 0);

                // 블록의 개수가 홀수이면 좌표를 그대로 사용하고, 짝수이면 (가로:0.5, 세로:-0.5)를  더해서 사용
                position.x = block.blockCount.x % 2 == 0 ? position.x + 0.5f : position.x;
                position.y = block.blockCount.y % 2 == 0 ? position.y - 0.5f : position.y;

                // 현재 블록(block)에 소속된 모든 자식 블록의 위치를 기준으로 맵 내부인지, 다른 블록이 없는지 검사
                for(int i = 0; i<block.ChildBlocks.Length; ++i)
                {
                    Vector3 blockPosition = block.ChildBlocks[i] + position;

                    // TryArrangementBlock() 메소드에서 했던 것과 동일하게
                    // 블록이 맵 내부에 위치하고 있는지, 현재 위치에 이미 다른 블록이 배치되어 있는지 검사
                    if (!IsBlockInsideMap(blockPosition)) break;
                    if (!IsOtherBlockInThisBlock(blockPosition)) break;

                    count++;
                }

                // 위의 for문에서 break로 중단되는 경우 count와 자식 블록 개수가 같지 않기 때문에 배치 불가
                // 개수가 같은 경우 배치 가능 (return true)
                if(count == block.ChildBlocks.Length)
                {
                    return true;
                }
            }
        }
        return false;
    }
    
}
