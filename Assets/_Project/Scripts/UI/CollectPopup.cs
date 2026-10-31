using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Vertigo.Data;
using Vertigo.Gameplay;

namespace Vertigo.UI
{
    public class CollectPopup : PopupView
    {
        #region Constants

        private const string PLAY_AGAIN_BUTTON_NAME = "ui_button_play_again";

        #endregion

        #region Serialized Fields

        [FormerlySerializedAs("playAgainButton")]
        [SerializeField] private Button _playAgainButton;

        [FormerlySerializedAs("rowContainer")]
        [SerializeField] private Transform _rowContainer;

        [FormerlySerializedAs("rowPrefab")]
        [SerializeField] private RewardRowView _rowPrefab;

        #endregion

        #region Fields

        private readonly List<RewardRowView> _rows = new List<RewardRowView>();

        #endregion

        #region Unity Methods

        private void OnValidate()
        {
            _playAgainButton = UIChildFinder.Find<Button>(transform, PLAY_AGAIN_BUTTON_NAME);
        }

        protected override void Awake()
        {
            _playAgainButton.onClick.AddListener(HandlePlayAgainClicked);
            base.Awake();
        }

        protected override void OnDestroy()
        {
            if (_playAgainButton)
            {
                _playAgainButton.onClick.RemoveListener(HandlePlayAgainClicked);
            }

            base.OnDestroy();
        }

        #endregion

        #region Overrides

        protected override bool ShouldShow(GameState state)
        {
            return state == GameState.Collected;
        }

        protected override void Subscribe()
        {
            base.Subscribe();
            EventBus.Subscribe<RewardsCollectedEvent>(HandleRewardsCollected);
        }

        protected override void Unsubscribe()
        {
            EventBus.Unsubscribe<RewardsCollectedEvent>(HandleRewardsCollected);
            base.Unsubscribe();
        }

        #endregion

        #region Private Methods

        // Arrives just before the state turns to Collected, so the rows are ready when the popup appears.
        private void HandleRewardsCollected(RewardsCollectedEvent rewardsCollected)
        {
            ClearRows();

            foreach (KeyValuePair<RewardTypeSO, int> reward in rewardsCollected.Rewards)
            {
                RewardRowView row = Instantiate(_rowPrefab, _rowContainer);
                row.Setup(reward.Key.Icon);
                row.SetAmount(reward.Value);
                _rows.Add(row);
            }
        }

        private void ClearRows()
        {
            foreach (RewardRowView row in _rows)
            {
                Destroy(row.gameObject);
            }

            _rows.Clear();
        }

        private void HandlePlayAgainClicked()
        {
            EventBus.Publish(new RestartRequestedEvent());
        }

        #endregion
    }
}
