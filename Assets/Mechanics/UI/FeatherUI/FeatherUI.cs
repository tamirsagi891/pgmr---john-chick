using Elad.Scripts;
using Elad.Scripts.Arrows;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Mechanics.UI.FeatherUI
{
    public class FeatherUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text myText;
        [SerializeField] private float messageChangeInterval = 2f;  // Change this value as needed
        [SerializeField] private Image fillBar;

        private static int CollectedFeatherCount => PlayerStatus.FeathersToCollectManager.CollectedFeatherAmount;
        private static int TotalFeatherCount => PlayerStatus.FeathersToCollectManager.StartFeatherAmount;

        private int currentMessageIndex = 0;

        private void OnEnable()
        {
            FeathersToCollectManager.OnPercentageChange += CollectedFeather;
            StartCoroutine(ChangeMessage());
        }

        private void OnDisable()
        {
            FeathersToCollectManager.OnPercentageChange -= CollectedFeather;
            StopCoroutine(ChangeMessage());
        }

        private void CollectedFeather([CanBeNull] object sender, float e)
        {
            UpdateMessage();
            UpdateBarFill();
        }

        private IEnumerator ChangeMessage()
        {
            while (true)
            {
                yield return new WaitForSeconds(messageChangeInterval);
                currentMessageIndex = (currentMessageIndex + 1) % 3;
                UpdateMessage();
            }
        }

        private void UpdateBarFill()
        {
            if (fillBar)
            {
                fillBar.fillAmount = GetPercentageCollected() / 100f;
            }
        }

        private void UpdateMessage()
        {
            switch (currentMessageIndex)
            {
                case 0:
                    myText.text = GetLevelStatusText();
                    break;
                case 1:
                    myText.text = GetGradeStatusText();
                    break;
                case 2:
                    myText.text = GetCurrentAmountText();
                    break;
            }
        }

        private string GetLevelStatusText()
        {
            // Return your level status text here
            if (GetPercentageCollected() < 60)
            {
                return $"NEXT LEVEL : LOCKED!";
            }
            return $"NEXT LEVEL : UNLOCKED!"; 
        }

        private string GetGradeStatusText()
        {
            int percentageCollected = GetPercentageCollected();
            string grade;

            if (percentageCollected < 60)
            {
                return $"COLLECT 60% TO UNLOCK NEXT LEVEL"; 
            }
            if (percentageCollected < 70)
            {
                grade = "D";
            }
            else if (percentageCollected < 80)
            {
                grade = "C";
            }
            else if (percentageCollected < 90)
            {
                grade = "B";
            }
            else if (percentageCollected < 100)
            {
                grade = "A";
            }
            else
            {
                grade = "S";
            }

            return $"GRADE : {grade}"; 
        }

        private string GetCurrentAmountText()
        {
            return $"{CollectedFeatherCount} / {TotalFeatherCount} ({GetPercentageCollected()}%)";
        }

        private int GetPercentageCollected()
        {
            if (TotalFeatherCount == 0)
            {
                return 0;
            }

            return Mathf.FloorToInt(((float) CollectedFeatherCount / TotalFeatherCount) * 100);
        }
    }
}
