using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Lab2_oop.AvaloniaApp.Models;
using Lab2_oop.AvaloniaApp.LogLibrary;

namespace Lab2_oop.AvaloniaApp.Parsers;

/// <summary>
/// Стратегія парсингу XML за допомогою LINQ to XML
/// Найзручніша для роботи з C#
/// </summary>
public class LINQParsingStrategy : IXmlParserStrategy
{
    public string StrategyName => "LINQ to XML";
    
    /// <summary>
    /// Парсить XML файл за допомогою LINQ to XML
    /// </summary>
    public List<Student> ParseStudents(string xmlPath, string searchAttribute, string searchValue)
    {
        Logger.Instance.Log("Low", $"LINQ парсинг розпочато | Атрибут: {searchAttribute} | Значення: {searchValue}");
        
        var students = new List<Student>();
        
        try
        {
            // Завантажуємо XML документ
            XDocument doc = XDocument.Load(xmlPath);
            
            // Отримуємо всіх студентів
            var studentElements = doc.Descendants("Student");
            
            foreach (var studentElement in studentElements)
            {
                // Парсимо студента
                var student = ParseStudentElement(studentElement);
                
                // Перевіряємо чи відповідає критерію пошуку
                if (string.IsNullOrWhiteSpace(searchValue) || MatchesSearchCriteria(student, searchAttribute, searchValue))
                {
                    students.Add(student);
                }
            }
            
            Logger.Instance.Log("Low", $"LINQ парсинг завершено | Знайдено: {students.Count} студент(ів)");
        }
        catch (Exception ex)
        {
            Logger.Instance.Error($"Помилка LINQ парсингу: {ex.Message}");
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
            XDocument doc = XDocument.Load(xmlPath);
            
            var students = doc.Descendants("Student");
            
            // Збираємо всі унікальні атрибути
            foreach (var student in students)
            {
                foreach (var attr in student.Attributes())
                {
                    string attrName = attr.Name.LocalName;
                    attributes.Add(attrName);
                }
            }
            
            // Додаємо поля з PersonalInfo
            attributes.Add("FullName");
            attributes.Add("Faculty");
            attributes.Add("Department");
            
            // Додаємо Subject
            attributes.Add("Subject");
            
            Logger.Instance.Log("Low", $"LINQ: знайдено {attributes.Count} атрибутів");
        }
        catch (Exception ex)
        {
            Logger.Instance.Error($"Помилка читання атрибутів LINQ: {ex.Message}");
        }
        
        return attributes.OrderBy(a => a).ToList();
    }
    
    /// <summary>
    /// Парсить один елемент Student в об'єкт Student
    /// </summary>
    private Student ParseStudentElement(XElement studentElement)
    {
        var student = new Student
        {
            Year = ParseNullableInt(studentElement.Attribute("year")?.Value)
        };
        
        // Парсимо PersonalInfo
        var personalInfoElement = studentElement.Element("PersonalInfo");
        if (personalInfoElement != null)
        {
            student.PersonalInfo = new PersonalInfo
            {
                FullName = personalInfoElement.Element("FullName")?.Value ?? "",
                Faculty = personalInfoElement.Element("Faculty")?.Value ?? "",
                Department = personalInfoElement.Element("Department")?.Value ?? ""
            };
        }
        
        // Парсимо Subjects
        var subjectsElement = studentElement.Element("Subjects");
        if (subjectsElement != null)
        {
            foreach (var subjectElement in subjectsElement.Elements("Subject"))
            {
                var subject = new Subject
                {
                    Name = subjectElement.Element("Name")?.Value ?? "",
                    Grade = subjectElement.Element("Grade")?.Value ?? ""
                };
                
                student.Subjects.Add(subject);
            }
        }
        
        return student;
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
