using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ButtonPress : Button
    {
        private bool _pressed;

        public override void OnPointerDown(PointerEventData eventData)
        {
            _pressed = true;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            _pressed = false;
        }

        public bool IsPressedNow => _pressed;
    }
}