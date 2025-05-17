using System;
using UnityEngine;
using UnityEditor;

using Object = UnityEngine.Object;

namespace PinBoard.Editor
{
    [Serializable]
    internal sealed class PinReference
    {
        public string _globalIdString;

        public static PinReference FromObject(Object globalObject)
        {
            if(globalObject == null)
                throw new ArgumentNullException(nameof(globalObject));

            GlobalObjectId globalId = GlobalObjectId.GetGlobalObjectIdSlow(globalObject);
            PinReference reference = new() { _globalIdString = globalId.ToString() };

            return reference;
        }

        public Object Resolve()
        {
            Object objectInstance = null;

            if(string.IsNullOrEmpty(_globalIdString))
            {
                Debug.LogWarning($"Reference id corrupted!");
            }
            else if(GlobalObjectId.TryParse(_globalIdString, out GlobalObjectId globalId))
            {
                objectInstance = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(globalId);
            }
            else
            {
                Debug.LogWarning($"Failed to resolve object by id:[{_globalIdString}]");
            }

            return objectInstance;
        }
    }
}
