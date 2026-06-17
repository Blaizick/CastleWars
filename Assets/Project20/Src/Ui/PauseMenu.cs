
using UnityEngine;
using UnityEngine.UI;

namespace Proj21
{
    public class PauseMenu : MonoBehaviour
    {
        public GameObject root;

        public Button backToMenuBtn;
        public Button restartBtn;

        public void Init()
        {
            restartBtn.onClick.AddListener(() =>
            {
                Hide();
                Vars.restart.Restart();
            });
            backToMenuBtn.onClick.AddListener(() =>
            {
                Hide();
                Vars.levels.BackToMenu();
            });
            Hide();
        }

        public void Restart()
        {
            Hide();
        }

        public void Update()
        {
            
        }

        public void Show()
        {
            root.SetActive(true);
            Time.timeScale = 0.0f;
        }

        public void Hide()
        {
            root.SetActive(false);
            Time.timeScale = 1.0f;
        }
    }
}