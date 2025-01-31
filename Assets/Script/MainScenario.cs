using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainScenario : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textHighScore;

    private void Awake()
    {
        // 저장되어 있는 최고 점수 데이터를 불러와서 출력
        textHighScore.text = PlayerPrefs.GetInt("HighScore").ToString();
    }

    public void BtnClickGaameStart()
    {
        SceneManager.LoadScene("02Game");
    }

    public void BtnCliickGameExit()
    {
        #if UNITY_EDITOR
        //UnityEditor.EditorAppliication.isPlaying = false;
        UnityEditor.EditorApplication.ExitPlaymode();
        #else
        Application.Quit(); 
        #endif
    }
}
