using Avrahamy.EditorGadgets;
using BitStrap;
using UnityEngine;

namespace Nemesh.ShaderAccessors
{
    public class SetRandomSeedOnStart : MonoBehaviour
    {

        #region Public Static Properties

        #endregion

        #region Private Static

        private static readonly int RandomSeed = Shader.PropertyToID("_Random_Seed");

        #endregion

        #region Inspector

        [Header("Random Seed")]
        [SerializeField]
        private bool useManuallyEnteredSeed;
        
        [ConditionalHide("useManuallyEnteredSeed")]
        [SerializeField]
        private Vector2 seed;
        
        [SerializeField]
        private Renderer rendererToRandomize;

        [SerializeField]
        private int matNumber;

        [Space]
        [Header("Current Parameters:")]
        [SerializeField]
        [BitStrap.ReadOnly]
        private bool hasRandomSeedVectorInMaterial;
        
        [ConditionalHide("hasRandomSeedVectorInMaterial")]
        [SerializeField]
        [BitStrap.ReadOnly]
        private Vector2 currentSeed;

        [Header("Debug")]
        [SerializeField]
        [Tooltip("Should debug functions be used (rays, logs, etc)")]
        private bool debug;


        #endregion

        #region Public Properties

        #endregion

        #region Private Fields

        private Material _myMaterial;

        #endregion

        #region MonoBehaviour

        public void Awake()
        {
            if (rendererToRandomize == null && !TryGetComponent(out rendererToRandomize))
            {
                Logger.LogWarning("Missing Renderer!", this);
                return;
            }
            if (matNumber == 0)
            {
                _myMaterial = rendererToRandomize.material;
            }
            else
            {
                var materials = rendererToRandomize.materials;
                _myMaterial = materials.Length > matNumber ? materials[matNumber] : rendererToRandomize.material;
            }
            
            hasRandomSeedVectorInMaterial = _myMaterial.HasVector(RandomSeed);
            ResetSeed();
        }

        #endregion

        #region Public Methods
        
        [Button]
        public void ResetSeed()
        {
            currentSeed = useManuallyEnteredSeed ? seed : Random.insideUnitCircle;

            if (!hasRandomSeedVectorInMaterial || _myMaterial == null)
            {
                Logger.LogWarning("No Material with random seed or not in Play!", this);
                return;
            }

            if (Application.isPlaying)
            {
                _myMaterial.SetVector(RandomSeed, currentSeed);
            }
            else
            {
                Logger.LogWarning("Cant set material in Editor! because prefab stuff.", this);
            }
        }
        
        #endregion

        #region Private Methods

        #endregion

    }
}
