using UnityEngine;

namespace ShootAR
{
	/// <summary>
	///	Holds the game-state.
	/// </summary>
	/// <remarks>
	/// Changes are made outside this class.
	/// </remarks>
	public class GameState : MonoBehaviour
	{
		public delegate void GameOverHandler(); // 게임 오버 이벤트 핸들러를 선언합니다.
		public event GameOverHandler OnGameOver; // 게임 오버 이벤트를 선언합니다.
		public delegate void RoundWonHandler(); // 라운드 승리 이벤트 핸들러를 선언합니다.
		public event RoundWonHandler OnRoundWon; // 라운드 승리 이벤트를 선언합니다.
		public delegate void PauseHandler(); // 일시정지 이벤트 핸들러를 선언합니다.
		public event PauseHandler OnPause; // 일시정지 이벤트를 선언합니다.
		public delegate void RoundStartHandler(); // 라운드 시작 이벤트 핸들러를 선언합니다.
		public event RoundStartHandler OnRoundStart; // 라운드 시작 이벤트를 선언합니다.

		/// <summary>
		/// Stores the round's index number
		/// </summary>
		public int Level { get; set; } // 라운드의 인덱스 번호를 저장하는 속성입니다.

		private bool gameOver;
		/// <summary>
		/// True when player has lost
		/// </summary>
		public bool GameOver {
			get { return gameOver; } // 게임 오버 상태를 반환하는 getter입니다.
			set {
				gameOver = value; // 게임 오버 상태를 설정하는 setter입니다.
				if (value) {
					RoundStarted = false; // 게임 오버 상태가 true이면 라운드 시작 상태를 false로 설정합니다.
					OnGameOver?.Invoke(); // 게임 오버 이벤트를 호출합니다.
#if DEBUG
					Debug.Log("Game over"); // 디버그 로그에 "Game over"를 출력합니다.
#endif
				}
			}
		}

		private bool roundWon;
		/// <summary>
		/// True when player wins the round
		/// </summary>
		public bool RoundWon {
			get { return roundWon; } // 라운드 승리 상태를 반환하는 getter입니다.
			set {
				roundWon = value; // 라운드 승리 상태를 설정하는 setter입니다.
				if (value) {
					RoundStarted = false; // 라운드 승리 상태가 true이면 라운드 시작 상태를 false로 설정합니다.
					OnRoundWon?.Invoke(); // 라운드 승리 이벤트를 호출합니다.
#if DEBUG
					Debug.Log("Round won"); // 디버그 로그에 "Round won"을 출력합니다.
#endif
				}
			}
		}

		private bool paused;
		public bool Paused {
			get { return paused; } // 일시정지 상태를 반환하는 getter입니다.
			set {
				paused = value; // 일시정지 상태를 설정하는 setter입니다.
				Time.timeScale = value ? 0f : 1f; // 일시정지 상태가 true이면 시간 스케일을 0으로, false이면 1로 설정합니다.
				Time.fixedDeltaTime = value ? 0f : 0.02f;   // 일시정지 상태가 true이면 고정 델타 시간을 0으로, false이면 0.02로 설정합니다.
				if (value && OnPause != null) OnPause(); // 일시정지 상태가 true이고 OnPause 이벤트가 null이 아니면 OnPause 이벤트를 호출합니다.
			}
		}

		private bool roundStarted;
		/// <summary>
		/// True when the game is in "playable" state after everything
		/// is been set and running.
		/// Automatically resets to false at round end or game over.
		/// </summary>
		public bool RoundStarted {
			get => roundStarted; // 라운드 시작 상태를 반환하는 getter입니다.
			set {
				roundStarted = value; // 라운드 시작 상태를 설정하는 setter입니다.
				if (value && OnRoundStart != null) OnRoundStart(); // 라운드 시작 상태가 true이고 OnRoundStart 이벤트가 null이 아니면 OnRoundStart 이벤트를 호출합니다.
			}
		}

		public static GameState Create(int level) {
			var o = new GameObject(nameof(GameState)).AddComponent<GameState>(); // GameState 이름의 새 GameObject를 생성하고 GameState 컴포넌트를 추가합니다.

			o.Level = level; // Level을 설정합니다.

			return o; // GameState 인스턴스를 반환합니다.
		}
	}
}

