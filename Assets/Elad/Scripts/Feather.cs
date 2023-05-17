using UnityEngine;

namespace Elad.Scripts
{
    public class Feather : MonoBehaviour
    {
        [SerializeField] private FeathersManager.FeatherKind myFeatherKind;
        
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerController pC = other.GetComponent<PlayerController>();
            if (pC)
            {
                Damageable dam = other.GetComponent<Damageable>();
                characterEvents.AddFeather.Invoke(myFeatherKind);
                Destroy(gameObject);
            }
        }
    }
}
