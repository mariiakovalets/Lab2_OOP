using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Lab2_oop.AvaloniaApp.Views;

public partial class XslNotFoundDialog : Window
{
    private readonly string _directory;
    
    public XslNotFoundDialog() : this(string.Empty)
    {
    }
    public XslNotFoundDialog(string directory)
    {
        _directory = directory;
        
        AvaloniaXamlLoader.Load(this);
        
        var txtPath = this.Find<TextBlock>("TxtPath");
        var btnOk = this.Find<Button>("BtnOk");
        
        if (txtPath != null && !string.IsNullOrEmpty(_directory))
            txtPath.Text = $"Поставте його в папку:\n{_directory}";
        
        if (btnOk != null)
            btnOk.Click += (s, e) => Close();
    }
}