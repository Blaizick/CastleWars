using System.Collections;
using Blaze.Runtime.Cms;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Proj21
{
    public class TutorialSystem : MonoBehaviour
    {
        public void Init()
        {
            
        }

        public IEnumerator TutorialCoroutine()
        {
            Vars.teams.ally.castles.StartConstructing(Cms.GetEntity("TestPlayerCastle0"), Vector2.zero).onAppear.AddListener(castle => Vars.player.castle = (PlayerCastle)castle);
            // Vars.player.castle = (PlayerCastle)Vars.teams.ally.castles.Create(Cms.GetEntity("TestPlayerCastle0"), Vector2.zero);
            Vars.items.Add(new ItemStack(Items.Essence, 50));
            Vars.enemySpawner.active = false;
            Vars.ui.timeBlockRoot.gameObject.SetActive(false);

            Vars.ui.textScreen.canvasGroup.alpha = 1.0f;
            Vars.ui.textScreen.root.SetActive(true);
            yield return Vars.ui.textScreen.WriteTextCoroutine("Hi!");            
            yield return Vars.ui.textScreen.WaitUntilClick();
            yield return Vars.ui.textScreen.WriteTextCoroutine("Your goal is to survive and kill all enemy castles.");
            yield return Vars.ui.textScreen.WaitUntilClick();
            yield return Vars.ui.textScreen.WriteTextCoroutine("Enemies will attack you from all sides.");
            yield return Vars.ui.textScreen.WaitUntilClick();
            Vars.ui.taskUiRoot.SetActive(true);
            var collector = BuildingTypes.Collector;
            var coroutine = Vars.ui.taskUiWriter.WriteCoroutine($"Select the {collector.GetComponent<CmsNameComp>().name} in the bottom-left panel.");
            yield return Vars.ui.textScreen.HideCoroutine();
            if (coroutine != null)
            {
                yield return coroutine;
            }
            while (Vars.input.sBuild != collector)
            {
                yield return null;
            }
            yield return Vars.ui.taskUiWriter.WriteCoroutine($"Build a {collector.GetComponent<CmsNameComp>().name} on your castle (Left Mouse Button).");
            while (!Vars.player.castle.buildings.Find(i => i.cmsEntity == collector))
            {
                yield return null;
            }
            var turret = BuildingTypes.Turret;
            var turretCost = turret.GetComponent<CmsBuildCostComp>().cost.AsItemStack();
            yield return Vars.ui.taskUiWriter.WriteCoroutine($"Good boy! Collectors gather Essence from the air. Wait until you have {turretCost.count} Essence.");
            while (!Vars.items.Has(turretCost))
            {
                yield return null;
            }
            yield return Vars.ui.taskUiWriter.WriteCoroutine($"Build a {turret.GetComponent<CmsNameComp>().name}");
            while (!Vars.player.castle.buildings.Find(i => i.cmsEntity == turret))
            {
                yield return null;
            }
            yield return Vars.ui.taskUiWriter.WriteCoroutine("Turrets automatically attack enemy castles and buildings.");
            yield return new WaitForSeconds(3.0f);
            yield return Vars.teams.ally.castles.StartConstructingCoroutine(Cms.GetEntity("TestEnemyCastle0"), Vector2.zero, null);
            // Vars.teams.enemy.castles.Create(Cms.GetEntity("TestEnemyCastle0"), new Vector2(10.0f, 0.0f));
            while (Vars.teams.enemy.castles.castles.Count != 0)
            {
                yield return null;
            }
            yield return Vars.ui.taskUiWriter.WriteCoroutine($"You can move your castle. Switch to Castle Control Mode ({Vars.input.actions.Player.SwitchCastlesMode.GetBindingDisplayString()} or the button in the bottom-left corner), then Right Mouse Button to move.");
            while (!Vars.player.castle.moving)
            {
                yield return null;
            }
            yield return Vars.ui.taskUiWriter.WriteCoroutine("Remove all the shit you recently placed (select them with Right Mouse Button in Building Mode).");
            while (Vars.player.castle.buildings.Count != 0)
            {
                yield return null;
            }
            coroutine = StartCoroutine(Vars.ui.textScreen.ShowCoroutine());
            yield return Vars.ui.textScreen.WriteTextCoroutine("Congratulations! You've completed the tutorial. Good luck in battle!");            
            if (coroutine != null)
            {
                yield return coroutine;
            }
            yield return Vars.ui.textScreen.WaitUntilClick();
        
            Vars.levels.Complete(LevelsSystem.level);
            
            Vars.ui.sceneTransitionScreen.PlayShowAnim(() =>
            {
                Vars.levels.BackToMenu();
            });
        }

        public void Update()
        {
            
        }
    }
}