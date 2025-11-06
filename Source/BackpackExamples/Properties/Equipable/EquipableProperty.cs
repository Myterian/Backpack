// Copyright © 2024 - 2025 Thomas Jungclaus. All rights reserved. Released under the MIT License.

using FlaxEngine;

namespace Backpack;

/// <summary>ItemEquipableProperty class.</summary>
public class EquipableProperty : ItemPropertyBase
{
#if FLAX_EDITOR
    public override string PropertyName => "Equipable Prefab";
#endif

    [ShowInEditor, Serialize] private Prefab equipment = null;
    
    public override void TryPerform(ItemInteraction interaction)
    {
        if (equipment == null) return;
        
        if (interaction.TryGetQuery<EquipableQuery>() is EquipableQuery query)
            query.Equipment = equipment;
    }

    public override ItemPropertyBase Clone()
    {
        return new EquipableProperty() { equipment = equipment };
    }

}
