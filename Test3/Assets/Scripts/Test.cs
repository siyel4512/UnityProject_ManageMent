using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

    BasicCharacterController player;
    Vector3 _direction;

    bool isJump;
    bool isCrouch;

    // Use this for initialization
    void Start () {
        player = GetComponent<BasicCharacterController>();
    }
	
	// Update is called once per frame
	void Update () {
        TryMove();
    }

    void TryMove()
    {
        float dir_x = Input.GetAxis("Horizontal");
        float dir_z = Input.GetAxis("Vertical");
        _direction = new Vector3(dir_x, 0, dir_z);

        if (Input.GetKey(KeyCode.LeftShift)) _direction *= 0.5f;

        player.TryMove(_direction, false, true);
    }

    void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isJump)
        {
            isJump = true;
        }
    }

    void TryCrouch()
    {
        if (Input.GetKey(KeyCode.C) && !isCrouch)
        {
            isCrouch = true;
        }
    }
}
