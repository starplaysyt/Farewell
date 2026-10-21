using System.Buffers;

namespace Farewell.Presentation.Internal;

internal static class PresentationWriter
{
    public static readonly SpanAction<char, (string[] Buffer, int Count)> Instance = Write;

    private static void Write(Span<char> span, (string[] Buffer, int Count) state)
    {
        var buf = state.Buffer;
        var pos = 0;
        for (var i = 0; i < state.Count; i++)
        {
            var s = buf[i];
            s.AsSpan().CopyTo(span[pos..]);
            pos += s.Length;
        }
    }
}