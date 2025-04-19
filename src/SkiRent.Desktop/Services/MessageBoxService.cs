using System.Windows;

namespace SkiRent.Desktop.Services;

/// <summary>
/// Provides an abstraction over <see cref="MessageBox"/> to enable unit testing and decouple UI logic.
/// </summary>
public interface IMessageBoxService
{
    /// <inheritdoc cref="MessageBox.Show(string)" />
    public MessageBoxResult Show(string messageBoxText);

    /// <inheritdoc cref="MessageBox.Show(string, string)" />
    public MessageBoxResult Show(string messageBoxText, string caption);

    /// <inheritdoc cref="MessageBox.Show(string, string, MessageBoxButton)" />
    public MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button);

    /// <inheritdoc cref="MessageBox.Show(string, string, MessageBoxButton, MessageBoxImage)" />
    public MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon);

    /// <inheritdoc cref="MessageBox.Show(string, string, MessageBoxButton, MessageBoxImage, MessageBoxResult)" />
    public MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult);

    /// <inheritdoc cref="MessageBox.Show(string, string, MessageBoxButton, MessageBoxImage, MessageBoxResult, MessageBoxOptions)" />
    public MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult, MessageBoxOptions options);
}

/// <summary>
/// Default implementation of <see cref="IMessageBoxService"/> that directly calls <see cref="MessageBox.Show"/>.
/// </summary>
public class MessageBoxService : IMessageBoxService
{
    public MessageBoxResult Show(string messageBoxText)
        => MessageBox.Show(messageBoxText);

    public MessageBoxResult Show(string messageBoxText, string caption)
        => MessageBox.Show(messageBoxText, caption);

    public MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button)
        => MessageBox.Show(messageBoxText, caption, button);

    public MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon)
        => MessageBox.Show(messageBoxText, caption, button, icon);

    public MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult)
        => MessageBox.Show(messageBoxText, caption, button, icon, defaultResult);

    public MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult, MessageBoxOptions options)
        => MessageBox.Show(messageBoxText, caption, button, icon, defaultResult, options);
}
