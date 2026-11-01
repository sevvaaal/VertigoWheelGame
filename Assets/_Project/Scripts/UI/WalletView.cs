using System.Collections.Generic;
using UnityEngine;
using Vertigo.Core;
using Vertigo.Data;
using Vertigo.Economy;

namespace Vertigo.UI
{
    // The currencies the player has taken home, one row per currency. Items stay in the wallet but are not listed here.
    public class WalletView : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private Transform _rowContainer;
        [SerializeField] private RewardRowView _rowPrefab;

        #endregion

        #region Fields

        private readonly Dictionary<RewardTypeSO, RewardRowView> _rows = new Dictionary<RewardTypeSO, RewardRowView>();
        private RewardRowPool _rowPool;
        private IEventBus _eventBus;

        #endregion

        #region Unity Methods

        private void OnValidate()
        {
            ReferenceValidator.Require(this, _rowContainer, nameof(_rowContainer));
            ReferenceValidator.Require(this, _rowPrefab, nameof(_rowPrefab));
        }

        private void Awake()
        {
            _rowPool = new RewardRowPool(_rowPrefab, _rowContainer);
        }

        private void OnDestroy()
        {
            if (_eventBus != null)
            {
                _eventBus.Unsubscribe<WalletChangedEvent>(HandleWalletChanged);
            }
        }

        #endregion

        #region Public Methods

        public void Construct(IEventBus eventBus)
        {
            _eventBus = eventBus;
            _eventBus.Subscribe<WalletChangedEvent>(HandleWalletChanged);
        }

        #endregion

        #region Private Methods

        private void HandleWalletChanged(WalletChangedEvent walletChanged)
        {
            if (walletChanged.Type.Kind != RewardKind.Currency)
            {
                return;
            }

            if (!_rows.TryGetValue(walletChanged.Type, out RewardRowView row))
            {
                row = _rowPool.Get(walletChanged.Type.Icon);
                _rows[walletChanged.Type] = row;
            }

            row.SetAmount(walletChanged.Total);
        }

        #endregion
    }
}
