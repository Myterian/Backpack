// Copyright © 2024 - 2025 Thomas Jungclaus. All rights reserved. Released under the MIT License.

using System;
using System.Collections.Generic;

namespace Backpack;

/// <summary>ItemQueryAll class.</summary>
public class MultiQuery : ItemQuery
{
    public HashSet<ItemQuery> QueryComponents = new();

    public override ItemQuery TryGetQuery(Type type)
    {
        if(GetType() == type)
            return this;

        foreach (var item in QueryComponents)        
            if(item.GetType() == type)
                return item;
        
        return null;
    }
}
