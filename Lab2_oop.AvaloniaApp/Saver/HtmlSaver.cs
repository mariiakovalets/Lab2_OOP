using System;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using Lab2_oop.AvaloniaApp.LogLibrary;

namespace Lab2_oop.AvaloniaApp.Saver;

public class HtmlSaver
{
    public bool TransformToHtml(string xmlPath, string xslPath, string outputPath)
    {
        try
        {
            Logger.Instance.Log("Low", $"Початок XSL трансформації: {xmlPath} → {outputPath}");
            
            if (!File.Exists(xmlPath))
            {
                Logger.Instance.Error($"XML файл не знайдено: {xmlPath}");
                return false;
            }
            
            if (!File.Exists(xslPath))
            {
                Logger.Instance.Error($"XSL файл не знайдено: {xslPath}");
                return false;
            }
            
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
            
            Logger.Instance.Log("High", $"HTML успішно створено: {outputPath}");
            return true;
        }
        catch (Exception ex)
        {
            Logger.Instance.Error($"Помилка XSL трансформації: {ex.Message}");
            return false;
        }
    }
}