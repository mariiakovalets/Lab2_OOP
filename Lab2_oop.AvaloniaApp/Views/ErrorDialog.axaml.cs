using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Lab2_oop.AvaloniaApp.Views;

public partial class ErrorDialog : Window
{
    public ErrorDialog() : this("") { }

    public ErrorDialog(string message)
    {
        InitializeComponent();
        
        var txtMessage = this.FindControl<TextBlock>("TxtErrorMessage");
        if (txtMessage != null)
        {
            txtMessage.Text = message;
        }
    }
    
    private void BtnOk_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}