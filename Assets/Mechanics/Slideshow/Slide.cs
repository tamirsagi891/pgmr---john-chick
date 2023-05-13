using System;
using System.Collections.Generic;
using Avrahamy;
using Avrahamy.EditorGadgets;
using UnityEngine;
using UnityEngine.Events;
using Logger = Nemesh.Logger;

namespace Mechanics.Slideshow
{
    [Serializable]
    public class Slide
    {
        public string name;

        [TextArea(2, 5)]
        public string text;

        [Space]
        public bool nextOnTime;

        [Min(0)]
        public float time;

        [Space]
        public Sprite image;

        [Space]
        public SlideEvents events = new();

        private float _elapsedTime;
        private PassiveTimer _timer;

        public Slide()
        {
            name = "Slide 1";
            time = 2;
            _timer = new PassiveTimer(2f);
            text = "This will be used as subtitles.";
            nextOnTime = true;
        }

        public void ClearTimer()
        {
            _timer.Clear();
            _elapsedTime = 0f;
        }

        public void StartTimer()
        {
            _timer.Start(time);
            _elapsedTime = 0;
        }

        public float PauseTimer()
        {
            _elapsedTime = _timer.ElapsedTime;
            _timer.Clear();
            Logger.Log($"{_elapsedTime}  {TimeUp}");
            return _elapsedTime;
        }

        public float ResumeTimer()
        {
            _timer.Start(time);
            _timer.ElapsedTime = _elapsedTime;
            Logger.Log($"{_timer.ElapsedTime}  {TimeUp}  {_timer.IsActive}");
            return _elapsedTime;
        }

        public bool TimeUp => nextOnTime && _timer.IsSet && !_timer.IsActive;
    }

    [Serializable]
    public class SlideEvents
    {
        public UnityEvent onSlideStart = new();
        public UnityEvent onSlideEnd = new();
    }
}