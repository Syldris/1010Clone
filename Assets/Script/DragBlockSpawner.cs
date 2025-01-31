using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragBlockSpawner : MonoBehaviour
{
    [SerializeField] private BlockArrangeSystem blockArrangeSystem; //DragBlock 클래스에서 사용
    [SerializeField] private Transform[] blockSpawnPoints; // 드래그 가능한 블록이 배치되는 위치
    [SerializeField] private GameObject[] blockPrefabs; // 생성가능한 모든 블록 프리팹
    [SerializeField] private Vector3 spawnGapAmount = new Vector3(10, 0, 0); // 처음 생성할 때 부모와 떨어진 거리

    public Transform[] BlockSpawnPoints => blockSpawnPoints;

    //private void Awake()

    public void SpawnBlocks()
    {
        StartCoroutine("OnSpawnBlocks");
    }

    IEnumerator OnSpawnBlocks()
    {
        // 드래그 블록 3개(blocksParent.Length) 생성
        for(int i = 0; i< blockSpawnPoints.Length; ++i)
        {
            yield return new WaitForSeconds(0.1f);

            // 생성할 드래그 블록 순번
            int index = Random.Range(0, blockPrefabs.Length);
            // 드래그 블록이 생성되는 위치
            Vector3 spawnPosition = blockSpawnPoints[i].position + spawnGapAmount;
            //드래그 블록 생성 ( 원본 프리팹, 생성 위치, 초기 회전값, 부모 Transform)
            GameObject clone = Instantiate(blockPrefabs[index], spawnPosition, Quaternion.identity, blockSpawnPoints[i]);

            // 드래그 블록을 생성하고, 부모의 위치까지 이동하는 애니메이션 재생
            clone.GetComponent<DragBlock>().Setup(blockArrangeSystem, blockSpawnPoints[i].position);
        }
    }
}
