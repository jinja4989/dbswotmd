using UnityEngine;
using System;

namespace ShootAR.Menu {
	// ListSelectionController 클래스는 메뉴에서 리스트 선택을 관리합니다.
	public class ListSelectionController : MonoBehaviour
	{
		// 현재 선택된 요소를 나타냅니다.
		private ListElement selected;

		// Start 메서드에서 패턴 디렉토리로부터 리스트를 채웁니다.
		public void Start() {
			// 패턴 리스트의 항목 템플릿을 로드합니다.
			var itemTemplate = Resources.Load<GameObject>(Prefabs.PATTERN_LIST_CONTENT);
			// 각 패턴 이름에 대해
			foreach(string name in Configuration.Instance.SpawnPatterns) {
				// 새로운 리스트 항목을 생성합니다.
				GameObject newListItem = Instantiate(itemTemplate);

				// 적절한 위치로 게임 오브젝트 계층을 이동시킵니다.
				newListItem.transform.SetParent(transform, false);

				// 리스트 항목의 텍스트를 설정합니다.
				newListItem.GetComponent<ListElement>().SetText(name);
			}

			// 구성 파일에서 선택된 패턴을 가져옵니다.
			selected = Array.Find(
				GetComponentsInChildren<ListElement>(),
				element => element.Id == Configuration.Instance.SpawnPatternSlot
			);

			// 리스트의 선택된 항목의 배경색을 변경합니다.
			selected.Selected = true;
		}

		// 선택을 변경하는 메서드입니다.
		public void ChangeSelection(int id) {
			// 이전에 선택된 항목의 선택을 취소합니다.
			if (selected != null && selected.Id != id)
				selected.Selected = false;

			// 새로 선택된 항목을 찾습니다.
			selected = Array.Find(
				GetComponentsInChildren<ListElement>(),
				element => element.Id == id
			);

			// 구성의 SpawnPatternSlot을 업데이트합니다.
			Configuration.Instance.SpawnPatternSlot = id;
		}

		// 선택된 항목을 삭제하는 메서드입니다.
		public void DeleteSelected() {
			// 리스트의 마지막 항목을 삭제하는 것은 허용되지 않습니다.
			if (Configuration.Instance.SpawnPatterns.Length == 1) return;

			var deleted = selected; // 삭제될 항목

			// 선택을 리스트의 기존 항목으로 전환합니다.
			if (Configuration.Instance.SpawnPatternSlot != 0) {
				ChangeSelection(Configuration.Instance.SpawnPatternSlot - 1);
			}
			else {
				ChangeSelection(1);
			}

			// 삭제된 항목 다음의 모든 항목을 한 칸 위로 이동시킵니다.
			foreach (ListElement item in GetComponentsInChildren<ListElement>())
				if (item.Id > deleted.Id) item.DecrementId();

			// 삭제될 항목을 임시로 선택된 항목으로 설정합니다.
			var newSelected = selected;
			selected = deleted;

			// 항목을 리스트에서 제거합니다.
			DestroyImmediate(selected.gameObject);

			// 패턴을 삭제하면 리스트가 다시 생성되므로, 기존의
			// 객체에 대한 참조가 무효화되므로, 그 전에 필요한 함수를 실행하고,
			// 그 후에 새로운 참조를 생성해야 합니다.
			Configuration.Instance.DeletePattern();

			// 선택된 항목을 재설정합니다.
			selected = newSelected;

			selected.Selected = true;
		}
	}
}

