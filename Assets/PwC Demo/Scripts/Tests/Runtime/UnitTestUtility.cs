using UnityEngine;
using UnityEditor;

namespace PWCDemo.UnitTests
{
    public abstract class UnitTestUtility : ScriptableObject
    {
        /// <summary>
        /// Handles retrieving an instance of the given <see cref="UnitTestUtility"/> type by searching the asset database.
        /// returns null if no matching utility can be found
        /// </summary>
        /// <typeparam name="T">The type of <see cref="UnitTestUtility"/> we are looking for</typeparam>
        /// <returns>A matching instance of a <see cref="UnitTestUtility"/></returns>
        protected static T GetInstance<T>() where T : UnitTestUtility
        {
#if UNITY_EDITOR
            string[] paths = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            if (paths.Length == 0) return null;
            if (paths.Length > 1) Debug.LogWarning("Multiple applicable asset utilities found : defaulting the the first one");
            return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(paths[0]));
#else
            return null;
#endif
        }
    }
}
