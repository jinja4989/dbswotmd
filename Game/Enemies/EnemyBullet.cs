using UnityEngine;

namespace ShootAR.Enemies
{
	// Boopboop 클래스를 상속받는 EnemyBullet 클래스입니다.
	public class EnemyBullet : Boopboop
	{
		// 기본 속도와 점수를 정의합니다.
		private const float DEFAULT_SPEED = 1F;
		private const int DEFAULT_POINTS = 5;

		// 프리팹의 속도와 점수를 저장하는 변수입니다.
		private static float? prefabSpeed = null;
		private static ulong? prefabPointValue = null;

		// Awake 메서드에서 프리팹의 속도와 점수를 로드합니다.
		protected override void Awake() {
			base.Awake();

			EnemyBullet prefab;
			if (prefabSpeed == null || prefabPointValue == null) {
				prefab = Resources.Load<EnemyBullet>(Prefabs.ENEMY_BULLET);

				if (prefabSpeed == null)
					prefabSpeed = prefab.Speed;
				if (prefabPointValue == null)
					prefabPointValue = prefab.PointsValue;
			}
		}

		// OnEnable 메서드에서 객체를 월드의 중심으로 이동시킵니다.
		protected override void OnEnable() {
			base.OnEnable();
			MoveTo(Vector3.zero);
		}

		// ResetState 메서드에서 객체의 상태를 초기화합니다.
		public override void ResetState() {
			base.ResetState();
			Speed = (float)prefabSpeed;
			PointsValue = (ulong)prefabPointValue;
		}

		// Attack 메서드에서 객체를 월드의 중심으로 이동시킵니다.
		public override void Attack(){
			MoveTo(Vector3.zero);
		}

		// Harm 메서드에서 플레이어에게 피해를 입히고 객체를 파괴합니다.
		protected override void Harm(Player target) {
			
			target.GetDamaged(Damage);
			Destroy();
		}

		// Destroy 메서드에서 객체를 파괴하고 풀로 반환합니다.
		public override void Destroy() {
			base.Destroy();
			ReturnToPool<EnemyBullet>();
		}
	}
}
