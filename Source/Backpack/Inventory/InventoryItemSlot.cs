// Copyright © 2024 - 2025 Thomas Jungclaus. All rights reserved. Released under the MIT License.

using System;
using System.Collections.Generic;
using FlaxEngine;

namespace Backpack;

/// <summary>InventoryItemSlot Class. Is used exclusivley in the inventory class for managing item instance storage</summary>
public class InventoryItemSlot
{ 
    public ItemInstance itemInstance { get; private set; } = null;

    // Metadata
    [ShowInEditor, ReadOnly, EditorDisplay("Meta Data")] private List<string> permittedItemIds = new();
    [ShowInEditor, ReadOnly, EditorDisplay("Meta Data")] public string ItemID => itemInstance != null? itemInstance.Id : null;
    [ShowInEditor, ReadOnly, EditorDisplay("Meta Data")] public int Amount => itemInstance != null ? itemInstance.Amount : 0;
    [ShowInEditor, ReadOnly, EditorDisplay("Meta Data")] public int Capacity => itemInstance != null ? itemInstance.StackCapacity : int.MaxValue;
    [ShowInEditor, ReadOnly, EditorDisplay("Meta Data")] public bool IsOccupied => itemInstance != null;

    #if FLAX_EDITOR
    [ShowInEditor, Serialize, EditorDisplay("Settings"), EditorOrder(4)] private List<JsonAssetReference<ItemClass>> PermittedItems 
    {   
        get{ return permittedItems; } 
        set
        { 
            permittedItems = value;
            SetPermittedItemIds(value);
        } 
    }
    private List<JsonAssetReference<ItemClass>> permittedItems = new();

    private void SetPermittedItemIds(List<JsonAssetReference<ItemClass>> itemClassAssets)
    {
        if(permittedItemIds == null)
            permittedItemIds = new();

        permittedItemIds.Clear();

        for (int i = 0; i < itemClassAssets.Count; i++)
        {
            if( itemClassAssets[i].Instance == null )
                continue;
            
            string newID = itemClassAssets[i].Instance.Id;
            int index = permittedItemIds.FindIndex(itemId => itemId == newID);
            if( index != -1 )
                continue;
        
            permittedItemIds.Add(newID); 
        }   
    }
#endif

    /// <summary>Tries to add a given item to an item slot or increases the amount of the item present in a given slot</summary>
    /// <param name="newItemInstance">The item instance to add</param>
    /// <param name="returnItemInstance">Method returns an item instance when the slots capacity is too small. Null if item is fully added.</param>
    /// <returns>true if the item was added or amount was increased. Returns false when item isn't allowed or stored items don't match.</returns> 
    public bool TryAddItem(ItemInstance newItemInstance, out ItemInstance returnItemInstance)
    {
        returnItemInstance = newItemInstance;

        if (Capacity == 0 || (IsOccupied && !ItemID.Equals(newItemInstance.Id)) || !IsPermittedItem(newItemInstance))
            return false;

        itemInstance ??= newItemInstance.Clone(0);                              
        int addedAmount = Math.Min(itemInstance.StackCapacity, newItemInstance.Amount);

        itemInstance.SetAmount(itemInstance.Amount + addedAmount);
        newItemInstance.SetAmount(newItemInstance.Amount - addedAmount);

        returnItemInstance = newItemInstance.Amount <= 0 ? null : newItemInstance;
        return true;
    }

    /// <summary>Tries to remove an amount of this item form this slot or removes the item itself from the slot, if the amount get's to 0.</summary>
    /// <param name="amount">The amount to remove</param>
    /// <param name="returnItemInstance">This outputs the item instance that is stored in this slot</param>
    /// <returns>true if an amount or the item instance itself was removed. False if no item instance was found (slot is not occupied).</returns>
    public bool TryRemoveItem(int amount, out ItemInstance returnItemInstance)
    {
        // Default
        returnItemInstance = null;

        // Check if slot is occupied
        if (itemInstance == null)
            return false;

        if (0 < Amount - amount)
        {
            returnItemInstance = itemInstance.Clone(amount);
            itemInstance.SetAmount(Amount - amount);
            return true;
        }

        // If the stored amount is 0, we clear the slot
        returnItemInstance = itemInstance;
        itemInstance = null;
        return true;
    }

    /// <summary>Check if an item is permitted in this item slot</summary>
    /// <param name="item">The item to check</param>
    /// <returns>true if permitted. False if not</returns>
    public bool IsPermittedItem(ItemInstance item)
    {
        if(item == null)
            return false;

        if(permittedItemIds == null || permittedItemIds.Count == 0)
            return true;

        // TODO: PermittedQuery, with the permittedItemIds list inside and properties that listen to that
        //      But is that the right approach?

        return permittedItemIds.Contains(item.Id);
    }
}
