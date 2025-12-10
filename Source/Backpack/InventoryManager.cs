// Copyright © 2024 - 2025 Thomas Jungclaus. All rights reserved. Released under the MIT License.

using System;
using System.Collections.Generic;
using FlaxEngine;

namespace Backpack;

/// <summary>InventoryManager. Contains all item databases and item lookup methods.</summary>
public class InventoryManager : GamePlugin
{
    public static InventoryManager Instance => instance ??= PluginManager.GetPlugin<InventoryManager>();
    private static InventoryManager instance = null;

    public static Version PluginVersion => new Version(2, 5, 13);

    private List<ItemDatabase> itemDatabases = new();

    public override void Initialize()
    {
        base.Initialize();

        // Get all relevant item databases
        itemDatabases?.Clear();
        Guid[] itemDatabasesIDs = Content.GetAllAssetsByType(typeof(ItemDatabase));

        for (int i = 0; i < itemDatabasesIDs.Length; i++)
        {
            JsonAssetReference<ItemDatabase> itemAsset = (JsonAssetReference<ItemDatabase>)Content.Load(itemDatabasesIDs[i]);

            if (itemAsset.Instance != null && !itemAsset.Instance.IgnoreDatabase)
                itemDatabases.Add(itemAsset.Instance);
        }

        Debug.Log($"Backpack: Inventory Manager loaded and {itemDatabases.Count} Item Database(s) were found!");
    }

    public override void Deinitialize()
    {
        itemDatabases.Clear();
        base.Deinitialize();
    }


    /// <summary>Search all available item databases (vanilla, mod and dlc) to find an item base from via id</summary>
    /// <param name="itemId">The item id, of which we want an instance from.</param>
    /// <param name="item">Outputs the item base we're looking for. Null is item id is invalid.</param>
    /// <returns>true if instance was returned. False if item could not be found in any database.</returns>
    public bool GetItemByID(string itemId, out ItemClass item)
    {
        // Search all databases for the given item id
        for (int i = 0; i < itemDatabases.Count; i++)
        {
            bool isItemFound = itemDatabases[i].ItemByID.ContainsKey(itemId);

            if (!isItemFound)
                continue;

            item = itemDatabases[i].ItemByID[itemId];
            return true;
        }

        item = null;
        return false;
    }

    public InventoryManager()
    {
        _description = new()
        {
            Name = "Backpack - Flax Engine Inventory System",
            Description = "A modular and extenable inventory system",
            Author = "Thomas Jungclaus",
            AuthorUrl = "https://github.com/Myterian/",
            RepositoryUrl = "https://github.com/Myterian/Backpack/",
            Category = "Backpack",
            IsAlpha = false,
            IsBeta = false,
            Version = PluginVersion,
        };
    }
}
