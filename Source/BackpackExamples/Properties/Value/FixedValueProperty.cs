// Copyright © 2024 - 2025 Thomas Jungclaus. All rights reserved. Released under the MIT License.

namespace Backpack;

/// <summary>ItemValueProperty Script.</summary>
public class FixedValueProperty : ItemPropertyBase
{
#if FLAX_EDITOR
    public override string PropertyName => "Fixed Value";
#endif

    public int Amount = 10;

    public override void TryPerform(ItemInteraction interaction)
    {
        if(interaction.TryGetQuery<ValueQuery>() is ValueQuery valueQuery)
            valueQuery.AddBaseValue(Amount);
    }

    public override ItemPropertyBase Clone()
    {
        return new FixedValueProperty() { Amount = Amount };
    }

}
