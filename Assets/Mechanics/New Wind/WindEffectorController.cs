using BitStrap;
using UnityEngine;

namespace Mechanics.New_Wind
{
    
    [AddComponentMenu("Wind/Utils/Effector Controller")]
    [RequireComponent(typeof(AreaEffector2D))]
    [RequireComponent(typeof(ParticleSystemForceField))]
    public class WindEffectorController : MonoBehaviour
    {
        [SerializeField]
        [RequiredReference]
        private AreaEffector2D windEffector;  // TODO: get from code instead

        [SerializeField]
        [RequiredReference]
        private ParticleSystemForceField myForceField;

        public AreaEffector2D WindEffector
        {
            get => windEffector;
            set => windEffector = value;
        }

        public ParticleSystemForceField MyForceField
        {
            get => myForceField;
            set => myForceField = value;
        }
    }
}
