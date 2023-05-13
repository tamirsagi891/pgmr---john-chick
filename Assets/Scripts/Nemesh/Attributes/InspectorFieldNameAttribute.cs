using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nemesh.Attributes
{
    public class InspectorFieldNameAttribute : PropertyAttribute
    {
        public readonly string NewName;

        public InspectorFieldNameAttribute(string newName)
        {
            this.NewName = newName;
        }
    }
}
