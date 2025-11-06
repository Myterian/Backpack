// Copyright © 2024 - 2025 Thomas Jungclaus. All rights reserved. Released under the MIT License.

#if FLAX_EDITOR
using FlaxEditor.CustomEditors;
using FlaxEditor.GUI;
using FlaxEngine;
using FlaxEngine.GUI;
using FlaxEditor.Windows.Assets;
using FlaxEditor;
using FlaxEditor.Options;

namespace Backpack;

/// <summary>Item Editor Window. Mainly needed for refreshing the thumbnail on save.</summary>
public class ItemWindow : AssetEditorWindowBase<JsonAsset>
{
    private ItemObjectItem itemAsset;
    private JsonAssetReference<ItemClass> itemObject;

    private ToolStrip toolStrip;
    private ToolStripButton saveButton;

    private CustomEditorPresenter contentPresenter;

    public override void Save()
    {
        if (!IsEdited)
            return;

        if (Editor.SaveJsonAsset(itemAsset.Path, itemObject.Instance))
        {
            Debug.LogWarning("Could not save item");
            return;
        }

        itemAsset.RefreshThumbnail();
        saveButton.Enabled = false;
        SetTitle();
        ClearEditedFlag();
    }

    private void ContentHightlight()
    {
        Editor.Windows.ContentWin.Select(itemAsset);
    }

    private void SetTitle()
    {
        string newTitle = IsEdited ? itemAsset.ShortName + "*" : itemAsset.ShortName;
        Title = newTitle;
    }

    public ItemWindow(Editor editor, ItemObjectItem newItem) : base(editor, newItem)
    {
        itemAsset = newItem;

        // Load Asset data
        JsonAsset jsonAsset = Content.LoadAsync<JsonAsset>(itemAsset.Path);
        jsonAsset.WaitForLoaded();
        itemObject = new(jsonAsset);

        // Toolstrip
        toolStrip = new();
        toolStrip.Parent = this;
        toolStrip.AddButton(editor.Icons.Search64, ContentHightlight);

        saveButton = toolStrip.AddButton(editor.Icons.Save64, Save);
        saveButton.Enabled = false;

        Panel panel = new(ScrollBars.Vertical)
        {
            AnchorPreset = AnchorPresets.StretchAll,
            Offsets = new Margin(0, 0, toolStrip.Bottom, 0),
            Parent = this
        };

        // Window contents
        contentPresenter = new(null, "Nothing selected");
        contentPresenter.Panel.Parent = panel;
        contentPresenter.Modified += MarkAsEdited;
        contentPresenter.Modified += () => saveButton.Enabled = true;
        contentPresenter.Select(itemObject.Instance);

        // Shortcut
        InputActions.Add((InputOptions options) => options.Save, Save);
        SetTitle();
    }
}
#endif
