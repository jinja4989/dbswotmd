using UnityEngine;

namespace ShootAR
{
    // 이 클래스는 더 이상 사용되지 않습니다. 객체 풀(Object Pool) 도입 이후에는 더 이상 파괴가 필요하지 않으며,
    // 대신에 총알(Bullet)은 플레이어와의 거리를 확인하고 객체 풀로 돌아갑니다.
    [System.Obsolete("Objects don't need to be destroyed anymore after the " +
        "introduction of object pools. Bullets themselves check their " +
        "distance from the player and then return to the pool.")]
    public class Boundary : MonoBehaviour
    {
        // OnTriggerExit 메서드: Collider가 트리거를 떠날 때 호출됩니다.
        private void OnTriggerExit(Collider other) {
            // 트리거를 떠나는 모든 GameObject를 파괴합니다.
            Destroy(other.gameObject);
        }
    }
}

