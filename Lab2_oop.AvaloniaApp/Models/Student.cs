using System.Collections.Generic;
using System.Linq;

namespace Lab2_oop.AvaloniaApp.Models;

public class Student
{
    public int RowNumber { get; set; }
    
    public int? Year { get; set; }

    public PersonalInfo PersonalInfo { get; set; } = new PersonalInfo();
    
    public List<Subject> Subjects { get; set; } = new List<Subject>();
    

    public double AverageGrade
{
    get
    {
        if (Subjects.Count == 0) return 0;
        
        double sum = 0;
        foreach (var subject in Subjects)
        {
            if (double.TryParse(subject.Grade, out var grade))
            {
                sum += grade;
            }
        }
        return sum / Subjects.Count;
    }
}

    public string AllGradesDisplay
    {
        get
        {
            if (Subjects == null || !Subjects.Any())
                return "Немає даних";
            
        
            return string.Join("\n", Subjects.Select(s => $"{s.Name}: {s.Grade}"));
        }
    }
    
    
    public override string ToString()
    {
        string year = Year.HasValue ? $" | Курс: {Year}" : "";
        return $"{PersonalInfo.FullName}{year} | Середній бал: {AverageGrade:F2}";
    }
}