using UnityEngine;

public class BackgroundBlockSpawner : MonoBehaviour
{
    [SerializeField] private GameObject blockPrefab; // ������� ��ġ�Ǵ� ��� ������
    [SerializeField] private int orderInLayer; // ��ġ�Ǵ� ��ϵ��� �׷����� ����

    // �������·� �����ȴ� ����� ����, ��� �ϳ��� ���� ũ��
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
                // ��� ���� �߾��� (0, 0, 0)�� �ǵ��� ��ġ
                float px = -blockCount.x * 0.5f + blockHalf.x + x;
                float py = blockCount.y * 0.5f - blockHalf.y - y;

                Vector3 position = new Vector3(px, py, 0);
                // ��� ���� (���� ������, ��ġ, ȸ��, �θ� Transform)
                GameObject clone = Instantiate(blockPrefab, position, Quaternion.identity, transform);
                // ��� �����ϳ� ����� �׷����� ���� ����
                clone.GetComponent<SpriteRenderer>().sortingOrder = orderInLayer;
                // ������ ��� ����� ������ ��ȯ�ϱ� ���� block[] �迭�� ����
                blocks[y * blockCount.x + x] = clone.GetComponent<BackgroundBlock>();
            }
        }
        return blocks;
    }
}
