using Microsoft.Extensions.Configuration;

namespace Asciis.Terminal.Helpers;

internal class AsciiUtilities
{
    public static bool ParseBool(IConfiguration configuration, string key)
    {
        return string.Equals("true", configuration[key], StringComparison.OrdinalIgnoreCase)
            || string.Equals("1", configuration[key], StringComparison.OrdinalIgnoreCase);
    }
}
