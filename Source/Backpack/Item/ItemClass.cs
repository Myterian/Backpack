// Copyright © 2024 - 2025 Thomas Jungclaus. All rights reserved. Released under the MIT License.

using System.Collections.Generic;
using FlaxEngine;

namespace Backpack;

/// <summary>Basis for items. Stores default name, description, icon, properties, etc.</summary>
public class ItemClass
{
    [EditorDisplay("Item Data"), EditorOrder(0), Tooltip("In-game display name of the item")]
    public LocalizedString Name;

    [EditorDisplay("Item Data"), EditorOrder(1), MultilineText, Tooltip("In-game description of the item")]
    public LocalizedString Description;

    [EditorDisplay("Item Data"), EditorOrder(2), Tooltip("Icon for in-game display")]
    public Texture Icon;

    [EditorDisplay("Item Data"), EditorOrder(3), Tooltip("Prefab for world representation of the item. Make sure the prefab links back to this item, when using a form of pickup.")]
    public Prefab Prefab;

    [EditorDisplay("Database"), EditorOrder(-1), ReadOnly, Tooltip("Internal item id. Set when a Item Database compiles this item.")]
    public string Id;

    [EditorDisplay("Item Data"), EditorOrder(6), Tooltip("Max amount a stack of this item can have. Set to 0 or below for infinite stacking.")]
    public int MaxStack = 0;

    [EditorDisplay("Properties"), EditorOrder(7), Collection(CanResize = true, Display = CollectionAttribute.DisplayType.Header, NotNullItems = true), ExpandGroups, Tooltip("Modular properties of an item. Extent however you like.")]
    public List<ItemPropertyBase> Properties = new();

    [HideInEditor] public bool IsCompiledInDatabase = false;

    // Contructor
    public ItemClass()
    {
        Name = "Default Cube";
        Description = "If you read this, something went wrong. Tell Allan to add details, pls.";
        Properties = new();
    }
}
