using System.Collections.Generic;
using Blaze.Runtime.Cms;
using UnityEngine;

namespace Proj21
{
    public static class Levels
    {
        public static CmsEntity TutorialLevel => Cms.GetEntity("TutorialLevel");
        public static CmsEntity Level1 => Cms.GetEntity("Level1");
    
        public static List<CmsEntity> All => new(){TutorialLevel, Level1};
    }

    public class LevelsMap : MonoBehaviour
    {
        public LevelMapLevelUiCntPfb tutorialLevel;
        public LevelMapLevelUiCntPfb level1;

        public Dictionary<CmsEntity, LevelMapLevelUiCntPfb> levelToUiCntPfbDic = new();

        public void Init()
        {
            Register(Levels.TutorialLevel, tutorialLevel);
            Register(Levels.Level1, level1);

            foreach (var (level, uiCntPfb) in levelToUiCntPfbDic)
            {
                List<Vector3> positions = new();
                foreach (var i in level.GetAllComponentsOfType<CmsRequireCompletedLevel>())
                {
                    positions.Add(uiCntPfb.transform.position);
                    positions.Add(levelToUiCntPfbDic[i.requiredLevel.GetCmsEntity()].transform.position);
                }
                foreach (var s in uiCntPfb.AllStates)
                {
                    s.lineRenderer.positionCount = positions.Count;
                    s.lineRenderer.SetPositions(positions.ToArray());
                }
            }
        }

        public void Update()
        {
            foreach (var (level, uiCntPfb) in levelToUiCntPfbDic)
            {
                uiCntPfb.awailableState.root.SetActive(!Vars.levels.IsCompleted(level) && Vars.levels.IsAwailable(level));
                uiCntPfb.notAwailableState.root.SetActive(!Vars.levels.IsCompleted(level) && !Vars.levels.IsAwailable(level));
                uiCntPfb.completedState.root.SetActive(Vars.levels.IsCompleted(level));
            }
        }

        public void Register(CmsEntity level, LevelMapLevelUiCntPfb uiCntPfb)
        {
            levelToUiCntPfbDic[level] = uiCntPfb;
            foreach (var state in uiCntPfb.AllStates)
            {
                state.name.text = level.GetComponent<CmsNameComp>().name;
                state.image.sprite = level.GetComponent<CmsSpriteComp>().sprite;
                state.btn.onClick.AddListener(() =>
                {
                    if (Vars.levels.IsAwailable(level))
                    {
                        Vars.uiMenu.sceneTransitionScreen.PlayShowAnim(() =>
                        {
                            Vars.levels.Set(level);
                            Vars.levels.LoadLevel();
                        });
                    }
                });
            }
        }
    }
}