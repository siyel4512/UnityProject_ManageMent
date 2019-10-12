using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoLeftDoor_Control : MonoBehaviour {

    [Header("Door senser가 있는 객체")]
    [SerializeField] Door_DetectObject detectSenser;
    Animator leftDoor_ani;

    // Use this for initialization
    void Start()
    {
        leftDoor_ani = GetComponent<Animator>();
    }

    private void Update()
    {
        if (detectSenser.isDetected)
            leftDoor_ani.SetBool("Open", true);
        else
            leftDoor_ani.SetBool("Open", false);
    }
}
