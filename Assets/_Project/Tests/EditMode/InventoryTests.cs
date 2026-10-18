using NUnit.Framework;
using UnityEngine;

// The inventory, checked without any scene or UI.
public class InventoryTests
{
    private Inventory inventory;
    private RewardTypeSO gold;
    private RewardTypeSO cash;

    [SetUp]
    public void CreateInventory()
    {
        inventory = new Inventory();

        // Real reward types, created in memory only (not saved as assets).
        gold = ScriptableObject.CreateInstance<RewardTypeSO>();
        cash = ScriptableObject.CreateInstance<RewardTypeSO>();
    }

    [TearDown]
    public void DestroyRewardTypes()
    {
        Object.DestroyImmediate(gold);
        Object.DestroyImmediate(cash);
    }

    [Test]
    public void Add_SameTypeTwice_AddsUp()
    {
        inventory.Add(new Reward(gold, 100));
        inventory.Add(new Reward(gold, 50));

        Assert.AreEqual(150, inventory.Amounts[gold]);
    }

    [Test]
    public void Add_DifferentTypes_StaySeparate()
    {
        inventory.Add(new Reward(gold, 100));
        inventory.Add(new Reward(cash, 300));

        Assert.AreEqual(100, inventory.Amounts[gold]);
        Assert.AreEqual(300, inventory.Amounts[cash]);
    }

    [Test]
    public void Add_AnnouncesNewTotal()
    {
        int announcedTotal = 0;
        inventory.AmountChanged += (type, total) => announcedTotal = total;

        inventory.Add(new Reward(gold, 100));
        inventory.Add(new Reward(gold, 50));

        Assert.AreEqual(150, announcedTotal);
    }

    [Test]
    public void Clear_EmptiesEverythingAndAnnounces()
    {
        bool wasCleared = false;
        inventory.Cleared += () => wasCleared = true;
        inventory.Add(new Reward(gold, 100));

        inventory.Clear();

        Assert.AreEqual(0, inventory.Amounts.Count);
        Assert.IsTrue(wasCleared);
    }
}
