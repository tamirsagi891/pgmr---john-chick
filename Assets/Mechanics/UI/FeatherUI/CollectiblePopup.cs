using System;
using Avrahamy;
using BitStrap;
using Elad.Scripts;
using Elad.Scripts.Arrows;
using TMPro;
using UnityEngine;

namespace Mechanics.UI.FeatherUI
{
    public class CollectiblePopup : MonoBehaviour
    {
        private static int CollectedFeatherCount => PlayerStatus.FeathersToCollectManager.CollectedFeatherAmount;
        private static int TotalFeatherCount => PlayerStatus.FeathersToCollectManager.StartFeatherAmount;

        [SerializeField]
        private PassiveTimer popupTime = new(2f);

        [SerializeField]
        [AnimatorField("myAnimator")]
        private BoolAnimationParameter showPopup;

        [SerializeField]
        private TMP_Text myText;

        [SerializeField]
        private Animator myAnimator;


        private void OnEnable()
        {
            FeathersToCollectManager.OnPercentageChange += CollectedFeather;
        }

        private void OnDisable()
        {
            FeathersToCollectManager.OnPercentageChange -= CollectedFeather;
        }

        private void Update()
        {
            if (popupTime.IsSet && !popupTime.IsActive)
            {
                showPopup.Set(myAnimator, false);
                popupTime.Clear();
            }
        }

        private void CollectedFeather(object sender, float percentage)
        {
            DoPopup();
        }

        private void DoPopup()
        {
            popupTime.Start();
            myText.text = $@"{CollectedFeatherCount}
<indent=10>/{TotalFeatherCount}";
            showPopup.Set(myAnimator, true);
        }
    }
}