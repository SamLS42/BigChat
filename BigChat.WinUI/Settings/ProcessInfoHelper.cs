using System.Diagnostics;

namespace BigChat.Settings;


public static partial class ProcessInfoHelper
{
    static ProcessInfoHelper()
    {
        Process = Process.GetCurrentProcess();
        FileVersionInfo = Process.MainModule!.FileVersionInfo;
    }

    /// <summary>
    /// Returns the version string prefixed with 'v'.
    /// </summary>
    public static string VersionWithPrefix => $"v{Version}";

    /// <summary>
    /// Retrieves the product name. If not available, it returns 'Unknown Product'.
    /// </summary>
    public static string ProductName => FileVersionInfo?.ProductName ?? "Unknown Product";

    /// <summary>
    /// Combines the product name and version into a single string. The version includes a prefix.
    /// </summary>
    public static string ProductNameAndVersion => $"{ProductName} {VersionWithPrefix}";

    /// <summary>
    /// Returns the company name of the publisher. If not available, it defaults to 'Unknown Publisher'.
    /// </summary>
    public static string Publisher => FileVersionInfo?.CompanyName ?? "Unknown Publisher";

    public static Version Version => new(FileVersionInfo.FileMajorPart, FileVersionInfo.FileMinorPart, FileVersionInfo.FileBuildPart, FileVersionInfo.FilePrivatePart);

    /// <summary>
    /// Retrieves the file version information for the current assembly.
    /// </summary>
    /// <returns>Returns a FileVersionInfo object containing version details.</returns>
    public static FileVersionInfo FileVersionInfo { get; private set; }

    /// <summary>
    /// Retrieves the current process instance.
    /// </summary>
    /// <returns>Returns the current Process object.</returns>
    public static Process Process { get; private set; }
}