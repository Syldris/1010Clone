using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragBlock : MonoBehaviour
{
    [SerializeField]
    private AnimationCurve curveMovement; // 이동 제어 그래프
    [SerializeField]
    private AnimationCurve curveScale; // 크기 제어 그래프

    private BlockArrangeSystem blockArrangeSystem;

    private float appearTime = 0.5f; // 블록이 등장할 떄 소요되는 시간
    private float returnTime = 0.1f; //블록이 원 위치로 돌아갈 때 소요되는 시간

    [field:SerializeField]
    public Vector2Int blockCount { private set; get; } //자식 블록 개수

    public Color Color { private set; get; } // 블록 색상 
    public Vector3[] ChildBlocks { private set; get; } //자식 블록들의 지역 좌표

    public void Setup(BlockArrangeSystem blockArrangeSystem, Vector3 parentPosition)
    {
        this.blockArrangeSystem = blockArrangeSystem;

        // 자식 블록은 모두 같은 색상이기 때문에 자식 블록 중 누구의 색상을 가져와도 상관없다.
        Color = GetComponentInChildren<SpriteRenderer>().color;

        // 블록의 모양에 따라 자식 개수가 다르기 때문에 자식 개수 만큼 배열 방을 생성하고,
        // 모든 자식 오브젝트의 지역 좌표를 저장
        ChildBlocks = new Vector3[transform.childCount];
        for (int i = 0; i < ChildBlocks.Length; ++i)
        {
            ChildBlocks[i] = transform.GetChild(i).localPosition;
        }


        // 우측 하면 바깥에 생성된 블록이 부모 오브젝트 위치(parent.position)까지 이동
        StartCoroutine(OnMoveTo(parentPosition, appearTime));
    }

    /// <summary>
    /// 해당 오브젝트를 클릭할 때 1회 호출
    /// </summary>
    private void OnMouseDown()
    {
        // 드래그 가능한 블록을 처음 0.5의 크기로 생성되는데
        // 배경 블록의 크기는 1이기 때문에 배경 블록의 크기와 동일한 1로 확대
        StopCoroutine("OnScaleTo");
        StartCoroutine("OnScaleTo", Vector3.one);
    }
    /// <summary>
    /// 해당 오브젝트를 드래그할 때 매 프레임 호출 
    /// </summary>
    private void OnMouseDrag()
    {
        // 현재 모든 블록은 Pivot이 블록셋의 정중앙으로 설정되어 있기 때문에 x 위치는 그대로 사용하,

        // y 위치는 y축 블록 개수의 절반(BlockCount.y * 0.5f)에 gap만큼 추가한 위치로 사용

        // Camera.main.ScreenToWorldPoint()로 Vector3 좌표를 구하면 z값은 카메라의 위치인 -10이 나오기 때문에
        // gap에서 z 값을 +10 해줘야 현재 오브젝트들이 배치되어 있는 z=0으로 설정된다.
        Vector3 gap = new Vector3(0, blockCount.y * 0.5f + 1, 10);
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + gap;
    }

    /// <summary>
    /// 해당 오브젝트의 클릭을 종료할 때 1회 호출
    /// </summary>
    private void OnMouseUp()
    {
        // 자식 블록 개수가 홀수, 짝수 일 때 다르게 계산
        // 값을 반올림하는 Mathf.RoundToInt()를 이용해 블록을 배경 블록판에 스탭(Snap)해서 배치
        float x = Mathf.RoundToInt(transform.position.x - blockCount.x % 2 * 0.5f) + blockCount.x % 2 * 0.5f;
        float y = Mathf.RoundToInt(transform.position.y - blockCount.y % 2 * 0.5f) + blockCount.y % 2 * 0.5f;

        transform.position = new Vector3(x, y, 0);

        // 현재 위치에 블록을 배치할 수 있는지 검사하고 결과를 반환
        bool isSuccess = blockArrangeSystem.TryArrangementBlock(this);

        // 현재 위치에 블록을 배치할 수 없으면 마지막 위치, 크기로 설정
        if(isSuccess == false)
        {
            // 현재 크기에서 0.5 크기로 축소
            StopCoroutine("OnScaleTo");
            StartCoroutine("OnScaleTo", Vector3.one * 0.5f);
            // 현재 위치에서 부모 오브젝트 위치로 이동
            StartCoroutine(OnMoveTo(transform.parent.position, returnTime));
        }

        // 현재 크기에서 0.5 크기로 축소
        //StopCoroutine("OnScaleTo");
        //StartCoroutine("OnScaleTo", Vector3.one * 0.5f);
        // 현재 위치에서 부모 오브젝트 위치로 이동
        //StartCoroutine(OnMoveTo(transform.parent.position, returnTime));
    }

    /// <summary>
    ///  현재 위치에서 end 위치까지 time 시간동안 이동
    /// </summary>
    IEnumerator OnMoveTo(Vector3 end, float time)
    {
        Vector3 start = transform.position;
        float current = 0;
        float percent = 0;

        while(percent < 1)
        {
            current += Time.deltaTime;
            percent = current / time;

            transform.position = Vector3.Lerp(start, end, curveMovement.Evaluate(percent));

            yield return null;
        }
    }

    /// <summary>
    /// 현재 위치에서 end 위치까지 scaleTime 시간동안 확대 or 축소
    /// </summary>
    private IEnumerator OnScaleTo(Vector3 end)
    {
        Vector3 start = transform.localScale;
        float current = 0;
        float percent = 0;

        while(percent < 1)
        {
            current += Time.deltaTime;
            percent = current / returnTime;

            transform.localScale = Vector3.Lerp(start, end, curveScale.Evaluate(percent));

            yield return null;
        }
    }
}
