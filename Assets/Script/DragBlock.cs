using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragBlock : MonoBehaviour
{
    [SerializeField]
    private AnimationCurve curveMovement; // �̵� ���� �׷���
    [SerializeField]
    private AnimationCurve curveScale; // ũ�� ���� �׷���

    private BlockArrangeSystem blockArrangeSystem;

    private float appearTime = 0.5f; // ����� ������ �� �ҿ�Ǵ� �ð�
    private float returnTime = 0.1f; //����� �� ��ġ�� ���ư� �� �ҿ�Ǵ� �ð�

    [field:SerializeField]
    public Vector2Int blockCount { private set; get; } //�ڽ� ��� ����

    public Color Color { private set; get; } // ��� ���� 
    public Vector3[] ChildBlocks { private set; get; } //�ڽ� ��ϵ��� ���� ��ǥ

    public void Setup(BlockArrangeSystem blockArrangeSystem, Vector3 parentPosition)
    {
        this.blockArrangeSystem = blockArrangeSystem;

        // �ڽ� ����� ��� ���� �����̱� ������ �ڽ� ��� �� ������ ������ �����͵� �������.
        Color = GetComponentInChildren<SpriteRenderer>().color;

        // ����� ��翡 ���� �ڽ� ������ �ٸ��� ������ �ڽ� ���� ��ŭ �迭 ���� �����ϰ�,
        // ��� �ڽ� ������Ʈ�� ���� ��ǥ�� ����
        ChildBlocks = new Vector3[transform.childCount];
        for (int i = 0; i < ChildBlocks.Length; ++i)
        {
            ChildBlocks[i] = transform.GetChild(i).localPosition;
        }


        // ���� �ϸ� �ٱ��� ������ ����� �θ� ������Ʈ ��ġ(parent.position)���� �̵�
        StartCoroutine(OnMoveTo(parentPosition, appearTime));
    }

    /// <summary>
    /// �ش� ������Ʈ�� Ŭ���� �� 1ȸ ȣ��
    /// </summary>
    private void OnMouseDown()
    {
        // �巡�� ������ ����� ó�� 0.5�� ũ��� �����Ǵµ�
        // ��� ����� ũ��� 1�̱� ������ ��� ����� ũ��� ������ 1�� Ȯ��
        StopCoroutine("OnScaleTo");
        StartCoroutine("OnScaleTo", Vector3.one);
    }
    /// <summary>
    /// �ش� ������Ʈ�� �巡���� �� �� ������ ȣ�� 
    /// </summary>
    private void OnMouseDrag()
    {
        // ���� ��� ����� Pivot�� ��ϼ��� ���߾����� �����Ǿ� �ֱ� ������ x ��ġ�� �״�� �����,

        // y ��ġ�� y�� ��� ������ ����(BlockCount.y * 0.5f)�� gap��ŭ �߰��� ��ġ�� ���

        // Camera.main.ScreenToWorldPoint()�� Vector3 ��ǥ�� ���ϸ� z���� ī�޶��� ��ġ�� -10�� ������ ������
        // gap���� z ���� +10 ����� ���� ������Ʈ���� ��ġ�Ǿ� �ִ� z=0���� �����ȴ�.
        Vector3 gap = new Vector3(0, blockCount.y * 0.5f + 1, 10);
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + gap;
    }

    /// <summary>
    /// �ش� ������Ʈ�� Ŭ���� ������ �� 1ȸ ȣ��
    /// </summary>
    private void OnMouseUp()
    {
        // �ڽ� ��� ������ Ȧ��, ¦�� �� �� �ٸ��� ���
        // ���� �ݿø��ϴ� Mathf.RoundToInt()�� �̿��� ����� ��� ����ǿ� ����(Snap)�ؼ� ��ġ
        float x = Mathf.RoundToInt(transform.position.x - blockCount.x % 2 * 0.5f) + blockCount.x % 2 * 0.5f;
        float y = Mathf.RoundToInt(transform.position.y - blockCount.y % 2 * 0.5f) + blockCount.y % 2 * 0.5f;

        transform.position = new Vector3(x, y, 0);

        // ���� ��ġ�� ����� ��ġ�� �� �ִ��� �˻��ϰ� ����� ��ȯ
        bool isSuccess = blockArrangeSystem.TryArrangementBlock(this);

        // ���� ��ġ�� ����� ��ġ�� �� ������ ������ ��ġ, ũ��� ����
        if(isSuccess == false)
        {
            // ���� ũ�⿡�� 0.5 ũ��� ���
            StopCoroutine("OnScaleTo");
            StartCoroutine("OnScaleTo", Vector3.one * 0.5f);
            // ���� ��ġ���� �θ� ������Ʈ ��ġ�� �̵�
            StartCoroutine(OnMoveTo(transform.parent.position, returnTime));
        }

        // ���� ũ�⿡�� 0.5 ũ��� ���
        //StopCoroutine("OnScaleTo");
        //StartCoroutine("OnScaleTo", Vector3.one * 0.5f);
        // ���� ��ġ���� �θ� ������Ʈ ��ġ�� �̵�
        //StartCoroutine(OnMoveTo(transform.parent.position, returnTime));
    }

    /// <summary>
    ///  ���� ��ġ���� end ��ġ���� time �ð����� �̵�
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
    /// ���� ��ġ���� end ��ġ���� scaleTime �ð����� Ȯ�� or ���
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
