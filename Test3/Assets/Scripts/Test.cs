using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BasicController;

namespace UserControl
{
    public class Test : MonoBehaviour
    {

        BasicCharacterController m_Character;
        Vector3 _direction;

        //bool isJump;
        bool isCrouch;

        // Use this for initialization
        void Start()
        {
            m_Character = GetComponent<BasicCharacterController>();
        }

        // Update is called once per frame
        void Update()
        {
            TryJump();
            TryCrouch();
            TryMove();
        }

        // 플레이어 움직임 입력
        void TryMove()
        {
            float dir_x = Input.GetAxis("Horizontal");
            float dir_z = Input.GetAxis("Vertical");
            _direction = new Vector3(dir_x, 0, dir_z);

            m_Character.TryMove(_direction, isCrouch, m_Character.isJump);
        }

        // 플레이어 점프 입력
        void TryJump()
        {
            if (Input.GetKeyDown(KeyCode.Space) && !m_Character.isJump)
            {
                m_Character.isJump = true;
            }
        }

        // 플레이어 앉기 입력
        void TryCrouch()
        {
            if (Input.GetKey(KeyCode.C) && !isCrouch)
            {
                isCrouch = true;
            }
            else if (Input.GetKeyUp(KeyCode.C) && isCrouch)
            {
                isCrouch = false;
            }
        }
    }
}

