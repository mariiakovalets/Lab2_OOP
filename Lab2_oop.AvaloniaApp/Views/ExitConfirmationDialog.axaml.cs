using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Lab2_oop.AvaloniaApp.Views;

public partial class ExitConfirmationDialog : Window
{
    public bool Result { get; private set; }
    
    public ExitConfirmationDialog()
    {
        AvaloniaXamlLoader.Load(this);
        
        var btnYes = this.Find<Button>("BtnYes");
        var btnNo = this.Find<Button>("BtnNo");
        
        if (btnYes != null)
            btnYes.Click += BtnYes_Click;
        
        if (btnNo != null)
            btnNo.Click += BtnNo_Click;
    }
    
    private void BtnYes_Click(object? sender, RoutedEventArgs e)
    {
        Result = true;
        Close();
    }
    
    private void BtnNo_Click(object? sender, RoutedEventArgs e)
    {
        Result = false;
        Close();
    }
}