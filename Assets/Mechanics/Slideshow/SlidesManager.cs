using System;
using System.Collections.Generic;
using Avrahamy;
using BitStrap;
using Nemesh.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
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
        
        
        public void EndSlideAndGoToNextImmediate()
        {
            if (SlideshowRolling && CurrentSlide != -1)
            {
                var slide = slides[CurrentSlide % slides.Count];
                EndSlide(slide);
            }

            NextSlide(true);
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
            if (SlideshowRolling && CurrentSlide != -1)
            {
                var slide = slides[CurrentSlide % slides.Count];
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
                if (timeBetweenSlides.IsSet && timeBetweenSlides.IsActive)
                {
                    // TODO: pause this instead
                    Logger.Log("Cant pause while transition for now, WIP", Color.yellow);
                    return;
                }

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
            if (SlideshowRolling)
            {
                return;
            }

            // TODO: Continue when transition is paused
            // if (timeBetweenSlides.IsSet && timeBetweenSlides.IsActive)
            // {
            // }
            var slide = slides[CurrentSlide % slides.Count];
            slide.ResumeTimer();
            SlideshowRolling = true;
            Logger.Log($"Continue slide {CurrentSlide} : {slide.name}");
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
                Logger.Log($"Slide {CurrentSlide} : {slide.name} TimeUp");
                EndSlideAndGoToNext(slide);
            }
        }

        #endregion

        #region Private Methods

        private void StartSlide(int slideNumber)
        {
            SlideshowRolling = true;
            _slideToSwitchTo = slideNumber;
            if (timeBetweenSlides.IsSet && timeBetweenSlides.IsActive)
            {
                Logger.Log($"Switching next slide to slide {slideNumber} : {slides[slideNumber].name}");
                return;
            }
            Logger.Log($"Starting time to go to slide {slideNumber} : {slides[slideNumber].name}");
            timeBetweenSlides.Start();
        }

        private void StartSlideHelper(int slideNumber)
        {
            slideNumber %= slides.Count;

            CurrentSlide = slideNumber;

            SlideshowRolling = true;

            var slide = slides[slideNumber];
            Logger.Log($"Starting to transition to slide {slideNumber} : {slide.name}");

            text.text = slide.text;
            image.sprite = slide.image != null ? slide.image : null;

            // TODO: combine into a function of slide called StartSlide or smthg
            slide.events.onSlideStart.Invoke();
            slide.StartTimer();
            events.onSlideStart.Invoke();
        }
        
        private bool NextSlide(bool immediate = false)
        {
            var slideNumber = CurrentSlide + 1;
            if (timeBetweenSlides.IsSet && timeBetweenSlides.IsActive)
            {
                slideNumber = _slideToSwitchTo + 1;
            }

            if (slideNumber >= slides.Count)
            {
                Logger.Log($"Slideshow Ended");
                CurrentSlide = -1;
                events.onSlideshowEnd.Invoke();
                SlideshowRolling = false;
                return false;
            }

            if (slideNumber == 0)
            {
                Logger.Log($"Slideshow Started");
                immediate = true;
                events.onSlideshowStart.Invoke();
            }

            if (immediate)
            {
                StartSlideHelper(slideNumber);
            }
            else
            {
                StartSlide(slideNumber);
            }
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