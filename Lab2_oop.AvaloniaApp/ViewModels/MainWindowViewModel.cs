using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Lab2_oop.AvaloniaApp.Models;
using Lab2_oop.AvaloniaApp.Parsers;
using Lab2_oop.AvaloniaApp.Saver;
using Lab2_oop.AvaloniaApp.LogLibrary;
using System.Threading.Tasks;

namespace Lab2_oop.AvaloniaApp.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    // ========== INotifyPropertyChanged ==========
    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    // ========== ПРИВАТНІ ПОЛЯ ==========
    private string? _currentXmlFilePath;
    private IXmlParserStrategy? _currentStrategy;
    private List<Student> _currentFilteredStudents = new();
    
    private string _fileName = "Файл не обрано";
    private bool _isTableHeaderVisible;
    
    // ========== ПУБЛІЧНІ ВЛАСТИВОСТІ ==========
    
    public string FileName
    {
        get => _fileName;
        set
        {
            if (_fileName != value)
            {
                _fileName = value;
                OnPropertyChanged();
            }
        }
    }
    
    public bool IsTableHeaderVisible
    {
        get => _isTableHeaderVisible;
        set
        {
            if (_isTableHeaderVisible != value)
            {
                _isTableHeaderVisible = value;
                OnPropertyChanged();
            }
        }
    }
    
    public ObservableCollection<Student> StudentsTable { get; set; } = new();
    public ObservableCollection<string> ParsingStrategies { get; set; } = new()
    {
        "LINQ to XML",
        "DOM API",
        "SAX API"
    };
    
    public ObservableCollection<object> SearchAttributes { get; set; } = new();
    public ObservableCollection<string> SearchValues { get; set; } = new();
    
    public int SelectedStrategyIndex { get; set; } = -1;
    public int SelectedAttributeIndex { get; set; } = -1;
    public int SelectedValueIndex { get; set; } = -1;
    
    // ========== ACTIONS ==========
    public Action<string>? ShowErrorAction;
    public Func<Task>? ShowExitConfirmationAction;
    public Func<Task<string?>>? ShowFileSaveDialogAction;
    
    // ========== КОНСТРУКТОР ==========
    public MainWindowViewModel()
    {
        Logger.Instance.Log("High", "ViewModel ініціалізовано");
    }
    
    // ========== МЕТОДИ ==========
    
    public void SetFilePath(string filePath)
    {
        _currentXmlFilePath = filePath;
        string fileName = System.IO.Path.GetFileName(filePath);
        FileName = fileName;
        
        Logger.Instance.Log("Medium", $"Обрано файл: {filePath}");
        
        ClearSearchData();
        LoadAttributesForFile();
    }
    
    public void SelectStrategy(int index)
    {
        if (index < 0)
        {
            _currentStrategy = null;
            SelectedStrategyIndex = -1;
            return;
        }
        
        string strategyName = ParsingStrategies[index];
        _currentStrategy = strategyName switch
        {
            "LINQ to XML" => new LINQParsingStrategy(),
            "DOM API" => new DOMParsingStrategy(),
            "SAX API" => new SAXParsingStrategy(),
            _ => null
        };
        
        SelectedStrategyIndex = index;
        
        if (_currentStrategy != null)
        {
            Logger.Instance.Log("Low", $"Вибрана стратегія: {_currentStrategy.StrategyName}");
        }
    }
    
    private void LoadAttributesForFile()
    {
        if (string.IsNullOrEmpty(_currentXmlFilePath))
            return;
        
        var strategy = _currentStrategy ?? new LINQParsingStrategy();
        var attributes = strategy.GetAvailableAttributes(_currentXmlFilePath);
        
        SearchAttributes.Clear();
        SearchAttributes.Add(new { Display = "всі студенти", Tag = "" });
        
        var translations = new Dictionary<string, string>
        {
            { "FullName", "ПІБ" },
            { "year", "Курс" },
            { "Faculty", "Факультет" },
            { "Department", "Кафедра" },
            { "Subject", "Дисципліна" }
        };
        
        foreach (var attr in attributes)
        {
            string display = translations.ContainsKey(attr) ? translations[attr] : attr;
            SearchAttributes.Add(new { Display = display, Tag = attr });
        }
        
        SelectedAttributeIndex = 0;
    }
    
    public void SelectAttribute(int index)
    {
        SelectedAttributeIndex = index;
        SearchValues.Clear();
        SelectedValueIndex = -1;
        
        if (index <= 0 || string.IsNullOrEmpty(_currentXmlFilePath) || _currentStrategy == null)
            return;
        
        var attrObj = SearchAttributes[index] as dynamic;
        string attribute = attrObj?.Tag ?? "";
        
        LoadAttributeValues(attribute);
    }
    
    private void LoadAttributeValues(string attribute)
    {
        if (string.IsNullOrEmpty(attribute) || string.IsNullOrEmpty(_currentXmlFilePath) || _currentStrategy == null)
            return;
        
        var allStudents = _currentStrategy.ParseStudents(_currentXmlFilePath, "", "");
        if (allStudents == null || allStudents.Count == 0)
            return;
        
        var uniqueValues = new HashSet<string>();
        
        foreach (var student in allStudents)
        {
            string? value = attribute.ToLower() switch
            {
                "year" => student.Year?.ToString(),
                "fullname" => student.PersonalInfo?.FullName,
                "faculty" => student.PersonalInfo?.Faculty,
                "department" => student.PersonalInfo?.Department,
                "subject" => null,
                _ => null
            };
            
            if (attribute.ToLower() == "subject")
            {
                foreach (var subj in student.Subjects)
                {
                    if (!string.IsNullOrWhiteSpace(subj.Name))
                        uniqueValues.Add(subj.Name);
                }
            }
            else if (!string.IsNullOrWhiteSpace(value))
            {
                uniqueValues.Add(value);
            }
        }
        
        SearchValues.Clear();
        foreach (var val in uniqueValues.OrderBy(v => v))
        {
            SearchValues.Add(val);
        }
    }
    
    public void Search()
    {
        if (string.IsNullOrEmpty(_currentXmlFilePath))
        {
            ShowErrorAction?.Invoke("Оберіть XML файл!");
            return;
        }
        
        if (_currentStrategy == null)
        {
            ShowErrorAction?.Invoke("Оберіть метод парсингу!");
            return;
        }
        
        string searchAttribute = "";
        string searchValue = "";
        
        if (SelectedAttributeIndex > 0)
        {
            var attrObj = SearchAttributes[SelectedAttributeIndex] as dynamic;
            searchAttribute = attrObj?.Tag ?? "";
        }
        
        if (SelectedValueIndex >= 0 && SelectedValueIndex < SearchValues.Count)
        {
            searchValue = SearchValues[SelectedValueIndex];
        }
        
        if (!string.IsNullOrEmpty(searchAttribute) && string.IsNullOrEmpty(searchValue))
        {
            ShowErrorAction?.Invoke("Оберіть значення зі списку");
            return;
        }
        
        try
        {
            var results = _currentStrategy.ParseStudents(_currentXmlFilePath, searchAttribute, searchValue);
            results = results.OrderByDescending(s => s.AverageGrade).ToList();
            
            for (int i = 0; i < results.Count; i++)
            {
                results[i].RowNumber = i + 1;
            }
            
            _currentFilteredStudents = results;
            
            StudentsTable.Clear();
            foreach (var student in results)
            {
                StudentsTable.Add(student);
            }
            
            IsTableHeaderVisible = results.Count > 0;
            
            Logger.Instance.Log("Medium", $"Знайдено {results.Count} студент(ів) | Стратегія: {_currentStrategy.StrategyName}");
        }
        catch (Exception ex)
        {
            Logger.Instance.Error($"Помилка пошуку: {ex.Message}");
            ShowErrorAction?.Invoke($"Помилка пошуку: {ex.Message}");
            IsTableHeaderVisible = false;
        }
    }
    
    public void Clear()
    {
        _currentXmlFilePath = null;
        _currentStrategy = null;
        _currentFilteredStudents.Clear();
        
        FileName = "Файл не обрано";
        SelectedStrategyIndex = -1;
        SelectedAttributeIndex = -1;
        SelectedValueIndex = -1;
        IsTableHeaderVisible = false;
        
        SearchAttributes.Clear();
        SearchValues.Clear();
        StudentsTable.Clear();
        
        Logger.Instance.Log("Medium", "Дані очищені");
    }
    
    public async Task TransformToHtml()
    {
        if (string.IsNullOrEmpty(_currentXmlFilePath))
        {
            ShowErrorAction?.Invoke("Оберіть XML файл!");
            return;
        }
        
        if (_currentFilteredStudents == null || _currentFilteredStudents.Count == 0)
        {
            ShowErrorAction?.Invoke("Немає даних для трансформації! Натисніть 'Показати' спочатку.");
            return;
        }
        
        try
        {
            string xmlDirectory = System.IO.Path.GetDirectoryName(_currentXmlFilePath) ?? "";
            string xslPath = System.IO.Path.Combine(xmlDirectory, "students.xsl");
            
            if (!System.IO.File.Exists(xslPath))
            {
                Logger.Instance.Error($"XSL не знайдено: {xslPath}");
                ShowErrorAction?.Invoke($"XSL файл не знайдено!\nПоставте students.xsl в папку:\n{xmlDirectory}");
                return;
            }
            
            string? outputPath = null;
            if (ShowFileSaveDialogAction != null)
            {
                outputPath = await ShowFileSaveDialogAction.Invoke();
            }
            
            if (string.IsNullOrEmpty(outputPath))
            {
                return;
            }
            
            // Використовуємо новий інтерфейс ISaver
            var htmlSaver = new HtmlSaver();
            bool success = htmlSaver.Save(_currentFilteredStudents, outputPath, xslPath);
            
            if (success)
            {
                Logger.Instance.Log("High", $"HTML створено: {outputPath}");
                
                try
                {
                    var psi = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = outputPath,
                        UseShellExecute = true
                    };
                    System.Diagnostics.Process.Start(psi);
                }
                catch { }
            }
            else
            {
                ShowErrorAction?.Invoke("Помилка при створенні HTML файлу!");
            }
        }
        catch (Exception ex)
        {
            Logger.Instance.Error($"Помилка трансформації: {ex.Message}");
            ShowErrorAction?.Invoke($"Помилка: {ex.Message}");
        }
    }
    
    private void ClearSearchData()
    {
        _currentStrategy = null;
        SelectedStrategyIndex = -1;
        SelectedAttributeIndex = -1;
        SelectedValueIndex = -1;
        IsTableHeaderVisible = false;
        SearchAttributes.Clear();
        SearchValues.Clear();
        StudentsTable.Clear();
        _currentFilteredStudents.Clear();
    }
}