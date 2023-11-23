using UnityEngine;

namespace ShootAR.Enemies
{
    // SphereCollider 컴포넌트가 필요한 Crasher 클래스입니다.
    [RequireComponent(typeof(SphereCollider))]
    public class Crasher : Boopboop
    {
        // 프리팹의 초기 속도, 포인트 값, 데미지 값을 저장하는 변수들입니다.
        private static float? prefabSpeed = null;
        private static ulong? prefabPoints = null;
        private static int? prefabDamage = null;

        // Awake 메서드는 초기화 메서드로, 프리팹의 초기값을 설정합니다.
        protected override void Awake()
        {
            base.Awake();
            if (prefabSpeed is null || prefabPoints is null || prefabDamage is null)
            {
                // 프리팹에서 Crasher 클래스를 로드하여 초기값을 설정합니다.
                Crasher prefab = Resources.Load<Crasher>(Prefabs.CRASHER);
                if (prefabSpeed is null)
                    prefabSpeed = prefab.Speed;
                if (prefabPoints is null)
                    prefabPoints = prefab.PointsValue;
                if (prefabDamage is null)
                    prefabDamage = prefab.Damage;
            }
        }

        // ResetState 메서드는 부모 클래스의 상태를 초기화하는 메서드입니다.
        public override void ResetState()
        {
            base.ResetState();
            // 속도, 포인트 값, 데미지를 초기화합니다.
            Speed = (float)prefabSpeed;
            PointsValue = (ulong)prefabPoints;
            Damage = (int)prefabDamage;
        }

        // Destroy 메서드는 부모 클래스의 Destroy 메서드를 호출하고, Crasher를 풀에 반환합니다.
        public override void Destroy()
        {
            base.Destroy();
            ReturnToPool<Crasher>();
        }

        // Attack 메서드는 플레이어를 향해 이동하도록 설정합니다.
        public override void Attack()
        {
            transform.LookAt(Vector3.zero, Vector3.up);
            MoveTo(Vector3.zero);
        }

        // Harm 메서드는 플레이어에게 데미지를 입히는 동작을 구현합니다.
        protected override void Harm(Player target)
        {
            StopMoving();
            target.GetDamaged(Damage);

            /* 적의 모델이 플레이어를 통과할 때 미관적으로 보이므로,
             * 적이 플레이어 뒤의 무작위 위치로 순간이동합니다. 
            if (Camera.main != null)
            {
                Vector3 cameraForward = Camera.main.transform.forward;
                transform.position =
                    target.transform.position - cameraForward * 100f;

                transform.Translate(
                    x: Random.Range(0f, 25f) * cameraForward.x,
                    y: Random.Range(0f, 25f) * cameraForward.y,
                    z: Random.Range(0f, 25f) * cameraForward.z
                );
            }*/

            StopMoving();
        }

        // FixedUpdate 메서드는 고정된 주기로 호출되는 메서드로, AI 동작이 구현되어 있습니다.
        protected void FixedUpdate()
        {
            // AI가 활성화되어 있을 때 동작합니다.
            if (AiEnabled)
            {
                if (!IsMoving)
                {
                    // 이동 중이 아니면 공격합니다.
                    Attack();
                }
                else
                {
                    transform.LookAt(Vector3.zero, Vector3.up);
                }
            }
        }
    }
}
