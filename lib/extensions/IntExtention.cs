namespace leveling.lib.extensions;

using System;
using System.Threading.Tasks;

public static class IntExtension {
    public static void Times(this int me, Action<int> action) {
        for (var i = 0; i < me; i++) {
            action(i);
        }
    }

    public static async Task TimesAsync(this int me, Func<int, Task> func) {
        for (var i = 0; i < me; i++) {
            await func(i);
        }
    }
}
