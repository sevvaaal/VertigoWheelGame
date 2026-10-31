using NUnit.Framework;
using UnityEditor;
using Vertigo.Data;

namespace Vertigo.Tests.EditMode
{
    // Checks the game's real data, so a wheel edited into breaking a rule fails the tests, not the player's game.
    public class ZoneConfigAssetTests
    {
        #region Constants

        private const string ZONE_CONFIG_PATH = "Assets/_Project/Data/ZoneConfig.asset";

        #endregion

        #region Tests

        [Test]
        public void ProjectZoneConfig_HasNoValidationErrors()
        {
            ZoneConfigSO config = AssetDatabase.LoadAssetAtPath<ZoneConfigSO>(ZONE_CONFIG_PATH);

            Assert.IsNotNull(config, $"No zone config at {ZONE_CONFIG_PATH}.");
            CollectionAssert.IsEmpty(config.GetValidationErrors());
        }

        [Test]
        public void ProjectZoneConfig_NormalZonesGetBetterWheelsLater()
        {
            ZoneConfigSO config = AssetDatabase.LoadAssetAtPath<ZoneConfigSO>(ZONE_CONFIG_PATH);
            ZoneTypeSO normal = config.DefaultZoneType;

            Assert.AreNotEqual(normal.GetWheel(1), normal.GetWheel(10));
            Assert.AreNotEqual(normal.GetWheel(10), normal.GetWheel(20));
        }

        #endregion
    }
}
