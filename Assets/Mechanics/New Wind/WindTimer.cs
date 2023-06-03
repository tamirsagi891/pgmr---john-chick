using System;
using Avrahamy;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using Logger = Nemesh.Logger;

namespace Mechanics.New_Wind
{
    [AddComponentMenu("Wind/Wind Timer")]
    [RequireComponent(typeof(WindController))]
    public class WindTimer : MonoBehaviour
    {
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
            if (explodeOnFirstFrame)
            {
                DoTimedExplosion();
            }
        }
        
        private void Update()
        {
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

        private void DoTimedExplosion()
        {
            timeBetweenBursts.Start();
            timeForAlert.Start();
            onAlertStart.Invoke(_controllerToReportTo);
        }


    }
}
