using UnityEngine;

namespace ShootAR
{
    // Bullet 클래스는 Spawnable 클래스를 상속받음
	[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
	public class Bullet : Spawnable
	{
        // 총알이 최대 이동 가능한 거리
		public const float MAX_TRAVEL_DISTANCE = 150f;

        // 현재 라운드 동안 생성된 전체 총알 수
		public static int Count { private set; get; }
        // 현재 활성화된 총알 수
		public static int ActiveCount { get; private set; }

        // 총알의 속도를 저장하는 정적 변수
		private static float? bulletPrefabSpeed = null;

        // 외부에서 총알을 생성하는 정적 메서드
		public static Bullet Create(float speed) {
			var o = new GameObject(nameof(Bullet)).AddComponent<Bullet>();

            // Rigidbody와 SphereCollider를 추가하고 설정
			o.GetComponent<Rigidbody>().useGravity = false;
			o.GetComponent<SphereCollider>().isTrigger = true;
			o.Speed = speed;

            // 총알을 초기에 비활성화 상태로 설정
			o.gameObject.SetActive(false);
			return o;
		}

        // Awake 메서드: bulletPrefabSpeed 초기화
		protected void Awake() {
			if (bulletPrefabSpeed is null)
				bulletPrefabSpeed = Resources.Load<Bullet>(Prefabs.BULLET).Speed;
		}

        // Start 메서드: 총알의 초기 회전 설정
		protected void Start() {
            // 총알의 초기 회전을 메인 카메라의 회전으로 설정
			transform.rotation =
					Camera.main?.transform.rotation
					?? new Quaternion(0, 0, 0, 0);  
		}

        // OnEnable 메서드: 총알이 활성화될 때 호출되는 메서드
		private void OnEnable() {
            // Rigidbody에 속도를 설정하여 총알을 발사
			GetComponent<Rigidbody>().velocity = transform.forward * Speed;

            // 생성된 총알 및 현재 활성화된 총알 수 증가
			Count++;
			ActiveCount++;
		}

        // OnDisable 메서드: 총알이 비활성화될 때 호출되는 메서드
		private void OnDisable() {
            // 현재 활성화된 총알 수 감소
			ActiveCount--;
		}

        // LateUpdate 메서드: 총알의 이동 제한 거리에 도달하면 파괴
		private void LateUpdate() {
			if (transform.position.magnitude >= MAX_TRAVEL_DISTANCE) Destroy();
		}

        // OnTriggerEnter 메서드: 총알이 다른 Collider에 충돌했을 때 호출
		protected void OnTriggerEnter(Collider other) {
            // 총알이 적이나 Capsule과 충돌하면 해당 객체들을 파괴하고 총알도 파괴
			Spawnable o;
			if ((o = other.GetComponent<Enemies.Enemy>()) != null
			|| (o = other.GetComponent<Capsule>()) != null) {
				o.Destroy();
				Destroy();
			}
		}

        // ResetState 메서드: 총알의 상태를 초기 상태로 재설정
		public override void ResetState() {
			Speed = (float)bulletPrefabSpeed;
		}

        // Destroy 메서드: 총알을 풀에 반환
		public override void Destroy() {
			ReturnToPool<Bullet>();
		}
	}
	
}
