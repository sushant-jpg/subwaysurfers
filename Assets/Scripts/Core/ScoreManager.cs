namespace LumenRush
{
    public sealed class ScoreManager
    {
        double distancePoints;
        public void Reset() => distancePoints = 0;
        public void Advance(float meters, int multiplier)
        {
            if (meters > 0)
                distancePoints += meters * System.Math.Max(1, multiplier);
        }

        public int Total(int coins) => (int)System.Math.Min(int.MaxValue, distancePoints + (double)coins * 10);
    }
}
