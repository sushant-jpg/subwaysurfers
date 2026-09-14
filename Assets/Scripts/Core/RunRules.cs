using System;

namespace LumenRush
{
    public static class RunRules
    {
        public const float LaneWidth = 2.7f;
        public static int ClampLane(int lane) => Math.Max(-1, Math.Min(1, lane));
        public static float Speed(float meters) => Math.Min(30f, 12f + meters / 240f);
        public static int Score(float meters, int coins, int multiplier) => (int)(meters * multiplier) + coins * 10;
        public static int UpgradeCost(int level) => 150 * (level + 1);
        public static int Level(int xp) => 1 + (int)Math.Sqrt(Math.Max(0, xp) / 250.0);
        public static int SafeLane(int seed, int row)
        {
            unchecked
            {
                uint n = (uint)(seed + row * 374761393);
                n = (n ^ (n >> 13)) * 1274126177;
                return (int)((n ^ (n >> 16)) % 3) - 1;
            }
        }

        public static bool Crosses(float previous, float current, float radius) => previous >= -radius && current <= radius;
        public static bool CanClaimDaily(string previous, DateTime utc) => !DateTime.TryParse(previous, out var date) || utc.Date > date.Date;
    }
}
