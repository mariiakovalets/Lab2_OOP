using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using Lab2_oop.AvaloniaApp.Models;
using Lab2_oop.AvaloniaApp.LogLibrary;

namespace Lab2_oop.AvaloniaApp.Saver;


public class HtmlSaver : ISaver
{
    public string FormatName => "HTML";
    
    public string FileExtension => "html";
    
 
    public bool Save(List<Student> students, string outputPath, string? xslPath = null)
    {
        try
        {
            Logger.Instance.Log("Low", $"Початок збереження в HTML: {outputPath}");
            
            if (students == null || students.Count == 0)
            {
                Logger.Instance.Warning("Немає студентів для збереження в HTML");
                return false;
            }
            
            if (string.IsNullOrEmpty(xslPath) || !File.Exists(xslPath))
            {
                Logger.Instance.Error($"XSL файл не знайдено: {xslPath}");
                return false;
            }
            
            string tempXmlPath = Path.Combine(
                Path.GetTempPath(), 
                $"temp_students_{Guid.NewGuid()}.xml"
            );
            
            CreateTempXml(students, tempXmlPath);
            
            bool result = TransformToHtml(tempXmlPath, xslPath, outputPath);
            
            if (File.Exists(tempXmlPath))
            {
                File.Delete(tempXmlPath);
            }
            
            if (result)
            {
                Logger.Instance.Log("High", $"HTML успішно збережено: {outputPath} ({students.Count} студент(ів))");
            }
            
            return result;
        }
        catch (Exception ex)
        {
            Logger.Instance.Error($"Помилка збереження HTML: {ex.Message}");
            return false;
        }
    }
    

    private void CreateTempXml(List<Student> students, string outputPath)
    {
        var xDoc = new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            new XElement("Students",
                students.Select(student => 
                    new XElement("Student",
                        student.Year.HasValue ? new XAttribute("year", student.Year.Value) : null,
                        new XAttribute("averageGrade", student.AverageGrade.ToString("F2")),
                        
                        new XElement("PersonalInfo",
                            new XElement("FullName", student.PersonalInfo.FullName),
                            new XElement("Faculty", student.PersonalInfo.Faculty),
                            new XElement("Department", student.PersonalInfo.Department)
                        ),
                        
                        new XElement("Subjects",
                            student.Subjects.Select(subject =>
                                new XElement("Subject",
                                    new XElement("Name", subject.Name),
                                    new XElement("Grade", subject.Grade)
                                )
                            )
                        )
                    )
                )
            )
        );
        
        xDoc.Save(outputPath);
    }
    
    private bool TransformToHtml(string xmlPath, string xslPath, string outputPath)
    {
        try
        {
            XslCompiledTransform xslt = new XslCompiledTransform();
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
            
            return true;
        }
        catch (Exception ex)
        {
            Logger.Instance.Error($"Помилка XSL трансформації: {ex.Message}");
            return false;
        }
    }
}
