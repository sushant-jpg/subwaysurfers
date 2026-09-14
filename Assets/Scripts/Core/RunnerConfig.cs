using UnityEngine;

namespace LumenRush
{
    [CreateAssetMenu(menuName = "Lumen Rush/Runner configuration")]
    public sealed class RunnerConfig : ScriptableObject
    {
        public float laneResponse = 15, jumpHeight = 2.6f, gravity = 26, slideDuration = .8f;
        public float powerDuration = 8;
        public int segmentCount = 6, rowsPerSegment = 4;
        public float rowSpacing = 18;
        public Color cyan = new Color(.1f, .93f, .85f);
        public Color amber = new Color(1f, .57f, .18f);
    }
}
