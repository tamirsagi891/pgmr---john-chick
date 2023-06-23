using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BitStrap;
using Cinemachine;
using Elad.Scripts;
using Elad.Scripts.Save_Load_System;
using Managers;
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
        private LevelScoresContainer _levelHighScore;

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

        private string _dir;

        public string HighScoreSavePath => Path.Combine(_dir, $"Level_{GeneralGameManager.CurrentSceneIndex}.json");

        private void Awake()
        {
            _dir = Path.Combine(Application.persistentDataPath, "HighScores");
            if (!Directory.Exists(_dir))
            {
                Directory.CreateDirectory(_dir);
            }

            if (!File.Exists(HighScoreSavePath))
            {
                _levelHighScore = new LevelScoresContainer(level: GeneralGameManager.CurrentSceneIndex);
                return;
            }

            var json = File.ReadAllText(HighScoreSavePath);
            _levelHighScore = JsonUtility.FromJson<LevelScoresContainer>(json);
        }

        public void SaveLevelScoreIfHighScore(LevelScore score)
        {
            SaveScores(score);
        }

        private void SaveScores(LevelScore score)
        {
            _levelHighScore.entries.Add(score);
            _levelHighScore.entries.Sort(LevelScore.LevelScoreComparator);
            if (_levelHighScore.entries.Count > LevelScoresContainer.SaveCount)
            {
                _levelHighScore.entries = _levelHighScore.entries.GetRange(0, 5);
            }

            if (!_levelHighScore.entries.Contains(score))
            {
                return;
            }

            Logger.Log($"Set new HighScore! Place: {_levelHighScore.entries.IndexOf(score)} " +
                       $"Saved scores to {HighScoreSavePath}", Color.green);
            var json = JsonUtility.ToJson(_levelHighScore, true);
            File.WriteAllText(HighScoreSavePath, json);
        }

        private void DecrementScore(int value)
        {
            var percent = value / (float) Total;
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
            var oldPercent = _currentDisplayed / (float) Total;
            var percent = value / (float) Total;
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

    [Serializable]
    public struct LevelScore
    {
        public int deathCount;
        public float completionTime;
        public long completionDate;
        public int feathersCollected;
        public int totalFeathers;
        public int level;
        public string player;
        
        public override string ToString()
        {
            return $"By: {player} || Level {level}| " +
                   $"Time: {TimeSpan.FromSeconds(completionTime):m\\:ss\\.ff}| " +
                   $"Feathers: {feathersCollected}/{totalFeathers}| " +
                   $"Deaths: {deathCount}| Date: {DateTime.FromBinary(completionDate)}";
        }

        public static int LevelScoreComparator(LevelScore a, LevelScore b)
        {
            // First, compare by completion time (ascending order)
            var compareTime = a.completionTime.CompareTo(b.completionTime);
            if (compareTime != 0)
            {
                return compareTime;
            }

            // If completion dates are also equal, compare by feathers collected (descending order)
            var compareFeathers = b.feathersCollected.CompareTo(a.feathersCollected);
            if (compareFeathers != 0)
            {
                return compareFeathers;
            }

            // If completion times are equal, compare by death count (ascending order)
            var compareDeaths = a.deathCount.CompareTo(b.deathCount);
            if (compareDeaths != 0)
            {
                return compareDeaths;
            }

            // If death counts are also equal, compare by completion date (descending order)
            var dateA = DateTime.FromBinary(a.completionDate);
            var dateB = DateTime.FromBinary(b.completionDate);
            var compareDates = dateB.CompareTo(dateA);
            if (compareDates != 0)
            {
                return compareDates;
            }

            // If all previous criteria are equal, compare by level (ascending order)
            return a.level.CompareTo(b.level);
        }
    }

    [Serializable]
    public class LevelScoresContainer
    {
        public static int SaveCount = 5;
        public int level;
        public List<LevelScore> entries = new();

        public LevelScoresContainer()
        {
        }

        public LevelScoresContainer(int level)
        {
            this.level = level;
        }
    }
}
