using System;
using System.IO;

namespace Lab2_oop.AvaloniaApp.LogLibrary;

/// <summary>
/// Простий логер для запису подій в файл
/// Використовує Singleton pattern
/// </summary>
public sealed class Logger
{
    private static readonly Lazy<Logger> _instance = new Lazy<Logger>(() => new Logger());
    private readonly string _logFilePath;
    private readonly object _lockObject = new object();
    
    public static Logger Instance => _instance.Value;
    
    private Logger()
    {
        // Шлях до файлу логів (в папці з програмою)
        string basePath = AppDomain.CurrentDomain.BaseDirectory;
        _logFilePath = Path.Combine(basePath, "events.log");
        
        // Створюємо файл якщо його немає
        if (!File.Exists(_logFilePath))
        {
            File.WriteAllText(_logFilePath, $"=== LOG STARTED at {DateTime.Now:yyyy-MM-dd HH:mm:ss} ==={Environment.NewLine}");
        }
    }
    
    /// <summary>
    /// Записує інформаційне повідомлення
    /// </summary>
    public void Log(string importance, string message)
    {
        WriteLog("INFO", importance, message);
    }
    
    /// <summary>
    /// Записує попередження
    /// </summary>
    public void Warning(string message)
    {
        WriteLog("WARN", "Medium", message);
    }
    
    /// <summary>
    /// Записує помилку
    /// </summary>
    public void Error(string message)
    {
        WriteLog("ERROR", "High", message);
    }
    
    /// <summary>
    /// Основний метод запису в файл
    /// </summary>
    private void WriteLog(string level, string importance, string message)
    {
        lock (_lockObject)
        {
            try
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string logEntry = $"[{timestamp}] [{level}] {{{importance}}} {message}{Environment.NewLine}";
                
                File.AppendAllText(_logFilePath, logEntry);
            }
            catch (Exception ex)
            {
                // Тільки для критичних помилок логування
                Console.WriteLine($"Failed to write log: {ex.Message}");
            }
        }
    }
    
    /// <summary>
    /// Очищає файл логів
    /// </summary>
    public void ClearLog()
    {
        lock (_lockObject)
        {
            try
            {
                File.WriteAllText(_logFilePath, $"=== LOG CLEARED at {DateTime.Now:yyyy-MM-dd HH:mm:ss} ==={Environment.NewLine}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to clear log: {ex.Message}");
            }
        }
    }
    
    /// <summary>
    /// Отримує шлях до файлу логів
    /// </summary>
    public string GetLogFilePath() => _logFilePath;
}