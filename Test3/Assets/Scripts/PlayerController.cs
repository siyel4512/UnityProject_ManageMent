using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [SerializeField] [Header("속도")]
    float moveSpeed;
    [SerializeField] [Header("회전 속도")]
    float turnSpeed;
    [SerializeField] [Header("점프력")]
    float jumpForce;
    //[SerializeField] [Header("Push & Pull 속도")]
    //float pushNPullSpeed;

    CapsuleCollider col;
    public Animator ani;
    public Vector3 _direction;
    public Rigidbody rigid;

    bool isGround;
    bool isWalk;
    bool isJump;
    bool isCrouch;
    bool isFall;
    //bool isJumpFail;
    bool isFallDown;
    bool isFreeFall;
    bool isEnoughHigh;
    public bool isPushNPull;

    int mask;

    // Use this for initialization
    void Start()
    {
        col = GetComponent<CapsuleCollider>();
        rigid = GetComponent<Rigidbody>();
        ani = GetComponent<Animator>();

        mask = 1 << 31;
        mask = ~mask;
    }

    // Update is called once per frame
    void Update()
    {
        CheckGround();
        TryJump();
        TryCrouch();
        TryMove();
    }

    // 플레이어 이동
    void TryMove()
    {
        float dir_x = Input.GetAxis("Horizontal");
        float dir_z = Input.GetAxis("Vertical");

        _direction = new Vector3(dir_x, 0, dir_z);

        TryTurn();
        if (!isCrouch && !isPushNPull)
        {
            Move();
            if (!isJump) // 점프시 애니메이션 잠금
                UndateAnimation(_direction.z, _direction.x, isGround);
        }
    }

    // 실질적인 플레이어 이동
    void Move()
    {
        // 달릴때
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            isWalk = false;

            if ((_direction.z >= -0.25f && _direction.z < 0) || (_direction.z <= 0.25f && _direction.z > 0) || (_direction.x >= -0.25f && _direction.x < 0) || (_direction.x <= 0.25f && _direction.x > 0))
                rigid.MovePosition(transform.position + _direction.normalized * moveSpeed * 0.25f * Time.deltaTime);
            if ((_direction.z >= -0.5f && _direction.z < -0.25f) || (_direction.z <= 0.5f && _direction.z > 0.25f) || (_direction.x >= -0.5f && _direction.x < -0.25f) || (_direction.x <= 0.5f && _direction.x > 0.25f))
                rigid.MovePosition(transform.position + _direction.normalized * moveSpeed * 0.5f * Time.deltaTime);
            else if ((_direction.z >= -1 && _direction.z < -0.5f) || (_direction.z <= 1 && _direction.z > 0.5f) || (_direction.x >= -1 && _direction.x < -0.5f) || (_direction.x <= 1 && _direction.x > 0.5f))
                rigid.MovePosition(transform.position + _direction.normalized * moveSpeed * Time.deltaTime);
        }
        // 걸을때
        else
        {
            isWalk = true;
            rigid.MovePosition(transform.position + _direction.normalized * moveSpeed * 0.5f * Time.deltaTime);
        }
    }

    // 이동 방향 변경시 캐릭터 회전
    void TryTurn()
    {
        if (!isPushNPull)
        {
            if (_direction == Vector3.zero) return;
            Quaternion turn = Quaternion.LookRotation(_direction);

            rigid.rotation = Quaternion.Slerp(rigid.rotation, turn, turnSpeed * Time.deltaTime);
        }
    }

    // 플레이어가 바닥에 붙어있는지 확인
    void CheckGround()
    {
        RaycastHit hitInfo;
        //Debug.Log("isJump : " + isJump + ", isFall : " + isFall + ", isFreeFall : " + isFreeFall + ", isPushNPull : " + isPushNPull + ", isEnoughHigh : " + isEnoughHigh + ", isJumpFail : " + isJumpFail + ", isFreeFall : " + isFreeFall);
        //Debug.Log("isJump : " + isJump + ", isFall : " + isFall + ", isFreeFall : " + isFreeFall + ", isPushNPull : " + isPushNPull + ", isEnoughHigh : " + isEnoughHigh + ", isFreeFall : " + isFreeFall);
        // 바닥에 붙어있을때
        if (Physics.Raycast(transform.position + col.center, -transform.up, out hitInfo, col.bounds.extents.y + 0.1f) // 중앙
            || Physics.Raycast(transform.position + col.center + new Vector3(0, 0, 0.2f), -transform.up, out hitInfo, col.bounds.extents.y + 0.1f) // 북
            || Physics.Raycast(transform.position + col.center + new Vector3(-0.2f, 0, 0), -transform.up, out hitInfo, col.bounds.extents.y + 0.1f) // 서
            || Physics.Raycast(transform.position + col.center + new Vector3(0, 0, -0.2f), -transform.up, out hitInfo, col.bounds.extents.y + 0.1f) // 남
            || Physics.Raycast(transform.position + col.center + new Vector3(0.2f, 0, 0), -transform.up, out hitInfo, col.bounds.extents.y + 0.1f)) // 동
        {
            isGround = true;
            Debug.Log("바닥");
            TryLanding();
        }
        // 공중에 떠있을때
        else
        {
            Debug.Log("공중");
            isGround = false;
            isFall = true;
        }

        CheckFallHight();
    }

    // 착지시 애니메이션 및 상태 정리
    void TryLanding()
    {
        // 정상 착지
        //if (isFall && isEnoughHigh && !isJumpFail)
        if (isFall && isEnoughHigh)
        {
            Debug.Log("정상착지");
            isEnoughHigh = false;
            StartCoroutine(JumpingAnimationCoroutine());
        }
        //// 점프 실패 1
        //else if (isJump && !isFall && !isEnoughHigh && isJumpFail && !isFreeFall)
        //{
        //    isJump = false;
        //    StartCoroutine(JumpingAnimationCoroutine());
        //}
        //// 점프 실패 2
        //else if (isJump && isFall && !isEnoughHigh && isJumpFail && !isFreeFall)
        //{
        //    isJump = false;
        //    StartCoroutine(JumpingAnimationCoroutine());
        //}
        // 점프 실패 후 상태 정리(초기화)
        //else if (!isJump && isFall && !isEnoughHigh && !isJumpFail && !isFreeFall)
        else if (!isJump && isFall && !isEnoughHigh && !isFreeFall)
            isFall = false;
    }

    // 착지 애니메이션 실행 및 상태 초기화
    IEnumerator JumpingAnimationCoroutine()
    {
        //ani.SetBool("Crouch", isFall);
        ani.SetBool("Crouch", true);
        yield return new WaitForSeconds(0.2f);
        isFall = false;
        isJump = false;
        isFreeFall = false;
        //isJumpFail = false;
        //ani.SetBool("Crouch", isFall);
        ani.SetBool("Crouch", false);
    }

    // 충분한 높이로 떠있는지 확인
    void CheckFallHight()
    {
        RaycastHit freeFallInfo;

        //공중에 있을때 실행
        //if (!isGround)
        //{
        //    //Debug.Log("isJump : " + isJump + ", isFall : " + isFall + ", isFreeFall : " + isFreeFall + ", isPushNPull : " + isPushNPull + ", isEnoughHigh : " + isEnoughHigh);
        //    // 충분한 높이로 떴을때
        //    if (!Physics.Raycast(transform.position + col.center, -transform.up, out freeFallInfo, col.bounds.extents.y + 1f))
        //    {
        //        Debug.Log("충분한 높이");
        //        isEnoughHigh = true;

        //        // 자유 낙하시
        //        if (!isJump && isFall && !isFreeFall && !isPushNPull)
        //        {
        //            Debug.Log("자유낙하");
        //            isFreeFall = true;
        //            ani.SetTrigger("Jumping");
        //        }
        //    }
        //}

        //Debug.Log("isJump : " + isJump + ", isFall : " + isFall + ", isFreeFall : " + isFreeFall + ", isPushNPull : " + isPushNPull + ", isEnoughHigh : " + isEnoughHigh);
        // 충분한 높이로 떴을때
        Debug.DrawRay(transform.position + col.center, -transform.up * (col.bounds.extents.y + 0.5f), Color.red, 0.2f);
        if (!Physics.Raycast(transform.position + col.center, -transform.up, out freeFallInfo, col.bounds.extents.y + 1f))
        if (!Physics.Raycast(transform.position + col.center, -transform.up, out freeFallInfo, col.bounds.extents.y + 0.5f))
        {
            isEnoughHigh = true;
            Debug.Log("충분한 높이");

            // 자유 낙하시
            if (!isJump && isFall && !isFreeFall && !isPushNPull)
            {
                //Debug.Log("자유낙하");
                isFreeFall = true;
                ani.SetTrigger("Jumping");
            }
        }

        //// 점프 실패시
        //if (Physics.Raycast(transform.position + col.center, transform.forward, out freeFallInfo, col.bounds.extents.x + 0.1f, mask))
        //{
        //    // 벽 및 오브젝트에 막힌상태에서
        //    if (isJump && _direction != Vector3.zero && !isEnoughHigh && !isJumpFail)
        //        isJumpFail = true;
        //}
    }

    // 플레이어 점프
    void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround && !isCrouch && !isJump && !isFall && !isFreeFall && !isPushNPull)
        {
            

            RaycastHit freeFallInfo;

            if (Physics.Raycast(transform.position + col.center, transform.forward, out freeFallInfo, col.bounds.extents.x + 0.1f, mask)
                && _direction != Vector3.zero)
            {
                //// 벽 및 오브젝트에 막힌상태에서
                //if (isJump && _direction != Vector3.zero && !isEnoughHigh && !isJumpFail)
                //    //isJumpFail = true;
                Debug.Log("벽에 부딧침");
            }
            else
            {
                isJump = true;
                rigid.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                UndateAnimation(_direction.z, _direction.x, isGround);
            }
        }
    }

    IEnumerator FallDown()
    {
        yield return new WaitForSeconds(0.2f);

        isFallDown = false;
    }

    // 플레이어 앉기
    void TryCrouch()
    {
        // 앉았을때
        if (Input.GetKey(KeyCode.C) && isGround)
        {
            isCrouch = true;
            ani.SetBool("Crouch", isCrouch);
            Crouch();
        }
        // 일어났을때
        else if (Input.GetKeyUp(KeyCode.C) && isGround && isCrouch)
        {
            isCrouch = false;
            ani.SetBool("Crouch", isCrouch);
        }
    }

    // 실질적인 플레이어 앉기
    void Crouch()
    {
        if ((_direction.z >= -0.7f && _direction.z < 0) || (_direction.z <= 0.6f && _direction.z > 0) || (_direction.x >= -0.7f && _direction.x < 0) || (_direction.x <= 0.7f && _direction.x > 0))
        {
            //Debug.Log("시작");
            rigid.MovePosition(transform.position + _direction.normalized * Time.deltaTime * moveSpeed * 0.05f);
        }
        else if ((_direction.z >= -1f && _direction.z < -0.7f) || (_direction.z <= 1f && _direction.z > 0.7f) || (_direction.x >= -1f && _direction.x < -0.7f) || (_direction.x <= 1f && _direction.x > 0.7f))
        {
            //Debug.Log("진행");
            rigid.MovePosition(transform.position + _direction.normalized * Time.deltaTime * moveSpeed * 0.2f);
        }

        UndateAnimation(_direction.z, _direction.x, isGround);
    }

    // 플레이어 애니메이션 컨트롤
    void UndateAnimation(float _forward, float _right, bool _isGround)
    {
        // 대기
        if (_direction == Vector3.zero)
            ControlAnimation(0f, _isGround, 0.1f);
        // 아래로 이동
        else if (_direction.z < 0)
        {
            if (!isWalk)
                ControlAnimation(-_forward, _isGround, 0.45f);
            else
                ControlAnimation(0.1f, _isGround, 0.1f);
        }
        // 위로 이동
        else if (_direction.z > 0)
        {
            if (!isWalk)
                ControlAnimation(_forward, _isGround, 0.45f);
            else
                ControlAnimation(0.1f, _isGround, 0.1f);
        }
        // 왼쪽으로 이동
        else if (_direction.x < 0)
        {
            if (!isWalk)
                ControlAnimation(-_right, _isGround, 0.45f);
            else
                ControlAnimation(0.1f, _isGround, 0.1f);
        }
        // 오른쪽으로 이동
        else if (_direction.x > 0)
        {
            if (!isWalk)
                ControlAnimation(_right, _isGround, 0.45f);
            else
                ControlAnimation(0.1f, _isGround, 0.1f);
        }
    }

    // 실질적인 애니메이션 실행
    void ControlAnimation(float _moveValue, bool _isGround, float _dampTime)
    {
        if (_isGround && isJump && !isCrouch)
            ani.SetTrigger("Jumping");
        else if (_isGround && !isJump && isCrouch)
            ani.SetFloat("Forward", _moveValue, _dampTime, Time.deltaTime);
        else
            ani.SetFloat("Forward", _moveValue, _dampTime, Time.deltaTime);
    }
}
