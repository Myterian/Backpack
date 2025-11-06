// Copyright © 2024 - 2025 Thomas Jungclaus. All rights reserved. Released under the MIT License.

using System;

namespace Backpack;

/// <summary>GuidUtility class.</summary>
public static class Utility
{
    /// <summary>Converts a Content Asset ID to a Guid for i.e. Content.LoadAsync</summary>
    /// <param name="assetID">Asset ID from Content window</param>
    /// <returns>string</returns>
    public static Guid GuidFromAssetID(this string assetID)
    {
        return FlaxEngine.Json.JsonSerializer.ParseID(assetID);
    }
}

