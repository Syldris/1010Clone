using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPausePanelAnimation : MonoBehaviour
{
    [SerializeField] private GameObject imageBackgroundOverlaay;
    [SerializeField] private Animator animator;

    public void OnAppear()
    {
        // 배경을 흐리게 가려주는 Image 활성화
        imageBackgroundOverlaay.SetActive(true);
        // 게임 일시정지 상태일 때 출력된는 Panel 활성화
        gameObject.SetActive(true);

        // 일시정지 Panel 등장 애니메이션 재생
        animator.SetTrigger("onAppear");
    }

    public void OnDisappear()
    {
        // 일시정지 Panel 퇴장 애니메이션 재생
        animator.SetTrigger("onDisappear");
    }

    /// <summary>
    /// 일시정지 Panel 퇴장 애니메이션이 종료된 후 호출
    /// </summary>
    public void EndOfDisappear()
    {
        // 배경을 흐리게 가려주는 Image 비활성화
        imageBackgroundOverlaay.SetActive(false);
        // 게임 일시정지 상태일 때 출력되는 Panel 비활성화
        gameObject.SetActive(false);
    }
}
