// Unity 엔진과 UI 시스템을 사용하기 위한 네임스페이스를 가져옵니다.
using UnityEngine;
using UnityEngine.UI;

// ShootAR 네임스페이스 안에 ScoreManager 클래스를 정의합니다.
namespace ShootAR {
	public class ScoreManager : MonoBehaviour {
		// 점수를 표시할 UI 텍스트 요소를 참조하는 private 필드입니다.
		[SerializeField] private Text scoreLabel;

		// 현재 점수를 저장하는 프로퍼티입니다.
		public ulong Score { get; private set; }

		// 새 ScoreManager 인스턴스를 생성하고 초기화하는 정적 메서드입니다.
		public static ScoreManager Create(Text scoreLabel = null, ulong score = 0) {
			var o = new GameObject(nameof(ScoreManager)).AddComponent<ScoreManager>();
			o.Score = score;
			o.scoreLabel = scoreLabel;
			return o;
		}

		// 게임 오브젝트가 활성화될 때 점수 레이블의 텍스트를 초기화합니다.
		private void Start() {
			scoreLabel.text = "Score: 0";
		}

		// 점수를 추가하고 UI를 업데이트하는 메서드입니다.
		public void AddScore(ulong points) {
			Score += points;

			if (scoreLabel != null) {
				scoreLabel.text = "Score: " + Score;
			}
		}
	}
}

