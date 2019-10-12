using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideObject : MonoBehaviour {

    [Header("숨길 오브젝트")]
    [SerializeField] GameObject hideObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            //Debug.Log("숨기기");
            hideObject.SetActive(false);
            //Debug.Log(other.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            //Debug.Log("숨김 해제");
            hideObject.SetActive(true);
            //Debug.Log(other.name);
        }
    }
}
