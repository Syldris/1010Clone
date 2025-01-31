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
        // 일시정지 Panel 활성화, 등장 애니메이션 재생
        pausePanel.OnAppear();
    }

    public void BtnClickHome()
    {
        SceneManager.LoadScene("01Main");
    }

    public void BtnClickRestart()
    {
        SceneManager.LoadScene("02Game");
        // 현재 활성화되어 있는 씬이 "02Game"이기 때문에 아래와 같이 써도 됨
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BtnClickkPlay()
    {
        // 일시정지 Panel 퇴장 애니메이션 재생, 비활성화
        pausePanel.OnDisappear();
    }

    public void GameOver()
    {
        // 게임오버 될 때 게임화면 스크린샷을 촬영하고,
        // 블록이 배치된 부분만 잘라내서 결과 화면에 출력
        imagescreenshot.sprite = screenshot.ScreenshotToSprite();
        textResultScore.text = stageController.CurrentScore.ToString();

        panelGameOver.SetActive(true);
    }
}
