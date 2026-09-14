using System.Collections.Generic;
using UnityEngine;

namespace LumenRush
{
    public sealed class LevelGenerator : MonoBehaviour
    {
        sealed class Segment
        {
            public Transform Root;
            public readonly List<TrackItem> Items = new List<TrackItem>();
        }

        readonly List<Segment> segments = new List<Segment>();
        GameManager game;
        ObjectPoolManager pool;
        int seed, row;
        float Length => game.Config.rowSpacing * game.Config.rowsPerSegment;
        public string District => "NEON METRO / AFTERLIGHT LINE";
        public void Initialize(GameManager game)
        {
            this.game = game;
            pool = new ObjectPoolManager(new GameObject("Item pool").transform);
            var floor = new GameObject("Continuous controller ground");
            floor.transform.position = new Vector3(0, -.3f, 0);
            var collider = floor.AddComponent<BoxCollider>();
            collider.size = new Vector3(12, .6f, 40);
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = .008f;
            RenderSettings.fogColor = new Color(.065f, .13f, .19f);
            RenderSettings.ambientLight = new Color(.4f, .52f, .65f);
            var sun = new GameObject("Moon - single directional light").AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.color = new Color(.65f, .8f, 1);
            sun.intensity = 1.3f;
            sun.shadows = LightShadows.Soft;
            sun.transform.rotation = Quaternion.Euler(40, -35, 0);
            for (int i = 0; i < game.Config.segmentCount; i++)
            {
                var s = new Segment{Root = new GameObject("Modular city section " + i).transform};
                s.Root.SetParent(transform, false);
                BuildScenery(s.Root, i);
                s.Root.gameObject.AddComponent<SceneryBatch>().Combine();
                segments.Add(s);
            }

            ResetWorld();
        }

        void BuildScenery(Transform t, int id)
        {
            Visuals.Box("Asphalt", t, new Vector3(0, -.18f, Length / 2), new Vector3(9, .3f, Length), Visuals.Road);
            foreach (float x in new[]{-4.7f, 4.7f})
            {
                Visuals.Box("Raised sidewalk", t, new Vector3(x, .1f, Length / 2), new Vector3(.6f, .4f, Length), Visuals.Dark);
                Visuals.Box("Guide light", t, new Vector3(x, .33f, Length / 2), new Vector3(.055f, .055f, Length), Visuals.Glow);
            }

            for (float z = 0; z < Length; z += 6)
                foreach (float x in new[]{-1.35f, 1.35f})
                    Visuals.Box("Lane dash", t, new Vector3(x, .005f, z), new Vector3(.065f, .02f, 2.5f), Visuals.Glass);
            var random = new System.Random(id * 57 + 9);
            for (float z = 8; z < Length; z += 18)
                foreach (int side in new[]{-1, 1})
                {
                    float h = 10 + random.Next(22), x = side * (8 + random.Next(5));
                    Visuals.Box("Building", t, new Vector3(x, h / 2, z), new Vector3(5, h, 12), Visuals.Dark);
                    for (float y = 3; y < h; y += 3)
                        Visuals.Box("Window ribbon", t, new Vector3(x - side * 2.52f, y, z), new Vector3(.05f, .7f, 9), id % 2 == 0 ? Visuals.Glass : Visuals.Teal);
                    Visuals.Box("Light mast", t, new Vector3(side * 4.8f, 3, z), new Vector3(.12f, 6, .12f), Visuals.Dark);
                    Visuals.Box("Light blade", t, new Vector3(side * 4.3f, 6, z), new Vector3(1.2f, .12f, .3f), Visuals.Glow);
                }

            if (id % 3 == 1)
            {
                Visuals.Box("Sky bridge", t, new Vector3(0, 7, Length * .65f), new Vector3(26, 1.8f, 5), Visuals.Dark);
                Visuals.Box("Bridge neon", t, new Vector3(0, 6.15f, Length * .65f - 2.55f), new Vector3(14, .12f, .1f), Visuals.Glow);
            }
        }

        public void ResetWorld()
        {
            seed = Random.Range(1, 1000000);
            row = 0;
            for (int i = 0; i < segments.Count; i++)
            {
                segments[i].Root.localPosition = Vector3.forward * (i * Length - 18);
                Populate(segments[i], i == 0);
            }
        }

        void Populate(Segment s, bool first = false)
        {
            foreach (var item in s.Items)
                pool.Return(item);
            s.Items.Clear();
            for (int r = 0; r < game.Config.rowsPerSegment; r++, row++)
            {
                float z = r * game.Config.rowSpacing + 9;
                int safe = RunRules.SafeLane(seed, row);
                if (!(first && r < 3))
                    for (int lane = -1; lane <= 1; lane++)
                        if (lane != safe)
                        {
                            ItemKind[] kinds = {ItemKind.Barrier, ItemKind.Gate, ItemKind.Cargo, ItemKind.Vehicle, ItemKind.Pipe};
                            var kind = kinds[(row + lane + 6) % kinds.Length];
                            s.Items.Add(pool.Take(kind, s.Root, new Vector3(lane * RunRules.LaneWidth, 0, z)));
                        }

                for (int c = 0; c < 5; c++)
                    s.Items.Add(pool.Take(ItemKind.Coin, s.Root, new Vector3(safe * RunRules.LaneWidth, 1, z + c * 1.5f)));
                if (row % 5 == 4)
                {
                    var p = pool.Take(ItemKind.Power, s.Root, new Vector3(safe * RunRules.LaneWidth, 1.3f, z + 8));
                    p.Power = (PowerKind)((row / 5) % 9);
                    s.Items.Add(p);
                }
            }
        }

        public void ClearNearPlayer()
        {
            foreach (var s in segments)
                foreach (var item in s.Items)
                    if (Mathf.Abs(item.transform.position.z) < 12 && item.Kind != ItemKind.Coin)
                        item.gameObject.SetActive(false);
        }

        void Update()
        {
            if (game == null || game.State != RunState.Running)
                return;
            float delta = game.Speed * Time.deltaTime;
            foreach (var s in segments)
            {
                s.Root.position += Vector3.back * delta;
                foreach (var item in s.Items)
                    if (item.gameObject.activeSelf)
                    {
                        item.Check(game);
                        if (game.State != RunState.Running)
                            break;
                    }

                if (game.State != RunState.Running)
                    break;
                if (s.Root.position.z < -Length - 20)
                {
                    float max = float.MinValue;
                    foreach (var other in segments)
                        max = Mathf.Max(max, other.Root.position.z);
                    s.Root.position = Vector3.forward * (max + Length);
                    Populate(s);
                }
            }
        }
    }
}
