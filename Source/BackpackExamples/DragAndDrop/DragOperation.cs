// Copyright © 2024 - 2025 Thomas Jungclaus. All rights reserved. Released under the MIT License.

namespace Backpack;

/// <summary>DragOperation class.</summary>
public class DragOperation
{
    public Inventory SourceInventory { get; }
    public int SourceSlotIndex { get; }

    public ItemInstance Item => item ??= SourceInventory?.ItemSlots[SourceSlotIndex]?.itemInstance;
    private ItemInstance item;

    // public void TryDropItem(Inventory target, int targetSlot = -1)
    // {
    //     bool isAdded = target.AddItem(Item, targetSlot);
    //     SourceInventory.RemoveItem(SourceSlotIndex, 1);
    // }

    public void TryDropItems(Inventory target, int targetSlot)
    {
        if (Item == null || target == null)
            return;

        
        int itemAmount = Item.Amount;
        target.AddItem(Item, targetSlot);

        SourceInventory.RemoveItem(SourceSlotIndex, itemAmount);
    }

    // public void TryDropHalfItems(Inventory target, int targetSlot)
    // {
    //     int itemAmount = SourceInventory.ItemSlots[SourceSlotIndex].Amount / 2;
    //     target.AddItem(Item, targetSlot);

    //     SourceInventory.RemoveItem(SourceSlotIndex, itemAmount);
    // }

    public DragOperation(Inventory source, int sourceSlotIndex)
    {
        SourceInventory = source;
        SourceSlotIndex = sourceSlotIndex;
    }
}
