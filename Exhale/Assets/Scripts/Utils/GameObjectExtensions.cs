using UnityEngine;

namespace Exhale.Utils
{
    public static class GameObjectExtensions
    {
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            T existing = gameObject.GetComponent<T>();
            if (existing != null)
                return existing;

#if UNITY_EDITOR
            return UnityEditor.Undo.AddComponent<T>(gameObject);
#else
			return gameObject.AddComponent<T>();
#endif
        }

        public static string GetPathToParent(this GameObject obj, GameObject parent, bool trimWhiteSpace = false)
        {
            string path = obj.name;
            while (obj.transform.parent != null && obj.transform.parent.gameObject != parent)
            {
                obj = obj.transform.parent.gameObject;
                path = (trimWhiteSpace ? obj.name.Trim() : obj.name) + "/" + path;
            }

            return path;
        }
        
        public static void SetTileTransparency(this GameObject obj, float alpha)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                Color color = renderer.material.color;
                color.a = alpha;
                renderer.material.color = color;
            }
        }

        public static void DestroyChildObjects(this GameObject obj)
        {
            foreach (Transform child in obj.transform)
            {
                Object.Destroy(child.gameObject);
            }
        }
    }
}