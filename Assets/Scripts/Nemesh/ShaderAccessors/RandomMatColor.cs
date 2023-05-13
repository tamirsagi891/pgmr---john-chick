using Avrahamy.EditorGadgets;
using BitStrap;
using UnityEngine;
using Avrahamy.Math;
using FloatRange = Avrahamy.Math.FloatRange;

namespace Nemesh
{
    public class RandomMatColor : MonoBehaviour
    {
        [SerializeField]
        private bool useManuallyEnteredColor;

        [ConditionalHide("useManuallyEnteredColor")]
        [SerializeField]
        private Color color = Color.grey;

        [SerializeField]
        [Min(0)]
        private int matNumber = 0;

        [SerializeField]
        [MinMaxRange(0f, 1f)]
        private FloatRange hueRange = new FloatRange(0f, 0.1f);

        [SerializeField]
        [MinMaxRange(0f, 1f)]
        private FloatRange saturationRange = new FloatRange(0f, 1f);

        [SerializeField]
        [MinMaxRange(0f, 1f)]
        private FloatRange valueRange = new FloatRange(0.85f, 1f);

        private Material _myMaterial;

        private void Awake()
        {
            var hasRenderer = TryGetComponent(out Renderer myRenderer);
            if (hasRenderer)
            {
                if (matNumber == 0)
                {
                    _myMaterial = myRenderer.material;
                }
                else
                {
                    var materials = myRenderer.materials;
                    _myMaterial = materials.Length > matNumber ? materials[matNumber] : myRenderer.material;
                }
            }

            ResetSeed();
        }

        [Button]
        public void ResetSeed()
        {
            if (_myMaterial == null)
            {
                Logger.LogWarning("No Material with random seed or not in Play!", this);
                return;
            }

            if (useManuallyEnteredColor)
            {
                _myMaterial.color = color;
                return;
            }

            if (Application.isPlaying)
            {
                _myMaterial.color = Random.ColorHSV(
                    hueRange.min,
                    hueRange.max,
                    saturationRange.min,
                    saturationRange.max,
                    valueRange.min,
                    valueRange.max
                );
            }
            else
            {
                Logger.LogWarning("Cant set material in Editor! because prefab stuff.", this);
            }
        }
    }
}
