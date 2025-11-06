// Copyright © 2024 - 2025 Thomas Jungclaus. All rights reserved. Released under the MIT License.

#if FLAX_EDITOR
using System;
using FlaxEditor.Content;
using FlaxEngine;

namespace Backpack;

[HideInEditor] // Hides this class in the ui control type selection, because it shows up there for some fucky reason
public class DatabaseObjectItem : JsonAssetItem
{
    public DatabaseObjectItem(string path, Guid id, string typeName) : base(path, id, typeName)
    {
        SpriteAtlas texture = Content.LoadAsync<SpriteAtlas>("7c45e3ff41d94cd7ed4bda9de02497fa".GuidFromAssetID());
        if (!texture || texture.WaitForLoaded())
            return;

        _thumbnail = texture.FindSprite("Database");
    }
}

[ContentContextMenu("New/Backpack/New Item Database")]
public class ItemDatabaseProxy : SpawnableJsonAssetProxy<ItemDatabase>
{
    public override string NewItemName => "New Database";

    public override Color AccentColor => Color.Orange;


    public override AssetItem ConstructItem(string path, string typeName, ref Guid id)
    {
        return new DatabaseObjectItem(path, id, typeName);
    }
}
#endif