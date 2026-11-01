using System.Globalization;
using System.Threading;
using NUnit.Framework;
using Vertigo.Core;

namespace Vertigo.Tests.EditMode
{
    public class NumberFormatterTests
    {
        #region Tests

        [TestCase(0, "0")]
        [TestCase(950, "950")]
        [TestCase(19500, "19,500")]
        [TestCase(1250000, "1,250,000")]
        public void FormatAmount_UsesThousandsSeparators(int amount, string expected)
        {
            Assert.AreEqual(expected, NumberFormatter.FormatAmount(amount));
        }

        [Test]
        public void FormatMultiplier_MatchesAmountWithPrefix()
        {
            Assert.AreEqual("x19,500", NumberFormatter.FormatMultiplier(19500));
        }

        [Test]
        public void FormatAmount_SameOnTurkishPhone()
        {
            CultureInfo original = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");

                Assert.AreEqual("19,500", NumberFormatter.FormatAmount(19500));
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = original;
            }
        }

        #endregion
    }
}
