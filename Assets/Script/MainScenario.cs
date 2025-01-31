using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainScenario : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textHighScore;

    private void Awake()
    {
        // ����Ǿ� �ִ� �ְ� ���� �����͸� �ҷ��ͼ� ���
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
