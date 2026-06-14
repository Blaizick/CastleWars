using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Blaze.Runtime.Cms;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace Proj21
{
    public class ImpsMain : MonoBehaviour
    {
        public Player player;
        public DesktopInput input;
        public CinemachineCamera _camera;
        public UiMain ui;
        public TutorialSystem tutorial;
        public EffectsSystem effects;

        void Start()
        {
            StartCoroutine(InitCoroutine());
        }

        IEnumerator InitCoroutine()
        {
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

            if (LevelsSystem.level.HasComponent<CmsTutorialLevelTag>())
            {
                yield return StartCoroutine(Vars.tutorial.TutorialCoroutine());
            }
            else
            {
                Vars.restart.Restart();
            }

            yield break;
        }

        void Update()
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
        }
    }

    public static class Vars
    {
        public static Player player;
        public static DesktopInput input;
        public static CinemachineCamera camera;
        public static UiMain ui;
        public static ItemsSystem items; 
        public static TeamSystem teams;
        public static EnemySpawner enemySpawner;
        public static SessionTimer sessionTimer;
        public static TutorialSystem tutorial;
        public static RestartSystem restart;
        public static EffectsSystem effects;
        public static LevelsSystem levels;
        public static UiMenu uiMenu;
        public static DesktopInputMenu inputMenu;
        public static SaveSystem saveSystem;
    }

    [Serializable]
    public class CmsPfbComp : CmsComponent
    {
        public GameObject pfb;
    }

    public class Team
    {
        public CmsEntity cmsEntity;
        public CastlesSystem castles;
        public List<Team> enemyTeams = new();
        public TeamSystem teams;

        public bool IsEnemy(Team team)
        {
            return enemyTeams.Contains(team);
        }
        public bool IsAlly(Team team)
        {
            return !IsEnemy(team);
        }

        public void Init()
        {
            castles = new(this);
            
            enemyTeams = cmsEntity.
                GetAllComponentsOfType<CmsEnemyTeamComp>().
                Select(i => Vars.teams.GetTeam(i.enemyTeam.GetCmsEntity())).
                ToList();
        }
    }

    public class TeamSystem
    {
        public Team ally;
        public Team enemy;

        public List<Team> All => new() {ally, enemy,};

        public void Init()
        {
            ally = new()
            {
                cmsEntity = Cms.GetEntity("AllyTeam"),
                teams = this,
            };
            enemy = new()
            {
                cmsEntity = Cms.GetEntity("EnemyTeam"),
                teams = this,
            };

            ally.Init();
            enemy.Init();
        } 

        public Team GetTeam(CmsEntity cmsEntity)
        {
            return All.Find(i => i.cmsEntity == cmsEntity);
        }

        public void Restart()
        {
            // foreach (var t in Vars.teams.All)
            // {
            //     foreach (var c in new List<Castle>(t.castles.castles))
            //     {
            //         t.castles.Destroy(c);
            //     }
            // }
        }
    }

    [Serializable]
    public class CmsEnemyTeamComp : CmsComponent
    {
        public CmsEntityPfb enemyTeam;
    }

    public class SessionTimer
    {
        public float startTime;

        public void Restart()
        {
            startTime = Time.time;
        }

        public float GetTime() => Time.time - startTime;
    }

    public class RestartSystem
    {
        public List<GameObject> destroyOnRestart = new();

        public void Restart()
        {
            foreach (var i in destroyOnRestart)
            {
                GameObject.Destroy(i);
            }
            destroyOnRestart = new();

            Vars.sessionTimer.Restart();
            Vars.camera.transform.position = new Vector3(0.0f, 0.0f, -10.0f);
            Vars.camera.Lens.OrthographicSize = 8.0f;
            Vars.enemySpawner.Restart();
            Vars.input.Restart();
            Vars.items.Reset();
            Vars.teams.Restart();
            Vars.ui.Restart();
            Vars.levels.Restart();
        }
    }

    public class LevelsSystem
    {
        public static CmsEntity level = null;

        public float Duration => level.HasComponent<CmsLevelDurationComp>() ? level.GetComponent<CmsLevelDurationComp>().duration : 0;
        public bool Finished => Vars.sessionTimer.GetTime() >= Duration;
        public bool CanBeFinished => !level.HasComponent<CmsTutorialLevelTag>();

        public List<CmsEntity> completedLevels = new();

        public void Set(CmsEntity level)
        {
            LevelsSystem.level = level;
        }

        public void LoadLevel()
        {
            SceneManager.LoadScene("SampleScene");
        }

        public void Restart()
        {
            Vars.teams.ally.castles.StartConstructing(level.GetComponent<CmsPlayerCastleComp>().playerCastle.GetCmsEntity(), Vector2.zero).onAppear.AddListener(castle => Vars.player.castle = (PlayerCastle)castle);
            // Vars.player.castle = (PlayerCastle)Vars.teams.ally.castles.Create(level.GetComponent<CmsPlayerCastleComp>().playerCastle.GetCmsEntity(), Vector2.zero);
            foreach (var i in level.GetAllComponentsOfType<CmsAddItemStackOnInitComp>())
            {
                Vars.items.Add(i.itemStack.AsItemStack());
            }
        }

        public void BackToMenu()
        {
            SceneManager.LoadScene("MenuScene");
        }

        public void Complete(CmsEntity _level)
        {
            if (!completedLevels.Contains(_level))
            {
                completedLevels.Add(_level);
                Vars.saveSystem.Save();
            }
        }

        public bool IsCompleted(CmsEntity _level)
        {
            return completedLevels.Contains(_level);
        }

        public bool IsAwailable(CmsEntity _level)
        {
            foreach (var i in _level.GetAllComponentsOfType<CmsRequireCompletedLevel>())
            {
                if (!IsCompleted(i.requiredLevel.GetCmsEntity()))
                {
                    return false;
                }
            }
            return true;
        }
    }

    public class Save
    {
        public List<string> completedLevels = new();
    }

    public class SaveSystem
    {
        public string SavePath => Path.Combine(UnityEngine.Application.persistentDataPath, "save.json");

        public void Save()
        {
            var save = new Save();
            save.completedLevels = Vars.levels.completedLevels.Select(i => i.Id).ToList();
            var json = JsonUtility.ToJson(save);
            File.WriteAllText(SavePath, json);
        }

        public void Load()
        {
            if (!File.Exists(SavePath))
            {
                Save();
                return;
            }
            var json = File.ReadAllText(SavePath);
            var save = JsonUtility.FromJson<Save>(json);
            Vars.levels.completedLevels = save.completedLevels.Select(i => Cms.GetEntity(i)).ToList();
        }
    }

    [Serializable]
    public class CmsPlayerCastleComp : CmsComponent
    {
        public CmsEntityPfb playerCastle;
    }
    [Serializable]
    public class CmsAddItemStackOnInitComp : CmsComponent
    {
        public CmsItemStack itemStack;
    }

    [Serializable]
    public class CmsTutorialLevelTag : CmsComponent
    {
        
    }

    [Serializable]
    public class CmsRequireCompletedLevel : CmsComponent
    {
        public CmsEntityPfb requiredLevel;
    }
}