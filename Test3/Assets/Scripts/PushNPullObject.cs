using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushNPullObject : MonoBehaviour {
    
    [Header("fixed joint 연결용 rigidbody")]
    [SerializeField] Rigidbody childRigid;
    [Header("Push & Pull 시 플레이어가 위치할 Point")]
    [SerializeField] GameObject pullNPushPoint;
    [Header("Push & Pull 시 속도")]
    [SerializeField] float pushNPullSpeed;
    [Header("Push & Pull 시 회전 속도")]
    [SerializeField] float pushNPullTurnSpeed;
    [Header("오브젝트 무게 타입")][Tooltip("1 = 무거움, 2 = 보통")]
    [SerializeField] int objectTyep;

    PlayerController player;
    GameObject playerObject;
    FixedJoint fixedJoint;

    bool isPlayerEnter; // 플레이어가 push & pull 할 준비가 되었는지 확인용
    bool resetPushNPull; // push & pull 애니메이션 초기화 유무 판별용
    bool isUsethis;

    // Use this for initialization
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        fixedJoint = GetComponent<FixedJoint>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckPushNPull();
        MovingObject();
    }

    // 플레이어가 해당 오브젝트를 밀것인지 확인
    void CheckPushNPull()
    {
        if (isPlayerEnter && Input.GetKeyDown(KeyCode.E) && !player.isPushNPull && !isUsethis)
        {
            isUsethis = true; // 해당 오브젝트를 사용하고 있다는 의미

            // 위치 초기화
            player.transform.position = Vector3.Lerp(player.transform.position, pullNPushPoint.transform.position, 1f);
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, pullNPushPoint.transform.rotation, 1f);

            // 오브젝트와 fixed Joint
            fixedJoint.connectedBody = playerObject.transform.parent.GetComponent<PlayerController>().rigid;
            player.isPushNPull = true;

            // 애니메이션 실행
            SelectAnimation(player.isPushNPull);
        }
        else if (!isPlayerEnter && Input.GetKeyDown(KeyCode.E) && player.isPushNPull && isUsethis)
        {
            isUsethis = false; // 해당 오브젝트 사용 종료
            player.isPushNPull = false;
            resetPushNPull = true; // 애니메이션 리셋 시작

            // 애니메이션 정리(리셋)
            PushNRollAniControl();
            SelectAnimation(player.isPushNPull);

            resetPushNPull = false;// 애니메이션 리셋 종료

            // fixed joint 연결 해제
            fixedJoint.connectedBody = childRigid;
            
        }
    }

    // 플레이어가 오브젝트를 밀때 플레이어 움직임 컨트롤
    void MovingObject()
    {
        if (isUsethis && player.isPushNPull)
        {
            PushNRollAniControl();
            MovingObjectTurn();
            player.rigid.MovePosition(player.transform.position + player._direction.normalized * pushNPullSpeed * Time.deltaTime);
        }
    }

    // 오브젝트롤 밀방향으로 플레이어 회전
    void MovingObjectTurn()
    {
        if (player._direction == Vector3.zero) return;

        Quaternion turn = Quaternion.LookRotation(player._direction);
        player.rigid.rotation = Quaternion.Slerp(player.rigid.rotation, turn, pushNPullTurnSpeed * Time.deltaTime);
    }

    // 플레이어 애니메이션 컨트롤
    void PushNRollAniControl()
    {
        // 대기
        if (player._direction == Vector3.zero)
        {
            // push & pull 종료시 플레이어의 움직임이 없을시
            if (!resetPushNPull)
                player.ani.SetFloat("Push & pull Control", 0f);
            else
                player.ani.SetFloat("Push & pull Control", 1f);
        }
        // z 축, x 축
        else if (player._direction.z > 0 || player._direction.z < 0 || player._direction.x > 0 || player._direction.x < 0)
            player.ani.SetFloat("Push & pull Control", 1f);
    }

    // 오브텍트 종류에 따라 애니메이션 선택
    void SelectAnimation(bool _aniState)
    {
        // 무거움
        if (objectTyep == 1)
            player.ani.SetBool("Push & Pull (Heavy)", player.isPushNPull);
        // 보통
        else if (objectTyep == 2)
            player.ani.SetBool("Push & Pull", player.isPushNPull);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Push & Pull")
        {
            isPlayerEnter = true;
            playerObject = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Push & Pull")
        {
            isPlayerEnter = false;
        }
    }
}
