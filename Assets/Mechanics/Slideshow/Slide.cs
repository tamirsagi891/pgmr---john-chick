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
        public Sprite image;

        [Space]
        public SlideEvents events = new();

        public Slide()
        {
            name = "Slide 1";
            text = "This will be used as subtitles.";
        }
        
    }

    [Serializable]
    public class SlideEvents
    {
        public UnityEvent onSlideStart = new();
        public UnityEvent onSlideEnd = new();
    }
}