using System.Windows;

namespace SkiRent.Desktop.Services;

public interface IMessageBoxService
{
    /// <inheritdoc cref="System.Windows.MessageBox.Show(string, string, MessageBoxButton, MessageBoxImage)" />
    public MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon);
}

public class MessageBoxService : IMessageBoxService
{
    public MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon)
        => MessageBox.Show(messageBoxText, caption, button, icon);
}
