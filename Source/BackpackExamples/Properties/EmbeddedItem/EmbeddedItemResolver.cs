// Copyright © 2024 - 2025 Thomas Jungclaus. All rights reserved. Released under the MIT License.

using System.Collections.Generic;

namespace Backpack;

/// <summary>Extension methods for item instance class, to check item ids of embedded items.</summary>
public static class EmbeddedItemResolver
{
    /// <summary>Check if the id of this item or any embedded item is on a list of ids. Useful when i.e. checking if an item is permitted in an inventory, based on the embedded items.</summary>
    /// <param name="instance">The item instance to check</param>
    /// <param name="permittedIds">The list of ids to check against</param>
    /// <returns>true if this item id or an embedded item id is on the list, false if not</returns>
    public static bool IsItemPermittedRecursive(this ItemInstance instance, List<string> permittedIds)
    {
        ItemInteraction interaction = new();
        EmbeddedItemQuery query = new(instance);

        interaction.AddQuery(query);
        instance.QueryProperties(interaction);

        for (int i = 0; i < permittedIds.Count; i++)
            if (query.ContainsId(permittedIds[i]))
                return true;

        return false;
    }
}
