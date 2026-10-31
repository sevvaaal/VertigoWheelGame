using UnityEngine;
using UnityEngine.Serialization;

namespace Vertigo.Data
{
    [CreateAssetMenu(fileName = "RewardType", menuName = "Vertigo/Reward Type")]
    public class RewardTypeSO : ScriptableObject
    {
        #region Serialized Fields

        [FormerlySerializedAs("displayName")]
        [SerializeField] private string _displayName;

        [FormerlySerializedAs("icon")]
        [SerializeField] private Sprite _icon;

        [SerializeField] private RewardKind _kind = RewardKind.Item;

        #endregion

        #region Properties

        public string DisplayName
        {
            get { return _displayName; }
        }

        public Sprite Icon
        {
            get { return _icon; }
        }

        public RewardKind Kind
        {
            get { return _kind; }
        }

        #endregion
    }
}
