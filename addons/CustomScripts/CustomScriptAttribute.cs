namespace dragcrops.addons.CustomScripts;

using System;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class CustomScriptAttribute : Attribute
{
    public string BaseType { get; }
    public string IconPath { get; }

    public CustomScriptAttribute(string baseType = "Node", string iconPath = "")
    {
        BaseType = baseType;
        IconPath = iconPath;
    }
}
