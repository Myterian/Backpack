// Copyright © 2024 - 2025 Thomas Jungclaus. All rights reserved. Released under the MIT License.

namespace Backpack;

/// <summary>Extension methods for the inventory class, to add functionlity for embedded inventory items at the master inventory level</summary>
public static class InventoryResolver
{
    /// <summary>Add an item to this inventory or to a stored items inventory</summary>
    /// <param name="inventory">The target inventory, which may hold more inventories embedded in items</param>
    /// <param name="item">The item instance to add</param>
    /// <param name="slotIndex">(Optional) The specific inventory slot to which the item should be added. If the chosen slot is occupied the next available slot will be used.</param>
    /// <returns>true if item was added to any inventory. Returns false when failed.</returns>
    public static bool AddItemRecursive(this Inventory inventory, ItemInstance item, int slotIndex = -1)
    {
        // For performance, check if we can add the item to this inventory, first
        bool isAdded = inventory.AddItem(item, slotIndex);
        if (isAdded) return true;

        // Query every item slot for an inventory component to find all stored inventories
        ItemInteraction interaction = new ItemInteraction();
        InventoryQuery inventoryQuery = new();
        interaction.AddQuery(inventoryQuery);

        for (int i = 0; i < inventory.ItemSlots.Count; i++)
            inventory.ItemSlots[i].itemInstance?.QueryProperties(interaction);

        // Try to add the item to every found inventory. 
        for (int i = 0; i < inventoryQuery.Inventories.Count; i++)
        {
            // Technically we could call 'AddItemRecursive' on these inventories as well,
            // instead of just trying to add the item to the top level inventory we found. 
            // But this can escalate quickly in terms of performance and it risks an 
            // utterly shit user experience, where items disappear in an inventory of 
            // an inventory of an inventory and so on, making it look like the 
            // item is just gone.
            isAdded = inventoryQuery.Inventories[i].AddItem(item, slotIndex);
            if (isAdded) return true;
        }

        return false;
    }
}
