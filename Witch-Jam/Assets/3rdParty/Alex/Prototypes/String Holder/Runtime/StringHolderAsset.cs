using UnityEngine;

namespace Prototypes.StringHolder
{
    public class StringHolderAsset : ScriptableObject
    {
        [SerializeField, Multiline] private string _value;

        public string Value => _value;

        internal static string ValueFieldName => nameof(_value);

        public string GetValue() => _value;
    }
}
