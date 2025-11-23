using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Lab2_oop.AvaloniaApp.Models;

namespace Lab2_oop.AvaloniaApp.Parsers;

public class LINQParsingStrategy : IXmlParserStrategy
{
    public string StrategyName => "LINQ to XML";
    
    public List<Student> ParseStudents(string xmlPath, string searchAttribute, string searchValue)
    {
        var doc = XDocument.Load(xmlPath);
        var students = doc.Descendants("Student").Select(ParseStudent).ToList();
        
        if (string.IsNullOrWhiteSpace(searchValue))
            return students;
        
        return students.Where(s => MatchesSearchCriteria(s, searchAttribute, searchValue)).ToList();
    }
    
    public List<string> GetAvailableAttributes(string xmlPath)
    {
        var attributes = new HashSet<string>();
        
        var doc = XDocument.Load(xmlPath);
        
        foreach (var student in doc.Descendants("Student"))
        {
            foreach (var attr in student.Attributes())
            {
                attributes.Add(attr.Name.LocalName);
            }
        }
        
        attributes.Add("FullName");
        attributes.Add("Subject");
        
        return attributes.OrderBy(a => a).ToList();
    }
    
    
    private Student ParseStudent(XElement el)
    {
        var personalInfo = el.Element("PersonalInfo");
        
        string faculty = el.Attribute("faculty")?.Value ?? "";
        string department = el.Attribute("department")?.Value ?? "";
        
        if (string.IsNullOrEmpty(faculty))
            faculty = personalInfo?.Element("Faculty")?.Value ?? "";
        
        if (string.IsNullOrEmpty(department))
            department = personalInfo?.Element("Department")?.Value ?? "";
        
        return new Student
        {
            Year = (int?)el.Attribute("year"),
            FullName = personalInfo?.Element("FullName")?.Value ?? "",
            Faculty = faculty,
            Department = department,
            Subjects = el.Element("Subjects")?
                .Elements("Subject")
                .Select(s => new Subject
                {
                    Name = s.Element("Name")?.Value ?? "",
                    Grade = s.Element("Grade")?.Value ?? ""
                })
                .ToList() ?? new List<Subject>()
        };
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
}