// Copyright © 2024 - 2025 Thomas Jungclaus. All rights reserved. Released under the MIT License.

namespace Backpack;

/// <summary>ItemOverrideNameProperty Script.</summary>
public class NameOverrideProperty : ItemPropertyBase
{
#if FLAX_EDITOR
    public override string PropertyName => "Name Override";
#endif

    public string DisplayName = "New Name";
    public override bool IsShared => false;



    public override void TryPerform(ItemInteraction interaction)
    {
        if (interaction.TryGetQuery<NameQuery>() is NameQuery query)
            query.Name = DisplayName;
    }

    public override ItemPropertyBase Clone()
    {
        return new NameOverrideProperty() { DisplayName = DisplayName };
    }

}
