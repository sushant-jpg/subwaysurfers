using UnityEngine;

namespace LumenRush
{
    public enum ItemKind
    {
        Coin,
        Barrier,
        Gate,
        Cargo,
        Vehicle,
        Power,
        Pipe
    }

    public sealed class TrackItem : MonoBehaviour
    {
        public ItemKind Kind;
        public PowerKind Power;
        public int Lane;
        public float PreviousZ;
        public void Check(GameManager game)
        {
            float z = transform.position.z;
            float x = transform.position.x - game.Player.transform.position.x;
            if (Kind == ItemKind.Coin)
            {
                bool magnet = game.PowerUps.Active(PowerKind.Magnet) && Mathf.Abs(z) < 10;
                if (magnet)
                    transform.position = Vector3.MoveTowards(transform.position, game.Player.transform.position + Vector3.up, Time.deltaTime * 24);
                if ((magnet && Mathf.Abs(z) < 1.2f) || (RunRules.Crosses(PreviousZ, z, .8f) && Mathf.Abs(x) < .8f && Mathf.Abs(transform.position.y - (game.Player.Height + 1)) < 1.4f))
                {
                    game.CollectCoin();
                    gameObject.SetActive(false);
                }
                else
                    transform.Rotate(0, 120 * Time.deltaTime, 0, Space.Self);
            }
            else if (RunRules.Crosses(PreviousZ, z, Kind == ItemKind.Vehicle ? 2.7f : .75f) && Mathf.Abs(x) < 1)
            {
                if (Kind == ItemKind.Power)
                {
                    game.PowerUps.Activate(Power);
                    gameObject.SetActive(false);
                }
                else
                {
                    bool clear = game.PowerUps.Active(PowerKind.AirDash) || (Kind == ItemKind.Barrier && game.Player.Height > 1.15f) || (Kind == ItemKind.Pipe && game.Player.Height > .65f) || (Kind == ItemKind.Gate && game.Player.Sliding);
                    if (!clear)
                        game.Hit(Kind != ItemKind.Pipe);
                    gameObject.SetActive(false);
                }
            }

            PreviousZ = z;
        }
    }
}
