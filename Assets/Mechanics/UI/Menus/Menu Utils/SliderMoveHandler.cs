using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Mechanics.UI.Menus.Menu_Utils
{

    [RequireComponent(typeof(Slider))]
    public class SliderMoveHandler : MonoBehaviour, IMoveHandler, IEndDragHandler
    {
        [SerializeField]
        [Tooltip("The desired step size")]
        public float step = 0.1f;

        private Slider _slider;
        private float _previousSliderValue;

        private void Awake()
        {
            _slider = GetComponent<Slider>();
            if (_slider)
            {
                _previousSliderValue = _slider.value;
            }
        }

        public void OnMove(AxisEventData eventData)
        {
            // override the slider value using our previousSliderValue and the desired step
            if (eventData.moveDir == MoveDirection.Left)
            {
                _slider.value = _previousSliderValue - step;
            }

            if (eventData.moveDir == MoveDirection.Right)
            {
                _slider.value = _previousSliderValue + step;
            }

            // keep the slider value for future use
            _previousSliderValue = _slider.value;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            // keep the last slider value if the slider was dragged by mouse
            _previousSliderValue = _slider.value;
        }
    }
}
