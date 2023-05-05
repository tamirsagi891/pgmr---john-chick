using UnityEditor;
using UnityEngine;

namespace BitStrap
{
	[CustomPropertyDrawer( typeof( ReadOnlyAttribute ) )]
	public sealed class ReadOnlyDrawer : PropertyDrawer
	{
		public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
		{
			PropertyDrawerHelper.LoadAttributeTooltip( this, label );

			var readOnlyAttribute = attribute as ReadOnlyAttribute;
			

			var condition = readOnlyAttribute.onlyInEditor && !EditorApplication.isPlayingOrWillChangePlaymode ||
			                readOnlyAttribute.onlyInPlaymode && EditorApplication.isPlayingOrWillChangePlaymode ||
			                !readOnlyAttribute.onlyInEditor && !readOnlyAttribute.onlyInPlaymode;
			using( DisabledGroup.Do( condition ) )
			{
				EditorGUI.PropertyField( position, property, label, true );
			}
		}
		
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUI.GetPropertyHeight(property);
		}
	}
}