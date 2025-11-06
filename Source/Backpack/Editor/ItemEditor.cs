// Copyright © 2024 - 2025 Thomas Jungclaus. All rights reserved. Released under the MIT License.

#if FLAX_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using FlaxEditor.CustomEditors;
using FlaxEditor.CustomEditors.Editors;
using FlaxEditor.CustomEditors.Elements;
using FlaxEditor.GUI;
using FlaxEditor.Scripting;
using FlaxEngine;
using FlaxEngine.GUI;

namespace Backpack;

/// <summary>ItemEditor class.</summary>
[CustomEditor(typeof(ItemClass))]
public class ItemEditor : GenericEditor
{
    ClickableLabel warningLabel = null;

    public override void Initialize(LayoutElementsContainer layout)
    {
        base.Initialize(layout);

        // Create a button under a specific parent if available and listen for user input
        ContainerControl buttonParent = null;
        ContainerControl warningParent = null;
        for (int i = 0; i < layout.ContainerControl.ChildrenCount; i++)
        {
            if (layout.ContainerControl.Children[i] is not DropPanel panel)
                continue;

            if (panel.HeaderText == "Properties")
                buttonParent = panel;

            if (panel.HeaderText == "Database")
                warningParent = panel;
        }

        // Expand the properties list by default. For some reason it's always showes as closed.
        if (buttonParent != null)
        {
            // Right now there is only one child in the properties editor group and it is the properties list.
            // If an item class update affects the properties editor group, this needs to be more sophisticated.
            DropPanel propertiesListPanel = buttonParent.Children[0] as DropPanel;
            propertiesListPanel?.Open();
        }

        buttonParent ??= layout.ContainerControl;

        var addPropertyButton = new ButtonElement();
        var button = addPropertyButton.Button;
        button.Text = "Add Property";
        button.Parent = buttonParent;
        // button.AnchorPreset = AnchorPresets.TopLeft; // Default
        button.ButtonClicked += OnAddPropertyButtonClicked;


        // Warning label that is shown when the item database needs a rebuild to reflect changes made to items
        warningParent ??= layout.ContainerControl;

        var warningLabelElement = new LabelElement();
        warningLabel = warningLabelElement.Label;
        warningLabel.Text = "The item was modified. Please rebuild the containing database to save changes.";
        warningLabel.HorizontalAlignment = TextAlignment.Center;
        warningLabel.TextColor = Color.LightYellow;
        warningLabel.Parent = warningParent;
        warningLabel.Visible = !(Values[0] as ItemClass).IsCompiledInDatabase;
    }

    private void OnAddPropertyButtonClicked(Button button)
    {
        // Get all derived types of ItemPropertyBase and show them in a selection context menu

        // NOTE: This list could probably be cached, but I haven't had any performance issues so far
        Type baseType = typeof(ItemPropertyBase);
        List<Type> types = AppDomain.CurrentDomain.GetAssemblies(). SelectMany(assemly => assemly.GetTypes()).
                                                                    Where(type =>   baseType.IsAssignableFrom(type)
                                                                                    && type != baseType
                                                                                    && !type.IsAbstract).
                                                                    ToList();
        if (types.Count == 0)
            return;


        var contextMenu = new ItemsListContextMenu();
        for (int i = 0; i < types.Count; i++)
        {
            ScriptType propertyType = new(types[i]);
            contextMenu.AddItem(new TypeSearchPopup.TypeItemView(propertyType));
        }

        contextMenu.ItemClicked += item => AddPropertyToItem((ScriptType)item.Tag);
        contextMenu.SortItems();
        contextMenu.Show(button.Parent, button.BottomLeft);
    }

    private void AddPropertyToItem(ScriptType newPropertyType)
    {
        // Add a new item property to the current class
        ItemPropertyBase newProperty = (ItemPropertyBase)Activator.CreateInstance(newPropertyType.Type);
        ItemClass activeItem = (ItemClass)Values[0];
        activeItem.Properties.Add(newProperty);
        activeItem.IsCompiledInDatabase = false;
        warningLabel.Visible = true;

        SetValue(activeItem);
    }
}
#endif