using System;
using System.Collections;
using System.Collections.Generic;
using Avrahamy;
using BitStrap;
using Nemesh.Attributes;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Logger = Nemesh.Logger;

namespace Mechanics.Slideshow
{
    [AddComponentMenu("Slideshow/Slides Manager")]
    public class SlidesManager : MonoBehaviour
    {
        #region Inspector

        [SerializeField]
        private List<Slide> slides = new() { new Slide() };

        [SerializeField]
        private bool startShowOnStart;

        [SerializeField]
        private PassiveTimer timeBetweenSlides = new(0.75f);

        [Space]
        [Header("References")]
        [RequiredReference]
        [SerializeField]
        private Image image;

        [RequiredReference]
        [SerializeField]
        private TMP_Text text;

        [RequiredReference]
        [SerializeField]
        private Button continueButton;

        [Space]
        [SerializeField]
        [InspectorFieldName("Events On All Slides")]
        public SlideshowEvents events;

        #endregion

        #region Public Properties

        public bool SlideshowRolling
        {
            get => _slideshowRolling;
            set { _slideshowRolling = value; }
        }

        public int CurrentSlide
        {
            get => _currentSlide;
            set => _currentSlide = value;
        }

        #endregion

        #region Private Fields

        private bool _slideshowRolling;

        private int _currentSlide = -1;
        private int _slideToSwitchTo;

        #endregion

        #region Public Methods

        [Button("Restart Slideshow")]
        public void StartSlideshow(int slideToStart = 0)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif
            CurrentSlide = -1;
            SlideshowRolling = true;
            NextSlide();
        }

        [Button("Go To Next Slide")]
        public void EndSlideAndGoToNext()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif
            if (SlideshowRolling)
            {
                var slide = slides[_currentSlide % slides.Count];
                EndSlide(slide);
            }

            NextSlide();
        }

        [Button]
        public void PauseSlide() // TODO: move both pause and continue to SlideshowRolling getter.
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif
            if (SlideshowRolling)
            {
                var slide = slides[CurrentSlide % slides.Count];
                SlideshowRolling = false;
                slide.PauseTimer();
                Logger.Log($"Pause slide {slide.name}");
            }
        }

        [Button]
        public void ContinueSlide()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif
            if (SlideshowRolling || CurrentSlide == -1)
            {
                return;
            }

            var slide = slides[CurrentSlide % slides.Count];
            slide.ResumeTimer();
            SlideshowRolling = true;
            Logger.Log($"Continue slide {slide.name}");
        }

        #endregion

        #region MonoBehaviour

        private void Start()
        {
            if (startShowOnStart)
            {
                StartSlideshow();
            }
        }

        private void Update()
        {
            if (!SlideshowRolling)
            {
                return;
            }

            if (timeBetweenSlides.IsSet)
            {
                if (!timeBetweenSlides.IsActive)
                {
                    timeBetweenSlides.Clear();
                    StartSlideHelper(_slideToSwitchTo);
                }
                else
                {
                    return;
                }
            }

            var slide = slides[CurrentSlide];
            if (slide.TimeUp)
            {
                Logger.Log($"Slide {slide.name} TimeUp");
                EndSlideAndGoToNext(slide);
            }
        }

        #endregion

        #region Private Methods

        private void StartSlide(int slideNumber)
        {
            Logger.Log($"Starting to time between slides {slideNumber}");
            _slideToSwitchTo = slideNumber;
            timeBetweenSlides.Start();
        }

        private void StartSlideHelper(int slideNumber)
        {
            slideNumber %= slides.Count;

            CurrentSlide = slideNumber;

            SlideshowRolling = true;

            var slide = slides[slideNumber];
            Logger.Log($"Starting to slide {slide.name}");

            text.text = slide.text;
            image.sprite = slide.image != null ? slide.image : null;

            // TODO: combine into a function of slide called StartSlide or smthg
            slide.events.onSlideStart.Invoke();
            slide.StartTimer();

            events.onSlideStart.Invoke();
        }

        private bool NextSlide()
        {
            var slideNumber = CurrentSlide + 1;
            if (slideNumber >= slides.Count)
            {
                Logger.Log($"Slideshow Ended");
                events.onSlideshowEnd.Invoke();
                return false;
            }

            if (slideNumber == 0)
            {
                Logger.Log($"Slideshow Started");
                events.onSlideshowStart.Invoke();
            }

            StartSlide(slideNumber);
            return true;
        }

        private void EndSlideAndGoToNext(Slide slide)
        {
            EndSlide(slide);
            NextSlide();
        }

        private void EndSlide(Slide slide)
        {
            slide.events.onSlideEnd.Invoke();
            slide.ClearTimer();
            events.onSlideEnd.Invoke();
        }

        #endregion
    }

    [Serializable]
    public struct SlideshowEvents
    {
        public UnityEvent onSlideshowStart;
        public UnityEvent onSlideshowEnd;
        public UnityEvent onSlideStart;
        public UnityEvent onSlideEnd;
    }
}