using System;
using BitStrap;
using Elad.Scripts;
using Elad.Scripts.Arrows;
using Elad.Scripts.Save_Load_System;
using Managers;
using Mechanics.Dark_Levels;
using Mechanics.UI.Menus.Menu_Utils;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Logger = Nemesh.Logger;

namespace Mechanics.UI.Menus
{
    [AddComponentMenu("Menus/End Level Menu")]
    public class EndLevelMenu : BaseMenuController
    {
        [SerializeField]
        [RequiredReference]
        private FeatherSlider featherSlider;

        [SerializeField]
        private MenuCounter deathCounter;

        [SerializeField]
        private MenuCounter timeCounter;

        [SerializeField]
        private Button darkLevelButton; 

        private int Total => PlayerStatus.FeathersToCollectManager.StartFeatherAmount;

        public override void OpenMenu()
        {
            base.OpenMenu();
            featherSlider.ScoreManager.OnPass += EnableDarkButton;
            featherSlider.StartFeatherAnimation();
            deathCounter.Count = $"{PlayerStatus.PlayerDamageable.DeathAmounts}";
            var time = TimeSpan.FromSeconds(Time.timeSinceLevelLoad);
            timeCounter.Count = $"{time:m\\:ss\\.ff}";
            var score = new LevelScore
            {
                deathCount = PlayerStatus.PlayerDamageable.DeathAmounts,
                completionTime = Time.timeSinceLevelLoad,
                completionDate = DateTime.Now.ToBinary(),
                totalFeathers = PlayerStatus.FeathersToCollectManager.StartFeatherAmount,
                feathersCollected = PlayerStatus.FeathersToCollectManager.CollectedFeatherAmount,
                level = GeneralGameManager.CurrentSceneIndex,
                player = GeneralGameManager.PlayerName,
                isDark = DarkLevelManager.isCurrentLevelDark
            };
            Logger.Log($"Level Score: {score}");
            featherSlider.ScoreManager.SaveLevelScoreIfHighScore(score);
            darkLevelButton.interactable = DarkLevelManager.isCurrentLevelDark;
        }

        public override void CloseMenu()
        {
            base.CloseMenu();
            featherSlider.ScoreManager.OnPass -= EnableDarkButton;
        }

        private void EnableDarkButton(object sender, EventArgs e)
        {
            if (!darkLevelButton.IsInteractable())
            {
                Logger.Log("Here");
                darkLevelButton.interactable = true;
            }
        }
    }
}
