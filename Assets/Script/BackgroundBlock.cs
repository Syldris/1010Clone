using System.Collections;
using UnityEngine;

public enum BlockState { Empty = 0, Fill = 1 }

public class BackgroundBlock : MonoBehaviour
{
    private SpriteRenderer spriiteRenderer;

    public BlockState BlockState { private set; get; }

    private void Awake()
    {
        spriiteRenderer = GetComponent<SpriteRenderer>();
        BlockState = BlockState.Empty;
    }

    /// <summary>
    /// 드래그 블록을 배경 블록에 배치했을 때
    /// 배경 블록의 색상을 드래그 블록과 동일하게 설정
    /// </summary>
    public void FillBlock(Color color)
    {
        BlockState = BlockState.Fill;
        spriiteRenderer.color = color;
    }

    /// <summary>
    /// 줄이 완성되면 배경 블록의 Scaale 애니메이션을 재생하고,
    /// 배경 블록 정보를 초기화
    /// </summary>
    
   public void EmptyBlock()
    {
        BlockState = BlockState.Empty;

        StartCoroutine("ScaleTo", Vector3.zero);
    }

    /// <summary>
    /// 0.15초 동안 블록의 크기를 1에서 0으로 축소하고,
    /// 블록의 색상과 크기 재설정
    /// </summary>
    private IEnumerator ScaleTo(Vector3 end)
    {
        Vector3 start = transform.localScale;
        float current = 0;
        float percent = 0;
        float time = 0.15f;

        while(percent < 1)
        {
            current += Time.deltaTime;
            percent = current / time;

            transform.localScale = Vector3.Lerp(start, end, percent);

            yield return null;
        }

        // 축소 애니메이션이 종료되면 블록의 색상을 하얀색으로 설정되고, 블록 크기를 1로 설정
        spriiteRenderer.color = Color.white;
        transform.localScale = Vector3.one;
    }
}

