using UnityEngine;
using UnityEngine.UI;

namespace ShootAR.Menu {
	// HighscoreTable 클래스는 게임의 최고 점수 표를 관리합니다.
	public class HighscoreTable : MonoBehaviour	{
		// 표의 각 행을 나타내는 게임 오브젝트 배열입니다.
		[SerializeField] private GameObject[] rows;

		// 점수 목록을 저장하는 ScoreList 객체입니다.
		private ScoreList scores;

		// OnEnable 메서드는 HighscoreTable 게임 오브젝트가 활성화될 때 호출됩니다.
		public void OnEnable() {
			// rows가 null이면 메서드를 종료합니다.
			if (rows == null) return;

			// 점수를 로드합니다.
			scores = ScoreList.LoadScores();

			// 각 행에 대해
			for (int i = 0; i < ScoreList.POSITIONS; i++) {
				// 각 열(플레이어 이름과 점수)을 가져옵니다.
				Text[] column = rows[i].GetComponentsInChildren<Text>();

				// 점수 정보를 가져옵니다.
				var scoreInfo = (name: scores[i].Item1, points: scores[i].Item2);
				// 플레이어 이름을 설정합니다.
				column[0].text = scoreInfo.name;
				// 점수를 설정합니다.
				column[1].text = scoreInfo.points.ToString();
			}
		}
	}
}

