using System;
using System.Reflection;

namespace Exhale.Extensions
{
	public static class EditorTypeExtensions
	{
		// See: https://forum.unity.com/threads/bool-telling-whether-a-class-have-a-designated-property-drawer.261202/#post-7815036
		public static Type GetPropertyDrawerType(this Type type)
		{
			Assembly assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
			object scriptAttributeUtility = assembly.CreateInstance("UnityEditor.ScriptAttributeUtility");
			Type scriptAttributeUtilityType = scriptAttributeUtility.GetType();
 
 
			BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Static;
			MethodInfo getDrawerTypeForType = scriptAttributeUtilityType.GetMethod("GetDrawerTypeForType", bindingFlags);
 
			return (Type)getDrawerTypeForType.Invoke(scriptAttributeUtility, new object[] { type });
		}

		public static bool HasCustomPropertyDrawer(this Type type)
		{
			Type propertyDrawerType = GetPropertyDrawerType(type);
			return propertyDrawerType != null;
		}
	}
}