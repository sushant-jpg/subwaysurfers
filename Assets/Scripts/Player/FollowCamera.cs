using UnityEngine;

namespace LumenRush
{
    [RequireComponent(typeof(Camera))]
    public sealed class FollowCamera : MonoBehaviour
    {
        GameManager game;
        Camera view;
        Transform drone;
        public void Initialize(GameManager game)
        {
            this.game = game;
            view = GetComponent<Camera>();
            view.nearClipPlane = .1f;
            view.farClipPlane = 230;
            view.fieldOfView = 64;
            view.backgroundColor = new Color(.055f, .11f, .17f);
            view.clearFlags = CameraClearFlags.SolidColor;
            transform.position = new Vector3(0, 4.3f, -8);
            drone = new GameObject("Recovery patrol drone").transform;
            Visuals.Sphere("Drone hull", drone, Vector3.zero, new Vector3(.75f, .3f, .5f), Visuals.Dark);
            foreach (float x in new[]{-.6f, .6f})
                Visuals.Box("Rotor", drone, new Vector3(x, 0, 0), new Vector3(.65f, .05f, .25f), Visuals.Orange);
            Visuals.Sphere("Patrol eye", drone, new Vector3(0, 0, .27f), Vector3.one * .16f, Visuals.Gold);
        }

        void LateUpdate()
        {
            if (game == null)
                return;
            var target = new Vector3(game.Player.transform.position.x * .25f, 4.3f + game.Player.Height * .2f, -8);
            transform.position = Vector3.Lerp(transform.position, target, 1 - Mathf.Exp(-Time.unscaledDeltaTime * 5));
            transform.rotation = Quaternion.LookRotation(new Vector3(game.Player.transform.position.x * .2f, 1.5f, 12) - transform.position);
            view.fieldOfView = Mathf.Lerp(view.fieldOfView, game.PowerUps.Active(PowerKind.SpeedBurst) ? 76 : 64, Time.unscaledDeltaTime * 3);
            drone.gameObject.SetActive(game.Chase > 0);
            drone.position = new Vector3(-1.5f, 2 + Mathf.Sin(Time.time * 3) * .15f, -5 + game.Chase * 3);
        }
    }
}
