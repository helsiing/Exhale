using System;
using System.Collections.Generic;
using BrunoMikoski.ScriptableObjectCollections;
using Exhale.Extensions;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Exhale.Collections
{
	/// <summary>
	/// Draws a dictionary wrapper pair in a convenient way on a single line.
	/// </summary>
	[CustomPropertyDrawer(typeof(DictionaryWrapperPairBase), true)]
	public class DictionaryWrapperPairEditor : PropertyDrawer
	{
		private const float HandleWidth = 13.0f;
		private const float KeyValueSpacing = 4;

		private static readonly GUIContent ListGuiContent = new GUIContent("List");
		private const string ListPptrPrefix = "PPtr<$";

		private SerializedProperty keyProperty;
		private SerializedProperty valueProperty;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			valueProperty = property.FindPropertyRelative("Value");
			float height = 0.0f;

			float valueHeight = EditorGUI.GetPropertyHeight(valueProperty, label, true);

			bool isMultiLine = valueProperty.hasVisibleChildren;

			DictionaryWrapperPairBase pair = property.GetActualObject<DictionaryWrapperPairBase>(fieldInfo);
			bool hasCustomPropertyDrawer = pair.GetValueType().HasCustomPropertyDrawer();
			if (valueProperty.isArray || isMultiLine)
			{
				// Arrays get an extra foldout because we can't draw other controls on top of the new headers any more..
				keyProperty = property.FindPropertyRelative("Key");
				height = EditorGUI.GetPropertyHeight(keyProperty, label, true);
				if (keyProperty.isExpanded || hasCustomPropertyDrawer)
					height += valueHeight;
			}
			else
			{
				height = valueHeight;
			}

			return height;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			keyProperty = property.FindPropertyRelative("Key");
			valueProperty = property.FindPropertyRelative("Value");

			float labelWidth = EditorGUIUtility.labelWidth - HandleWidth;

			bool isMultiLine = valueProperty.hasVisibleChildren;

			Rect keyRect;
			Rect valueRect;

			DictionaryWrapperPairBase pair = property.GetActualObject<DictionaryWrapperPairBase>(fieldInfo);
			bool hasCustomPropertyDrawer = pair.GetValueType().HasCustomPropertyDrawer();
			
			if (isMultiLine)
			{
				if (hasCustomPropertyDrawer)
				{
					keyRect = position.GetControlFirstRect();
					EditorGUI.indentLevel++;
					valueRect = position.GetSubRectFromBottom(
						position.height - keyRect.height - EditorGUIUtility.standardVerticalSpacing).Indent();
					EditorGUI.indentLevel--;
					EditorGUI.PropertyField(keyRect, keyProperty, GUIContent.none);
					EditorGUI.PropertyField(valueRect, valueProperty, GUIContent.none, true);
				}
				else if (valueProperty.isArray)
				{
					// Then we draw the key value ON TOP of the value to utilize the empty space in its foldout.
					// That way you can still have the key on a single line, and you can click the foldout to open
					// the contents of the value and tweak that, too.
					keyRect = new Rect(
						position.xMin + HandleWidth, position.yMin,
						labelWidth, EditorGUIUtility.singleLineHeight);
					EditorGUI.PropertyField(keyRect, keyProperty, GUIContent.none);

					// Draw a foldout to open the list. We can't just use the foldout of the list itself here like we
					// used to, because if you draw anything on top of the new list headers, clicks will pass through
					// to the list regardless, rendering the thing on top of useless.
					Rect foldoutRect = new Rect(
						position.xMin + HandleWidth, position.yMin,
						labelWidth, EditorGUIUtility.singleLineHeight);
					keyProperty.isExpanded = EditorGUI.Foldout(foldoutRect, keyProperty.isExpanded, "");

					// Now draw the contents themselves, below the key.
					if (keyProperty.isExpanded)
					{
						valueRect = new Rect(
							position.xMin + HandleWidth,
							position.yMin + EditorGUIUtility.singleLineHeight +
							EditorGUIUtility.standardVerticalSpacing,
							position.width - 30 - HandleWidth, position.height);

						// Force the list to be expanded to save you a click...
						if (!valueProperty.isExpanded)
							valueProperty.isExpanded = true;
						string typeName = valueProperty.arrayElementType;
						if (typeName.StartsWith(ListPptrPrefix))
						{
							typeName = typeName.Substring(
								ListPptrPrefix.Length, typeName.Length - ListPptrPrefix.Length - 1);
						}

						EditorGUI.PropertyField(valueRect, valueProperty, new GUIContent($"{typeName} List"), true);
						if (!valueProperty.isExpanded)
							valueProperty.isExpanded = true;
					}
				}
				else
				{
					// Slightly weird construction, but if this is a multi-line value then we draw the value first,
					// which will have a little foldout.
					valueRect = new Rect(
						position.xMin + HandleWidth, position.yMin,
						position.width - 30 - HandleWidth, position.height);

					EditorGUI.PropertyField(valueRect, valueProperty, GUIContent.none, true);

					// Then we draw the key value ON TOP of the value to utilize the empty space in its foldout.
					// That way you can still have the key on a single line, and you can click the foldout to open
					// the contents of the value and tweak that, too.
					keyRect = new Rect(
						position.xMin + HandleWidth, position.yMin,
						labelWidth, EditorGUIUtility.singleLineHeight);
					EditorGUI.PropertyField(keyRect, keyProperty, GUIContent.none);
				}

				return;
			}

			keyRect = new Rect(
				position.xMin, position.yMin,
				labelWidth, position.height);

			valueRect = new Rect(
				keyRect.xMax + KeyValueSpacing, position.yMin,
				position.width - labelWidth - 16 - KeyValueSpacing, position.height);

			EditorGUI.PropertyField(keyRect, keyProperty, GUIContent.none);
			EditorGUI.PropertyField(valueRect, valueProperty, GUIContent.none, true);
		}
	}

	/// <summary>
	/// Draws the dictionary wrapper pairs in a convenient reorderable list.
	/// </summary>
	[CustomPropertyDrawer(typeof(DictionaryWrapperBase), true)]
	public class DictionaryWrapperEditor : PropertyDrawer
	{
		private const int SerializedObjectMax = 30;
		
		private SerializedProperty pairsProperty;
		
		private GUIContent labelForDictionaryWrapperToDraw;

		[NonSerialized] private static readonly Dictionary<SerializedObject, Dictionary<string, ReorderableList>>
			targetObjectToPathToCachedList = new Dictionary<SerializedObject, Dictionary<string, ReorderableList>>();

		public ReorderableList GetReorderableList(SerializedProperty property, GUIContent label)
		{
			pairsProperty = property.FindPropertyRelative("pairs");
			labelForDictionaryWrapperToDraw = label;
			
			// Make sure the dictionary doesn't get huge, realistically there only ought to be a handful.
			// It's unclear to me though what the lifecycle of a SerializedObject is exactly, but this is a good stopgap
			// so that if you select a bunch of different objects and a bunch of new SerializedObjects are created the
			// whole time, those will not stick around forever.
			if (targetObjectToPathToCachedList != null && targetObjectToPathToCachedList.Count > SerializedObjectMax)
				targetObjectToPathToCachedList.Clear();

			// Need to cache it like this because PropertyDrawers are re-used between properties but every property
			// *needs* to have its own ReorderableList because a ReorderableList requires to know the specific property.
			// So the code is a bit convoluted but if we don't cache these reorderable lists and a make a new one every
			// time then that is just *TERRIBLE* for performance.
			
			// First find the property dictionary for this serialized object.
			SerializedObject target = property.serializedObject;
			bool propertyDictionaryExisted = targetObjectToPathToCachedList.TryGetValue(
				target, out Dictionary<string, ReorderableList> propertyDictionary);
			if (!propertyDictionaryExisted)
			{
				propertyDictionary = new Dictionary<string, ReorderableList>();
				targetObjectToPathToCachedList.Add(target, propertyDictionary);
			}
			
			// Now check if a list exists for this property. We need to use the path and not the property itself because
			// serialized properties do not support equality checks or hashing...
			bool existed = propertyDictionary.TryGetValue(
				pairsProperty.propertyPath, out ReorderableList reorderableList);
			if (!existed)
			{
				reorderableList = new ReorderableList(property.serializedObject, pairsProperty);

				reorderableList.drawHeaderCallback += DrawHeader;
				reorderableList.drawElementCallback += DrawElement;
				reorderableList.elementHeightCallback += GetElementHeight;
				
				propertyDictionary.Add(pairsProperty.propertyPath, reorderableList);
			}

			return reorderableList;
		}

		private void DrawHeader(Rect rect)
		{
			EditorGUI.LabelField(rect, labelForDictionaryWrapperToDraw);
		}

		private float GetElementHeight(int index)
		{
			int count = pairsProperty.arraySize;
			if (count == 0)
				return 0;
			return EditorGUI.GetPropertyHeight(
				pairsProperty.GetArrayElementAtIndex(index), GUIContent.none, true);
		}

		private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			EditorGUI.PropertyField(
				rect, pairsProperty.GetArrayElementAtIndex(index), GUIContent.none, false);
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return GetReorderableList(property, label).GetHeight() + 6;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			position.y += 3;

			EditorGUI.BeginChangeCheck();
			GetReorderableList(property, label).DoList(position);
			bool didChange = EditorGUI.EndChangeCheck();

			if (didChange)
			{
				DictionaryWrapperBase dictionaryWrapper = property.GetActualObject<DictionaryWrapperBase>(fieldInfo);
				dictionaryWrapper.ClearCache();
			}
		}
	}
}