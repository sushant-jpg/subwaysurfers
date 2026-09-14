using UnityEngine;

namespace LumenRush
{
    public static class Visuals
    {
        static Material dark, road, teal, orange, purple, glow, gold, glass;
        public static Material Dark => dark ?? (dark = Mat(new Color(.055f, .085f, .12f), .25f));
        public static Material Road => road ?? (road = Mat(new Color(.1f, .14f, .18f), .35f));
        public static Material Teal => teal ?? (teal = Mat(new Color(.06f, .65f, .63f), .4f));
        public static Material Orange => orange ?? (orange = Mat(new Color(.98f, .35f, .13f), .35f));
        public static Material Purple => purple ?? (purple = Mat(new Color(.55f, .27f, .88f), .3f));
        public static Material Glow => glow ?? (glow = Mat(new Color(.16f, 1, .89f), .4f, true));
        public static Material Gold => gold ?? (gold = Mat(new Color(1, .73f, .17f), .65f, true));
        public static Material Glass => glass ?? (glass = Mat(new Color(.2f, .39f, .48f), .8f));
        static Material Mat(Color c, float smooth, bool emission = false)
        {
            var m = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            m.color = c;
            m.SetFloat("_Smoothness", smooth);
            m.enableInstancing = true;
            if (emission)
            {
                m.EnableKeyword("_EMISSION");
                m.SetColor("_EmissionColor", c * 1.5f);
            }

            return m;
        }

        public static GameObject Box(string name, Transform parent, Vector3 position, Vector3 scale, Material mat) => Shape(PrimitiveType.Cube, name, parent, position, scale, mat);
        public static GameObject Sphere(string name, Transform parent, Vector3 position, Vector3 scale, Material mat) => Shape(PrimitiveType.Sphere, name, parent, position, scale, mat);
        public static GameObject Shape(PrimitiveType kind, string name, Transform parent, Vector3 p, Vector3 s, Material m)
        {
            var go = GameObject.CreatePrimitive(kind);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = p;
            go.transform.localScale = s;
            Object.Destroy(go.GetComponent<Collider>());
            go.GetComponent<Renderer>().sharedMaterial = m;
            return go;
        }
    }
}
