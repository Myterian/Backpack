// Copyright © 2024 - 2025 Thomas Jungclaus. All rights reserved. Released under the MIT License.

#if FLAX_EDITOR
using System;
using FlaxEditor;
using FlaxEditor.Content;
using FlaxEditor.Content.Thumbnails;
using FlaxEditor.Windows;
using FlaxEngine;
using FlaxEngine.GUI;

namespace Backpack;

[HideInEditor] // Hides this class in the ui control type selection, because it shows up there for some fucky reason
public class ItemObjectItem : JsonAssetItem
{
    // Custom thumbnails will only show, if there is no default sprite handle
    public override SpriteHandle DefaultThumbnail => SpriteHandle.Invalid;

    // Get access to the default _thumbnail. DefaultThumbnail needs to be an invalid handle for custom thumbnails to appear,
    // but we also want to use _thumbnails as a backup.
    public SpriteHandle SneakyThumbnail => _thumbnail;

    public ItemObjectItem(string path, Guid id, string typeName) : base(path, id, typeName)
    {
        SpriteAtlas texture = Content.LoadAsync<SpriteAtlas>("7c45e3ff41d94cd7ed4bda9de02497fa".GuidFromAssetID());
        if (!texture || texture.WaitForLoaded())
            return;

        _thumbnail = texture.FindSprite("Item");
    }

    // Spawn a new item actor
    public override Actor OnEditorDrop(object context)
    {
        JsonAssetReference<ItemClass> itemAsset = (JsonAssetReference<ItemClass>)Content.LoadAsync<JsonAsset>(ID);

        if (string.IsNullOrEmpty(itemAsset.Instance.Id))
        {
            Debug.LogWarning("Item could not be created, because it's missing an id. Items get ids assigned, when their parent database is (re)build. Please make sure you do that.");
            return null;
        }

        ItemActor itemActor = new ItemActor();
        itemActor.ItemReference = itemAsset;
        return itemActor;
    }

    // Default is false. This method needs to return true, in order to drop items from content to scene or hierarchy window
    public override bool OnEditorDrag(object context)
    {
        return true;
    }
}

[ContentContextMenu("New/Backpack/New Item")]
public class ItemClassProxy : SpawnableJsonAssetProxy<ItemClass>
{
    public override string NewItemName => "New Item";

    public override Color AccentColor => Color.Gray;

    public override AssetItem ConstructItem(string path, string typeName, ref Guid id)
    {
        return new ItemObjectItem(path, id, typeName);
    }

    public override EditorWindow Open(Editor editor, ContentItem item)
    {
        return new ItemWindow(editor, (ItemObjectItem)item);
    }

    public override void OnThumbnailDrawBegin(ThumbnailRequest request, ContainerControl guiRoot, GPUContext context)
    {
        // Try to do custom thumbnail first
        Texture texture = ((JsonAssetReference<ItemClass>)request.Asset).Instance.Icon;

        // It seem that a couple of style colors are hardcoded alterations of colors set in theme style. 
        // Background.RGBMultiplied(1.25f) for icon backgrounds of json assets comes from ContentItem.cs, Line 713.
        // If this ever gets a custom style setting, this needs to be changed.
        Color backgroundColor = Style.Current.Background.RGBMultiplied(1.25f);

        if (texture != null)
        {
            guiRoot.AddChild(new Image()
            {
                Brush = new TextureBrush(texture),
                AnchorPreset = AnchorPresets.StretchAll,
                Offsets = Margin.Zero,
                BackgroundBrush = null,
                BackgroundColor = backgroundColor
            });

            return;
        }

        // Try default thumbnail second
        SpriteHandle handle = ((ItemObjectItem)request.Item).SneakyThumbnail;

        if (handle != null)
        {
            guiRoot.AddChild(new Image()
            {
                Brush = new SpriteBrush(handle),
                AnchorPreset = AnchorPresets.StretchAll,
                Offsets = Margin.Zero,
                BackgroundColor = backgroundColor
            });

            return;
        }

        // Fallback
        base.OnThumbnailDrawBegin(request, guiRoot, context);
    }
}
#endif