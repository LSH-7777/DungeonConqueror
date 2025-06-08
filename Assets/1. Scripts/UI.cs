using UnityEngine;

public class UI : MonoBehaviour
{
    public Transform target;          // 캐릭터 transform
    public Vector3 offset;            // 캐릭터 기준 위쪽 오프셋 (ex. new Vector3(0, 2.5f, 0))
    private Camera mainCamera;
    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        // 위치 따라가기
        transform.position = target.position + offset;

        // 카메라 바라보기
        transform.forward = mainCamera.transform.forward;
    }
}
