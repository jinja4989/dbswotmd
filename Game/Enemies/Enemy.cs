using UnityEngine;
using System.Collections;

namespace ShootAR.Enemies
{
    /// <summary>
    /// 모든 종류의 적의 부모 클래스입니다.
    /// </summary>
    public abstract class Enemy : Spawnable
    {
        [SerializeField] private ulong pointsValue;
        /// <summary>
        /// 파괴될 때 플레이어 점수에 추가되는 포인트 양입니다.
        /// </summary>
        public ulong PointsValue
        {
            get { return pointsValue; }
            protected set { pointsValue = value; }
        }

        /// <summary>
        /// 플레이어가 이 객체의 공격으로 받는 데미지 양입니다.
        /// </summary>
        [Range(-Player.MAXIMUM_HEALTH, Player.MAXIMUM_HEALTH), SerializeField]
        private int damage;
        public int Damage { get { return damage; } set { damage = value; } }

        /// <summary>
        /// 현재 활성화된 적의 수입니다.
        /// </summary>
        public static int ActiveCount { get; protected set; }

        /// <summary>
        /// 게임 플레이 상태 이펙트로 인해 이 적이 이동할 수 있는지 여부를 나타냅니다.
        /// </summary>
        public bool CanMove { get; set; } = true;

        public bool IsMoving { get; protected set; } = false;

        /// <summary>
        /// 공격 및 이동 AI가 활성화되어 있는지 여부입니다.
        /// </summary>
        /// <remarks>대부분 테스트 중에 유용합니다.</remarks>
        public bool AiEnabled { get; set; } = true;

        [SerializeField] protected GameObject explosion;
        [SerializeField] protected static ScoreManager score;

        protected virtual void Awake()
        {
            if (score == null) score = FindObjectOfType<ScoreManager>();
        }

        protected virtual void Start()
        {
        }

        protected virtual void OnEnable()
        {
            ActiveCount++;
        }

        public override void Destroy()
        {
            score?.AddScore(PointsValue);
            if (explosion != null)
                Instantiate(explosion, transform.position, transform.rotation);
        }

        protected virtual void OnDisable()
        {
            ActiveCount--;
        }

        private Coroutine lastMoveAction;

        /// <summary>
        /// 지점으로 이동합니다.
        /// </summary>
        public void MoveTo(Vector3 point)
        {
            if (!CanMove) return;
            if (IsMoving) StopMoving();

            IEnumerator LerpTo()
            {
                Vector3 start = transform.position;
                float startTime = Time.time;
                float distance = Vector3.Distance(start, point);
                float moveRatio;

                do
                {
                    if (distance == 0f) break;

                    moveRatio = (Time.time - startTime) * Speed / distance;
                    if (moveRatio == 0f)
                    {
                        yield return new WaitForEndOfFrame();
                        continue;
                    }

                    transform.position = Vector3.Slerp(start, point, moveRatio);
                    yield return new WaitForEndOfFrame();
                } while (CanMove && Speed > 0 && transform.position != point);

                IsMoving = false;
            };

            IsMoving = true;
            lastMoveAction = StartCoroutine(LerpTo());
        }

        public void StopMoving()
        {
            if (lastMoveAction == null) return;

            StopCoroutine(lastMoveAction);
            IsMoving = false;
        }

        /// <summary>
        /// 정의된 지점 주위를 지정된 속도로 회전합니다.
        /// </summary>
        /// <param name="orbit">이동할 궤도</param>
        public void OrbitAround(Orbit orbit)
        {
            transform.LookAt(orbit.direction, orbit.perpendicularAxis);
            transform.RotateAround(
                orbit.direction, orbit.perpendicularAxis, Speed * Time.deltaTime);
        }

        /// <summary>
        /// 적에게 공격 명령을 내립니다.
        /// </summary>
        public abstract void Attack();

        public override void ResetState()
        {
            StopMoving();
            CanMove = true;
        }
    }
}
