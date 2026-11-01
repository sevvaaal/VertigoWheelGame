using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Vertigo.Core;
using Vertigo.Data;
using Vertigo.Gameplay;

namespace Vertigo.UI
{
    // The rewards won in this run, one row per reward type. It follows the run inventory over the event bus.
    public class RewardListView : MonoBehaviour
    {
        #region Serialized Fields

        [FormerlySerializedAs("rowContainer")]
        [SerializeField] private Transform _rowContainer;

        [FormerlySerializedAs("rowPrefab")]
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

        private void OnDestroy()
        {
            if (_eventBus == null)
            {
                return;
            }

            _eventBus.Unsubscribe<RunRewardChangedEvent>(HandleRunRewardChanged);
            _eventBus.Unsubscribe<RunRewardsClearedEvent>(HandleRunRewardsCleared);
        }

        #endregion

        #region Public Methods

        public void Construct(IEventBus eventBus)
        {
            // Created here rather than in Awake, so the pool exists before the first event even if the panel starts inactive.
            _rowPool = new RewardRowPool(_rowPrefab, _rowContainer);
            _eventBus = eventBus;
            _eventBus.Subscribe<RunRewardChangedEvent>(HandleRunRewardChanged);
            _eventBus.Subscribe<RunRewardsClearedEvent>(HandleRunRewardsCleared);
        }

        // Where a won reward's icon flies to: its own row, or the panel if this type has no row yet.
        public Transform GetFlyTarget(RewardTypeSO type)
        {
            return _rows.TryGetValue(type, out RewardRowView row) ? row.transform : _rowContainer;
        }

        #endregion

        #region Private Methods

        private void HandleRunRewardChanged(RunRewardChangedEvent rewardChanged)
        {
            if (!_rows.TryGetValue(rewardChanged.Type, out RewardRowView row))
            {
                row = _rowPool.Get(rewardChanged.Type.Icon);
                _rows[rewardChanged.Type] = row;
            }

            row.SetAmount(rewardChanged.Total);
        }

        private void HandleRunRewardsCleared(RunRewardsClearedEvent rewardsCleared)
        {
            foreach (RewardRowView row in _rows.Values)
            {
                _rowPool.Release(row);
            }

            _rows.Clear();
        }

        #endregion
    }
}
