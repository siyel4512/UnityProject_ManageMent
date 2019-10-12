using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRighttDoor_Control : MonoBehaviour {

    [Header("Door senser가 있는 객체")]
    [SerializeField] Door_DetectObject detectSenser;
    Animator rightDoor_ani;

    // Use this for initialization
    void Start () {
        rightDoor_ani = GetComponent<Animator>();
    }

    private void Update()
    {
        if (detectSenser.isDetected)
            rightDoor_ani.SetBool("Open", true);
        else
            rightDoor_ani.SetBool("Open", false);
    }
}
