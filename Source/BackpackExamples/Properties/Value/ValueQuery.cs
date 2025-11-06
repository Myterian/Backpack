// Copyright © 2024 - 2025 Thomas Jungclaus. All rights reserved. Released under the MIT License.

namespace Backpack;

/// <summary>ItemValueQuery Script.</summary>
public class ValueQuery : ItemQuery
{
    public int Value { get{ return ValueBase + ( ValueBase * ValueScale / 100 ); }}
    private int ValueBase = 0;
    private int ValueScale = 0;

    /// <summary>Add a fixed amount to the value of an item. Can be negative.</summary>
    /// <param name="value">The amount to add</param>
    public void AddBaseValue(int value) => ValueBase += value;

    /// <summary>Increase or decrease an items value in a relative manner. Can be negative.</summary>
    /// <param name="percent">The bonus in percent</param>
    public void AddValueScale(int percent) => ValueScale += percent;
}
