using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class BankCustomer : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform doorSpawnPoint;
    public Transform counterPoint;
    public Transform exitPoint;

    [Header("Movement")]
    public float walkSpeed = 0.5f;

    Animator  m_animator;
    Rigidbody m_rigidbody;

    static readonly int HashWalk  = Animator.StringToHash("isWalking");
    static readonly int HashAngry = Animator.StringToHash("isAngry");

    CustomerData currentData;
    Vector3      target;
    bool         moving;
    bool         m_toCounter;   // 카운터로 향하는 중인지 (트리거 도착 판정용)
    Action       onDone;

    void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();

        // Rigidbody는 물리 간섭 없이 컴포넌트로만 사용 (이동은 Transform이 담당)
        // Discrete를 먼저 설정해야 isKinematic=true 전환 시 경고/되돌림이 없음
        m_rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
        m_rigidbody.isKinematic            = true;
        m_rigidbody.useGravity             = false;

        // 모든 Animator의 root motion을 끔 → 애니메이션이 캐릭터를 멋대로 이동시키지 않음
        // 컨트롤러가 달린 Animator를 실제 제어 대상으로 선택
        foreach (var a in GetComponentsInChildren<Animator>(true))
        {
            a.applyRootMotion = false;
            if (a.runtimeAnimatorController != null) m_animator = a;
        }

        gameObject.SetActive(false);
    }

    // 물리 이동은 FixedUpdate에서 Rigidbody가 직접 수행 (키네마틱 MovePosition)
    void FixedUpdate()
    {
        if (!moving) return;

        Vector3 pos = m_rigidbody.position;
        Vector3 dir = target - pos;
        dir.y = 0;

        // 목표 지점 도달 시 멈춤 (트리거 존이 없어도 거리 기준으로 항상 동작)
        if (dir.magnitude <= 0.1f)
        {
            m_rigidbody.MovePosition(new Vector3(target.x, pos.y, target.z));
            ArriveStop();
            return;
        }

        // Rigidbody.MovePosition/MoveRotation 으로 이동·회전 (물리 보간 적용)
        Vector3 step = dir.normalized * walkSpeed * Time.fixedDeltaTime;
        m_rigidbody.MovePosition(pos + step);
        m_rigidbody.MoveRotation(Quaternion.LookRotation(new Vector3(dir.x, 0f, dir.z)));
    }

    // 카운터 트리거 존(Collider)에 진입했을 때 호출 — 물리 충돌로 도착 감지
    public void NotifyEnterCounterZone()
    {
        if (moving && m_toCounter)
        {
            m_rigidbody.MovePosition(new Vector3(target.x, m_rigidbody.position.y, target.z));
            ArriveStop();
        }
    }

    void ArriveStop()
    {
        moving = false;
        m_toCounter = false;
        if (m_animator != null) m_animator.SetBool(HashWalk, false);
        var cb = onDone;
        onDone = null;
        cb?.Invoke();
    }

    void StartMove(Vector3 dest, Action callback)
    {
        target = new Vector3(dest.x, transform.position.y, dest.z);
        onDone = callback;
        moving = true;
        if (m_animator != null) m_animator.SetBool(HashWalk, true);
    }

    public void WalkToCounter(CustomerData data)
    {
        currentData = data;
        gameObject.SetActive(true);
        transform.position = doorSpawnPoint != null ? doorSpawnPoint.position : new Vector3(0, 0, 4f);
        transform.rotation = Quaternion.Euler(0, 180, 0);
        Vector3 dest = counterPoint != null ? counterPoint.position : new Vector3(0, 0, 1.55f);
        m_toCounter = true;
        StartMove(dest, () => BankGameManager.Instance.OnCustomerArrived(currentData));
    }

    public void WalkOut(Action callback)
    {
        StopAngry();
        Vector3 dest = exitPoint != null ? exitPoint.position : new Vector3(0, 0, 5f);
        StartMove(dest, () =>
        {
            gameObject.SetActive(false);
            Destroy(gameObject, 0.1f);
            callback?.Invoke();
        });
    }

    public void PlayAngry() { if (m_animator != null) m_animator.SetBool(HashAngry, true); }
    public void StopAngry() { if (m_animator != null) m_animator.SetBool(HashAngry, false); }
}
