using System;
using Avrahamy;
using Avrahamy.EditorGadgets;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using Logger = Nemesh.Logger;
using Random = UnityEngine.Random;

namespace Mechanics.New_Wind
{
    [AddComponentMenu("Wind/Wind Timer")]
    [RequireComponent(typeof(WindController))]
    public class WindTimer : MonoBehaviour
    {
        [SerializeField]
        private bool randomizeStartTime = true;

        [ConditionalHide("randomizeStartTime", true, true)]
        [SerializeField]
        private float startDelay = 1f;

        [SerializeField]
        private PassiveTimer timeForAlert = new(0.5f);

        [SerializeField]
        private PassiveTimer timeBetweenBursts = new(5f);

        [SerializeField]
        private bool explodeOnFirstFrame = true;

        [Space]
        [SerializeField]
        public UnityEvent<WindController> onAlertStart;

        [SerializeField]
        public UnityEvent<WindController> onExplodeStart;

        private WindController _controllerToReportTo;
        private bool _firstFrame = true;
        private PassiveTimer _startDelayTimer;

        private void Awake()
        {
            _controllerToReportTo = GetComponent<WindController>();
        }

        private void OnEnable()
        {
            timeBetweenBursts.Start();
        }

        private void Start()
        {
            startDelay = randomizeStartTime ? Random.value : startDelay;
            _startDelayTimer = new PassiveTimer(startDelay);
            _startDelayTimer.Start();
        }

        private void Update()
        {
            if (_firstFrame)
            {
                if (DelayTimingHandler())
                {
                    return;
                }
            }

            if (!timeBetweenBursts.IsActive)
            {
                DoTimedExplosion();
            }

            if (timeForAlert.IsSet && !timeForAlert.IsActive)
            {
                timeForAlert.Clear();
                _controllerToReportTo.Explode();
                onExplodeStart.Invoke(_controllerToReportTo);
            }
        }

        private bool DelayTimingHandler()
        {
            if (_startDelayTimer.IsSet && _startDelayTimer.IsActive)
            {
                return true;
            }

            if (explodeOnFirstFrame)
            {
                DoTimedExplosion();
            }

            _firstFrame = false;
            return false;
        }

        private void DoTimedExplosion()
        {
            timeBetweenBursts.Start();
            timeForAlert.Start();
            onAlertStart.Invoke(_controllerToReportTo);
        }
        
    }
}
