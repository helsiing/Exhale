using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Exhale.Editor.Dashboards
{
    public class GodModeDashboard : EditorWindow
    {
        [SerializeField] private VisualTreeAsset m_VisualTreeAsset = default;

        [MenuItem("Exhale/Dashboards/GodModeDashboard")]
        public static void ShowExample()
        {
            GodModeDashboard wnd = GetWindow<GodModeDashboard>();
            wnd.titleContent = new GUIContent("GodModeDashboard");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // VisualElements objects can contain other VisualElement following a tree hierarchy.
            VisualElement label = new Label("Hello World! From C#");
            root.Add(label);

            // Instantiate UXML
            VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
            root.Add(labelFromUXML);
        }
    }
}
