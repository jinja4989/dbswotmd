using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraRotator : MonoBehaviour
{
    private float mouseY;
    private float mouseX;
    /// <summary>
    /// 주 카메라는 이 컨테이너를 기준으로 회전합니다.
    /// </summary>
    private GameObject cameraContainer;
    /// <summary>
    /// 폰 카메라를 게임 카메라에 보정하는 데 사용되는 회전입니다.
    /// </summary>
    private Quaternion calibrationRotation;

    private void Awake()
    {
        // 카메라 회전을 위한 새로운 게임 오브젝트 생성
        cameraContainer = new GameObject("Camera Container");
    }

    private void Start()
    {
        // 카메라 컨테이너의 초기 위치 및 회전 설정
        cameraContainer.transform.SetPositionAndRotation(transform.position, transform.rotation);
        // 카메라를 카메라 컨테이너의 자식으로 설정
        transform.SetParent(cameraContainer.transform);
        // 카메라 컨테이너를 원하는 방향으로 회전
        cameraContainer.transform.Rotate(90f, -90f, 0f);
#if UNITY_ANDROID
        // 안드로이드일 경우 보정 회전 설정
        calibrationRotation = new Quaternion(0, 0, 1, 0);
#endif
    }

    private void Update()
    {
#if UNITY_ANDROID // 안드로이드에서의 카메라 제어

        // 자이로스코프에서의 피드백을 사용하여 카메라 회전. 입력이 뒤집혀 있음에 주의
        transform.localRotation = Input.gyro.attitude * calibrationRotation;
#endif

#if UNITY_EDITOR // PC에서의 카메라 제어
        if (Input.GetButton("Fire2"))
        {
            // 마우스 입력에 따라 카메라 회전
            mouseY += 5 * Input.GetAxis("Mouse Y");
            mouseX += 5 * Input.GetAxis("Mouse X");
            Camera.main.transform.eulerAngles = new Vector3(-mouseY, mouseX, 0);
        }
#endif
    }

    /*Debug: 카메라 회전
    #if DEBUG
        private void OnGUI()
        {
            GUILayout.Label(
                string.Format(
                    "자이로스코프 자세: {0}\n카메라 자세: {1}\n" +
                    "카메라 로컬 자세: {2}",
                    Input.gyro.attitude, transform.rotation, transform.localRotation
                )
            );
        }
    #endif*/
}
