using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using Lab2_oop.AvaloniaApp.Models;

namespace Lab2_oop.AvaloniaApp.Saver;

public class HtmlSaver : ISaver
{
    public string FormatName => "HTML";
    public string FileExtension => "html";
    
    public bool Save(List<Student> students, string outputPath, string? xslPath = null)
    {
        if (students == null || students.Count == 0)
            throw new ArgumentException("Немає студентів для збереження");
        
        if (string.IsNullOrEmpty(xslPath) || !File.Exists(xslPath))
            throw new FileNotFoundException($"XSL файл не знайдено: {xslPath}");
        
        string tempXmlPath = Path.Combine(
            Path.GetTempPath(), 
            $"temp_students_{Guid.NewGuid()}.xml"
        );
        
        try
        {
            var xDoc = XmlGenerator.CreateXmlDocument(students);
            XmlGenerator.SaveToFile(xDoc, tempXmlPath);
            
            TransformToHtml(tempXmlPath, xslPath, outputPath);
            return true;
        }
        finally
        {
            if (File.Exists(tempXmlPath))
                File.Delete(tempXmlPath);
        }
    }
    
    private void TransformToHtml(string xmlPath, string xslPath, string outputPath)
    {
        var xslt = new XslCompiledTransform();
        xslt.Load(xslPath);
        
        using (XmlReader reader = XmlReader.Create(xmlPath))
        using (XmlWriter writer = XmlWriter.Create(outputPath, new XmlWriterSettings
        {
            Indent = true,
            OmitXmlDeclaration = true
        }))
        {
            xslt.Transform(reader, writer);
        }
    }
}