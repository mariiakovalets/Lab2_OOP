using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Lab2_oop.AvaloniaApp.Models;
using Lab2_oop.AvaloniaApp.LogLibrary;

namespace Lab2_oop.AvaloniaApp.Parsers;

public class SAXParsingStrategy : IXmlParserStrategy
{
    public string StrategyName => "SAX API";
    
    public List<Student> ParseStudents(string xmlPath, string searchAttribute, string searchValue)
    {
        Logger.Instance.Log("Low", $"SAX парсинг розпочато | Атрибут: {searchAttribute} | Значення: {searchValue}");
        
        var students = new List<Student>();
        
        try
        {
            using (XmlReader reader = XmlReader.Create(xmlPath))
            {
                Student? currentStudent = null;
                PersonalInfo? currentPersonalInfo = null;
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
                                currentStudent = new Student();
                                
                                if (reader.HasAttributes)
                                {
                                    currentStudent.Year = ParseNullableInt(reader.GetAttribute("year"));
                                }
                            }
                            else if (reader.Name == "PersonalInfo")
                            {
                                currentPersonalInfo = new PersonalInfo();
                            }
                            else if (reader.Name == "Subject")
                            {
                                currentSubject = new Subject();
                            }
                            break;
                        
                        case XmlNodeType.Text:
                            if (currentElement == "FullName" && currentPersonalInfo != null)
                            {
                                currentPersonalInfo.FullName = reader.Value;
                            }
                            else if (currentElement == "Faculty" && currentPersonalInfo != null)
                            {
                                currentPersonalInfo.Faculty = reader.Value;
                            }
                            else if (currentElement == "Department" && currentPersonalInfo != null)
                            {
                                currentPersonalInfo.Department = reader.Value;
                            }
                            else if (currentElement == "Name" && currentSubject != null)
                            {
                                currentSubject.Name = reader.Value;
                            }
                            else if (currentElement == "Grade" && currentSubject != null)
                            {
                                currentSubject.Grade = reader.Value;
                            }
                            break;
                        
                        case XmlNodeType.EndElement:
                            if (reader.Name == "PersonalInfo" && currentStudent != null && currentPersonalInfo != null)
                            {
                                currentStudent.PersonalInfo = currentPersonalInfo;
                                currentPersonalInfo = null;
                            }
                            else if (reader.Name == "Subject" && currentStudent != null && currentSubject != null)
                            {
                                currentStudent.Subjects.Add(currentSubject);
                                currentSubject = null;
                            }
                            else if (reader.Name == "Student" && currentStudent != null)
                            {
                                if (string.IsNullOrWhiteSpace(searchValue) || MatchesSearchCriteria(currentStudent, searchAttribute, searchValue))
                                {
                                    students.Add(currentStudent);
                                }
                                currentStudent = null;
                            }
                            break;
                    }
                }
            }
            
            Logger.Instance.Log("Low", $"SAX парсинг завершено | Знайдено: {students.Count} студент(ів)");
        }
        catch (Exception ex)
        {
            Logger.Instance.Error($"Помилка SAX парсингу: {ex.Message}");
        }
        
        return students;
    }
    
    /// <summary>
    /// Отримує доступні атрибути з XML файлу
    /// </summary>
    public List<string> GetAvailableAttributes(string xmlPath)
    {
        var attributes = new HashSet<string>();
        
        try
        {
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
            
            // Додаємо поля з PersonalInfo
            attributes.Add("FullName");
            attributes.Add("Faculty");
            attributes.Add("Department");
            
            // Додаємо Subject
            attributes.Add("Subject");
            
            Logger.Instance.Log("Low", $"SAX: знайдено {attributes.Count} атрибутів");
        }
        catch (Exception ex)
        {
            Logger.Instance.Error($"Помилка читання атрибутів SAX: {ex.Message}");
        }
        
        return attributes.OrderBy(a => a).ToList();
    }
    
    /// <summary>
    /// Перевіряє чи відповідає студент критерію пошуку
    /// </summary>
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
    
    /// <summary>
    /// Допоміжний метод для парсингу nullable int
    /// </summary>
    private int? ParseNullableInt(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        
        return int.TryParse(value, out var result) ? result : null;
    }
}
