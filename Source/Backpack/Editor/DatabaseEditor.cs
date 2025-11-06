// Copyright © 2024 - 2025 Thomas Jungclaus. All rights reserved. Released under the MIT License.

#if FLAX_EDITOR
using FlaxEditor.CustomEditors;
using FlaxEditor.CustomEditors.Editors;
using FlaxEditor.GUI;
using FlaxEngine;
using FlaxEngine.GUI;

namespace Backpack;

/// <summary>Editor for item database assets</summary>
[CustomEditor(typeof(ItemDatabase))]
public class DatabaseEditor : GenericEditor
{
    ClickableLabel buildSuccessLabel;

    public override void Initialize(LayoutElementsContainer layout)
    {
        base.Initialize(layout);

        var collectButton = layout.Button("Collect Items from Content");
        var buildButton = layout.Button("Build Database");

        collectButton.Button.ButtonClicked += CollectItemsClicked;
        buildButton.Button.ButtonClicked += BuildDatabaseClicked;


        var buildLabel = layout.Label("Build Succeeded!");
        buildSuccessLabel = buildLabel.Label;
        buildSuccessLabel.HorizontalAlignment = TextAlignment.Center;
        buildSuccessLabel.TextColor = Color.LightYellow;
        buildSuccessLabel.Visible = false;
    }

    private void CollectItemsClicked(Button button)
    {
        ItemDatabase activeDatabase = (ItemDatabase)Values[0];
        activeDatabase.LoadItemAssetsToList();

        SetValue(activeDatabase);
    }

    private void BuildDatabaseClicked(Button button)
    {
        ItemDatabase activeDatabase = (ItemDatabase)Values[0];
        activeDatabase.BuildDatabase();
        
        buildSuccessLabel.Text = $"Build Succeeded! {activeDatabase.ItemByID.Count} Items are now build into the database";
        buildSuccessLabel.Visible = true;

        SetValue(activeDatabase);
    }
}
#endif