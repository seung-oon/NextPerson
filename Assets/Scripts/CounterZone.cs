using UnityEngine;

// 카운터 앞 트리거 영역. 손님(BankCustomer)의 Collider가 들어오면
// 물리 충돌(OnTriggerEnter)로 "도착"을 감지한다.
// → Collider(트리거) + 상대방 Rigidbody가 실제로 동작하는 지점.
[RequireComponent(typeof(Collider))]
public class CounterZone : MonoBehaviour
{
    void Reset()
    {
        var c = GetComponent<Collider>();
        if (c != null) c.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        var customer = other.GetComponentInParent<BankCustomer>();
        if (customer != null)
            customer.NotifyEnterCounterZone();
    }
}
