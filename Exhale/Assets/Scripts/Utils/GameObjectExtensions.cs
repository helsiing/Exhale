using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Exhale.Utils
{
    public static class GameObjectExtensions
    {
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            var existing = gameObject.GetComponent<T>();
            if (existing != null)
                return existing;

#if UNITY_EDITOR
            // Undo.AddComponent is only safe outside of play mode (edit-time prefab/scene editing).
            // At runtime (including editor play mode) use the standard AddComponent path.
            if (!Application.isPlaying)
                return Undo.AddComponent<T>(gameObject);
#endif
            return gameObject.AddComponent<T>();
        }

        public static string GetPathToParent(this GameObject obj, GameObject parent, bool trimWhiteSpace = false)
        {
            var path = obj.name;
            while (obj.transform.parent != null && obj.transform.parent.gameObject != parent)
            {
                obj = obj.transform.parent.gameObject;
                path = (trimWhiteSpace ? obj.name.Trim() : obj.name) + "/" + path;
            }

            return path;
        }

        public static void SetTileTransparency(this GameObject obj, float alpha)
        {
            var renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                var color = renderer.material.color;
                color.a = alpha;
                renderer.material.color = color;
            }
        }

        public static void DestroyChildObjects(this GameObject obj)
        {
            foreach (Transform child in obj.transform) Object.Destroy(child.gameObject);
        }
    }
}