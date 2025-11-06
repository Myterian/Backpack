// Copyright © 2024 - 2025 Thomas Jungclaus. All rights reserved. Released under the MIT License.

namespace Backpack;

/// <summary>ItemValueProperty Script.</summary>
public class ScalingValueProperty : ItemPropertyBase
{
#if FLAX_EDITOR
    public override string PropertyName => "Value Bonus";
#endif

    public int Percent = 10;

    public override void TryPerform(ItemInteraction interaction)
    {
        if(interaction.TryGetQuery<ValueQuery>() is ValueQuery valueQuery)
            valueQuery.AddValueScale(Percent);
    }

    public override ItemPropertyBase Clone()
    {
        return new ScalingValueProperty() { Percent = Percent };
    }

}

