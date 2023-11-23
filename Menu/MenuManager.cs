using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

namespace ShootAR.Menu
{
    /// <summary>
    /// 메뉴를 관리하는 클래스입니다.
    /// 대부분의 기능은 주로 Inspector에서 버튼 이벤트로 할당됩니다.
    /// </summary>
    
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] private GameObject mainMenu;
        [SerializeField] private GameObject subMenu;
        [SerializeField] private GameObject creditsMenu;
        [SerializeField] private GameObject startMenu;
        [SerializeField] private GameObject waveEditorMenu;
        [SerializeField] private GameObject highscoreMenu;

        private void Awake()
        {
            // 안드로이드 플랫폼이면 Unity 로거를 비활성화합니다.
#if UNITY_ANDROID && !UNITY_EDITOR
            Debug.unityLogger.logEnabled = false;
#endif
        }

        private void Start()
        {
            // 설정 파일을 만듭니다.
            Configuration.Instance.CreateFiles();

            // 백그라운드에서 실행하지 않도록 설정합니다.
            Application.runInBackground = false;
        }

        /// <summary>
        /// 시작 메뉴로 이동합니다.
        /// </summary>
        public void ToStartMenu()
        {
            mainMenu.SetActive(false);
            subMenu.SetActive(true);
            startMenu.SetActive(true);
        }

        /// <summary>
        /// 게임을 시작합니다.
        /// </summary>
        public void StartGame()
        {
            SceneManager.LoadScene(1);
        }

        /// <summary>
        /// 웨이브 편집기 메뉴로 이동합니다.
        /// </summary>
        public void ToWaveEditor()
        {
            mainMenu.SetActive(false);
            subMenu.SetActive(true);
            waveEditorMenu.SetActive(true);
        }

        /// <summary>
        /// 크레딧 메뉴로 이동합니다.
        /// </summary>
        public void ToCredits()
        {
            mainMenu.SetActive(false);
            subMenu.SetActive(true);
            creditsMenu.SetActive(true);
        }

        /// <summary>
        /// 하이스코어 메뉴로 이동합니다.
        /// </summary>
        public void ToHighscores()
        {
            mainMenu.SetActive(false);
            subMenu.SetActive(true);
            highscoreMenu.SetActive(true);
        }

        /// <summary>
        /// 메인 메뉴로 돌아갑니다.
        /// </summary>
        public void ToMainMenu()
        {
            highscoreMenu.SetActive(false);
            creditsMenu.SetActive(false);
            startMenu.SetActive(false);
            waveEditorMenu.SetActive(false);
            subMenu.SetActive(false);
            mainMenu.SetActive(true);
        }

        /// <summary>
        /// 앱을 종료합니다.
        /// </summary>
        public void QuitApp()
        {
            // 변경사항이 저장되지 않았으면 설정을 저장합니다.
            if (Configuration.Instance.UnsavedChanges)
                Configuration.Instance.SaveSettings();

            // 앱을 종료합니다.
            Application.Quit();

#if UNITY_EDITOR
            // 에디터에서 실행 중이면 에디터를 종료합니다.
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
