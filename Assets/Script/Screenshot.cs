using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screenshot : MonoBehaviour
{
    [SerializeField]
    private Camera screenshotCamera;

    public Sprite ScreenshotToSprite()
    {
        int width = Screen.width;
        int height = Screen.height;

        // width, height 크기의 Render Texture 생성
        RenderTexture renderTexture = new RenderTexture(width, height, 24);
        // Camera 컴포넌트의 targetTexture 변수에 renderTexture 등록(카메라에 촬영된 화면을 renderTexture에 저장)
        screenshotCamera.targetTexture = renderTexture;

        // Render() 메소드를 호출해 현재 카메라의 화면을 촬영하고(renderTexture에 저장됨),
        // renderTexture를 RenderTexture.active로 설정
        screenshotCamera.Render();
        RenderTexture.active = renderTexture;

        // 현재 활성화되어 있는 RenderTexture(-renderTexture)의 Pixels 정보를 읽어와
        // Texture2D 타입의 screenshot에 저장
        Texture2D screenshot = new Texture2D(width, width, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, (height - width) * 0.5f, width, width), 0, 0);
        screenshot.Apply();

        screenshotCamera.targetTexture = null;

        // Texture2D -> Sprite로 타입 변환
        Rect rect = new Rect(0, 0, screenshot.width, screenshot.height);
        Sprite sprite = Sprite.Create(screenshot, rect, Vector2.one * 0.5f);

        return sprite;
    }
}
