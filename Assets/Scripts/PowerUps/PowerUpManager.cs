using UnityEngine;

namespace LumenRush
{
    public enum PowerKind
    {
        Magnet,
        ScoreBooster,
        Shield,
        SpeedBurst,
        DoubleCoins,
        AirDash,
        HoverBoard,
        SlowMotion,
        Invincibility
    }

    public sealed class PowerUpManager
    {
        readonly float[] timers = new float[9];
        readonly GameManager game;
        public PowerUpManager(GameManager game)
        {
            this.game = game;
        }

        public bool Active(PowerKind kind) => timers[(int)kind] > 0;
        public float Remaining(PowerKind kind) => timers[(int)kind];
        public void Activate(PowerKind kind)
        {
            timers[(int)kind] = game.Config.powerDuration + game.Save.Data.upgrade;
            game.Powers++;
            game.Audio.PlayTone(880, .3f);
        }

        public void Tick(float dt)
        {
            for (int i = 0; i < timers.Length; i++)
                timers[i] = Mathf.Max(0, timers[i] - dt);
        }

        public void Reset()
        {
            System.Array.Clear(timers, 0, timers.Length);
        }

        public bool Absorb()
        {
            if (Active(PowerKind.Invincibility) || Active(PowerKind.SpeedBurst) || Active(PowerKind.AirDash))
                return true;
            foreach (var p in new[]{PowerKind.Shield, PowerKind.HoverBoard})
                if (Active(p))
                {
                    timers[(int)p] = 0;
                    return true;
                }

            return false;
        }
    }
}
