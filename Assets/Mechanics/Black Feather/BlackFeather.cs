using Elad.Events;
using Managers;
using UnityEngine;
using UnityEngine.Events;
using static Mechanics.Enemies.CorotuineUtils;

namespace Mechanics.Black_Feather
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class BlackFeather : MonoBehaviour
    {
        [Header("Black Feather")] private bool _gotHit;

        public bool GotHit
        {
            get => _gotHit;
            set => _gotHit = value;
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(TagStrings.playerTag) && !GotHit)
            {
                GetComponent<SpriteRenderer>().enabled = false;
                GotHit = true;
            }
        }


        // protected override void CollectableFunction()
        // {
        //     base.CollectableFunction();
        //     var shake = CameraManager.CurrentVirtualCamara.gameObject.GetComponent<CamaraShake>();
        //     shake.DoShake(shakeTime: shakeTime, shakeIntensity: shakeIntensity);
        //     
        //     StartCoroutine(DelayExecution(shakeTime - 0.05f,
        //             () => { BossEvents.BossStart.Invoke(); }
        //         )
        //     );
        //     GetComponent<SpriteRenderer>().enabled = false;
        // }
    }
}