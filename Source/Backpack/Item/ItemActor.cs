// Copyright © 2024 - 2025 Thomas Jungclaus. All rights reserved. Released under the MIT License.

using FlaxEngine;

#if FLAX_EDITOR
using FlaxEditor;
using FlaxEditor.SceneGraph;
#endif

namespace Backpack;

/// <summary>ItemActor Script.</summary>
[ActorContextMenu("New/Backpack/Item Actor")]
public class ItemActor : EmptyActor
{
    public ItemInstance Instance { get; private set; } = null;
    [ShowInEditor, Serialize, ReadOnly, EditorDisplay("Item")] private string savedItemId;
    [ShowInEditor, Serialize, EditorDisplay("Item")] private int amount = 1;

    #if FLAX_EDITOR
    private JsonAssetReference<ItemClass> itemReference;
    [ShowInEditor, Serialize, EditorDisplay("Item")] public JsonAssetReference<ItemClass> ItemReference
    {
        get => itemReference;
        set
        {
            itemReference = value;
            savedItemId = value.Instance?.Id;
        }
    }
    #endif
    

    public override void OnEnable()
    {
        if(!string.IsNullOrEmpty(savedItemId))
        {
            ItemClass item = null;
            InventoryManager.Instance?.GetItemByID(savedItemId, out item);

            if(item != null)
            {
                Instance = new(item);
                Instance.SetAmount(amount);
            }
        }

        // Show Icon in the Scene View
        #if FLAX_EDITOR
        ViewportIconsRenderer.AddActor(this);
        #endif
    }

    public override void OnDisable()
    {
        // Remove Icon in the Scene View
        #if FLAX_EDITOR
        ViewportIconsRenderer.RemoveActor(this); 
        #endif
    }

    // Show Icon in the Scene View ===================================

#if FLAX_EDITOR
    static ItemActor()
    {
        ViewportIconsRenderer.AddCustomIcon(typeof(ItemActor), Content.LoadAsync<Texture>("3ea16fc94e813767b9321abea0bbfe20".GuidFromAssetID())); 
        SceneGraphFactory.CustomNodesTypes.Add(typeof(ItemActor), typeof(ItemActorNode));
    }

    /// <summary>Custom actor node for Editor.</summary>
    public sealed class ItemActorNode : ActorNodeWithIcon
    {
        public ItemActorNode(Actor actor) : base(actor){}
    }
#endif
}