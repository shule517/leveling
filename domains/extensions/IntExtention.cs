namespace leveling.domains.extensions;

using player;

public static class DomainIntExtension {
    public static int ToWalkSpeed(this int me) {
        // 歩く速度: 1セルを{me}m秒かけて移動する
        return (int)(Player.CellSize / (me / 1000f));
    }
}
