using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using System;
using System.Threading.Tasks;
using Lab2_oop.AvaloniaApp.Views;
using Lab2_oop.AvaloniaApp.ViewModels;
using Lab2_oop.AvaloniaApp.LogLibrary;

namespace Lab2_oop.AvaloniaApp.Views;

public partial class MainWindow : Window
{
    private MainWindowViewModel _viewModel;
    
    public MainWindow()
    {
        InitializeComponent();
        
        _viewModel = new MainWindowViewModel();
        DataContext = _viewModel;
        
        Logger.Instance.Log("High", "Програма запущена");
        
        BtnSelectFile.Click += BtnSelectFile_Click;
        BtnSearch.Click += BtnSearch_Click;
        BtnClear.Click += BtnClear_Click;
        BtnTransform.Click += BtnTransform_Click;
        BtnExit.Click += BtnExit_Click;
        
        CmbParsingStrategy.SelectionChanged += CmbParsingStrategy_SelectionChanged;
        CmbSearchAttribute.SelectionChanged += CmbSearchAttribute_SelectionChanged;
        
        _viewModel.ShowErrorAction = ShowError;
        _viewModel.ShowFileSaveDialogAction = ShowFileSaveDialog;
    }
    
    private async void BtnSelectFile_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Оберіть XML файл",
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("XML Files") { Patterns = new[] { "*.xml" } },
                    new FilePickerFileType("All Files") { Patterns = new[] { "*.*" } }
                }
            });
            
            if (files.Count > 0)
            {
                _viewModel.SetFilePath(files[0].Path.LocalPath);
                CmbParsingStrategy.SelectedIndex = -1;
                
                var table = this.FindControl<ItemsControl>("StudentsTable");
                if (table != null)
                {
                    table.ItemsSource = _viewModel.StudentsTable;
                }
                
                var header = this.FindControl<Grid>("TableHeader");
                if (header != null)
                {
                    header.IsVisible = _viewModel.IsTableHeaderVisible;
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Instance.Error($"Помилка вибору файлу: {ex.Message}");
            ShowError($"Помилка вибору файлу: {ex.Message}");
        }
    }
    
    private void CmbParsingStrategy_SelectionChanged(object? sender, SelectionChangedEventArgs e)
{
    if (CmbParsingStrategy.SelectedIndex >= 0)
    {
        _viewModel.SelectStrategy(CmbParsingStrategy.SelectedIndex);
        
        if (_viewModel.SearchAttributes.Count > 0)
        {
            CmbSearchAttribute.SelectedIndex = 0;
        }
    }
}
    
    private void CmbSearchAttribute_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (CmbSearchAttribute.SelectedIndex >= 0)
        {
            _viewModel.SelectAttribute(CmbSearchAttribute.SelectedIndex);
        }
    }
    
    private void BtnSearch_Click(object? sender, RoutedEventArgs e)
    {
        _viewModel.Search();
        
        var header = this.FindControl<Grid>("TableHeader");
        if (header != null)
        {
            header.IsVisible = _viewModel.IsTableHeaderVisible;
        }
    }
    private void BtnClear_Click(object? sender, RoutedEventArgs e)
    {
        _viewModel.Clear();
        
        CmbParsingStrategy.SelectedIndex = -1;
        CmbSearchAttribute.SelectedIndex = -1;
        CmbSearchValue.SelectedIndex = -1;
        
        var header = this.FindControl<Grid>("TableHeader");
        if (header != null)
        {
            header.IsVisible = false;
        }
    }
    
    private async void BtnTransform_Click(object? sender, RoutedEventArgs e)
    {
        await _viewModel.TransformToHtml();
    }
    
    private async void BtnExit_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            var dialog = new ExitConfirmationDialog();
            await dialog.ShowDialog(this);
            
            if (dialog.Result)
            {
                Logger.Instance.Log("High", "Програма закривається");
                Environment.Exit(0);
            }
        }
        catch (Exception ex)
        {
            Logger.Instance.Error($"Помилка при виході: {ex.Message}");
            Environment.Exit(0);
        }
    }
    
    private async void ShowError(string message)
    {
        var errorDialog = new ErrorDialog(message);
        await errorDialog.ShowDialog(this);
    }
    
    private async Task<string?> ShowFileSaveDialog()
    {
        var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Зберегти HTML файл",
            SuggestedFileName = "students_report.html",
            DefaultExtension = "html",
            FileTypeChoices = new[]
            {
                new FilePickerFileType("HTML Files") { Patterns = new[] { "*.html" } }
            }
        });
        
        return file?.Path.LocalPath;
    }
}