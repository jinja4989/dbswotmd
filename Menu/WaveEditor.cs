using UnityEngine;
using UnityEngine.UI;

namespace ShootAR.Menu
{
	/* WaveEditor 클래스는 웨이브 편집기를 관리합니다.
	Wave Editor는 게임에서 나타나는 적들의 웨이브를 설계하고 관리하는 도구입니다.
	 주로 다음과 같은 역할을 수행합니다*/
	public class WaveEditor : MonoBehaviour
	{
		// 메인 패널과 하위 패널을 나타내는 게임 오브젝트입니다.
		[SerializeField] private GameObject mainPanel/*, subPanel*/;

		// OnEnable 메서드는 WaveEditor 게임 오브젝트가 활성화될 때 호출됩니다.
		public void OnEnable() {
			// 메인 패널을 활성화합니다.
			mainPanel.SetActive(true);
			// 하위 패널을 비활성화합니다.
			//subPanel.SetActive(false);
		}

		// NewWave 메서드는 새 웨이브를 생성합니다.
		public void NewWave() {
			// 메인 패널을 비활성화합니다.
			mainPanel.SetActive(false);
			// 하위 패널을 활성화합니다.
			//subPanel.SetActive(true);
		}
	}
}

