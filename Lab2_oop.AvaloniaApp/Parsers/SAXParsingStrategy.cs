using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Lab2_oop.AvaloniaApp.Models;

namespace Lab2_oop.AvaloniaApp.Parsers;

public class SAXParsingStrategy : IXmlParserStrategy
{
    public string StrategyName => "SAX API";
    
    public List<Student> ParseStudents(string xmlPath, string searchAttribute, string searchValue)
    {
        var students = new List<Student>();
        
        using (XmlReader reader = XmlReader.Create(xmlPath))
        {
            Student? currentStudent = null;
            Subject? currentSubject = null;
            string? currentElement = null;
            
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        currentElement = reader.Name;
                        
                        if (reader.Name == "Student")
                        {
                            currentStudent = new Student
                            {
                                Year = ParseNullableInt(reader.GetAttribute("year")),
                                Faculty = reader.GetAttribute("faculty") ?? "",
                                Department = reader.GetAttribute("department") ?? ""
                            };
                        }
                        else if (reader.Name == "Subject")
                        {
                            currentSubject = new Subject();
                        }
                        break;
                    
                    case XmlNodeType.Text:
                        if (currentStudent == null) break;
                        
                        switch (currentElement)
                        {
                            case "FullName":
                                currentStudent.FullName = reader.Value;
                                break;
                            case "Faculty":
                                if (string.IsNullOrEmpty(currentStudent.Faculty))
                                    currentStudent.Faculty = reader.Value;
                                break;
                            case "Department":
                                if (string.IsNullOrEmpty(currentStudent.Department))
                                    currentStudent.Department = reader.Value;
                                break;
                            case "Name" when currentSubject != null:
                                currentSubject.Name = reader.Value;
                                break;
                            case "Grade" when currentSubject != null:
                                currentSubject.Grade = reader.Value;
                                break;
                        }
                        break;
                    
                    case XmlNodeType.EndElement:
                        if (reader.Name == "Subject" && currentStudent != null && currentSubject != null)
                        {
                            currentStudent.Subjects.Add(currentSubject);
                            currentSubject = null;
                        }
                        else if (reader.Name == "Student" && currentStudent != null)
                        {
                            students.Add(currentStudent);
                            currentStudent = null;
                        }
                        break;
                }
            }
        }
        
        if (string.IsNullOrWhiteSpace(searchValue))
            return students;
        
        return students.Where(s => MatchesSearchCriteria(s, searchAttribute, searchValue)).ToList();
    }
    
    public List<string> GetAvailableAttributes(string xmlPath)
    {
        var attributes = new HashSet<string>();
        
        using (XmlReader reader = XmlReader.Create(xmlPath))
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "Student")
                {
                    if (reader.HasAttributes)
                    {
                        for (int i = 0; i < reader.AttributeCount; i++)
                        {
                            reader.MoveToAttribute(i);
                            attributes.Add(reader.Name);
                        }
                        reader.MoveToElement();
                    }
                }
            }
        }
        
        attributes.Add("FullName");
        attributes.Add("Subject");
        
        return attributes.OrderBy(a => a).ToList();
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