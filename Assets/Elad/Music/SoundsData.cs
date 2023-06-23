using FMODUnity;
using UnityEngine;

namespace Elad.Music
{
    [CreateAssetMenu(fileName = "SoundsData", menuName = "SoundsData", order = 1)]
    public class SoundsData : ScriptableObject
    {
        
        [SerializeField] public EventReference collectFeatherSound;

    }
}
