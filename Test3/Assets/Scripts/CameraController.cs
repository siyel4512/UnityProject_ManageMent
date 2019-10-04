using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField]
    [Header("카메라 타겟")]
    GameObject player;
    [SerializeField]
    [Header("카메라의 x축 위치")]
    float offset_X;
    [SerializeField]
    [Header("카메라의 y축 위치")]
    float offset_Y;
    [SerializeField]
    [Header("카메라의 z축 위치")]
    float offset_Z;

    Vector3 cameraPosition;

    private void LateUpdate()
    {
        cameraPosition.x = player.transform.position.x + offset_X;
        cameraPosition.y = player.transform.position.y + offset_Y;
        cameraPosition.z = player.transform.position.z + offset_Z;

        transform.position = cameraPosition;
    }
}
