using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ZombieWar.UI
{
    public class ReleaseJoystick : Joystick
    {
        private bool isInputAlive = false;

        private void Update()
        {
            if (isInputAlive)
            {
                isInputAlive = false;
            }
            else
            {
                input = Vector2.zero;
            }
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (isReady)
            {
                cam = null;
                if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
                    cam = canvas.worldCamera;

                Vector2 position = RectTransformUtility.WorldToScreenPoint(cam, background.position);
                Vector2 radius = background.sizeDelta / 2;
                input = (eventData.position - position) / (radius * canvas.scaleFactor);
                FormatInput();
                HandleInput(input.magnitude, input.normalized, radius, cam);
                isInputAlive = true;
                handle.anchoredPosition = input * radius * handleRange;
                input = Vector2.zero;
            }
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (isReady)
            {
                Vector2 position = RectTransformUtility.WorldToScreenPoint(cam, background.position);
                Vector2 radius = background.sizeDelta / 2;
                input = (eventData.position - position) / (radius * canvas.scaleFactor);
                FormatInput();
                HandleInput(input.magnitude, input.normalized, radius, cam);
                isInputAlive = true;

                background.GetComponent<Image>().enabled = false;
                handle.anchoredPosition = Vector2.zero;
            }
        }
    }
}
