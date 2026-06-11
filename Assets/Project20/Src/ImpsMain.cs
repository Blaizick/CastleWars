using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Blaze.Runtime.Cms;
using Unity.Cinemachine;
using UnityEngine;

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

        public const float TimeToWin = 60.0f;

        void Start()
        {
            StartCoroutine(InitCoroutine());
        }

        IEnumerator InitCoroutine()
        {
            Cms.LoadAll("Content");

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

            Vars.sessionTimer.Restart();
            Vars.teams.Init();
            Vars.input.Init();
            Vars.player.Init();
            Vars.enemySpawner.Init();
            Vars.ui.Init();
            Vars.tutorial.Init();

            yield return StartCoroutine(Vars.tutorial.TutorialCoroutine());

            Vars.restart.Restart();

            yield return StartCoroutine(Vars.ui.textScreen.HideCoroutine());

            yield break;
        }

        void Update()
        {
            Vars.enemySpawner.Update();

            Vars.camera.GetComponent<CinemachineConfiner2D>().InvalidateLensCache();
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

            Vars.player.castle = (PlayerCastle)Vars.teams.ally.castles.Create(Cms.GetEntity("PlayerCastle0"), Vector2.zero);
            Vars.items.Add(new ItemStack(Items.Essence, 100));
        }
    }
}