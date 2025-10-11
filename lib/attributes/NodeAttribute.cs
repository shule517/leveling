using System;

[AttributeUsage(AttributeTargets.Field)]
public class NodeAttribute(string? path = null) : Attribute
{
    public string? Path { get; } = path;
}
