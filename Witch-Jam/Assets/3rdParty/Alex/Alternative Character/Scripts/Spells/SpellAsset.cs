using UnityEngine;

namespace Runtime
{
    [CreateAssetMenu(fileName = "SpellAsset", menuName = "Scriptable Objects/SpellAsset")]
    public class SpellAsset : ScriptableObject
    {
        [SerializeReference] private ISpell _spell; // <- no! `_spell.managedReferenceFieldTypename` gives me this type (the ISpell) but i need what is actually behind it, the implementor

        public ISpell Spell => _spell;
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(SpellAsset))]
    public class SpellAssetEditor : UnityEditor.Editor
    {
        private UnityEditor.SerializedProperty _spell;
        private GUIContent _chosenSpellTypeLabel;

        private void OnEnable()
        {
            _spell = serializedObject.FindProperty("_spell");
            SetupSpellName();
        }

        private void SetupSpellName()
        {
            if(_spell == null || _spell.managedReferenceValue == null)
            {
                _chosenSpellTypeLabel = GUIContent.none;
            }
            else
            {
                object spellValue = _spell.managedReferenceValue;
                string spellTypeName = spellValue.GetType().Name;
                _chosenSpellTypeLabel = new GUIContent(spellTypeName);
            }
        }

        public override void OnInspectorGUI()
        {
            if(GUILayout.Button("Choose spell"))
            {
                var menu = new UnityEditor.GenericMenu()
                {
                    allowDuplicateNames = false,
                };

                var spellTypes = UnityEditor.TypeCache.GetTypesDerivedFrom(typeof(ISpell));

                foreach(var type in System.Linq.Enumerable.Where(spellTypes, type => type.IsAbstract == false))
                {
                    const bool IsChecked = false;
                    menu.AddItem(new GUIContent(type.Name), IsChecked, () =>
                    {
                        _spell.managedReferenceValue = System.Activator.CreateInstance(type);
                        SetupSpellName();
                        serializedObject.ApplyModifiedProperties();
                    });
                }

                menu.ShowAsContext();
            }

            if(_spell != null)
                _ = UnityEditor.EditorGUILayout.PropertyField(_spell, _chosenSpellTypeLabel, includeChildren: true);

            if(GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif
}
