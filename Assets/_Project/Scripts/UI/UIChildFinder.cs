using UnityEngine;

// Finds UI references by object name, so components can fill their own slots in OnValidate
// instead of someone dragging them in by hand. This is why UI objects follow a strict naming rule.
public static class UIChildFinder
{
    public static T Find<T>(Transform root, string childName) where T : Component
    {
        // true = also search inactive (hidden) children.
        foreach (T component in root.GetComponentsInChildren<T>(true))
        {
            if (component.name == childName)
            {
                return component;
            }
        }

        Debug.LogWarning($"{root.name}: no {typeof(T).Name} named '{childName}' found in its children.", root);
        return null;
    }
}
