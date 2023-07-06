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

        [Space]
        [Header("References")]
        [RequiredReference]
        [SerializeField]
        private Image image;

        [RequiredReference]
        [SerializeField]
        private TMP_Text text;

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
            if (SlideshowRolling && CurrentSlide != -1)
            {
                var slide = slides[CurrentSlide % slides.Count];
                EndSlide(slide);
            }
            else
            {
                NextSlide();
            }

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

        #endregion

        #region Private Methods

        private void StartSlide(int slideNumber)
        {
            StartSlideHelper(slideNumber);
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
            events.onSlideStart.Invoke();
        }
        
        public bool NextSlide()
        {
            var slideNumber = CurrentSlide + 1;

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
                events.onSlideshowStart.Invoke();
            }
            
            StartSlide(slideNumber);
            
            return true;
        }
        
        private void EndSlide(Slide slide)
        {
            slide.events.onSlideEnd.Invoke();
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