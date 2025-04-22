using System.Diagnostics;

namespace SkiRent.Desktop.Services;

/// <summary>
/// Provides an abstraction over <see cref="Process"/> to enable unit testing.
/// </summary>
public interface IProcessService
{
    /// <inheritdoc cref="Process.Start(ProcessStartInfo)" />
    public Process? Start(ProcessStartInfo startInfo);
}

/// <summary>
/// Default implementation of <see cref="IProcessService"/> that directly calls <see cref="Process.Start"/>.
/// </summary>
public class ProcessService : IProcessService
{
    public Process? Start(ProcessStartInfo startInfo)
        => Process.Start(startInfo);
}
