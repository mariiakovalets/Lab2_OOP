using System.Collections.Generic;
using System.Linq;

namespace Lab2_oop.AvaloniaApp.Models;

public class Student
{
    /// <summary>
    /// Номер рядка для відображення в таблиці
    /// </summary>
    public int RowNumber { get; set; }
    
    /// <summary>
    /// Курс навчання (1, 2, 3, 4) - nullable бо не у всіх студентів є
    /// </summary>
    public int? Year { get; set; }
    
    /// <summary>
    /// Персональна інформація студента
    /// </summary>
    public PersonalInfo PersonalInfo { get; set; } = new PersonalInfo();
    
    /// <summary>
    /// Список навчальних дисциплін (вкладені елементи)
    /// </summary>
    public List<Subject> Subjects { get; set; } = new List<Subject>();
    
    // ========== ОБЧИСЛЮВАНІ ВЛАСТИВОСТІ ==========
    
    /// <summary>
    /// Середній бал студента
    /// </summary>
    public double AverageGrade
    {
        get
        {
            if (Subjects.Count == 0) return 0;
            
            double sum = 0;
            foreach (var subject in Subjects)
            {
                sum += ConvertGradeToNumber(subject.Grade);
            }
            return sum / Subjects.Count;
        }
    }
    
    /// <summary>
    /// Всі дисципліни з оцінками для відображення в таблиці
    /// </summary>
    public string AllGradesDisplay
    {
        get
        {
            if (Subjects == null || !Subjects.Any())
                return "Немає даних";
            
            // Кожна дисципліна з нового рядка
            return string.Join("\n", Subjects.Select(s => $"{s.Name}: {s.Grade}"));
        }
    }
    
    /// <summary>
    /// Конвертує літерну оцінку в числову
    /// </summary>
    private double ConvertGradeToNumber(string grade)
    {
        return grade.ToUpper() switch
        {
            "A" or "ВІДМІННО" => 100,
            "B" or "ДОБРЕ" => 85,
            "C" or "ЗАДОВІЛЬНО" => 70,
            "D" or "ДОСТАТНЬО" => 60,
            "F" or "НЕЗАДОВІЛЬНО" => 0,
            _ => double.TryParse(grade, out var numGrade) ? numGrade : 0
        };
    } 
    
    public override string ToString()
    {
        string year = Year.HasValue ? $" | Курс: {Year}" : "";
        return $"{PersonalInfo.FullName}{year} | Середній бал: {AverageGrade:F2}";
    }
}