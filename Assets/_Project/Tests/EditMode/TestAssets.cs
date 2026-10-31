using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using Vertigo.Data;

namespace Vertigo.Tests.EditMode
{
    // Builds data assets in memory for tests, filling their private fields the same way the Inspector does.
    public sealed class TestAssets
    {
        #region Fields

        private readonly List<Object> _created = new List<Object>();

        #endregion

        #region Public Methods

        public RewardTypeSO CreateRewardType(RewardKind kind = RewardKind.Item)
        {
            RewardTypeSO rewardType = Track(ScriptableObject.CreateInstance<RewardTypeSO>());

            SerializedObject serialized = new SerializedObject(rewardType);
            serialized.FindProperty("_kind").enumValueIndex = (int)kind;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            return rewardType;
        }

        public WheelSO CreateWheel(int rewardCount, params int[] bombSlots)
        {
            WheelSO wheel = Track(ScriptableObject.CreateInstance<WheelSO>());
            RewardTypeSO rewardType = CreateRewardType();

            SerializedObject serialized = new SerializedObject(wheel);
            SerializedProperty rewards = serialized.FindProperty("_rewards");
            rewards.arraySize = rewardCount;
            for (int i = 0; i < rewardCount; i++)
            {
                SerializedProperty reward = rewards.GetArrayElementAtIndex(i);
                reward.FindPropertyRelative("_type").objectReferenceValue = rewardType;
                reward.FindPropertyRelative("_amount").intValue = 1;
            }

            SerializedProperty slots = serialized.FindProperty("_bombSlots");
            slots.arraySize = bombSlots.Length;
            for (int i = 0; i < bombSlots.Length; i++)
            {
                slots.GetArrayElementAtIndex(i).intValue = bombSlots[i];
            }

            serialized.ApplyModifiedPropertiesWithoutUndo();
            return wheel;
        }

        // tiers: pairs of (from zone, wheel), e.g. (1, bronze), (10, betterBronze).
        public ZoneTypeSO CreateZoneType(int interval, int requiredBombCount, params (int FromZone, WheelSO Wheel)[] tiers)
        {
            ZoneTypeSO zoneType = Track(ScriptableObject.CreateInstance<ZoneTypeSO>());

            SerializedObject serialized = new SerializedObject(zoneType);
            serialized.FindProperty("_interval").intValue = interval;
            serialized.FindProperty("_requiredBombCount").intValue = requiredBombCount;

            SerializedProperty wheelTiers = serialized.FindProperty("_wheelTiers");
            wheelTiers.arraySize = tiers.Length;
            for (int i = 0; i < tiers.Length; i++)
            {
                SerializedProperty tier = wheelTiers.GetArrayElementAtIndex(i);
                tier.FindPropertyRelative("_fromZone").intValue = tiers[i].FromZone;
                tier.FindPropertyRelative("_wheel").objectReferenceValue = tiers[i].Wheel;
            }

            serialized.ApplyModifiedPropertiesWithoutUndo();
            return zoneType;
        }

        public void DestroyAll()
        {
            foreach (Object created in _created)
            {
                Object.DestroyImmediate(created);
            }

            _created.Clear();
        }

        #endregion

        #region Private Methods

        private T Track<T>(T asset) where T : Object
        {
            // Some test assets are invalid on purpose, so their OnValidate errors are expected, not failures.
            // Set here because the test framework resets it at the start of every test, after SetUp.
            LogAssert.ignoreFailingMessages = true;
            _created.Add(asset);
            return asset;
        }

        #endregion
    }
}
