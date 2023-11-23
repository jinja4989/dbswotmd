using System.Collections;
using UnityEngine;

namespace ShootAR.Enemies
{
    // CapsuleCollider 컴포넌트가 필요한 Drone 클래스입니다.
    [RequireComponent(typeof(CapsuleCollider))]
    public class Drone : Pyoopyoo
    {
        // 공격 딜레이 상수입니다.
        protected const float SHOOT_DELAY = 20f;
        // 이동 딜레이 상수입니다.
        protected const float MOVE_DELAY = 20F;

        // 다음 공격이 허용된 시간 지점입니다.
        protected float nextShot;

        // 다음 이동이 허용된 시간 지점입니다. 궤도 이동에는 해당하지 않습니다.
        protected float nextMove;

        // 프리팹의 초기 속도, 포인트 값, 데미지 값을 저장하는 변수들입니다.
        protected static float? prefabSpeed = null;
        protected static ulong? prefabPoints = null;
        protected static int? prefabDamage = null;

        // 부모 클래스의 상태를 초기화하는 메서드입니다.
        public override void ResetState()
        {
            base.ResetState();
            // 속도, 포인트 값, 데미지를 초기화합니다.
            Speed = (float)prefabSpeed;
            PointsValue = (ulong)prefabPoints;
            Damage = (int)prefabDamage;
        }

        // 초기화 메서드로, 프리팹의 초기 속도, 포인트 값, 데미지 값을 설정합니다.
        protected override void Awake()
        {
            base.Awake();

            if (prefabSpeed is null || prefabPoints is null || prefabDamage is null)
            {
                // 프리팹에서 Drone 클래스를 로드하여 초기값을 설정합니다.
                Drone prefab = Resources.Load<Drone>(Prefabs.DRONE);
                if (prefabSpeed is null)
                    prefabSpeed = prefab.Speed;
                if (prefabPoints is null)
                    prefabPoints = prefab.PointsValue;
                if (prefabDamage is null)
                    prefabDamage = prefab.Damage;
            }
        }

        // 시작 시 호출되는 메서드로, 드론이 나타날 때 곧바로 발사하지 않도록 초기 시간을 무작위로 설정합니다.
        protected override void Start()
        {
            base.Start();
            nextShot = Random.Range(5f, 10f);
        }

        // FixedUpdate 메서드는 고정된 주기로 호출되는 메서드로, 총알 발사 및 이동 동작이 구현되어 있습니다.
        protected void FixedUpdate()
        {
            // 총알 발사
            if (Time.time > nextShot)
            {
                transform.LookAt(Vector3.zero, Vector3.up);

                // Shoot 메서드를 호출하여 총알을 발사합니다.
                Shoot();
                nextShot = Time.time + SHOOT_DELAY;
            }

            // 주변을 이동
            float safetyDistance = 10f - transform.position.magnitude;
            if (safetyDistance > 0)
            {
                float randomFactor = Random.Range(10f, 25f);

                // 안전 거리에 따라 무작위로 이동합니다.
                Vector3 retreatPoint = -transform.forward * (safetyDistance + randomFactor);
                retreatPoint += Random.insideUnitSphere * randomFactor;

                transform.LookAt(retreatPoint, Vector3.up);
                MoveTo(retreatPoint);
            }
            // 궤도 이동을 위해 특정 시간 동안 휴식한 후 이동합니다.
            else if (Time.time > nextMove)
            {
                Vector3 halfPoint = transform.position / 2;
                Vector3 location = halfPoint
                        + Random.insideUnitSphere
                        * (halfPoint.magnitude
                            - halfPoint.magnitude >= 10f ? 10f : 0f);

                MoveTo(location);
                nextMove = Time.time + MOVE_DELAY;
            }
            // 플레이어에게 더 가까운 무작위 지점으로 이동합니다.
            else if (!IsMoving)
            {
                transform.LookAt(Vector3.zero, Vector3.up);
                transform.RotateAround(
                    point: Vector3.zero,
                    axis: Vector3.up,
                    angle: Time.fixedDeltaTime * Speed
                );
            }
        }

        // 부모 클래스의 Destroy 메서드를 호출하고, 드론을 풀에 반환합니다.
        public override void Destroy()
        {
            base.Destroy();
            ReturnToPool<Drone>();
        }

        // 부모 클래스의 Attack 메서드를 구현하고, Shoot 메서드를 호출합니다.
        public override void Attack() => Shoot();

        // 총알을 발사하는 메서드입니다.
        protected override void Shoot()
        {
            // 풀에서 EnemyBullet 객체를 요청하고, 해당 객체를 발사 위치에 배치하고 활성화합니다.
            EnemyBullet bullet = Pool<EnemyBullet>.Instance.RequestObject();
            if (bullet == null) return;

            bullet.transform.position = bulletSpawnPoint.position;
            bullet.transform.rotation = bulletSpawnPoint.rotation;
            bullet.Damage = Damage;
            bullet.gameObject.SetActive(true);
        }
    }
}
