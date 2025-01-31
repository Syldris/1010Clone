using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCResolution : MonoBehaviour
{
    private void Awake()
    {
        Screen.SetResolution(1080, 1920, false);
    }
}

/// <sumare>
/// 게임을 실행하고 가장 먼저 실행되는 클래스의 메소드에서
/// Screen.SetResolution(1080, 1920, true); 과 같이 해상도를 설정할 수 있습니다.
/// 매개변수 => (가로, 세로, FullScreen 여부)
/// 
/// 보통 다양한 해상도 대응을 위해 아래와 같이 많이 쓰는데
/// 지금처럼 PC에서 풀스크린으로 설정헀을 경우에는 해상도 값을 입력하면
/// 좌/우 여백 부분은 검은색 화면으로 처리됩니다.
/// Screen.SetResolution(Screen.width, Screen.width * 16 / 9, false);
/// </sumare>