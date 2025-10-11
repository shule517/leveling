using System;
using Godot;

public class Scene<TNode> where TNode : Node2D
{
    private readonly PackedScene _packedScene;

    // シーンの読み込み
    public Scene(string scenePath)
    {
        // TODO: 起動時にエラーを投げたい
        var packedScene = GD.Load<PackedScene>(scenePath) ?? throw new InvalidOperationException($"シーンが見つかりませんでした: {scenePath}");
        _packedScene = packedScene;
    }

    // シーンを生成する
    public TNode Instantiate() => _packedScene.Instantiate<TNode>();
}
