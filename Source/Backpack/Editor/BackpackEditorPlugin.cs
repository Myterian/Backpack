// Copyright © 2024 - 2025 Thomas Jungclaus. All rights reserved. Released under the MIT License.

#if FLAX_EDITOR
using FlaxEditor;

namespace Backpack;


public class BackpackEditorPlugin : EditorPlugin
{
    ItemClassProxy itemProxy;
    ItemDatabaseProxy dbProxy;

    public override void Initialize()
    {
        base.Initialize();

        itemProxy = new();
        dbProxy = new();

        Editor.ContentDatabase.AddProxy(itemProxy);
        Editor.ContentDatabase.AddProxy(dbProxy);
    }

    public override void Deinitialize()
    {
        if (itemProxy != null)
            Editor.ContentDatabase.RemoveProxy(itemProxy);

        if(dbProxy != null)
            Editor.ContentDatabase.RemoveProxy(dbProxy);

        base.Deinitialize();
    }

    public BackpackEditorPlugin()
    {
        _description = new()
        {
            Name = "Backpack - Editor Stuff",
            Description = "Editor assets and scene icons for Backpack - Flax Engine Inventory System",
            Author = "Thomas Jungclaus",
            Category = "Backpack",
            IsAlpha = false,
            IsBeta = false,
            Version = InventoryManager.PluginVersion
        };
    }


}
#endif