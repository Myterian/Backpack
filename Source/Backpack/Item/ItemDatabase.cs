// Copyright © 2024 - 2025 Thomas Jungclaus. All rights reserved. Released under the MIT License.

using System;
using System.Collections.Generic;
using FlaxEngine;


#if FLAX_EDITOR
using FlaxEditor;
using System.Text;
#endif

namespace Backpack;

/// <summary>ItemDatabaseClass</summary>
public class ItemDatabase
{
    // [ShowInEditor, Serialize] public Dictionary<string, ItemClass> GetItemByID { get; private set; } = new();
    [ShowInEditor, Serialize] private string idPrefix = "Default_Database_";
    [Serialize] public Dictionary<string, ItemClass> ItemByID { get; private set; } = new();
    [ShowInEditor, Serialize] public bool IgnoreDatabase { get; private set; } = false;

#if FLAX_EDITOR
    [ShowInEditor, Serialize] private List<JsonAssetReference<ItemClass>> Items { get; set; } = new();



    // [Button("Build Database", "Saves all listed items into this database for in-game use"), EditorOrder(1)]
    public void BuildDatabase()
    {
        if (string.IsNullOrEmpty(idPrefix))
        {
            Debug.LogWarning("Database idPrefix was not set. Pls fix.");
            return;
        }
        
        // Clear old
        ItemByID.Clear();

        if (Items == null)
            return;

        // Setup
        System.Diagnostics.Stopwatch stopwatch = new();
        stopwatch.Start();
        string id;

        // Reset all item ids to avoid skipping different items, that -for whatever reason- have been saved with the same id
        for (int i = 0; i < Items.Count; i++)
            if (Items[i].Instance != null)
                Items[i].Instance.Id = "";

        // Create database
        for (int i = 0; i < Items.Count; i++)
        {
            // Get current item
            ItemClass itemReference = Items[i].Instance;

            if (itemReference == null)
                continue;

            if (ItemByID.ContainsKey(itemReference.Id))
                continue;

            // Create id based on prefix and current loop iteration
            // String[] idValue = { idPrefix, i.ToString() };
            // id = string.Join("-", idValue);

            StringBuilder idBuilder = new(idPrefix);
            idBuilder.Append(i);
            id = idBuilder.ToString();

            // Store values
            itemReference.Id = id;
            itemReference.IsCompiledInDatabase = true;
            ItemByID.Add(id, itemReference);

            // Resave the item object. The only thing we're modifying is the id,
            // which will be used by inventories and other components.
            Editor.SaveJsonAsset(Items[i].Asset.Path, itemReference);
        }

        // Output
        stopwatch.Stop();
        Debug.Log($"Item database was successfully build with {ItemByID.Count} entries in {stopwatch.ElapsedMilliseconds} ms.");
        
    }

    // [Button("Get Items"), EditorOrder(0)]
    public void LoadItemAssetsToList()
    {
        Guid[] itemAssetsIDs = Content.GetAllAssetsByType(typeof(ItemClass));

        for (int i = 0; i < itemAssetsIDs.Length; i++)
        {
            JsonAssetReference<ItemClass> itemAsset = (JsonAssetReference<ItemClass>)Content.Load(itemAssetsIDs[i]);

            if(itemAsset == null || Items.Contains(itemAsset))
                continue;

            Items.Add(itemAsset);
        }
    }
    #endif
}
