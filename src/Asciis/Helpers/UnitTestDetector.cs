using System.Reflection;

namespace No8.Ascii;

/// <summary>
/// Detect if we are running as part of a nUnit unit test.
/// This is DIRTY and should only be used if absolutely necessary 
/// as its usually a sign of bad design.
/// </summary>    
internal static class UnitTestDetector
{
    static UnitTestDetector()
    {
        var asses = AppDomain.CurrentDomain.GetAssemblies();
        foreach (Assembly assem in asses)
        {
            if (assem.FullName?.ToLowerInvariant().StartsWith("nunit.framework") == true)
            {
                IsRunningFromNUnit = true;
                break;
            }
            if (assem.FullName?.ToLowerInvariant().StartsWith("xunit.") == true)
            {
                IsRunningFromNUnit = true;
                break;
            }
        }
    }

    public static bool IsRunningFromNUnit { get; } = false;
}
