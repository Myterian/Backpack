// Copyright © 2024 - 2025 Thomas Jungclaus. All rights reserved. Released under the MIT License.

using System;
using System.Collections.Generic;

namespace Backpack;

/// <summary>ItemInteraction class.</summary>
public class ItemInteraction
{
    public Dictionary<Type, ItemQuery> Queries { get; }
    public object Sender { get; }
    public object Target { get; }

    /// <summary>Tries to find a specific item query in this interaction</summary>
    /// <typeparam name="T">Query Type</typeparam>
    /// <returns>The found query. Null if failed</returns>
    public T TryGetQuery<T>() where T : ItemQuery
    {
        Type type = typeof(T);

        if (Queries.TryGetValue(type, out ItemQuery query))
            return (T)query;

        return null;
    }

    /// <summary>Adds an item query to this interaction</summary>
    /// <param name="query">The query to add</param>
    public void AddQuery(ItemQuery query) => Queries[query.GetType()] = query;


    public ItemInteraction(object sender = null, object target = null, Dictionary<Type, ItemQuery> queries = null)
    {
        Sender = sender;
        Target = target;
        Queries = queries ?? new();
    }
}
