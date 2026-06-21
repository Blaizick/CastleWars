

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Proj21
{
    public class UiMain : MonoBehaviour
    {
        public BuildingTypeSelectUi buildingTypeSelect;
        public ItemsUi items;
        [Serializable]
        public class CameraFollowCastleBtnState
        {
            public Button btn;
            public GameObject root;
        }
        public CameraFollowCastleBtnState cameraFollowCastleState;
        public CameraFollowCastleBtnState cameraNotFollowCastleState;
        public List<CameraFollowCastleBtnState> AllCameraFollorCastleStates => new(){cameraFollowCastleState, cameraNotFollowCastleState,};
        [Serializable]
        public class CastlesModeBtnState
        {
            public Button btn;
            public GameObject root;
        }
        public CastlesModeBtnState setCastlesModeState;
        public CastlesModeBtnState setBuildingsModeState;
        public List<CastlesModeBtnState> AllCastlesModeBtnStates => new(){setCastlesModeState, setBuildingsModeState,};

        public TextScreen textScreen;

        public TextTypewriter taskUiWriter;
        public GameObject taskUiRoot;

        public GameObject timeBlockRoot;
        public TMP_Text timeText;

        public BuildingTypeTooltip buildingTypeTooltip;

        public GameObject winScreenRoot;
        public Button winScreenRestartBtn;
        public Button winScreenBackToMenuBtn;

        public GameObject loseScreenRoot;
        public Button loseScreenRestartBtn;
        public Button loseScreenBackToMenuBtn;

        public SceneTransitionScreen sceneTransitionScreen;

        public PauseMenu pauseMenu;

        public Button pauseMenuBtn;

        public TechTreeUi techTree;
        public Button techTreeBtn;

        public void Init()
        {
            buildingTypeSelect.Init();
            items.Init();
            
            foreach (var s in AllCameraFollorCastleStates)
            {
                s.btn.onClick.AddListener(() =>
                {
                    Vars.input.cameraFollowCastle = !Vars.input.cameraFollowCastle;
                });    
            }
            foreach (var s in AllCastlesModeBtnStates)
            {
                s.btn.onClick.AddListener(() =>
                {
                    Vars.input.castlesMode = !Vars.input.castlesMode;
                });
            }

            textScreen.Init();
            taskUiRoot.SetActive(false);
        
            winScreenRoot.SetActive(false);
            winScreenRestartBtn.onClick.AddListener(() =>
            {
                Vars.restart.Restart();
            });
            winScreenBackToMenuBtn.onClick.AddListener(() =>
            {
                Vars.levels.BackToMenu();
            });

            loseScreenRoot.SetActive(false);
            loseScreenRestartBtn.onClick.AddListener(() =>
            {
                Vars.restart.Restart();
            });
            loseScreenBackToMenuBtn.onClick.AddListener(() =>
            {
                Vars.levels.BackToMenu();
            });

            pauseMenu.Init();

            pauseMenuBtn.onClick.AddListener(() =>
            {
                if (sceneTransitionScreen.tween == null)
                {
                    if (pauseMenu.root.activeInHierarchy)
                    {
                        pauseMenu.Hide();
                    }
                    else
                    {
                        pauseMenu.Show();
                    }
                }
            });

            sceneTransitionScreen.PlayHideAnim();
            
            techTree.Init();
            techTreeBtn.onClick.AddListener(() =>
            {
                techTree.Show();
            });
        }

        public void Restart()
        {
            loseScreenRoot.SetActive(false);
            winScreenRoot.SetActive(false);
            pauseMenu.Restart();
            techTree.root.SetActive(false);
        }

        public void _Update()
        {
            cameraFollowCastleState.root.SetActive(!Vars.input.cameraFollowCastle);
            cameraNotFollowCastleState.root.SetActive(Vars.input.cameraFollowCastle);
        
            setCastlesModeState.root.SetActive(!Vars.input.castlesMode);
            setBuildingsModeState.root.SetActive(Vars.input.castlesMode);
        
            if (Vars.levels.Finished)
            {
                timeText.text = $"Finished";
            }
            else
            {
                timeText.text = $"{(int)Vars.sessionTimer.GetTime()}/{(int)Vars.levels.Duration} sec";
            }
            
            buildingTypeTooltip.root.SetActive(Vars.input.sBuild != null);
        
            items._Update();
            buildingTypeTooltip._Update();
        }
    }
}