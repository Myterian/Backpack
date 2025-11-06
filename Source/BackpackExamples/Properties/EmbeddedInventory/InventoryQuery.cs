// Copyright © 2024 - 2025 Thomas Jungclaus. All rights reserved. Released under the MIT License.

using System.Collections.Generic;

namespace Backpack;

/// <summary>ItemQueryInventory Script.</summary>
public class InventoryQuery : ItemQuery
{
    public List<Inventory> Inventories { get; private set; } = new();

    public void AddInventory(Inventory inventory) => Inventories.Add(inventory);
}
