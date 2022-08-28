using System.Diagnostics;

namespace TheKarixPL.TriodeFM.Extensions;

public static class ProcessExtensions
{
    public static bool IsRunning(this Process process)
    {
        if (process == null) throw new ArgumentNullException(nameof(process));
        try
        {
            Process.GetProcessById(process.Id);
            return true;
        }
        catch (Exception e)
        {
            if (e is InvalidOperationException or ArgumentException)
                return false;
            throw;
        }
    }
}