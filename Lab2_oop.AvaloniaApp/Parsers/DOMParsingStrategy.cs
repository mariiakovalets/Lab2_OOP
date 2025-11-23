using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Lab2_oop.AvaloniaApp.Models;

namespace Lab2_oop.AvaloniaApp.Parsers;

public class DOMParsingStrategy : IXmlParserStrategy
{
    public string StrategyName => "DOM API";
    
    public List<Student> ParseStudents(string xmlPath, string searchAttribute, string searchValue)
    {
        var doc = new XmlDocument();
        doc.Load(xmlPath);
        
        var studentNodes = doc.SelectNodes("//Student");
        if (studentNodes == null) return new List<Student>();
        
        var students = studentNodes.Cast<XmlNode>().Select(ParseStudent).ToList();
        
        if (string.IsNullOrWhiteSpace(searchValue))
            return students;
        
        return students.Where(s => MatchesSearchCriteria(s, searchAttribute, searchValue)).ToList();
    }
    
    public List<string> GetAvailableAttributes(string xmlPath)
    {
        var attributes = new HashSet<string>();
        
        var doc = new XmlDocument();
        doc.Load(xmlPath);
        
        var studentNodes = doc.SelectNodes("//Student");
        if (studentNodes != null)
        {
            foreach (XmlNode node in studentNodes)
            {
                if (node.Attributes != null)
                {
                    foreach (XmlAttribute attr in node.Attributes)
                    {
                        attributes.Add(attr.Name);
                    }
                }
            }
        }
        
        attributes.Add("FullName");
        attributes.Add("Subject");
        
        return attributes.OrderBy(a => a).ToList();
    }
    
    // ========== PRIVATE ==========
    
    private Student ParseStudent(XmlNode node)
    {
        var student = new Student
        {
            Year = ParseNullableInt(node.Attributes?["year"]?.Value)
        };
        
        // Спочатку читаємо з атрибутів
        student.Faculty = node.Attributes?["faculty"]?.Value ?? "";
        student.Department = node.Attributes?["department"]?.Value ?? "";
        
        // Потім читаємо PersonalInfo (якщо атрибутів не було)
        var personalInfo = node.SelectSingleNode("PersonalInfo");
        if (personalInfo != null)
        {
            student.FullName = personalInfo.SelectSingleNode("FullName")?.InnerText ?? "";
            
            // Якщо факультет не був в атрибутах - беремо з елемента
            if (string.IsNullOrEmpty(student.Faculty))
                student.Faculty = personalInfo.SelectSingleNode("Faculty")?.InnerText ?? "";
            
            // Якщо кафедра не була в атрибутах - беремо з елемента
            if (string.IsNullOrEmpty(student.Department))
                student.Department = personalInfo.SelectSingleNode("Department")?.InnerText ?? "";
        }
        
        var subjectNodes = node.SelectNodes("Subjects/Subject");
        if (subjectNodes != null)
        {
            foreach (XmlNode subjectNode in subjectNodes)
            {
                student.Subjects.Add(new Subject
                {
                    Name = subjectNode.SelectSingleNode("Name")?.InnerText ?? "",
                    Grade = subjectNode.SelectSingleNode("Grade")?.InnerText ?? ""
                });
            }
        }
        
        return student;
    }
    
    private bool MatchesSearchCriteria(Student student, string searchAttribute, string searchValue)
    {
        if (string.IsNullOrWhiteSpace(searchValue))
            return true;
        
        searchValue = searchValue.ToLower();
        
        return searchAttribute.ToLower() switch
        {
            "year" => student.Year?.ToString().Contains(searchValue) ?? false,
            "fullname" => student.FullName.ToLower().Contains(searchValue),
            "faculty" => student.Faculty.ToLower().Contains(searchValue),
            "department" => student.Department.ToLower().Contains(searchValue),
            "subject" => student.Subjects.Any(s => s.Name.ToLower().Contains(searchValue)),
            _ => false
        };
    }
    
    private int? ParseNullableInt(string? value)
    {
        return int.TryParse(value, out var result) ? result : null;
    }
}