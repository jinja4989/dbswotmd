using UnityEngine;
using UnityEngine.UI;

namespace ShootAR.Menu {
	// Text 컴포넌트가 필요한 ListElement 클래스입니다.
	[RequireComponent(typeof(Text))]
	public class ListElement : MonoBehaviour {
		// 선택되지 않은 배경과 선택된 배경의 색상을 정의합니다.
		private static Color unselectedBackground = new Color(0.32f, 0.35f, 0.42f, 0.25f);
		private static Color selectedBackground = new Color(0.54f, 0.44f, 0.32f, 0.25f);

		// 각 요소에 대해 자동으로 id 번호를 생성하는 데 사용됩니다.
		private static int elementsCount = 0;

		// 요소의 ID를 가져옵니다.
		public int Id { get; private set; }

		/// <summary>Id를 적절하게 감소시킵니다.</summary>
		/// <remarks>
		/// Id는 설정할 수 없으며, 하나씩만 감소시킬 수 있습니다.
		/// </remarks>
		public int DecrementId() => --Id;

		// UI 텍스트를 나타내는 변수입니다.
		[SerializeField] private Text uiText;
		// UI 텍스트를 설정하는 메서드입니다.
		public void SetText(string value) => uiText.text = value;

		// 선택된 상태를 나타내는 변수입니다.
		private bool selected;

		// 선택된 상태를 가져오거나 설정하는 속성입니다.
		public bool Selected {
			get => selected;
			set {
				selected = value;

				uiText.GetComponentInChildren<Image>().color =
					value ? selectedBackground : unselectedBackground;

				if (value) {
					GetComponentInParent<ListSelectionController>()
						.ChangeSelection(Id);
				}
			}
		}

		// Awake 메서드에서 요소의 ID를 초기화합니다.
		public void Awake() {
			Id = elementsCount++;
		}

		// OnDestroy 메서드에서 요소의 수를 감소시킵니다.
		public void OnDestroy() {
			elementsCount--;
		}
	}
}

