using System;
using System.Collections.Generic;
using BitStrap;
using Elad.Scripts;
using TMPro;
using UnityEngine;
using Logger = Nemesh.Logger;

namespace Mechanics.UI.Menus.Menu_Utils
{
    public class EndScoreManager : MonoBehaviour
    {
        [SerializeField]
        [RequiredReference]
        private TMP_Text scoreText;

        [SerializeField]
        private List<LetterGrade> grades;
        
        private int _currentDisplayed;

        private int Total => PlayerStatus.FeathersToCollectManager.StartFeatherAmount;

        public int CurrentDisplayed
        {
            get => _currentDisplayed;
            set
            {
                if (value > _currentDisplayed)
                {
                    IncrementScore(value);
                }
                else
                {
                    DecrementScore(value);
                }
            }
        }
        
        private void DecrementScore(int value)
        {
            var percent = value / (float)Total;
            for (var i = grades.Count - 1; i >= 0; i--)
            {
                var grade = grades[i];
                if (grade.percent <= percent)
                {
                    scoreText.text = grade.text;
                    break;
                }
            }

            _currentDisplayed = value;
        }

        private void IncrementScore(int value)
        {
            var oldPercent = _currentDisplayed / (float)Total;
            var percent = value / (float)Total;
            foreach (var grade in grades)
            {
                if (grade.percent >= oldPercent && percent >= grade.percent)
                {
                    scoreText.text = grade.text;
                }
            }

            _currentDisplayed = value;
        }
    }

    [Serializable]
    public struct LetterGrade
    {
        public string text;
        public float percent;
    }
}