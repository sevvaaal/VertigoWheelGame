using UnityEngine;

namespace Vertigo.Core
{
    // Called from OnValidate, so a missing Inspector reference shows up as an error in the editor
    // (clicking it selects the object) instead of as a crash in Play mode.
    public static class ReferenceValidator
    {
        #region Public Methods

        public static void Require(Object owner, Object reference, string fieldName)
        {
            if (!reference)
            {
                Debug.LogError($"{owner.name} ({owner.GetType().Name}): '{fieldName}' is not assigned.", owner);
            }
        }

        #endregion
    }
}
