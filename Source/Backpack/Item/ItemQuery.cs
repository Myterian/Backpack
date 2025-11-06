// Copyright © 2024 - 2025 Thomas Jungclaus. All rights reserved. Released under the MIT License.

using System;

namespace Backpack;

/// <summary>ItemQuery Script.</summary>
public abstract class ItemQuery
{
    public virtual ItemQuery TryGetQuery(Type type)
    {
        if (GetType() == type)
            return this;

        return null;
    }
}