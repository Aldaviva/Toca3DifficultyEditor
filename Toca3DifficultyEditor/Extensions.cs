using System.Text;

namespace Toca3DifficultyEditor;

public static class Extensions {

    private static readonly Encoding DEFAULT_ENCODING = new UTF8Encoding(false, true);

    public static byte[] ToBytes(this string input, Encoding? encoding = null) => (encoding ?? DEFAULT_ENCODING).GetBytes(input);

    public static IEnumerable<KeyValuePair<K, V>> Compact<K, V>(this IEnumerable<KeyValuePair<K, V?>> input) where K: notnull where V: struct =>
        from pair in input
        where pair.Value.HasValue
        select new KeyValuePair<K, V>(pair.Key, pair.Value!.Value);

}