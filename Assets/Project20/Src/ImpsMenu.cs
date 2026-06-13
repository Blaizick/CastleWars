

using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

namespace Proj21
{
    public class ImpsMenu : MonoBehaviour
    {
        public UiMenu ui;
        public CinemachineCamera _camera;
        public DesktopInputMenu input;
        public ContentLoader contentLoader;

        void Start()
        {
            StartCoroutine(InitCoroutine());
        }

        public IEnumerator InitCoroutine()
        {
            contentLoader.Init();

            Vars.uiMenu = ui;
            Vars.camera = _camera;
            Vars.inputMenu = input;
            Vars.levels = new();
            Vars.saveSystem = new();

            Vars.saveSystem.Load();

            Vars.inputMenu.Init();
            Vars.uiMenu.Init();

            yield break;
        }

        void Update()
        {
            
        }
    }
}