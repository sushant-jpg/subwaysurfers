using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LumenRush
{
    public sealed class SwipeInputManager : MonoBehaviour
    {
        public event Action<int> Lane;
        public event Action Jump, Slide, Pause;
        Vector2 start;
        bool tracking;
        void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape) || UnityEngine.Input.GetKeyDown(KeyCode.P))
                Pause?.Invoke();
            if (UnityEngine.Input.GetKeyDown(KeyCode.LeftArrow) || UnityEngine.Input.GetKeyDown(KeyCode.A))
                Lane?.Invoke(-1);
            if (UnityEngine.Input.GetKeyDown(KeyCode.RightArrow) || UnityEngine.Input.GetKeyDown(KeyCode.D))
                Lane?.Invoke(1);
            if (UnityEngine.Input.GetKeyDown(KeyCode.UpArrow) || UnityEngine.Input.GetKeyDown(KeyCode.Space))
                Jump?.Invoke();
            if (UnityEngine.Input.GetKeyDown(KeyCode.DownArrow) || UnityEngine.Input.GetKeyDown(KeyCode.S))
                Slide?.Invoke();
            if (UnityEngine.Input.touchCount == 0)
                return;
            Touch t = UnityEngine.Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                start = t.position;
                tracking = EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject(t.fingerId);
            }

            if (t.phase == TouchPhase.Canceled || t.phase == TouchPhase.Ended)
            {
                tracking = false;
                return;
            }

            if (!tracking)
                return;
            var delta = t.position - start;
            if (delta.magnitude < Mathf.Max(35, Screen.width * .06f))
                return;
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                Lane?.Invoke(delta.x > 0 ? 1 : -1);
            else if (delta.y > 0)
                Jump?.Invoke();
            else
                Slide?.Invoke();
            tracking = false;
        }
    }
}
