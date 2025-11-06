// Copyright © 2024 - 2025 Thomas Jungclaus. All rights reserved. Released under the MIT License.

using System.Collections.Generic;

namespace Backpack;

/// <summary>ItemQueryEmbeddedItemComponent Script.</summary>
public class EmbeddedItemQuery : ItemQuery
{
    public HashSet<string> ItemIds { get; private set; } = new();

    public void AddItemId( string id )
    {
        if( ItemIds == null )
            ItemIds = new();

        ItemIds.Add( id );
    }

    public bool ContainsId(string id) => ItemIds.Contains(id);

    public EmbeddedItemQuery(ItemInstance itemInstance)
    {
        ItemIds = new();
        ItemIds.Add( itemInstance?.Id );
    }
}
