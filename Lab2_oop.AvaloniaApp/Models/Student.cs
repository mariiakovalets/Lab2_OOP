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
                sum += ConvertGradeToNumber(subject.Grade);
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