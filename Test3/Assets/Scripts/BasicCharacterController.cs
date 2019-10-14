using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BasicController
{
    // 필수 컴포넌트
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Animator))]

    public class BasicCharacterController : MonoBehaviour
    {
        [Header("캐릭터 회전속도")]
        [SerializeField] float m_turnSpeed = 10f;
        [Header("캐릭터 점프력")]
        [SerializeField] float m_JumpForce = 5f;
        [Header("바닥 감지 거리")]
        [SerializeField] float m_GroundCheckDistance = 0.1f;
        [Header("중력 크기 조절")]
        [Range(1f, 4f)] [SerializeField] float m_GravityMultiplier = 2f;
        [Header("플레이어 이동 속도 조절")]
        [SerializeField] float m_MoveSpeedMultiplier = 1f;

        // 필요 컴포넌트들
        Rigidbody m_Rigidbody;
        Animator m_Animator;
        CapsuleCollider m_Capsule;

        // 필요 상태변수들
        bool isGrounded; // 바닥 감지용
        bool isCrouching; // 앉음 유무 판별용
        public bool isJump; // 점프 유무 판별용
        //public bool isCrouch; // 앉기 유무 판별용

        // 필요 변수들
        const float k_Half = 0.5f;
        float m_OriginGroundCheckDistance; // 기본 바닥 감지 거리 저장용 변수
        Vector3 m_GroundNormal; // 바닥 법선벡터 저장용
        Vector3 m_CapsuleCenter; // capsule collider 의 중심 좌표 저장
        float m_CapsuleHeight; // capsule collider 의 높이 저장
        float m_ForwardAmount; // 이동할 크기 저장

        // Use this for initialization
        void Start()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Animator = GetComponent<Animator>();
            m_Capsule = GetComponent<CapsuleCollider>();

            m_OriginGroundCheckDistance = m_GroundCheckDistance; // 기본 바닥 감지 거리 저장
            m_CapsuleCenter = m_Capsule.center; // capsule 중심 좌표값 저장
            m_CapsuleHeight = m_Capsule.height; // capsule 높이 저장
            m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        }

        // 캐릭터 움직임 관련 함수
        public void TryMove(Vector3 _move, bool _isCrouch, bool _isJump)
        {
            TryTurn(_move); // 캐릭터 회전
            if (_move.magnitude > 1f) _move.Normalize(); // 정규화
            if (Input.GetKey(KeyCode.LeftShift)) _move *= 0.5f; // 걷기
            _move = transform.InverseTransformDirection(_move); // 로컬 포지션으로 변환
            CheckGroundStatus(); // 바닥 체크
            _move = Vector3.ProjectOnPlane(_move, m_GroundNormal); // 바닥 법선
            m_ForwardAmount = _move.z; // 로컬 z축 값 저장(애니메이터 float 파라미터 값으로 사용)

            // 바닥에 있을때
            if (isGrounded)
            {
                HandleAirboneMovement(_isJump, _isCrouch);
            }
            // 공중에 떠있을때
            else
            {
                HandleGroundedMovement();
            }

            CapsuleScaleControlForCrouching(_isCrouch); // capsule 크기 조절

            // 애니메이션 업데이트
            UpdateAnimator(_move, _isCrouch);
        }

        // 캐릭터 회전
        void TryTurn(Vector3 _move)
        {
            if (_move.magnitude == 0) return;
            Quaternion turn = Quaternion.LookRotation(_move);
            m_Rigidbody.rotation = Quaternion.Slerp(m_Rigidbody.rotation, turn, m_turnSpeed * Time.deltaTime);
        }

        // 공중에 있을때
        void HandleAirboneMovement(bool _isJump, bool _isCrouch)
        {
            if (_isJump && !_isCrouch && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
            {
                // 실질적인점프
                m_Rigidbody.AddForce(Vector3.up * m_JumpForce, ForceMode.Impulse);

                // 상태정리
                isGrounded = false;
                m_Animator.applyRootMotion = false;
                m_GroundCheckDistance = 0.1f; // 점프 중에는 바닥 감지 거리를 숨김
            }
        }

        // 바닥에 있을때
        void HandleGroundedMovement()
        {
            Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
            m_Rigidbody.AddForce(extraGravityForce); // 점프시 중력방향으로 힘을 가해서 비정상적으로 멀리뛰는 것을 막는다.

            m_GroundCheckDistance = m_Rigidbody.velocity.y < 0 ? m_OriginGroundCheckDistance : 0.01f; // 점프 할때를 재외하고 아래로 떨어질때, 바닥에 걸어다딜때는 velocity.y 값은 0 이하이다.
        }

        // 앉았을때 capsule 크기 조절
        void CapsuleScaleControlForCrouching(bool _isCrouch)
        {
            if (isGrounded && _isCrouch)
            {
                // 크기 줄임
                if (isCrouching) return; // _isCrouch 가 true 일때 이미 capsule 의 크기는 조절되었기때문에 여기서 함수 종료
                m_Capsule.height = m_Capsule.height / 2f;
                m_Capsule.center = m_Capsule.center / 2f;
                isCrouching = true;
            }
            else
            {
                // 원상태로 조절
                m_Capsule.height = m_CapsuleHeight;
                m_Capsule.center = m_CapsuleCenter;
                isCrouching = false;
            }
        }

        // 애니메이션에 의한 캐릭터 움직임
        private void OnAnimatorMove()
        {
            if (isGrounded)
            {
                Vector3 characterAniPos = (m_Animator.deltaPosition * m_MoveSpeedMultiplier) / Time.deltaTime;
                characterAniPos.y = m_Rigidbody.velocity.y; // y 축 값은 변하면 안되기 때문에
                m_Rigidbody.velocity = characterAniPos; // 실질적으로 캐릭터를 움직임
            }
        }

        // 애니메이션 컨트롤러
        void UpdateAnimator(Vector3 _move, bool _isCrouch)
        {
            m_Animator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
            m_Animator.SetBool("Crouch", _isCrouch);
            m_Animator.SetBool("OnGround", isGrounded);

            // 점프 시
            if (!isGrounded)
                m_Animator.SetFloat("Jump", m_Rigidbody.velocity.y);

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
                isGrounded = true;
                m_Animator.applyRootMotion = true;

                if (isJump) isJump = false; // 점프 초기화
            }
            // 공중에 떠있을때
            else
            {
                m_GroundNormal = Vector3.up;
                isGrounded = false;
                m_Animator.applyRootMotion = false;
            }
        }
    }
}

