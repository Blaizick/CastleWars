

using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

namespace Proj21
{
    public class ImpsMenu : MonoBehaviour
    {
        public UiMenu ui;
        public CustomCamera _camera;
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
            Vars.camera.zoomSpeed = 30.0f;
            Vars.camera.Init();

            yield break;
        }

        void Update()
        {
            
        }
    }
}