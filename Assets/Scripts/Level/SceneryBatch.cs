using System.Collections.Generic;
using UnityEngine;

namespace LumenRush
{
    public sealed class SceneryBatch : MonoBehaviour
    {
        readonly List<Mesh> owned = new List<Mesh>();
        public void Combine()
        {
            var groups = new Dictionary<Material, List<CombineInstance>>();
            var sources = GetComponentsInChildren<MeshFilter>();
            foreach (var source in sources)
            {
                var renderer = source.GetComponent<MeshRenderer>();
                if (renderer == null)
                    continue;
                if (!groups.TryGetValue(renderer.sharedMaterial, out var instances))
                {
                    instances = new List<CombineInstance>();
                    groups.Add(renderer.sharedMaterial, instances);
                }

                instances.Add(new CombineInstance{mesh = source.sharedMesh, transform = transform.worldToLocalMatrix * source.transform.localToWorldMatrix});
            }

            foreach (var group in groups)
            {
                var go = new GameObject("Batched city material");
                go.transform.SetParent(transform, false);
                var mesh = new Mesh{name = "Combined city geometry"};
                mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                mesh.CombineMeshes(group.Value.ToArray());
                owned.Add(mesh);
                go.AddComponent<MeshFilter>().sharedMesh = mesh;
                go.AddComponent<MeshRenderer>().sharedMaterial = group.Key;
            }

            foreach (var source in sources)
            {
                source.gameObject.SetActive(false);
                Destroy(source.gameObject);
            }
        }

        void OnDestroy()
        {
            foreach (var mesh in owned)
                Destroy(mesh);
        }
    }
}
