// Copyright © 2024 - 2025 Thomas Jungclaus. All rights reserved. Released under the MIT License.

using FlaxEngine;

namespace Backpack;

/// <summary>ItemEmbeddedProperty Script.</summary>
public class EmbeddedItemProperty : ItemPropertyBase
{
#if FLAX_EDITOR
    public override string PropertyName => "Embedded Item";

    [ShowInEditor, NoSerialize]
    public JsonAssetReference<ItemClass> Item
    {
        get => item;
        set
        {
            id = value == null ? null : value.Instance.Id;
            item = value;
        }
    }

    private JsonAssetReference<ItemClass> item = null;
#endif

    [ShowInEditor, Serialize, ReadOnly] private string id;
    [ShowInEditor, Serialize] public int Amount { get; private set; } = 1;
    

    public override void TryPerform(ItemInteraction interaction)
    {
        if(string.IsNullOrEmpty(id))
        {
            Debug.Log("Item id was not set in Embedded Item Property. Maybe item reference is missing. Pls fix.");
            return;
        }
        
        EmbeddedItemQuery query = interaction.TryGetQuery<EmbeddedItemQuery>();

        // We're checking if this item is the same as the parent item, to prevent an infinite loop of queries to happen
        if (query == null || query.ContainsId(id))
            return;

        query.AddItemId(id);
    }

    public override ItemPropertyBase Clone()
    {
        return new EmbeddedItemProperty() { id = id, Amount = Amount };
    }

}
