using UnityEngine;
using UnityEngine.Pool;

namespace Vertigo.UI
{
    // Reuses reward rows: a cleared list hands its rows back here instead of destroying them.
    public sealed class RewardRowPool
    {
        #region Fields

        private readonly RewardRowView _prefab;
        private readonly Transform _container;
        private readonly ObjectPool<RewardRowView> _pool;

        #endregion

        #region Constructor

        public RewardRowPool(RewardRowView prefab, Transform container)
        {
            _prefab = prefab;
            _container = container;
            _pool = new ObjectPool<RewardRowView>(CreateRow, HandleGet, HandleRelease, HandleDestroy);
        }

        #endregion

        #region Public Methods

        public RewardRowView Get(Sprite icon)
        {
            RewardRowView row = _pool.Get();

            // A reused row goes to the bottom of the list, like a new one would.
            row.transform.SetAsLastSibling();
            row.Setup(icon);
            return row;
        }

        public void Release(RewardRowView row)
        {
            _pool.Release(row);
        }

        #endregion

        #region Pool Callbacks

        private RewardRowView CreateRow()
        {
            return Object.Instantiate(_prefab, _container);
        }

        private void HandleGet(RewardRowView row)
        {
            row.gameObject.SetActive(true);
        }

        private void HandleRelease(RewardRowView row)
        {
            row.ResetRow();
            row.gameObject.SetActive(false);
        }

        private void HandleDestroy(RewardRowView row)
        {
            if (row)
            {
                Object.Destroy(row.gameObject);
            }
        }

        #endregion
    }
}
