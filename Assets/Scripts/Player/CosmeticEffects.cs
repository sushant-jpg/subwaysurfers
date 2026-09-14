using UnityEngine;

namespace LumenRush
{
    public sealed class CosmeticEffects : MonoBehaviour
    {
        GameManager game;
        GameObject board, shield;
        TrailRenderer trail;
        public void Initialize(GameManager game)
        {
            this.game = game;
            board = Visuals.Box("Aero deck cosmetic", transform, new Vector3(0, .09f, 0), new Vector3(.75f, .12f, 1.4f), Visuals.Purple);
            shield = Visuals.Shape(PrimitiveType.Cylinder, "Protection ring", transform, new Vector3(0, .04f, 0), new Vector3(1.4f, .015f, 1.4f), Visuals.Glow);
            var t = new GameObject("Afterglow trail");
            t.transform.SetParent(transform, false);
            t.transform.localPosition = new Vector3(0, .2f, -.4f);
            trail = t.AddComponent<TrailRenderer>();
            trail.sharedMaterial = Visuals.Glow;
            trail.time = .3f;
            trail.startWidth = .15f;
            trail.endWidth = 0;
            trail.minVertexDistance = .08f;
        }

        void LateUpdate()
        {
            if (game == null)
                return;
            board.SetActive(game.Save.Data.board == 1 || game.PowerUps.Active(PowerKind.HoverBoard));
            shield.SetActive(game.PowerUps.Active(PowerKind.Shield) || game.PowerUps.Active(PowerKind.Invincibility));
            trail.emitting = game.State == RunState.Running && game.Save.Data.trail == 1;
            // The world scrolls; supply trailing points explicitly to preserve a visible streak.
            if (trail.emitting)
            {
                trail.Clear();
                for (int i = 0; i < 12; i++)
                    trail.AddPosition(transform.position + new Vector3(Mathf.Sin(Time.time * 4 + i * .2f) * .05f, .18f, -i * .25f));
            }
        }
    }
}
