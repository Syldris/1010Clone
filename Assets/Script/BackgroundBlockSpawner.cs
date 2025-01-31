using UnityEngine;

public class BackgroundBlockSpawner : MonoBehaviour
{
    [SerializeField] private GameObject blockPrefab; // 배경으로 배치되는 블록 프리팹
    [SerializeField] private int orderInLayer; // 배치되는 블록들이 그려지는 순서

    // 격자형태로 생성된는 블록의 개수, 블록 하나의 절반 크기
    //private Vector2Int blockCount = new Vector2Int(10, 10);
    //private Vector2 blockHalf = new Vector2(0.5f, 0.5f);

    //private void Awake()
    public BackgroundBlock[] SpawnBlocks(Vector2Int blockCount, Vector2 blockHalf)
    {
        BackgroundBlock[] blocks = new BackgroundBlock[blockCount.x * blockCount.y];

        for(int y = 0; y<blockCount.y; ++y)
        {
            for(int x = 0; x<blockCount.x; ++x)
            {
                // 블록 판의 중앙이 (0, 0, 0)이 되도록 배치
                float px = -blockCount.x * 0.5f + blockHalf.x + x;
                float py = blockCount.y * 0.5f - blockHalf.y - y;

                Vector3 position = new Vector3(px, py, 0);
                // 블록 생성 (원본 프리팹, 위치, 회전, 부모 Transform)
                GameObject clone = Instantiate(blockPrefab, position, Quaternion.identity, transform);
                // 방금 생성하나 블록의 그려지는 순서 설정
                clone.GetComponent<SpriteRenderer>().sortingOrder = orderInLayer;
                // 생성한 모든 블록의 정보를 반환하기 위해 block[] 배열에 저장
                blocks[y * blockCount.x + x] = clone.GetComponent<BackgroundBlock>();
            }
        }
        return blocks;
    }
}
