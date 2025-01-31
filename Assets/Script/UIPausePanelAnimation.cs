using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPausePanelAnimation : MonoBehaviour
{
    [SerializeField] private GameObject imageBackgroundOverlaay;
    [SerializeField] private Animator animator;

    public void OnAppear()
    {
        // ����� �帮�� �����ִ� Image Ȱ��ȭ
        imageBackgroundOverlaay.SetActive(true);
        // ���� �Ͻ����� ������ �� ��µȴ� Panel Ȱ��ȭ
        gameObject.SetActive(true);

        // �Ͻ����� Panel ���� �ִϸ��̼� ���
        animator.SetTrigger("onAppear");
    }

    public void OnDisappear()
    {
        // �Ͻ����� Panel ���� �ִϸ��̼� ���
        animator.SetTrigger("onDisappear");
    }

    /// <summary>
    /// �Ͻ����� Panel ���� �ִϸ��̼��� ����� �� ȣ��
    /// </summary>
    public void EndOfDisappear()
    {
        // ����� �帮�� �����ִ� Image ��Ȱ��ȭ
        imageBackgroundOverlaay.SetActive(false);
        // ���� �Ͻ����� ������ �� ��µǴ� Panel ��Ȱ��ȭ
        gameObject.SetActive(false);
    }
}
