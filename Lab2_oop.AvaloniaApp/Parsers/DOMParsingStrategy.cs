using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Lab2_oop.AvaloniaApp.Models;
using Lab2_oop.AvaloniaApp.LogLibrary;

namespace Lab2_oop.AvaloniaApp.Parsers;


public class DOMParsingStrategy : IXmlParserStrategy
{
    public string StrategyName => "DOM API";
    

    public List<Student> ParseStudents(string xmlPath, string searchAttribute, string searchValue)
    {
        Logger.Instance.Log("Low", $"DOM парсинг розпочато | Атрибут: {searchAttribute} | Значення: {searchValue}");
        
        var students = new List<Student>();
        
        try
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlPath);
            
            XmlNodeList? studentNodes = doc.SelectNodes("//Student");
            
            if (studentNodes == null)
            {
                Logger.Instance.Warning("DOM: не знайдено вузлів Student");
                return students;
            }
            
            foreach (XmlNode studentNode in studentNodes)
            {

                var student = ParseStudentNode(studentNode);
                

                if (string.IsNullOrWhiteSpace(searchValue) || MatchesSearchCriteria(student, searchAttribute, searchValue))
                {
                    students.Add(student);
                }
            }
            
            Logger.Instance.Log("Low", $"DOM парсинг завершено | Знайдено: {students.Count} студент(ів)");
        }
        catch (Exception ex)
        {
            Logger.Instance.Error($"Помилка DOM парсингу: {ex.Message}");
        }
        
        return students;
    }
    
    public List<string> GetAvailableAttributes(string xmlPath)
    {
        var attributes = new HashSet<string>();
        
        try
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlPath);
            
            XmlNodeList? studentNodes = doc.SelectNodes("//Student");
            
            if (studentNodes == null)
                return new List<string>();
            
            foreach (XmlNode student in studentNodes)
            {
                if (student.Attributes != null)
                {
                    foreach (XmlAttribute attr in student.Attributes)
                    {
                        if (attr.Name == "year")
                        {
                            attributes.Add(attr.Name);
                        }
                    }
                }
            }
            attributes.Add("FullName");
            attributes.Add("Faculty");
            attributes.Add("Department");
            attributes.Add("Subject");
            
            Logger.Instance.Log("Low", $"DOM: знайдено {attributes.Count} атрибутів");
        }
        catch (Exception ex)
        {
            Logger.Instance.Error($"Помилка читання атрибутів DOM: {ex.Message}");
        }
        
        return attributes.OrderBy(a => a).ToList();
    }
    
 
    private Student ParseStudentNode(XmlNode studentNode)
    {
        var student = new Student();
        
        if (studentNode.Attributes != null)
        {
            student.Year = ParseNullableInt(studentNode.Attributes["year"]?.Value);
        }
        
        XmlNode? personalInfoNode = studentNode.SelectSingleNode("PersonalInfo");
        if (personalInfoNode != null)
        {
            student.PersonalInfo = new PersonalInfo
            {
                FullName = personalInfoNode.SelectSingleNode("FullName")?.InnerText ?? "",
                Faculty = personalInfoNode.SelectSingleNode("Faculty")?.InnerText ?? "",
                Department = personalInfoNode.SelectSingleNode("Department")?.InnerText ?? ""
            };
        }
        
        XmlNodeList? subjectNodes = studentNode.SelectNodes("Subjects/Subject");
        if (subjectNodes != null)
        {
            foreach (XmlNode subjectNode in subjectNodes)
            {
                var subject = new Subject
                {
                    Name = subjectNode.SelectSingleNode("Name")?.InnerText ?? "",
                    Grade = subjectNode.SelectSingleNode("Grade")?.InnerText ?? ""
                };
                
                student.Subjects.Add(subject);
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
            "fullname" => student.PersonalInfo.FullName.ToLower().Contains(searchValue),
            "faculty" => student.PersonalInfo.Faculty.ToLower().Contains(searchValue),
            "department" => student.PersonalInfo.Department.ToLower().Contains(searchValue),
            "subject" => student.Subjects.Any(s => s.Name.ToLower().Contains(searchValue)),
            _ => false
        };
    }

    private int? ParseNullableInt(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        
        return int.TryParse(value, out var result) ? result : null;
    }
}