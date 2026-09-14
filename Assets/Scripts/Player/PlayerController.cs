using UnityEngine;

namespace LumenRush
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerController : MonoBehaviour
    {
        CharacterController controller;
        GameManager game;
        Transform model, leftLeg, rightLeg, leftArm, rightArm;
        Renderer jacket;
        float velocity, slide, phase;
        int lane;
        public float Height => transform.position.y;
        public bool Sliding => slide > 0;
        public void Initialize(GameManager game)
        {
            this.game = game;
            controller = GetComponent<CharacterController>();
            controller.radius = .34f;
            controller.height = 1.8f;
            controller.center = Vector3.up * .9f;
            controller.minMoveDistance = 0;
            model = new GameObject("Replaceable animated model").transform;
            model.SetParent(transform, false);
            jacket = Visuals.Box("Courier jacket", model, new Vector3(0, 1.15f, 0), new Vector3(.65f, .75f, .4f), Visuals.Teal).GetComponent<Renderer>();
            Visuals.Box("Backpack", model, new Vector3(0, 1.17f, -.29f), new Vector3(.46f, .58f, .22f), Visuals.Dark);
            Visuals.Sphere("Helmet", model, new Vector3(0, 1.79f, 0), new Vector3(.48f, .5f, .48f), Visuals.Dark);
            Visuals.Box("Visor", model, new Vector3(0, 1.82f, .23f), new Vector3(.4f, .11f, .05f), Visuals.Glow);
            leftLeg = Limb("Left leg", new Vector3(-.19f, .77f, 0), new Vector3(.23f, .7f, .25f), Visuals.Dark);
            rightLeg = Limb("Right leg", new Vector3(.19f, .77f, 0), new Vector3(.23f, .7f, .25f), Visuals.Dark);
            leftArm = Limb("Left arm", new Vector3(-.43f, 1.48f, 0), new Vector3(.2f, .65f, .2f), Visuals.Teal);
            rightArm = Limb("Right arm", new Vector3(.43f, 1.48f, 0), new Vector3(.2f, .65f, .2f), Visuals.Teal);
            game.Input.Lane += MoveLane;
            game.Input.Jump += Jump;
            game.Input.Slide += Slide;
        }

        Transform Limb(string name, Vector3 p, Vector3 size, Material m)
        {
            var t = new GameObject(name).transform;
            t.SetParent(model, false);
            t.localPosition = p;
            Visuals.Box("Mesh", t, Vector3.down * size.y * .5f, size, m);
            return t;
        }

        public void ResetRunner()
        {
            controller.enabled = false;
            transform.position = Vector3.zero;
            controller.enabled = true;
            lane = 0;
            velocity = 0;
            slide = 0;
            model.localScale = Vector3.one;
            jacket.sharedMaterial = game.Save.Data.character == 0 ? Visuals.Teal : game.Save.Data.character == 1 ? Visuals.Orange : Visuals.Purple;
        }

        public void MoveLane(int direction)
        {
            if (game.State == RunState.Running)
                lane = RunRules.ClampLane(lane + direction);
        }

        public void Jump()
        {
            if (game.State != RunState.Running || Height > .08f)
                return;
            slide = 0;
            velocity = Mathf.Sqrt(2 * game.Config.gravity * game.Config.jumpHeight);
            game.Jumps++;
            game.Audio.PlayTone(420, .12f);
        }

        public void Slide()
        {
            if (game.State != RunState.Running)
                return;
            if (Height > .1f)
                velocity = -16;
            slide = game.Config.slideDuration;
            game.Audio.PlayTone(130, .13f);
        }

        void Update()
        {
            if (game == null || game.State != RunState.Running)
                return;
            float dt = Time.deltaTime;
            slide = Mathf.Max(0, slide - dt);
            controller.height = Sliding ? .8f : 1.8f;
            controller.center = Vector3.up * controller.height * .5f;
            if (controller.isGrounded && velocity < 0)
                velocity = -2;
            velocity -= game.Config.gravity * dt;
            float x = Mathf.Lerp(transform.position.x, lane * RunRules.LaneWidth, 1 - Mathf.Exp(-game.Config.laneResponse * dt));
            controller.Move(new Vector3(x - transform.position.x, velocity * dt, 0));
            phase += dt * game.Speed * .8f;
            float swing = Height > .1f ? 15 : Mathf.Sin(phase) * 38;
            leftLeg.localRotation = Quaternion.Euler(swing, 0, 0);
            rightLeg.localRotation = Quaternion.Euler(-swing, 0, 0);
            leftArm.localRotation = Quaternion.Euler(-swing, 0, -8);
            rightArm.localRotation = Quaternion.Euler(swing, 0, 8);
            model.localScale = Vector3.Lerp(model.localScale, new Vector3(1, Sliding ? .42f : 1, 1), dt * 18);
            model.localRotation = Quaternion.Euler(Sliding ? 20 : 0, 0, (lane * RunRules.LaneWidth - transform.position.x) * -5);
        }
    }
}
