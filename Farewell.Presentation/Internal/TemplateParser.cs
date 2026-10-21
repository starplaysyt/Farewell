namespace Farewell.Presentation.Internal;

internal readonly record struct TemplateSlot(string Name, bool IsRecursive);

internal readonly record struct ParsedTemplate(
    string[] Literals, // length == Slots.Length + 1
    TemplateSlot[] Slots,
    int ConstLiteralsLength);

internal static class TemplateParser
{
    public static ParsedTemplate Parse(string template)
    {
        ArgumentNullException.ThrowIfNull(template);

        var literals = new List<string>();
        var slots = new List<TemplateSlot>();

        var i = 0;
        var literalStart = 0;

        while (i < template.Length)
        {
            if (template[i] != '{')
            {
                i++;
                continue;
            }

            literals.Add(template.Substring(literalStart, i - literalStart));
            i++; // skip '{'

            var recursive = false;
            if (i < template.Length && template[i] == '$')
            {
                recursive = true;
                i++;
            }

            var nameStart = i;
            while (i < template.Length && template[i] != '}') i++;

            if (i >= template.Length)
                throw new FormatException($"Unclosed placeholder in template: \"{template}\"");

            var name = template.Substring(nameStart, i - nameStart);

            // {$} == self-presentation (empty name allowed ONLY with '$')
            // {}  == invalid
            if (name.Length == 0 && !recursive)
                throw new FormatException(
                    $"Empty placeholder in template: \"{template}\". Use {{$}} for self-presentation.");

            slots.Add(new TemplateSlot(name, recursive));
            i++; // skip '}'
            literalStart = i;
        }

        literals.Add(template[literalStart..]);

        var totalLiteralLen = literals.Sum(l => l.Length);

        return new ParsedTemplate([.. literals], [.. slots], totalLiteralLen);
    }
}