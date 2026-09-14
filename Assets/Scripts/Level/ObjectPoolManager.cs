using System.Collections.Generic;
using UnityEngine;

namespace LumenRush
{
    public sealed class ObjectPoolManager
    {
        readonly Dictionary<ItemKind, Stack<TrackItem>> pool = new Dictionary<ItemKind, Stack<TrackItem>>();
        readonly Transform root;
        public ObjectPoolManager(Transform root)
        {
            this.root = root;
            foreach (ItemKind kind in System.Enum.GetValues(typeof(ItemKind)))
            {
                pool[kind] = new Stack<TrackItem>();
                for (int i = 0; i < (kind == ItemKind.Coin ? 180 : 24); i++)
                {
                    var item = Create(kind);
                    item.gameObject.SetActive(false);
                    pool[kind].Push(item);
                }
            }
        }

        public TrackItem Take(ItemKind kind, Transform parent, Vector3 position)
        {
            var item = pool[kind].Count > 0 ? pool[kind].Pop() : Create(kind);
            item.transform.SetParent(parent, false);
            item.transform.localPosition = position;
            item.transform.localRotation = Quaternion.identity;
            item.PreviousZ = item.transform.position.z;
            item.gameObject.SetActive(true);
            return item;
        }

        public void Return(TrackItem item)
        {
            item.gameObject.SetActive(false);
            item.transform.SetParent(root, false);
            pool[item.Kind].Push(item);
        }

        TrackItem Create(ItemKind kind)
        {
            var go = new GameObject(kind.ToString());
            go.transform.SetParent(root, false);
            var item = go.AddComponent<TrackItem>();
            item.Kind = kind;
            switch (kind)
            {
                case ItemKind.Coin:
                    var coin = Visuals.Shape(PrimitiveType.Cylinder, "Lumen token", go.transform, Vector3.zero, new Vector3(.52f, .07f, .52f), Visuals.Gold);
                    coin.transform.localRotation = Quaternion.Euler(90, 0, 0);
                    break;
                case ItemKind.Power:
                    Visuals.Sphere("Power core", go.transform, Vector3.zero, Vector3.one * .75f, Visuals.Glow);
                    Visuals.Box("Core band", go.transform, Vector3.zero, new Vector3(.9f, .12f, .9f), Visuals.Purple);
                    break;
                case ItemKind.Gate:
                    foreach (float x in new[]{-.95f, .95f})
                        Visuals.Box("Gate post", go.transform, new Vector3(x, 1.5f, 0), new Vector3(.15f, 3, .25f), Visuals.Teal);
                    Visuals.Box("SLIDE clearance", go.transform, new Vector3(0, 2, 0), new Vector3(2.1f, 1.8f, .45f), Visuals.Orange);
                    break;
                case ItemKind.Vehicle:
                    Visuals.Box("Autonomous cargo pod", go.transform, new Vector3(0, 1.2f, 0), new Vector3(1.9f, 2.4f, 5), Visuals.Teal);
                    Visuals.Box("Windshield", go.transform, new Vector3(0, 1.65f, -2.52f), new Vector3(1.6f, .85f, .04f), Visuals.Glass);
                    foreach (float x in new[]{-.65f, .65f})
                        Visuals.Box("Tail light", go.transform, new Vector3(x, .6f, -2.55f), new Vector3(.35f, .15f, .04f), Visuals.Orange);
                    break;
                case ItemKind.Cargo:
                    Visuals.Box("Cargo container", go.transform, new Vector3(0, 1.6f, 0), new Vector3(2.1f, 3.2f, 2), Visuals.Dark);
                    for (int i = -2; i <= 2; i++)
                        Visuals.Box("Container rib", go.transform, new Vector3(i * .4f, 1.6f, -1.03f), new Vector3(.07f, 3, .06f), Visuals.Teal);
                    break;
                case ItemKind.Pipe:
                    Visuals.Box("Utility pipe", go.transform, new Vector3(0, .32f, 0), new Vector3(2, .64f, .7f), Visuals.Purple);
                    break;
                default:
                    Visuals.Box("Jump barrier", go.transform, new Vector3(0, .55f, 0), new Vector3(2, 1.1f, .65f), Visuals.Orange);
                    for (int i = -2; i <= 2; i++)
                        Visuals.Box("Reflective stripe", go.transform, new Vector3(i * .38f, .62f, -.34f), new Vector3(.16f, .6f, .04f), Visuals.Gold);
                    break;
            }

            return item;
        }
    }
}
