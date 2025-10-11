using System;
using Godot;

public class Scene<TNode> where TNode : Node2D
{
    private PackedScene _packedScene;

    // シーンの読み込み
    public Scene(string scenePath)
    {
        var packedScene = GD.Load<PackedScene>(scenePath);
        if (packedScene == null)
        {
            // TODO: 起動時にエラーを投げたい
            throw new InvalidOperationException($"シーンが見つかりませんでした: {scenePath}");
        }

        _packedScene = packedScene;
    }

    // シーンを生成する
    public TNode Instantiate()
    {
        return _packedScene.Instantiate<TNode>();
    }
}
