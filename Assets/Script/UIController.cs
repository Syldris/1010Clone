using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class UIController : MonoBehaviour
{
    [Header("Common")]
    [SerializeField]
    private StageController stageController;

    [Header("InGame")]
    [SerializeField] private TextMeshProUGUI textCurrentScore;
    [SerializeField] private TextMeshProUGUI textHighScore;
    [SerializeField] private UIPausePanelAnimation pausePanel;

    [Header("GameOver")]
    [SerializeField] private GameObject panelGameOver;
    [SerializeField] private Screenshot screenshot;
    [SerializeField] private Image imagescreenshot;
    [SerializeField] private TextMeshProUGUI textResultScore;

    private void Update()
    {
        textCurrentScore.text = stageController.CurrentScore.ToString();
        textHighScore.text = stageController.HighScore.ToString();
    }

    public void BtnClickPause()
    {
        // �Ͻ����� Panel Ȱ��ȭ, ���� �ִϸ��̼� ���
        pausePanel.OnAppear();
    }

    public void BtnClickHome()
    {
        SceneManager.LoadScene("01Main");
    }

    public void BtnClickRestart()
    {
        SceneManager.LoadScene("02Game");
        // ���� Ȱ��ȭ�Ǿ� �ִ� ���� "02Game"�̱� ������ �Ʒ��� ���� �ᵵ ��
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BtnClickkPlay()
    {
        // �Ͻ����� Panel ���� �ִϸ��̼� ���, ��Ȱ��ȭ
        pausePanel.OnDisappear();
    }

    public void GameOver()
    {
        // ���ӿ��� �� �� ����ȭ�� ��ũ������ �Կ��ϰ�,
        // ����� ��ġ�� �κи� �߶󳻼� ��� ȭ�鿡 ���
        imagescreenshot.sprite = screenshot.ScreenshotToSprite();
        textResultScore.text = stageController.CurrentScore.ToString();

        panelGameOver.SetActive(true);
    }
}
