using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_DetectObject : MonoBehaviour {

    [Header("키 카드 필요 유무")][Tooltip("1. 키 카드 필요, 2. 키 카드 필요없음")]
    [SerializeField] int isNeedKeycard; // 키 카드 필요유무
    public bool isDetected; // 플레이어 및 생명체가 센서에 감지 되었는지
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (isNeedKeycard == 2)
                isDetected = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (isNeedKeycard == 2)
                isDetected = false;
        }
    }
}
