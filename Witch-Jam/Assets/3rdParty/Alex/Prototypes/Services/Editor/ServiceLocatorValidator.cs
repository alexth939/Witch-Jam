using System;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace Prototypes.Services.Editor
{
    // Note: we looking towards developing roslyn analyzer, so it will catch those errors even in ide. (severity:error)
    internal static class ServiceLocatorValidator
    {
        private static readonly Type BaseLocatorType = typeof(ServiceLocator<>);

        [InitializeOnLoadMethod]
        public static void ValidateAllImplementations()
        {
            TypeCache.TypeCollection implementors = TypeCache.GetTypesDerivedFrom(BaseLocatorType);
            StringBuilder logBuilder = new StringBuilder();

            _ = logBuilder.AppendLine("Service locator implementors Analysis results:");

            foreach(Type implementor in implementors)
            {
                // Implementor should look like this:
                // class GameServiceLocator : ServiceLocator<GameServiceLocator>
                // Generic parameter must match implementor.
                Type genericParameterType = implementor.BaseType.GenericTypeArguments[0];

                if(genericParameterType != implementor)
                {
                    _ = logBuilder.AppendLine(
                        $"[Bad] {implementor} : {BaseLocatorType}<{genericParameterType}> — " +
                        $"wrong generic parameter. Should be {implementor} : {BaseLocatorType}<{implementor}>");
                }
                else
                {
                    _ = logBuilder.AppendLine(
                        $"[Good] {implementor} : {BaseLocatorType}<{genericParameterType}> — correct");
                }
            }

            Debug.Log(logBuilder.ToString());
        }
    }
}
