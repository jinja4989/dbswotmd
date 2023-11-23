using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace ShootAR
{
	public class Player : MonoBehaviour
	{
		/// <summary>플레이어의 최대 체력</summary>
		public const sbyte MAXIMUM_HEALTH = 6; // 플레이어의 최대 체력을 상수로 정의합니다.
		private const float SHOT_COOLDOWN = 0.1f; // 발사 쿨다운 시간을 상수로 정의합니다.
		private const float DAMAGE_COOLDOWN = 1f; // 데미지 쿨다운 시간을 상수로 정의합니다.

		/// <summary>
		/// 복구된 총알 수가 카운터 위에 나타낼 시간(초)
		/// </summary>
		private const float BULLET_PLUS_FLOAT_TIME = 3f; // 복구된 총알 수가 카운터 위에 나타낼 시간을 상수로 정의합니다.

		[SerializeField]
		private GameObject[] healthIndicator = new GameObject[MAXIMUM_HEALTH]; // 체력 표시기를 배열로 선언합니다.

		[Range(0, MAXIMUM_HEALTH), SerializeField]
		private int health; // 플레이어의 현재 체력을 저장하는 변수입니다.
		[Range(0, 999), SerializeField]
		private int ammo; // 플레이어의 현재 탄약 수를 저장하는 변수입니다.

		private float nextFire; // 다음 발사 가능 시간을 저장하는 변수입니다.
		private float nextDamage; // 다음 데미지 가능 시간을 저장하는 변수입니다.

		[SerializeField] private GameState gameState; // 게임 상태를 저장하는 변수입니다.
		
#pragma warning disable CS0649
		[SerializeField] private Text bulletCount; // 총약 수를 표시하는 UI 텍스트를 저장하는 변수입니다.
		[SerializeField] private Text bulletPlus; // 총약 추가를 표시하는 UI 텍스트를 저장하는 변수입니다.
#pragma warning restore CS0649

		/// <summary>
		/// 플레이어의 체력.
		/// 체력이 0이 되면 게임 오버.
		/// </summary>
		public int Health {
			get { return health; } // 체력을 가져오는 getter입니다.

			set {
				health = Mathf.Clamp(value, 0, MAXIMUM_HEALTH); // 체력을 설정하는 setter입니다. 체력은 0과 최대 체력 사이의 값으로 제한됩니다.
				if (health == 0 && gameState != null)
					gameState.GameOver = true; // 체력이 0이 되면 게임 오버 상태로 설정합니다.
				UpdateHealthUI(); // 체력 UI를 업데이트합니다.
			}
		}

		private void UpdateHealthUI() {
			if (healthIndicator[0] == null) return; // 체력 표시기가 null이면 함수를 종료합니다.

			for (int i = 0; i < MAXIMUM_HEALTH; i++) {
				healthIndicator[i].SetActive(i < health); // 체력 표시기를 현재 체력에 따라 활성화 또는 비활성화합니다.
			}
		}

		/// <summary>
		/// 플레이어가 가지고 있는 총알의 양.
		/// 이를 설정하면 UI 상의 총알 카운트도 설정됨.
		/// </summary>
		public int Ammo {
			get { return ammo; } // 탄약 수를 가져오는 getter입니다.
			set {
				// 플레이어에게 얼마나 많은 총알이 있는지 플레이어에게 알려줌
				if (bulletPlus != null && value - ammo > 0) {
					bulletPlus.text = $"+{value - ammo}"; // 총약 추가를 표시하는 UI 텍스트를 업데이트합니다.

					IEnumerator FadeBulletPlus() {
						bulletPlus.gameObject.SetActive(true); // 총약 추가를 표시하는 UI를 활성화합니다.
						bulletPlus.CrossFadeAlpha(1f, 0f, true); // 총약 추가를 표시하는 UI의 알파 값을 1로 설정합니다.

						yield return new WaitForSecondsRealtime(BULLET_PLUS_FLOAT_TIME); // 일정 시간 동안 대기합니다.

						do {
							bulletPlus.CrossFadeAlpha(0f, 5f, true); // 총약 추가를 표시하는 UI의 알파 값을 0으로 설정합니다.
							yield return new WaitForFixedUpdate();
						} while (bulletPlus.color.a != 0f);

						bulletPlus.gameObject.SetActive(false); // 총약 추가를 표시하는 UI를 비활성화합니다.
					}
					StartCoroutine(FadeBulletPlus()); // 코루틴을 시작합니다.
				}

				ammo = value; // 탄약 수를 설정합니다.
				if (bulletCount != null)
					bulletCount.text = ammo.ToString(); // 총약 수를 표시하는 UI 텍스트를 업데이트합니다.
			}
		}

		public bool HasArmor { get; set; } = false; // 플레이어가 방어구를 가지고 있는지 나타내는 속성입니다.
		public bool CanShoot { get; set; } = true; // 플레이어가 발사할 수 있는지 나타내는 속성입니다.

		public static Player Create(
			int health = MAXIMUM_HEALTH, Camera camera = null,
			int ammo = 0, GameState gameState = null) {
			var o = new GameObject(nameof(Player)).AddComponent<Player>(); // Player 이름의 새 GameObject를 생성하고 Player 컴포넌트를 추가합니다.

			var healthUI = new GameObject("HealthUI").transform; // "HealthUI" 이름의 새 GameObject를 생성하고 그 Transform을 가져옵니다.
			for (int i = 0; i < MAXIMUM_HEALTH; i++) {
				o.healthIndicator[i] = new GameObject("HealthIndicator"); // "HealthIndicator" 이름의 새 GameObject를 생성합니다.
				o.healthIndicator[i].transform.parent = healthUI; // 생성한 GameObject를 healthUI의 자식으로 설정합니다.
			}
			o.Health = health; // Health를 설정합니다.
			o.Ammo = ammo; // Ammo를 설정합니다.
			o.gameState = gameState; // gameState를 설정합니다.
			if (camera != null) camera.tag = "MainCamera"; // camera가 null이 아니면, camera의 태그를 "MainCamera"로 설정합니다.

			CapsuleCollider collider = o.GetComponent<CapsuleCollider>(); // CapsuleCollider 컴포넌트를 가져옵니다.
			collider.radius = 0.5f; // collider의 반지름을 0.5f로 설정합니다.
			collider.height = 1.9f; // collider의 높이를 1.9f로 설정합니다.
			collider.isTrigger = true; // collider를 트리거로 설정합니다.

			return o; // Player 인스턴스를 반환합니다.
		}




		/// <summary>
		/// 가능하다면 총알을 생성하고, 충분한 총알이 있고 쿨다운이 끝났다면 발사합니다.
		/// </summary>
		/// <returns>발사된 총알에 대한 참조 또는 조건이 충족되지 않으면 null</returns>
		public Bullet Shoot() {
			if (CanShoot && Ammo > 0 && Time.time >= nextFire) { // 발사 가능 상태이고, 탄약이 있으며, 쿨다운이 끝났다면
				Bullet bullet = Spawnable.Pool<Bullet>.Instance.RequestObject(); // Bullet 풀에서 Bullet 객체를 요청합니다.
				if (bullet is null) return null; // 요청한 Bullet 객체가 null이면 null을 반환합니다.

				Ammo--; // 탄약을 하나 줄입니다.
				nextFire = Time.time + SHOT_COOLDOWN; // 다음 발사 가능 시간을 설정합니다.

				bullet.transform.position = Vector3.zero; // Bullet의 위치를 원점으로 설정합니다.
				bullet.transform.rotation = Camera.main.transform.rotation; // Bullet의 회전을 메인 카메라의 회전으로 설정합니다.
				bullet.gameObject.SetActive(true); // Bullet 게임 오브젝트를 활성화합니다.
				return bullet; // Bullet을 반환합니다.
			}

			return null; // 조건을 만족하지 못하면 null을 반환합니다.
		}

		private void Start() {
			if (bulletCount != null)
				bulletCount.text = Ammo.ToString(); // bulletCount가 null이 아니면, bulletCount의 텍스트를 Ammo로 설정합니다.
			UpdateHealthUI(); // 체력 UI를 업데이트합니다.
		}

		/// <summary>
		/// 플레이어의 체력이 <paramref name="damage"/>만큼 감소합니다.
		/// 체력이 소진되면 게임오버 상태로 설정됩니다.
		/// </summary>
		/// <param name="damage">체력이 감소하는 양</param>
		/// <seealso cref="GameState.GameOver"/>
		public void GetDamaged(int damage) {
			if (damage <= 0 || Time.time < nextDamage) return; // 데미지가 0 이하이거나, 데미지 쿨다운이 끝나지 않았다면 함수를 종료합니다.
			nextDamage = Time.time + DAMAGE_COOLDOWN; // 다음 데미지 가능 시간을 설정합니다.

			if (HasArmor) {
				HasArmor = false; // 방어구가 있으면 방어구를 제거합니다.
				return;
			}

			Health -= damage; // 체력을 damage만큼 감소시킵니다.
		}
		// 오브젝트 프리팹을 참조합니다.
public GameObject myObjectPrefab;

// 새로운 오브젝트를 생성하고 씬에 추가하는 메서드입니다.
void AddObject() {
    // Instantiate 메서드를 사용하여 새로운 오브젝트를 생성합니다.
    GameObject newObject = Instantiate(myObjectPrefab);

    // 새로운 오브젝트의 위치를 설정합니다.
    newObject.transform.position = new Vector3(0, 0, 0);
}


		private void OnDestroy() {
			foreach (var h in healthIndicator) {
				Destroy(h.gameObject); // 모든 체력 표시기 게임 오브젝트를 파괴합니다.
			}
		}
	}
}

