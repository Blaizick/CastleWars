using System.Collections;
using Blaze.Runtime.Cms;
using UnityEngine;

namespace Proj21
{
    public class TutorialSystem : MonoBehaviour
    {
        public void Init()
        {
            
        }

        public IEnumerator TutorialCoroutine()
        {
            Vars.player.castle = (PlayerCastle)Vars.teams.ally.castles.Create(Cms.GetEntity("TestPlayerCastle0"), Vector2.zero);
            Vars.items.Add(new ItemStack(Items.Essence, 50));
            Vars.enemySpawner.playing = false;
            Vars.ui.timeBlockRoot.gameObject.SetActive(false);

            Vars.ui.textScreen.canvasGroup.alpha = 1.0f;
            Vars.ui.textScreen.root.SetActive(true);
            yield return Vars.ui.textScreen.WriteTextCoroutine("Hi!");            
            yield return Vars.ui.textScreen.WaitUntilClick();
            yield return Vars.ui.textScreen.WriteTextCoroutine("Your goal is to survive.");
            yield return Vars.ui.textScreen.WaitUntilClick();
            yield return Vars.ui.textScreen.WriteTextCoroutine("Enemies will attack you from all sides.");
            yield return Vars.ui.textScreen.WaitUntilClick();
            Vars.ui.taskUiRoot.SetActive(true);
            var coroutine = Vars.ui.taskUiWriter.WriteCoroutine("Select the Collector in the bottom-left panel.");
            yield return Vars.ui.textScreen.HideCoroutine();
            if (coroutine != null)
            {
                yield return coroutine;
            }
            while (Vars.input.sBuild != BuildingTypes.Collector)
            {
                yield return null;
            }
            yield return Vars.ui.taskUiWriter.WriteCoroutine("Build a Collector on your castle (Left Mouse Button).");
            while (!Vars.player.castle.buildings.Find(i => i.cmsEntity == BuildingTypes.Collector))
            {
                yield return null;
            }
            yield return Vars.ui.taskUiWriter.WriteCoroutine("Good boy! Collectors gather Essence from the air. Wait until you have 50 Essence.");
            while (!Vars.items.Has(new ItemStack(Items.Essence, 50)))
            {
                yield return null;
            }
            yield return Vars.ui.taskUiWriter.WriteCoroutine("Build a Turret");
            while (!Vars.player.castle.buildings.Find(i => i.cmsEntity == BuildingTypes.Turret))
            {
                yield return null;
            }
            yield return Vars.ui.taskUiWriter.WriteCoroutine("Turrets automatically attack enemy castles and buildings.");
            yield return new WaitForSeconds(3.0f);
            Vars.teams.enemy.castles.Create(Cms.GetEntity("TestEnemyCastle0"), new Vector2(10.0f, 0.0f));
            while (Vars.teams.enemy.castles.castles.Count != 0)
            {
                yield return null;
            }
            yield return Vars.ui.taskUiWriter.WriteCoroutine("You can move your castle. Switch to Castle Control Mode (F or the button in the bottom-left corner), then Right Mouse Button to move.");
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

            Vars.enemySpawner.playing = true;
            Vars.ui.taskUiRoot.SetActive(false);
            Vars.teams.ally.castles.Destroy(Vars.player.castle);
            Vars.items.Reset();
            Vars.ui.timeBlockRoot.gameObject.SetActive(true);
            Vars.sessionTimer.Restart();
        }

        public void Update()
        {
            
        }
    }
}