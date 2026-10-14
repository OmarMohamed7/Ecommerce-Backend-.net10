using System.Text.RegularExpressions;

namespace E_Commerce.Common;

/// Loads a .env file into environment variables before the host is built, so its values
/// become normal configuration (ConnectionStrings__DefaultConnection -> ConnectionStrings:DefaultConnection).
public static partial class DotEnv
{
    public static void Load()
    {
        var path = Find();
        if (path is null) return;

        var fileValues = new Dictionary<string, string>();

        foreach (var raw in File.ReadAllLines(path))
        {
            var line = raw.Trim();
            if (line.Length == 0 || line.StartsWith('#')) continue;
            if (line.StartsWith("export ")) line = line[7..].TrimStart();

            var eq = line.IndexOf('=');
            if (eq <= 0) continue;

            var key = line[..eq].Trim();
            var value = line[(eq + 1)..].Trim();

            if (value.Length >= 2 && ((value[0] == '"' && value[^1] == '"') || (value[0] == '\'' && value[^1] == '\'')))
                value = value[1..^1];

            value = Reference().Replace(value, m =>
                Environment.GetEnvironmentVariable(m.Groups[1].Value)
                ?? fileValues.GetValueOrDefault(m.Groups[1].Value, string.Empty));

            fileValues[key] = value;

            if (Environment.GetEnvironmentVariable(key) is null)
                Environment.SetEnvironmentVariable(key, value);
        }
    }

    // Looks for .env in the working directory and the build folder, then walks up the parents
    private static string? Find()
    {
        foreach (var start in new[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
        {
            var dir = new DirectoryInfo(start);
            for (var depth = 0; dir is not null && depth < 6; depth++, dir = dir.Parent)
            {
                var candidate = Path.Combine(dir.FullName, ".env");
                if (File.Exists(candidate)) return candidate;
            }
        }

        return null;
    }

    [GeneratedRegex(@"\$\{([A-Za-z_][A-Za-z0-9_]*)\}")]
    private static partial Regex Reference();
}