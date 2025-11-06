// Copyright © 2024 - 2025 Thomas Jungclaus. All rights reserved. Released under the MIT License.

#if FLAX_EDITOR
using FlaxEditor.CustomEditors;
using FlaxEditor.CustomEditors.Editors;
using FlaxEngine;

namespace Backpack;

/// <summary>ItemPropertyEditor Script.</summary>
[CustomEditor(typeof(ItemPropertyBase)), DefaultEditor]
public class ItemPropertyEditor : GenericEditor
{
    public override void Initialize(LayoutElementsContainer layout)
    {
        // Get class name and remove Item and Property, as they're only clutter in editor
        // string labelText = Values[0]?.GetType().Name;
        // labelText = labelText.Replace("Item", "");
        // labelText = labelText.Replace("Property", "");

        string labelText = (Values[0] as ItemPropertyBase).PropertyName;

        // Create label
        var label = layout.Label(labelText, TextAlignment.Near);
        label.Label.Enabled = false;

        // Base ui
        base.Initialize(layout);
    }
}
#endif