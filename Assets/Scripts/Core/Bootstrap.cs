using UnityEngine;

namespace LumenRush
{
    public static class Bootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Start()
        {
            if (Object.FindFirstObjectByType<GameManager>() == null)
                new GameObject("Lumen Rush - connected systems").AddComponent<GameManager>();
        }
    }
}
