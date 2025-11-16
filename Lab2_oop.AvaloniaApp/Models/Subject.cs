namespace Lab2_oop.AvaloniaApp.Models;

/// <summary>
/// Навчальна дисципліна студента
/// </summary>
public class Subject
{
    /// <summary>
    /// Назва дисципліни
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Оцінка (A, B, C або числова 90, 85 тощо)
    /// </summary>
    public string Grade { get; set; } = string.Empty;
    
    public override string ToString()
    {
        return $"{Name} - Оцінка: {Grade}";
    }
}