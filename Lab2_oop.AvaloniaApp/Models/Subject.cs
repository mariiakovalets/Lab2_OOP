namespace Lab2_oop.AvaloniaApp.Models;

/// <summary>
/// Навчальна дисципліна студента
/// </summary>
public class Subject
{

    public string Name { get; set; } = string.Empty;
    

    public string Grade { get; set; } = string.Empty;
    
    public override string ToString()
    {
        return $"{Name} - Оцінка: {Grade}";
    }
}