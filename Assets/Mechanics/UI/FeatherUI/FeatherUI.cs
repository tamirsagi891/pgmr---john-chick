using Elad.Scripts;
using Elad.Scripts.Arrows;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

namespace Mechanics.UI.FeatherUI
{
    public class FeatherUI : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text myText;

        private static int CollectedFeatherCount => PlayerStatus.FeathersToCollectManager.CollectedFeatherAmount;
        private static int TotalFeatherCount => PlayerStatus.FeathersToCollectManager.StartFeatherAmount;


        private void OnEnable()
        {
            FeathersToCollectManager.OnPercentageChange += CollectedFeather;
        }

        private void OnDisable()
        {
            FeathersToCollectManager.OnPercentageChange -= CollectedFeather;
        }

        private void CollectedFeather([CanBeNull] object sender, float e)
        {
            myText.text = $"{CollectedFeatherCount} / {TotalFeatherCount}";
        }
    }
}