using System;

namespace Vertigo.Data
{
    // One slice on a wheel at runtime. A bomb is created without a reward, so data can never attach one to it.
    public sealed class SliceData
    {
        #region Constructor

        private SliceData(bool isBomb, Reward reward)
        {
            IsBomb = isBomb;
            Reward = reward;
        }

        #endregion

        #region Properties

        public bool IsBomb { get; }

        public Reward Reward { get; }

        #endregion

        #region Factory Methods

        public static SliceData CreateBomb()
        {
            return new SliceData(true, null);
        }

        public static SliceData CreateReward(Reward reward)
        {
            if (reward == null)
            {
                throw new ArgumentNullException(nameof(reward));
            }

            return new SliceData(false, reward);
        }

        #endregion
    }
}
