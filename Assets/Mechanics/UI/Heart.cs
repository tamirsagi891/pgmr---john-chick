using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum HeartStatus
{
        Empty = 0,
        Half = 1,
        Full = 2
}

public class Heart : MonoBehaviour
{
        [SerializeField] private Sprite fullHeart, halfHeart, emptyHeart;
        private Image _image;

        private void Awake()
        { 
                _image = GetComponent<Image>();
        }

        public void SetHeartImage(HeartStatus status)
        {
                if (_image is null)
                {
                        Debug.Log("No Image Found");
                }
                _image.sprite = status switch
                {
                        HeartStatus.Empty => emptyHeart,
                        HeartStatus.Half => halfHeart,
                        HeartStatus.Full => fullHeart,
                        _ => _image.sprite
                };
        }
}
