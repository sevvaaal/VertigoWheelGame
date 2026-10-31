using System.Globalization;

namespace Vertigo.Core
{
    // The single place amounts are turned into text, so the wheel and the panels always agree.
    public static class NumberFormatter
    {
        #region Constants

        private const string AMOUNT_FORMAT = "N0";
        private const string MULTIPLIER_PREFIX = "x";

        #endregion

        #region Public Methods

        // A fixed culture: on a Turkish phone "N0" would otherwise print 19.500 instead of 19,500.
        public static string FormatAmount(int amount)
        {
            return amount.ToString(AMOUNT_FORMAT, CultureInfo.InvariantCulture);
        }

        public static string FormatMultiplier(int amount)
        {
            return MULTIPLIER_PREFIX + FormatAmount(amount);
        }

        #endregion
    }
}
