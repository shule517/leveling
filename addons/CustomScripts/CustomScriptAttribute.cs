namespace leveling.addons.CustomScripts;

using System;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class CustomScriptAttribute : Attribute
{
    public string BaseType { get; }
    public string IconPath { get; }

    public CustomScriptAttribute(string baseType = "Node", string iconPath = "res://addons/CustomScripts/icon.svg")
    {
        BaseType = baseType;
        IconPath = iconPath;
    }
}
