namespace leveling.lib.extensions;
using System;
using System.Linq;
using System.Reflection;
using attributes;
using Godot;

public static class NodeExtensions
{
    // 例) await this.WaitSeconds(0.1f);
    public static SignalAwaiter WaitSeconds(this Node me, float seconds)
    {
        var timer = me.GetTree().CreateTimer(seconds);
        return me.ToSignal(timer, "timeout");
    }

    public static void AutoLoad(this Node me)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var autoLoadClasses = assembly.GetTypes().Where(t => t.GetCustomAttribute<AutoLoadAttribute>() != null);
        foreach (var autoLoadClass in autoLoadClasses)
        {
            var instance = Activator.CreateInstance(autoLoadClass) as Node;
            if (instance != null)
            {
                instance.Name = autoLoadClass.Name;
                me.AddChild(instance);
            }

            // AutoLoadしたNodeは[Inject]で取得できる
            GD.Print($"autoLoadClass: {autoLoadClass}");
        }
    }

    public static void BindNodes(this Node me)
    {
        BindOnReadyNodes(me);
        BindInjectNodes(me);
    }

    private static void BindOnReadyNodes(this Node me)
    {
        var fields = me.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var field in fields)
        {
            var attribute = field.GetCustomAttribute<NodeAttribute>();
            if (attribute == null) { continue; }

            if (attribute.Path == null)
            {
                // 変数名と一致するNodeを取得
                // _hpProgressBar => HpProgressBarを取得する
                var fieldName = field.Name.TrimStart('_');
                var path = $"{char.ToUpper(fieldName[0])}{fieldName.Substring(1)}";
                var node = me.GetNode<Node>(path);
                field.SetValue(me, node);
            }
            else
            {
                // 指定されたパスのNodeを取得する
                var node = me.GetNode<Node>(attribute.Path);
                if (node == null) { throw new InvalidOperationException($"Nodeが見つかりませんでした: {attribute.Path}"); }

                field.SetValue(me, node);
            }
        }
    }

    private static void BindInjectNodes(this Node me)
    {
        var fields = me.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var field in fields)
        {
            var attribute = field.GetCustomAttribute<InjectAttribute>();
            if (attribute == null) { continue; }

            // AutoLoadからNodeを取得する
            var typeName = field.FieldType.Name;

            var sceneTree = (SceneTree)Engine.GetMainLoop();
            var node = sceneTree.Root.GetNode($"/root/AutoLoad/{typeName}");
            if (node == null) { throw new InvalidOperationException($"Nodeが見つかりませんでした: {typeName}"); }

            field.SetValue(me, node);
        }
    }
}
