using UnityEngine;

namespace LumenRush
{
    public sealed class SafeArea : MonoBehaviour
    {
        Rect last;
        Vector2Int size;
        void Update()
        {
            Rect safe = Screen.safeArea;
            if (last == safe && size.x == Screen.width && size.y == Screen.height)
                return;
            last = safe;
            size = new Vector2Int(Screen.width, Screen.height);
            var rect = (RectTransform)transform;
            rect.anchorMin = new Vector2(safe.x / Screen.width, safe.y / Screen.height);
            rect.anchorMax = new Vector2(safe.xMax / Screen.width, safe.yMax / Screen.height);
            rect.offsetMin = rect.offsetMax = Vector2.zero;
        }
    }
}
