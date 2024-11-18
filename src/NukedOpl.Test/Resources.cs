using System.IO;

namespace NukedOpl.Test;

public static class Resources
{
    public static Stream Open(string name)
    {
        return typeof(Resources)
            .Assembly
            .GetManifestResourceStream($"{typeof(Resources).Namespace}.Data.{name}");
    }
}