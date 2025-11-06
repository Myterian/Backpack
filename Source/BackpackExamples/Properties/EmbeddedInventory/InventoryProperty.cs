// Copyright © 2024 - 2025 Thomas Jungclaus. All rights reserved. Released under the MIT License.

using System.Collections.Generic;
using FlaxEngine;

namespace Backpack;

/// <summary>ItemInventoryProperty Script.</summary>
public class InventoryProperty : ItemPropertyBase
{
#if FLAX_EDITOR
    public override string PropertyName => "Embedded Inventory";
#endif

    public override bool IsShared => false;

    [ShowInEditor, Serialize, ReadOnly] private Inventory inventory;
    [ShowInEditor, Serialize] private LocalizedString title = new();
    [ShowInEditor, Serialize] private Texture icon = null;
    [ShowInEditor, Serialize] private InventorySettingsEnum preferences = InventorySettingsEnum.None;
    [ShowInEditor, Serialize] private int itemSlotCount = 3;

    public override void TryPerform(ItemInteraction interaction)
    {
        if (interaction.TryGetQuery<InventoryQuery>() is not InventoryQuery query)
            return;

        if (inventory == null)
        {
            inventory = new();

            List<InventoryItemSlot> itemSlots = new();
            for (int i = 0; i < itemSlotCount; i++)
                itemSlots.Add(new InventoryItemSlot());

            // Black magic
            // Prevents the use of permission list by unsetting relevant bits
            preferences &= ~(InventorySettingsEnum.PermittedItemsOnly | InventorySettingsEnum.ExcludedItemsOnly);

            inventory.SetPreferences(preferences);
            inventory.SetItemSlots(itemSlots);
            inventory.SetTitle(title);
            inventory.SetIcon(icon);

            Actor actor = interaction.Sender as Actor;
            inventory.Parent = actor ?? Level.Scenes[0];
        }

        query.AddInventory(inventory);
    }

    public override ItemPropertyBase Clone()
    {
        return new InventoryProperty(){ title = title, icon = icon, itemSlotCount = itemSlotCount, preferences = preferences };
    }

}
