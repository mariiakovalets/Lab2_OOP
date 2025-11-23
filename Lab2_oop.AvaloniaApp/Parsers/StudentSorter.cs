using System.Collections.Generic;
using System.Linq;
using Lab2_oop.AvaloniaApp.Models;

namespace Lab2_oop.AvaloniaApp.Parsers;

public static class StudentSorter
{
    public static List<Student> SortByAverageGrade(List<Student> students)
    {
        return students
            .OrderByDescending(s => s.AverageGrade)  
            .ThenBy(s => s.FullName)                 
            .ToList();
    }
}