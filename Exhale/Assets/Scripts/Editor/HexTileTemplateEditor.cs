using Exhale.Scripts.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
    [CustomEditor(typeof(HexTileTemplate))]
    public class HexTileTemplateEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            // Create the root container
            VisualElement root = new VisualElement();

            // Title
            Label title = new Label("Editor");
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            title.style.fontSize = 14;
            root.Add(title);

            // Bind the GameData object
            SerializedProperty hexagonDataProperty = serializedObject.FindProperty("Elements");
            if (hexagonDataProperty != null)
            {
                // Add HexagonData UI
                AddHexagonDataUI(root, serializedObject.FindProperty("Elements"));
            }

            return root;
        }

        private void AddHexagonDataUI(VisualElement root, SerializedProperty hexagonDataProperty)
        {
            if (hexagonDataProperty == null) return;

            // Add a section title for HexagonData
            Label hexagonTitle = new Label("Hexagon Element Editor");
            hexagonTitle.style.unityFontStyleAndWeight = FontStyle.Bold;
            hexagonTitle.style.fontSize = 12;
            root.Add(hexagonTitle);

            // Create UI for each side
            SerializedProperty sidesProperty = hexagonDataProperty.FindPropertyRelative("Sides");
            for (int i = 0; i < sidesProperty.arraySize; i++)
            {
                int sideIndex = i;

                // Create EnumField for each side
                EnumField sideField = new EnumField($"Side {sideIndex + 1}")
                {
                    name = sidesProperty.GetArrayElementAtIndex(sideIndex).displayName,
                    //value = sideIndex
                };

                sideField.RegisterValueChangedCallback(evt =>
                {
                    sidesProperty.GetArrayElementAtIndex(sideIndex).enumValueIndex = 1;
                    serializedObject.ApplyModifiedProperties();
                });

                root.Add(sideField);
            }
        }
    }
}