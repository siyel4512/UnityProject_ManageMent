using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 필수 컴포넌트
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]

public class BasicCharacterController : MonoBehaviour
{
    [Header("캐릭터 이동속도")]
    [SerializeField] float m_MoveSpeed = 5.5f;
    [Header("캐릭터 회전속도")]
    [SerializeField] float m_turnSpeed = 10f;
    [Header("캐릭터 점프력")]
    [SerializeField] float m_JumpForce = 5f;
    [Header("바닥 감지 거리")]
    [SerializeField] float m_GroundCheckDistance = 0.1f;

    [SerializeField] float m_MavingTurnSpeed = 360;
    [SerializeField] float m_StationaryTurnSpeed = 180;

    // 필요 컴포넌트들
    Rigidbody m_Rigidbody;
    Animator m_Animator;
    CapsuleCollider m_Capsule;

    // 필요 상태변수들
    bool isGrounded; // 바닥 감지용
    bool isCrouching; // 앉음 유무 판별용

    // 필요 변수들
    const float k_Half = 0.5f;
    float m_OriginGroundCheckDistance; // 기본 바닥 감지 거리 저장용 변수
    Vector3 m_GroundNormal; // 바닥 법선벡터 저장용
    //Vector3 m_CapsuleCenter; // capsule collider 의 중심 좌표 저장
    //float m_CapsuleHeight; // capsule collider 의 높이 저장
    float m_ForwardAmount;
    float m_TurnAmount;

    // Use this for initialization
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Animator = GetComponent<Animator>();
        m_Capsule = GetComponent<CapsuleCollider>();

        m_OriginGroundCheckDistance = m_GroundCheckDistance; // 기본 바닥 감지 거리 저장
        //Debug.Log("m_OriginGroundCheckDistance1 : " + m_OriginGroundCheckDistance);
        m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        //Debug.Log("m_OriginGroundCheckDistance2 : " +  m_OriginGroundCheckDistance);
    }

    //// Update is called once per frame
    //void Update()
    //{
    //    Debug.Log("ddd");
    //    //CheckGroundStatus();
    //}

    // 캐릭터 움직임 관련 함수
    public void TryMove(Vector3 _move, bool _isCrouch, bool _isGrounded)
    {
        //Debug.Log("_move1 : " + _move);
        TryTurn(_move); // 캐릭터 회전
        if (_move.magnitude > 1f) _move.Normalize();
        //Debug.Log("_move2 : " + _move);
        _move = transform.InverseTransformDirection(_move); // 로컬 포지션으로 변환
        //Debug.Log("_move3 : " + _move);
        CheckGroundStatus(); // 바닥 체크
        _move = Vector3.ProjectOnPlane(_move, m_GroundNormal); // 바닥 법선
        //Debug.Log("_move4 : " + _move);
        //Debug.Log("_move.z : " + _move.z);
        m_TurnAmount = Mathf.Atan2(_move.x, _move.y);
        m_ForwardAmount = _move.z;
        //TryTurn(_move); // 캐릭터 회전
        UpdateAnimator(_move);

        Debug.Log("m_Rigidbody.velocity.y : " + m_Rigidbody.velocity.y);
    }

    // 캐릭터 회전
    void TryTurn(Vector3 _move)
    {
        if (_move.magnitude == 0) return;
        Quaternion turn = Quaternion.LookRotation(_move);
        m_Rigidbody.rotation = Quaternion.Slerp(m_Rigidbody.rotation, turn, m_turnSpeed * Time.deltaTime);

        //float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MoveSpeed, m_ForwardAmount);
        //transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
    }

    // 애니메이션에 의한 캐릭터 움직임
    private void OnAnimatorMove()
    {
        //Debug.Log("isGrounded : " + isGrounded);
        if (isGrounded)
        {
            Vector3 v = m_Animator.deltaPosition / Time.deltaTime;
            v.y = m_Rigidbody.velocity.y;
            m_Rigidbody.velocity = v; // 실질적으로 캐릭터를 움직임
        }
    }

    // 애니메이션 컨트롤러
    void UpdateAnimator(Vector3 _move)
    {
        m_Animator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
        //m_Animator.SetFloat("Turn", m_TurnAmount, 0.1f, Time.deltaTime);
    }

    // 바닥 감지
    void CheckGroundStatus()
    {
        RaycastHit hitInfo;

#if UNITY_EDITOR
        // helper to visualise the ground check ray in the scene view
        Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance));
#endif

        // 바닥 붙어있을때
        if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))
        {
            m_GroundNormal = hitInfo.normal;
            //Debug.Log("m_GroundNormal1 : " + m_GroundNormal);
            isGrounded = true;
            m_Animator.applyRootMotion = true;
        }
        // 공중에 떠있을때
        else
        {
            m_GroundNormal = Vector3.up;
            //Debug.Log("m_GroundNormal2 : " + m_GroundNormal);
            isGrounded = false;
            m_Animator.applyRootMotion = false;
        }
    }
}
