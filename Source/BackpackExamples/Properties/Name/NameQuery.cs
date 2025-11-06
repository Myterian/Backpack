// Copyright © 2024 - 2025 Thomas Jungclaus. All rights reserved. Released under the MIT License.

namespace Backpack;

/// <summary>ItemQueryNameComponent Script.</summary>
public class NameQuery : ItemQuery
{
    public string Name;

    public NameQuery(string currentName)
    {
        Name = currentName;
    }

    public NameQuery()
    {
        Name = "";
    }
}
