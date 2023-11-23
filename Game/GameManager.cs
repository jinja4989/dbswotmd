using ShootAR.Enemies; // ShootAR.Enemies 네임스페이스를 사용합니다.
using System; // System 네임스페이스를 사용합니다.
using System.Collections.Generic; // System.Collections.Generic 네임스페이스를 사용합니다.
using System.IO; // System.IO 네임스페이스를 사용합니다.
using UnityEngine; // UnityEngine 네임스페이스를 사용합니다.
using UnityEngine.SceneManagement; // UnityEngine.SceneManagement 네임스페이스를 사용합니다.
using UnityEngine.UI; // UnityEngine.UI 네임스페이스를 사용합니다.
using System.Collections; // System.Collections 네임스페이스를 사용합니다.


namespace ShootAR // ShootAR 네임스페이스를 정의합니다.
{
	/// <summary>
	/// 게임 매니저 클래스. 게임 흐름을 관리하고 레벨 진행, 종료 등을 처리합니다.
	/// </summary>
	public class GameManager : MonoBehaviour // MonoBehaviour를 상속받는 GameManager 클래스를 정의합니다.
	{
		private const int CAPSULE_BONUS_POINTS = 50, // 캡슐 보너스 포인트 상수를 정의하고 50으로 설정합니다.
						  ROUND_AMMO_REWARD = 6; // 라운드 애모 리워드 상수를 정의하고 6으로 설정합니다.

		// 각 타입에 따른 스포너 그룹을 저장하는 딕셔너리
		private Dictionary<Type, List<Spawner>> spawnerGroups; // Type을 키로, Spawner 리스트를 값으로 가지는 딕셔너리를 정의합니다.

		[SerializeField] private ScoreManager scoreManager; // ScoreManager 타입의 private 필드를 정의하고 SerializeField 어트리뷰트를 붙여 Inspector에서 볼 수 있게 합니다.
		[Obsolete] private bool exitTap;    // 이게 왜 필요한지? 제거 가능한지 검토 필요

		[SerializeField] private GameState gameState; // GameState 타입의 private 필드를 정의하고 SerializeField 어트리뷰트를 붙여 Inspector에서 볼 수 있게 합니다.
		[SerializeField] private Button fireButton; // Button 타입의 private 필드를 정의하고 SerializeField 어트리뷰트를 붙여 Inspector에서 볼 수 있게 합니다.
		[SerializeField] private UIManager ui; // UIManager 타입의 private 필드를 정의하고 SerializeField 어트리뷰트를 붙여 Inspector에서 볼 수 있게 합니다.
		private WebCamTexture cam; // WebCamTexture 타입의 private 필드를 정의합니다.
		[SerializeField] private RawImage backgroundTexture; // RawImage 타입의 private 필드를 정의하고 SerializeField 어트리뷰트를 붙여 Inspector에서 볼 수 있게 합니다.
		[SerializeField] private Player player; // Player 타입의 private 필드를 정의하고 SerializeField 어트리뷰트를 붙여 Inspector에서 볼 수 있게 합니다.
		private Stack<Spawner> stashedSpawners; // Spawner 타입의 Stack을 정의합니다.

		private bool readyToRestart = false; // 재시작 준비 여부를 나타내는 bool 타입의 private 필드를 정의하고 false로 초기화합니다.

		/// <summary>
		/// GameManager의 인스턴스를 생성합니다.
		/// </summary>
		/// <param name="player">플레이어 객체</param>
		/// <param name="gameState">게임 상태 객체</param>
		/// <param name="scoreManager">스코어 매니저 객체</param>
		/// <param name="fireButton">발사 버튼</param>
		/// <param name="background">배경 텍스처</param>
		/// <param name="ui">UI 매니저 객체</param>
		/// <returns>GameManager 인스턴스</returns>
		public static GameManager Create(
			Player player, GameState gameState,
			ScoreManager scoreManager = null,
			Button fireButton = null, RawImage background = null,
			UIManager ui = null
		) {
			var o = new GameObject(nameof(GameManager)).AddComponent<GameManager>(); // GameManager 이름의 새 GameObject를 생성하고 GameManager 컴포넌트를 추가합니다.

			o.player = player; // player 매개변수를 GameManager의 player 필드에 할당합니다.
			o.gameState = gameState; // gameState 매개변수를 GameManager의 gameState 필드에 할당합니다.
			o.scoreManager = scoreManager; // scoreManager 매개변수를 GameManager의 scoreManager 필드에 할당합니다.
			o.fireButton = fireButton; // fireButton 매개변수를 GameManager의 fireButton 필드에 할당합니다.
			
			o.backgroundTexture =
				background
				??
				new GameObject("Background").AddComponent<RawImage>(); // background 매개변수가 null이 아니면 background를, null이면 "Background" 이름의 새 GameObject를 생성하고 RawImage 컴포넌트를 추가한 것을 GameManager의 backgroundTexture 필드에 할당합니다.
			o.ui = ui; // ui 매개변수를 GameManager의 ui 필드에 할당합니다.
			UIManager.Create(
				uiCanvas: new GameObject("UI"),
				pauseCanvas: new GameObject("PauseScreen"),
				bulletCount: new GameObject("Ammo").AddComponent<Text>(),
				bulletPlus: new GameObject("Ammo Reward").AddComponent<Text>(),
				messageOnScreen: new GameObject("Message").AddComponent<Text>(),
				score: new GameObject("Score").AddComponent<Text>(),
				roundIndex: new GameObject("Level").AddComponent<Text>(),
				gameState: o.gameState
			); // UIManager.Create 메서드를 호출하여 UI를 생성합니다.

			return o; // GameManager 인스턴스를 반환합니다.
		}

		private void Awake() {
#if UNITY_ANDROID && !UNITY_EDITOR
    // 이 부분은 안드로이드 디바이스에서 실행되며, 유니티 에디터에서는 실행되지 않습니다.
    if (!SystemInfo.supportsGyroscope)
    {
        // 자이로스코프를 지원하지 않는 경우, exitTap을 true로 설정하고 오류 메시지를 출력합니다.
        exitTap = true;
        const string error = "This device does not have Gyroscope";
        if (ui != null)
            ui.MessageOnScreen.text = error; // UI가 null이 아닌 경우, 화면에 오류 메시지를 출력합니다.
        throw new UnityException(error); // 오류를 발생시킵니다.
    }
    else
    {
        // 자이로스코프를 지원하는 경우, 자이로스코프를 활성화합니다.
        Input.gyro.enabled = true;
    }

    // 후면 카메라를 설정합니다.
    for (int i = 0; i < WebCamTexture.devices.Length; i++)
    {
        // 전면 카메라가 아닌 경우, 해당 카메라를 사용하여 WebCamTexture를 생성합니다.
        if (!WebCamTexture.devices[i].isFrontFacing)
        {
            cam = new WebCamTexture(WebCamTexture.devices[i].name, Screen.width, Screen.height);
            break;
        }
    }
#endif

			/* 이 부분은 유니티 에디터에서 실행됩니다.
			 * 유니티 리모트 5를 사용하여 테스트하는 경우, 폰의 카메라를 사용하지 않고 웹캠을 사용해야 합니다.
			 * 따라서 UNITY_ANDROID와 UNITY_EDITOR 둘 다 필요합니다. */
#if UNITY_EDITOR
    // 웹캠 텍스처를 생성합니다.
    cam = new WebCamTexture();
#endif
		}


		private void Start() {
			// 초기화 검사
			if (player == null)
				throw new UnityException("Player object not found"); // player 객체가 null인지 확인하고, null이면 예외를 발생시킵니다.
			if (gameState == null)
				throw new UnityException("GameState object not found"); // gameState 객체가 null인지 확인하고, null이면 예외를 발생시킵니다.
			if (cam == null) {
				const string error = "This device does not have a rear camera";
				ui.MessageOnScreen.text = error; // UI의 MessageOnScreen 텍스트를 error로 설정합니다.
				throw new UnityException(error); // 예외를 발생시킵니다.
			}

			// 카메라 시작 및 설정
			cam.Play(); // 카메라를 시작합니다.
			backgroundTexture.texture = cam; // backgroundTexture의 텍스처를 cam으로 설정합니다.
			backgroundTexture.rectTransform.localEulerAngles = new Vector3(0, 0, cam.videoRotationAngle); // backgroundTexture의 회전 각도를 cam의 videoRotationAngle로 설정합니다.
			float scaleY = cam.videoVerticallyMirrored ? -1.0f : 1.0f; // cam이 수직으로 뒤집혀있는지 확인하고, 뒤집혀있으면 scaleY를 -1.0f로, 그렇지 않으면 1.0f로 설정합니다.
			float videoRatio = (float)cam.width / (float)cam.height; // 비디오의 가로 세로 비율을 계산합니다.
			backgroundTexture.rectTransform.localScale = new Vector3(scaleY, scaleY / videoRatio, 1); // backgroundTexture의 로컬 스케일을 설정합니다.

			// 발사 버튼 리스너 설정
			fireButton?.onClick.AddListener(() => {
				if (gameState.GameOver) {
					if (readyToRestart) {
						SceneManager.LoadScene(1); // 게임이 종료되었고 재시작 준비가 되어있으면 씬을 로드합니다.
					}
				}
				else if (gameState.RoundWon) {
					ui.MessageOnScreen.text = ""; // 라운드를 이겼으면 화면에 메시지를 출력하지 않습니다.
					AdvanceLevel(); // 다음 레벨로 진행합니다.
				}
				else
					player.Shoot(); // 그 외의 경우에는 플레이어가 발사합니다.
			});

			ui.BulletCount.text = player.Ammo.ToString(); // ui의 BulletCount 텍스트를 player의 Ammo로 설정합니다.

			stashedSpawners = new Stack<Spawner>(2); // Spawner의 스택을 생성합니다.
			spawnerGroups = new Dictionary<Type, List<Spawner>>(); // Type을 키로, Spawner 리스트를 값으로 가지는 딕셔너리를 생성합니다.

			gameState.Level = 0; // gameState의 Level을 0으로 설정합니다.

			Spawnable.Pool<Bullet>.Instance.Populate(45); // Bullet의 풀을 생성하고 10개의 객체로 채웁니다.

			AdvanceLevel(); // 다음 레벨로 진행합니다.

			// 가비지 컬렉션 수동 호출
			GC.Collect();
		}

		private void OnEnable() {
			// 이벤트 등록
			if (gameState is null) return; // gameState가 null이면 return합니다.

			gameState.OnGameOver += OnGameOver; // OnGameOver 이벤트를 등록합니다.
			gameState.OnRoundWon += OnRoundWon; // OnRoundWon 이벤트를 등록합니다.
		}

		private void FixedUpdate() {
			// 라운드 진행 중인지, 게임 종료 여부 확인
			if (gameState.RoundStarted && !gameState.GameOver) {
				// 라운드 승리 여부 검사
				bool spawnersStoped = true;
				foreach (var type in spawnerGroups.Keys) {
					foreach (var spawner in spawnerGroups[type])
						if (type.IsSubclassOf(typeof(Enemy)) && spawner.IsSpawning) {
							spawnersStoped = false;
							break;
						}
				}
				if (spawnersStoped && Enemy.ActiveCount == 0) {
					gameState.RoundWon = true; // 모든 적이 사라지면 라운드 승리로 설정합니다.
				}
				// 패배 여부 검사
				else if (Enemy.ActiveCount > 0 && Bullet.ActiveCount == 0
						&& player.Ammo == 0) {
					gameState.GameOver = true; // 적이 남아있고, 총알이 없으며, 플레이어의 탄약이 없으면 게임 오버로 설정합니다.
				}
			}
		}

		private void OnDisable() {
			// 이벤트 해제
			if (gameState != null) {
				gameState.OnGameOver -= OnGameOver; // OnGameOver 이벤트를 해제합니다.
				gameState.OnRoundWon -= OnRoundWon; // OnRoundWon 이벤트를 해제합니다.
			}
		}

		private void OnDestroy() {
			/* cam.Stop() is required to stop the camera so it can be
			 * restarted when the scene loads again; else, after the
			 * scene reloads, the feedback will be blank. */
			cam.Stop(); // 카메라를 정지시킵니다. 이는 씬이 다시 로드될 때 카메라를 재시작하기 위해 필요합니다.
			ClearScene(); // 씬을 클리어합니다.
		}

		private void OnApplicationQuit() {
			if (Configuration.Instance.UnsavedChanges)
				Configuration.Instance.SaveSettings(); // 애플리케이션이 종료될 때, 설정에 저장되지 않은 변경사항이 있으면 설정을 저장합니다.


#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
		}

		public void OnApplicationPause() {
			if (Configuration.Instance.UnsavedChanges)
				Configuration.Instance.SaveSettings();
		}

		/// <summary>
		/// 다음 레벨로 진행하는 메소드.
		/// </summary>
		private void AdvanceLevel() {
			gameState.Level++;
#if DEBUG
            Debug.Log($"Advancing to level {gameState.Level}");
#endif

			// 스포너 패턴 설정
			Stack<Spawner.SpawnConfig>[] patterns
				= Spawner.ParseSpawnPattern(Configuration.Instance.SpawnPatternFile);

			Spawner.SpawnerFactory(patterns, 0, ref spawnerGroups, ref stashedSpawners);

			int totalEnemies = 0;
			foreach (var group in spawnerGroups) {
				group.Value.ForEach(spawner => {
					spawner.StartSpawning();

					if (group.Key.IsSubclassOf(typeof(Enemy))) {
						totalEnemies += spawner.SpawnLimit;
					}
				});
			}

			// 다음 레벨의 시작과 함께 플레이어에게 충분한 총알을 제공
			ulong difference = (ulong)(player.Ammo - totalEnemies);
			if (difference > 0)
				scoreManager.AddScore(difference * 10);
			else if (difference < 0) {
				// 1레벨 이전에는 플레이어가 놓칠 수 있는 총알을 제공
				const float bonusBullets = 0.55f;
				if (gameState.Level == 1) {
					difference *= (ulong)bonusBullets;
				}

				player.Ammo += (difference < int.MaxValue) ? -(int)difference : int.MaxValue;
			}

			gameState.RoundWon = false;
			gameState.RoundStarted = true;
		}

		/// <summary>
		/// 스폰된 모든 객체를 제거하는 메소드.
		/// </summary>
		private void ClearScene() {
			// 라운드 승리 시 사용되지 않은 캡슐에 대한 포인트 부여
			if (gameState.RoundWon) {
				Capsule[] capsules = FindObjectsOfType<Capsule>();
				scoreManager?.AddScore((ulong)(capsules.Length * CAPSULE_BONUS_POINTS));
				foreach (var c in capsules) c.Destroy();
			}

			Spawnable[] spawnables = FindObjectsOfType<Spawnable>();
			foreach (var s in spawnables) s.Destroy();

#if DEBUG
            Debug.Log("Scene cleared.");
#endif
		}

		/// <summary>
		/// 메뉴로 이동하는 메소드.
		/// </summary>
		public void GoToMenu() {
			cam.Stop();
			SceneManager.LoadScene("MainMenu");
		}

		private void OnGameOver() {
			if (ui != null) {
				ui.MessageOnScreen.text =
					$"Game Over\n\n" +
					$"Rounds Survived : {gameState.Level}";
			}

			// 모든 스포너의 스포닝 중단
			foreach (List<Spawner> spawners in spawnerGroups.Values) {
				spawners.ForEach(spawner => {
					spawner.StopSpawning();
				});
			}

			// 하이스코어 확인
			ScoreList highscores = ScoreList.LoadScores();
			if (!highscores.Exists(scoreManager.Score)) {
				// 점수 기록 전에 게임이 다시 시작되지 않도록 확인
				readyToRestart = false;

				/* 플레이어에게 이름을 비동기적으로 묻습니다.
				 * 이름이 제출되면 점수가 테이블에 추가되고
				 * 테이블이 파일에 저장됩니다. */
				StartCoroutine(
					ui.AskName(name => {
						highscores.AddScore(name, scoreManager.Score);

						using (BinaryWriter writer = new BinaryWriter(
							Configuration.Instance.Highscores.OpenWrite()
						)) {
							for (int i = 0; i < ScoreList.POSITIONS; i++) {
								(string, ulong) score = highscores[i];
								writer.Write(score.Item1 ?? ""); // 이름 기록
								writer.Write(score.Item2); // 포인트 기록
							}
						}

						readyToRestart = true;
					})
				);
			}

			ClearScene();
		}

		private void OnRoundWon() {
			ui.MessageOnScreen.text = "Round Clear!";
			ClearScene();
		}

#if DEBUG
        private void OnGUI()
        {
            GUILayout.Label(
                $"Build {Application.version}\n" +
                $"Game Over: {gameState.GameOver}\n" +
                $"Round Over: {gameState.RoundWon}"
            );
          }
#endif
	}
}

