#if TOOLS
namespace Godot;

using System.IO;
using System.Reflection;
using dragcrops.addons.CustomScripts;

[Tool]
public partial class CustomScriptsPlugin : EditorPlugin
{
    public override void _EnterTree()
    {
        // アセンブリを取得
        var assembly = Assembly.GetExecutingAssembly();
        foreach (var type in assembly.GetTypes())
        {
            var attr = type.GetCustomAttribute<CustomScriptAttribute>();
            if (attr != null && type.FullName != null)
            {
                var path = FindScriptPath(type.FullName);
                var script = GD.Load<Script>(path);
                var icon = GD.Load<Texture2D>(attr.IconPath);
                GD.Print($"AddCustomType: {type.FullName}");
                AddCustomType(type.Name, attr.BaseType, script, icon);
            }
        }
    }

    private static string FindScriptPath(string typeName)
    {
        foreach (var file in Directory.GetFiles(ProjectSettings.GlobalizePath("res://"), "*.cs",
                     SearchOption.AllDirectories))
        {
            if (Path.GetFileNameWithoutExtension(file) == typeName)
            {
                // OSパス → res:// パスに変換
                return ProjectSettings.LocalizePath(file);
            }
        }

        throw new FileNotFoundException($"Script not found: {typeName}");
    }

    public override void _ExitTree()
    {
        // 登録解除
        // AddCustomType で登録したクラス名を忘れずに RemoveCustomType する
    }
}
#endif
