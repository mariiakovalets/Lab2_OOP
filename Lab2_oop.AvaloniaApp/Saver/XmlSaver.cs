using System;
using System.Collections.Generic;
using Lab2_oop.AvaloniaApp.Models;

namespace Lab2_oop.AvaloniaApp.Saver;

public class XmlSaver : ISaver
{
    public string FormatName => "XML";
    public string FileExtension => "xml";
    
    public bool Save(List<Student> students, string outputPath, string? xslPath = null)
    {
        if (students == null || students.Count == 0)
            throw new ArgumentException("Немає студентів для збереження");
        
        var xDoc = XmlGenerator.CreateXmlDocument(students);
        XmlGenerator.SaveToFile(xDoc, outputPath);
        
        return true;
    }
}