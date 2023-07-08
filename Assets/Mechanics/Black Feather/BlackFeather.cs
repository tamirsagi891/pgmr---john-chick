using Elad.Events;
using Managers;
using UnityEngine;
using UnityEngine.Events;
using static Mechanics.Enemies.CorotuineUtils;

namespace Mechanics.Black_Feather
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class BlackFeather : Collectable
    {
        [Header("Black Feather")]
        [SerializeField]
        [Min(0.05f)]
        private float shakeTime = 3f;

        [SerializeField]
        private float shakeIntensity = 5f;
        
        [SerializeField]
        private UnityEvent onCollectFeather;
        
        protected override void CollectableFunction()
        {
            base.CollectableFunction();
            onCollectFeather.Invoke();
            var shake = CameraManager.CurrentVirtualCamara.gameObject.GetComponent<CamaraShake>();
            shake.DoShake(shakeTime: shakeTime, shakeIntensity: shakeIntensity);
            
            StartCoroutine(DelayExecution(shakeTime - 0.05f,
                    () => { BossEvents.BossStart.Invoke(); }
                )
            );
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}