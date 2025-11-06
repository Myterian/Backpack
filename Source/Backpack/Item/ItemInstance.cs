// Copyright © 2024 - 2025 Thomas Jungclaus. All rights reserved. Released under the MIT License.

using System.Collections.Generic;
using FlaxEngine;

namespace Backpack;

/// <summary>
/// Item Instance Class. This is used to represent items in game. 
/// Contrary to the item class, an item instance can be modified in game,
/// where as the item class is only used to store base item data in the database.
/// </summary>
public class ItemInstance
{
    [ShowInEditor] public LocalizedString Name => baseItem.Name;
    [ShowInEditor] public string Description => baseItem.Description;
    [ShowInEditor] public string Id => baseItem.Id;
    [ShowInEditor] public int MaxStack => baseItem.MaxStack;
    [ShowInEditor, ReadOnly] public int StackCapacity => MaxStack <= 0 ? int.MaxValue - Amount : MaxStack - Amount;
    [ShowInEditor] public Texture Icon => baseItem.Icon;
    [ShowInEditor] public Prefab Prefab => baseItem.Prefab;
    [ShowInEditor] public List<ItemPropertyBase> ItemProperties => baseItem.Properties;

    [ShowInEditor] public int Amount { get; private set; } = 1;
    [ShowInEditor] public List<ItemPropertyBase> InstanceProperties { get; private set; }

    private ItemClass baseItem;

    /// <summary>Sets the amount of the item available in this instance</summary>
    /// <param name="newAmount">New amount</param>
    public void SetAmount(int newAmount) => Amount = newAmount;

    /// <summary>Removes the first occurance of an item property from this item instance</summary>
    /// <param name="property">The property to remove</param>
    public void RemoveProperty(ItemPropertyBase property) => InstanceProperties?.Remove(property);

    /// <summary>Removes an item property from this item instance</summary>
    /// <param name="propertyIndex">Index to remove at</param>
    public void RemoveProperty(int propertyIndex) => InstanceProperties?.RemoveAt(propertyIndex);

    /// <summary>Adds a new item property to the properties list of this item instance</summary>
    /// <param name="newProperty">Item property to add</param>
    public void AddProperty(ItemPropertyBase newProperty)
    {
        InstanceProperties ??= new();
        InstanceProperties.Add(newProperty);
    }

    /// <summary>Queries all item properties in this instance</summary>
    /// <param name="interaction">The interaction the items should react to</param>
    public void QueryProperties(ItemInteraction interaction)
    {
        for (int i = 0; i < ItemProperties.Count; i++)
            if(ItemProperties[i].IsShared)
                ItemProperties[i].TryPerform(interaction);

        if (InstanceProperties != null)
            for (int i = 0; i < InstanceProperties.Count; i++)
                InstanceProperties[i].TryPerform(interaction);
    }

    /// <summary>Gets the first item property of type T in the item instance properties list</summary>
    /// <typeparam name="T">ItemPropertyBase and derived types</typeparam>
    /// <returns>First instance of T</returns>
    public T TryGetProperty<T>() where T : ItemPropertyBase
    {
        for (int i = 0; i < ItemProperties.Count; i++)
            if (ItemProperties[i] is T && ItemProperties[i].IsShared)
                return ItemProperties[i] as T;

        if (InstanceProperties != null)
            for (int i = 0; i < InstanceProperties.Count; i++)
                if (InstanceProperties[i] is T)
                    return InstanceProperties[i] as T;

        return null;
    }

    /// <summary>Gets all item properties of type T in the item instance properties list</summary>
    /// <typeparam name="T">ItemPropertyBase and derived types</typeparam>
    /// <returns>List of instances of T. Null if none are found</returns>
    public List<T> TryGetProperties<T>() where T : ItemPropertyBase
    {
        List<T> values = null;

        for (int i = 0; i < ItemProperties.Count; i++)
        {
            if (ItemProperties[i] is not T t || !ItemProperties[i].IsShared)
                continue;

            values ??= new();
            values.Add(t);
        }

        if (InstanceProperties != null)
            for (int i = 0; i < InstanceProperties.Count; i++)
            {
                if (InstanceProperties[i] is not T t)
                    continue;

                values ??= new();
                values.Add(t);
            }

        return values;
    }

    /// <summary>Creates a shallow copy of the item instance, with the same InstanceProperties and a set Amount</summary>
    /// <param name="amount">(Optional) Amount of the clone</param>
    /// <returns>ItemInstance</returns>
    public ItemInstance Clone(int amount = 1)
    {
        ItemInstance newItemInstance = new(baseItem);
        newItemInstance.SetAmount(amount);
        
        return newItemInstance;
    }

    // Constructors
    public ItemInstance(ItemClass itemClass)
    {
        baseItem = itemClass;

        for (int i = 0; i < baseItem.Properties.Count; i++)
            if (!baseItem.Properties[i].IsShared)
                AddProperty(baseItem.Properties[i].Clone());
        
    }
}
