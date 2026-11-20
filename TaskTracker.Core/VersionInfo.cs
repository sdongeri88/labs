using System.Reflection;

namespace TaskTracker.Core;

/// <summary>Повертає версію збірки з Directory.Build.props.</summary>
public static class VersionInfo
{
    /// <summary>Поточна версія у форматі МАЖОР.МІНОР.ПАТЧ.</summary>
    /// <returns>Версія без суфікса збірки після «+».</returns>
    public static string Current()
    {
        var assembly = typeof(VersionInfo).Assembly;
        var attribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        var raw = attribute?.InformationalVersion ?? "0.0.0";
        var plus = raw.IndexOf('+');
        return plus < 0 ? raw : raw[..plus];
    }
}
