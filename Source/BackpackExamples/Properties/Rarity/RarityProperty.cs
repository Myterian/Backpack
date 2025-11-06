// Copyright © 2024 - 2025 Thomas Jungclaus. All rights reserved. Released under the MIT License.

namespace Backpack;

/// <summary>ItemCommonStatusProperty</summary>
public class RarityProperty : ItemPropertyBase
{
#if FLAX_EDITOR
    public override string PropertyName => "Rarity";
#endif

    public RarityEnum Rarity = RarityEnum.Common;

    public override void TryPerform(ItemInteraction interaction)
    {
        if(interaction.TryGetQuery<RarityQuery>() is not RarityQuery rarityQuery)
            return;
        
        // Make sure only higher rarities than the current rarities are picked
        if( (int)Rarity <= (int)rarityQuery.itemRarity )
            return;

        rarityQuery.itemRarity = Rarity;
    }

    public override ItemPropertyBase Clone()
    {
        return new RarityProperty() { Rarity = Rarity };
    }
}
