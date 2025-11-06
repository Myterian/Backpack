// Copyright © 2024 - 2025 Thomas Jungclaus. All rights reserved. Released under the MIT License.

using System;

namespace Backpack;

/// <summary>ItemPropertyBase Script.</summary>
[Serializable]
public abstract class ItemPropertyBase
{
#if FLAX_EDITOR
    /// <summary>The property name that is displayed in the editor</summary>
    public virtual string PropertyName { get; private set; } = "Default Name";
#endif

    /// <summary>Indicates if a property is shared over multiple instances. I.e. value and weight are the same for every instance, but name override is not.</summary>
    public virtual bool IsShared => true;

    public abstract void TryPerform(ItemInteraction interaction);

    public abstract ItemPropertyBase Clone();
}
