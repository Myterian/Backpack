// Copyright © 2024 - 2025 Thomas Jungclaus. All rights reserved. Released under the MIT License.

using System;

namespace Backpack;

/// <summary>InventorySettingsEnum. Used in the inventory class to decide certain behaviours.</summary>
[Flags]
public enum InventorySettingsEnum 
{
    None = 0,
    AutoExtend = 1,
    PermittedItemsOnly = 2,
    ExcludedItemsOnly = 4,
    DontStackItems = 8
}
