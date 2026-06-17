

using Blaze.Runtime.Cms;
using DG.Tweening;
using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.UI;

namespace Proj21
{
    public class UiMenu : MonoBehaviour
    {
        public GameObject levelsMapRoot;

        public GameObject mainUiRoot;
        public GameObject mainUiBgRoot;
        public GameObject levelsMapUiRoot;
        public Button levelsMapBackButton;
        public LevelsMap levelsMap;

        public Button playButton;

        public SceneTransitionScreen sceneTransitionScreen;

        public void Init()
        {
            levelsMapRoot.SetActive(false);
            levelsMapUiRoot.SetActive(false);

            mainUiRoot.SetActive(true);
            mainUiBgRoot.SetActive(true);

            levelsMapBackButton.onClick.AddListener(() =>
            {
                levelsMapRoot.SetActive(false);
                levelsMapUiRoot.SetActive(false); 
                mainUiRoot.SetActive(true);
                mainUiBgRoot.SetActive(true);
                Vars.camera.transform.position = new Vector3(0.0f, 0.0f, -10.0f);
            });
            playButton.onClick.AddListener(() =>
            {
                mainUiRoot.SetActive(false);
                mainUiBgRoot.SetActive(false);
                levelsMapRoot.SetActive(true);
                levelsMapUiRoot.SetActive(true);
                Vars.camera.transform.position = new Vector3(0.0f, 0.0f, -10.0f);
            });

            levelsMap.Init();

            sceneTransitionScreen.PlayHideAnim();
        }

        public void Update()
        {
            
        }
    }
}