using UnityEngine;

namespace Vertigo.UI
{
    // Finds UI references by object name, so components fill their own slots in OnValidate
    // instead of someone dragging them in by hand. This is why UI objects follow a strict naming rule.
    public static class UIChildFinder
    {
        #region Public Methods

        // Returns null when nothing matches, and reports it as an error on the root, so the caller can check with if (result).
        public static T Find<T>(Transform root, string childName) where T : Component
        {
            foreach (T component in root.GetComponentsInChildren<T>(true))
            {
                if (component.name == childName)
                {
                    return component;
                }
            }

            Debug.LogError($"{root.name}: no {typeof(T).Name} named '{childName}' found in its children.", root);
            return null;
        }

        #endregion
    }
}
