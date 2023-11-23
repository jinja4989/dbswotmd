using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ShootAR {
	// UIManager 클래스는 게임의 UI를 관리합니다.
	public class UIManager : MonoBehaviour {
		// UI 요소들을 참조하기 위한 변수들입니다.
		[SerializeField] private GameObject uiCanvas;
		[SerializeField] private GameObject pauseCanvas;
		[SerializeField] private Text bulletCount;
		[SerializeField] private Text bulletPlus;
		[SerializeField] private Text messageOnScreen;
		[SerializeField] private Text score;
		[SerializeField] private Text roundIndex;
		[SerializeField] private GameState gameState;

#pragma warning disable CS0649
		[SerializeField] private Button pauseToMenuButton;
#pragma warning restore CS0649

		// 각 UI 요소에 대한 getter와 setter입니다.
		public Text BulletCount {
			get { return bulletCount; }
			set { bulletCount = value; }
		}

		public Text BulletPlus {
			get { return bulletPlus; }
			set { bulletPlus = value; }
		}

		public Text MessageOnScreen {
			get { return messageOnScreen; }
			set { messageOnScreen = value; }
		}

		public Text Score {
			get { return score; }
			set { score = value; }
		}

		public Text RoundIndex {
			get { return roundIndex; }
			set { roundIndex = value; }
		}

		[SerializeField] private NameAsker nameAsker;

		// UIManager 객체를 생성하는 메서드입니다.
		public static UIManager Create(
				GameObject uiCanvas, GameObject pauseCanvas,
				Text bulletCount, Text bulletPlus,
				Text messageOnScreen,
				Text score, Text roundIndex,
				
				GameState gameState) {
			var o = new GameObject(nameof(UIManager)).AddComponent<UIManager>();

			o.uiCanvas = uiCanvas;
			o.pauseCanvas = pauseCanvas;
			o.bulletCount = bulletCount;
			o.bulletPlus = bulletPlus;
			o.MessageOnScreen = messageOnScreen;
			o.Score = score;
			o.RoundIndex = roundIndex;
			
			
			o.gameState = gameState;

			return o;
		}

		// 게임이 시작될 때 호출되는 메서드입니다.
		public void Start() {
			// 메뉴로 돌아가는 버튼에 리스너를 추가합니다.
			pauseToMenuButton.onClick.AddListener(() => {
				gameState.Paused = false;
				UnityEngine.SceneManagement.SceneManager
					.LoadScene(0);
			});
		}

		// 일시정지 메뉴를 토글하는 메서드입니다.
		public void TogglePauseMenu() {
			if (!pauseCanvas.gameObject.activeSelf) {
				RoundIndex.text = "Round: " + gameState.Level;
				uiCanvas.SetActive(false);
				pauseCanvas.SetActive(true);
				gameState.Paused = true;
			}
			else {
				uiCanvas.SetActive(true);
				pauseCanvas.SetActive(false);
				gameState.Paused = false;
			}
#if DEBUG
			Debug.Log("UIMANAGER:: TimeScale: " + Time.timeScale);
#endif
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


		/// <summary>
		/// 플레이어의 이름을 입력받아 콜백으로 반환하는 메서드입니다.
		///
		/// 플레이어가 UI를 통해 이름을 입력할 때까지 기다린 후,
		/// 그 이름을 <paramref name="nameReturn"/> 안에서 사용합니다.
		/// </summary>
		/// <param name="nameReturn">
		/// 이름 입력이 사용되는 콜백
		/// </param>
		///
		/// <example>
		/// 플레이어에게 이름을 물어보는 예제입니다 (이 경우, 전달된 콜백
		/// 함수는 반환된 이름을 로컬 변수에 할당합니다):
		/// <code>
		/// string playerName = "";
		/// StartCoroutine(ui.AskName(name => playerName = name));
		/// </code>
		/// 이제 반환된 이름을 원하는 대로 사용할 수 있습니다:
		/// <code>
		/// highscores.AddScore(playerName, score);
		/// </code>
		/// </example>
		public IEnumerator AskName(Action<string> nameReturn) {
			nameAsker.gameObject.SetActive(true); // nameAsker를 활성화합니다.

			yield return new WaitWhile(() => nameAsker.PendingQuery);

			nameReturn(nameAsker.InputName);
		}
	}
}

