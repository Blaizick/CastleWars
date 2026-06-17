using System;
using System.Collections;
using Blaze.Runtime.Cms;
using Eflatun.SceneReference;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Proj21
{
    public class ImpsBase : MonoBehaviour
    {
        public Player player;
        public DesktopInput input;
        public CinemachineCamera _camera;
        public UiMain ui;
        public TutorialSystem tutorial;
        public EffectsSystem effects;

        public void Start()
        {
            StartCoroutine(InitCoroutine());
        }

        public IEnumerator InitCoroutine()
        {
            var scene = LevelsSystem.level.GetComponent<CmsLevelSceneComp>().scene;
            SceneManager.LoadScene(scene.BuildIndex, LoadSceneMode.Additive);

            yield return null;

            var impsMain = GameObject.FindAnyObjectByType<ImpsMain>();

            Vars.player = player;
            Vars.input = input;
            Vars.camera = _camera;
            Vars.ui = ui;
            Vars.items = new();
            Vars.teams = new();
            Vars.enemySpawner = new();
            Vars.sessionTimer = new();
            Vars.tutorial = tutorial;
            Vars.restart = new();
            Vars.effects = effects;
            Vars.levels = new();
            Vars.saveSystem = new();

            Vars.saveSystem.Load();

            Vars.sessionTimer.Restart();
            Vars.teams.Init();
            Vars.input.Init();
            Vars.player.Init();
            Vars.enemySpawner.Init();
            Vars.ui.Init();
            Vars.tutorial.Init();
            Vars.camera.GetComponent<CinemachineConfiner2D>().BoundingShape2D = impsMain.confiderCollider;

            if (LevelsSystem.level.HasComponent<CmsTutorialLevelTag>())
            {
                StartCoroutine(Vars.tutorial.TutorialCoroutine());
            }
            else
            {
                Vars.restart.Restart();
            }

            while (true)
            {
                Vars.enemySpawner.Update();
                Vars.camera.GetComponent<CinemachineConfiner2D>().InvalidateLensCache();
                if (Vars.levels.CanBeFinished && Vars.levels.Finished)
                {
                    Vars.enemySpawner.active = false;
                    if (Vars.teams.enemy.castles.castles.Count == 0)
                    {
                        Vars.levels.Complete(LevelsSystem.level);
                        Vars.ui.winScreenRoot.SetActive(true);
                    }
                }
                Vars.input._Update();
                Vars.ui._Update();
                yield return null;
            }
        }

        public void Update()
        {
            
        }
    }

    [Serializable]
    public class CmsLevelSceneComp : CmsComponent
    {
        public SceneReference scene;
    }
}