// Copyright © 2024 - 2025 Thomas Jungclaus. All rights reserved. Released under the MIT License.

using System;
using System.Collections.Generic;
using FlaxEngine;

#if FLAX_EDITOR
using FlaxEditor;
using FlaxEditor.SceneGraph;
#endif

namespace Backpack;

/// <summary>Inventory</summary>
[ActorContextMenu("New/Backpack/Inventory")]
public class Inventory : EmptyActor
{
    private List<InventoryItemSlot> itemSlots = new();

    [ShowInEditor, Serialize, EditorDisplay("Settings"), EditorOrder(1)] public LocalizedString Title { get; private set; } = new();
    [ShowInEditor, Serialize, EditorDisplay("Settings"), EditorOrder(2)] public Texture Icon { get; private set; } = null;
    [ShowInEditor, Serialize, EditorDisplay("Settings"), EditorOrder(3)] public InventorySettingsEnum Preferences = InventorySettingsEnum.None;

    [ShowInEditor, Serialize, EditorDisplay("Data"), EditorOrder(9), Collection(Display = CollectionAttribute.DisplayType.Header, Spacing = 25f)]
    public List<InventoryItemSlot> ItemSlots
    {
        get => itemSlots;
        private set
        {
            for (int i = 0; i < value.Count; i++)
                value[i] ??= new();

            itemSlots = value;
        }
    }

#if FLAX_EDITOR
    [ShowInEditor, Serialize, EditorDisplay("Settings"), EditorOrder(4)]
    private List<JsonAssetReference<ItemClass>> PermittedItems
    {
        get => permittedItems;
        set
        {
            permittedItems = value;
            SetPermittedItemIds(value);
        }
    }
    private List<JsonAssetReference<ItemClass>> permittedItems = new();
#endif

    // Events
    /// <summary>Callback event for when an item is added to the inventory. Provides the id of the changed slot.</summary>
    public event Action<int> OnItemAdded;

    /// <summary>Callback event for when an item is removed from the inventory. Provides the id of the changed slot and the item instance that was removed.</summary>
    public event Action<int> OnItemRemoved;

    /// <summary>Callback event for when an item is added or removed from the inventory. Provides the id of the changed slot.</summary>
    public event Action<int> OnItemChanged;

    // A list with permitted item ids, that is used in game for comparison when trying to add an item. List is created on saving scene and uses 'PermittedItems' as reference
    [ShowInEditor, Serialize, ReadOnly, EditorDisplay("Data"), EditorOrder(10)] public List<string> PermittedItemIds { get; private set; } = new();

    /// <summary>
    /// Set an item to the inventory. If the item can not be added to the specified slot, the item will be added to the first available slot or discarded if no alternative slots are available.
    /// Use <see cref="AddItem"/> if you want to make sure the item is permitted in this inventory.
    /// </summary>
    /// <param name="item">The item to add</param>
    /// <param name="slotIndex">(Optional) The specific inventory slot to which the item should be added. If the chosen slot is occupied the next available slot will be used.</param>
    /// <returns>true if the item was added. Returns false when failed.</returns> 
    public bool SetItem(ItemInstance item, int slotIndex = -1)
    {
        bool isStackable = !Preferences.HasFlag(InventorySettingsEnum.DontStackItems) && item.MaxStack <= 0;

        // Creating a loop here for when an item is added that is non-stackable, but the amount to add is more than 1

        // NOTE: Here is an interesting side effect of using a loop to add items: The first iteration will try to add
        // an item to the specified slot index (if it is not -1). But if that fails, the second iteration will run
        // and check if the item type is present in the inventory or if we have an empty slot.
        //
        // This means we add an item to the inventory (if the space allows it), even if the slot we're trying to
        // add the item to is occupied with a different item type.
        int safety = 0;
        while (0 < item.Amount && safety++ < 100)
        {
            int slotIndexCorrected = slotIndex;

            // Check if the item type is already in inventory, if the item is stackable
            if (isStackable && slotIndexCorrected == -1)
                slotIndexCorrected = ItemSlots.FindIndex(itemSlot => itemSlot?.ItemID == item.Id && 0 < itemSlot.Capacity);


            // Get empty slot, if item type is new in the inventory or if the item is not stackable
            if (slotIndexCorrected == -1)
                slotIndexCorrected = ItemSlots.FindIndex(itemSlot => itemSlot?.itemInstance == null);


            // Get slot has failed, i.e. because inventory is full and this item isn't stackable
            if (slotIndexCorrected == -1)
            {
                // Check if we can extend the inventory
                if (!Preferences.HasFlag(InventorySettingsEnum.AutoExtend))
                    return false;

                // Extend the inventory
                ItemSlots.Add(new InventoryItemSlot());
                slotIndexCorrected = ItemSlots.Count - 1;
            }

            // -------
            // If we get to here, then a slot has been found that: Is either empty (instance == null) or is not full (itemSlot.Amount < item.MaxStack)

            // Try add the item. If it's stackable, add entire amount. If it's not stackable and we want to
            // add more than one, we only add one of the item to the current empty slot and find another 
            // empty slot next iteration
            bool isItemAdded = ItemSlots[slotIndexCorrected].TryAddItem(item, out ItemInstance returnInstance);

            // Invoke events i.e. for ui
            if (isItemAdded)
            {
                OnItemAdded?.Invoke(slotIndexCorrected);
                OnItemChanged?.Invoke(slotIndexCorrected);
            }

            if (returnInstance == null)
                return true;

            // Prepare next iteration
            item = returnInstance;
            slotIndex = -1;
        }

        // We successfully finished the loop
        return true;
    }

    /// <summary>
    /// Adds an item to the inventory, if the item is permitted in the inventory. 
    /// If the item can not be added to the specified slot, the item will be added to the first available slot or discarded if no alternative slots are available.
    /// Use <see cref="SetItem"/> to skip the item permission check.
    /// </summary>
    /// <param name="item">The item to add</param>
    /// <param name="slotIndex">(Optional) The specific inventory slot to which the item should be added. If the chosen slot is occupied the next available slot will be used.</param>
    /// <returns>true if the item was added. Returns false when failed.</returns> 
    public bool AddItem(ItemInstance item, int slotIndex = -1)
    {
        // Make sure the item we're trying to add is permitted in this inventory
        if (!IsPermittedItem(item))
            return false;

        return SetItem(item, slotIndex);
    }

    /// <summary>Removes an item or amount from a specific inventory slot.</summary>
    /// <param name="slotIndex">The slot to remove from</param>
    /// <param name="amount">The amount to remove</param>
    public ItemInstance RemoveItem(int slotIndex, int amount = 1)
    {
        // Check if slot is occupied
        bool inRange = 0 <= slotIndex && slotIndex <= ItemSlots.Count - 1;
        if (!inRange || ItemSlots[slotIndex] == null)
            return null;

        // Try remove item
        bool isRemoved = ItemSlots[slotIndex].TryRemoveItem(amount, out ItemInstance returnedInstance);

        if (!isRemoved)
            return null;

        // Invoke events i.e. for ui
        OnItemRemoved?.Invoke(slotIndex);
        OnItemChanged?.Invoke(slotIndex);
        return returnedInstance;
    }

    /// <summary>Removes the first item with a given id.</summary>
    /// <param name="id">Id of the item</param>
    /// <param name="amount">The amount to remove</param>
    public void RemoveItem(string id, int amount = 1)
    {
        int removeIndex = ItemSlots.FindIndex(itemSlot => itemSlot?.ItemID == id);
        RemoveItem(removeIndex, amount);
    }

    /// <summary>
    /// Sets the content of the inventory to a specific list. 
    /// Original contents get deleted, use with caution. Useful for loading savegames. 
    /// Does not check for permitted/non-permitted items.
    /// </summary>
    /// <param name="newSlots">New inventory item slots</param>
    public void SetItemSlots(List<InventoryItemSlot> newSlots)
    {
        // Replace inventory slots
        ItemSlots.Clear();
        ItemSlots.AddRange(newSlots);

        // Invoke events i.e. for ui
        OnItemChanged?.Invoke(-1);
    }

    /// <summary>Removes all item instances from inventory. Use with caution, as all contents are deleted. Used in crafting for removing ingredients.</summary>
    public void ClearAllItems()
    {
        for (int i = 0; i < ItemSlots.Count; i++)
            ItemSlots[i].TryRemoveItem(int.MaxValue, out ItemInstance returnInstance);

        OnItemChanged?.Invoke(-1);
    }

    /// <summary>Check if an item (or one of it's embedded items) is permitted in this invententory</summary>
    /// <param name="item">The item to check</param>
    /// <returns>true if permitted. False if not</returns>
    public bool IsPermittedItem(ItemInstance item)
    {
        if (!Preferences.HasFlag(InventorySettingsEnum.PermittedItemsOnly) && !Preferences.HasFlag(InventorySettingsEnum.ExcludedItemsOnly))
            return true;

        if (item == null)
            return false;

        return PermittedItemIds.Contains(item.Id);
    }

    // Converts the editor only list of json assets to the runtime list of permitted item ids
    private void SetPermittedItemIds(List<JsonAssetReference<ItemClass>> itemClassAssets)
    {
        if (PermittedItemIds == null)
            PermittedItemIds = new();

        PermittedItemIds.Clear();

        for (int i = 0; i < itemClassAssets.Count; i++)
        {
            if (itemClassAssets[i].Instance == null)
                continue;

            string newID = itemClassAssets[i].Instance.Id;
            int index = PermittedItemIds.FindIndex(itemId => itemId == newID);
            if (index != -1)
                continue;

            PermittedItemIds.Add(newID);
        }
    }

    /// <summary>Sets the inventory's title</summary>
    /// <param name="titleString">Localized string containing title</param>
    public void SetTitle(LocalizedString titleString) => Title = titleString;

    /// <summary>Sets the inventory's icon</summary>
    /// <param name="icon">Icon texture</param>
    public void SetIcon(Texture icon) => Icon = icon;

    /// <summary>Sets the inventory's preferences</summary>
    /// <param name="preferences">The active preferences</param>
    public void SetPreferences(InventorySettingsEnum preferences) => Preferences = preferences;


#if FLAX_EDITOR
    // Show Icon in the Scene View ===================================

    public override void OnEnable()
    {
        // Show Icon in the Scene View
        ViewportIconsRenderer.AddActor(this);
    }

    public override void OnDisable()
    {
        // Remove Icon in the Scene View
        ViewportIconsRenderer.RemoveActor(this);
    }

    static Inventory()
    {
        ViewportIconsRenderer.AddCustomIcon(typeof(Inventory), Content.LoadAsync<Texture>("099e7c9f4d9e1df31a1ab3ba3df72bd0".GuidFromAssetID()));
        SceneGraphFactory.CustomNodesTypes.Add(typeof(Inventory), typeof(InventoryActorNode));
    }

    /// <summary>Custom actor node for Editor.</summary>
    public sealed class InventoryActorNode : ActorNodeWithIcon
    {
        public InventoryActorNode(Actor actor) : base(actor) { }
    }
#endif
}
