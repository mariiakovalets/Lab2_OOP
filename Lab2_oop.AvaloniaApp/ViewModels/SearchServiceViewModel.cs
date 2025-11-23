using System;
using System.Collections.Generic;
using System.Linq;
using Lab2_oop.AvaloniaApp.Models;
using Lab2_oop.AvaloniaApp.Parsers;

namespace Lab2_oop.AvaloniaApp.ViewModels;

public class StudentSearchService
{
   
    public List<Student> Search(
        string xmlPath,
        IXmlParserStrategy strategy,
        string searchAttribute,
        string searchValue,
        string? keyword)
    {
        var results = strategy.ParseStudents(xmlPath, searchAttribute, searchValue);
        
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            results = FilterByKeyword(results, keyword);
        }
        
        results = StudentSorter.SortByAverageGrade(results);
        
        AssignRowNumbers(results);
        
        return results;
    }
    

    private List<Student> FilterByKeyword(List<Student> students, string keyword)
    {
        string keywordLower = keyword.ToLower();
        
        return students.Where(s => 
            s.FullName.ToLower().Contains(keywordLower) ||
            s.Faculty.ToLower().Contains(keywordLower) ||
            s.Department.ToLower().Contains(keywordLower) ||
            s.Year?.ToString().Contains(keywordLower) == true ||
            s.AverageGrade.ToString("F2").Contains(keywordLower) || 
            s.Subjects.Any(subj => 
                subj.Name.ToLower().Contains(keywordLower) || 
                subj.Grade.Contains(keywordLower)) 
        ).ToList();
    }
    
    
    private void AssignRowNumbers(List<Student> students)
    {
        for (int i = 0; i < students.Count; i++)
        {
            students[i].RowNumber = i + 1;
        }
    }
}