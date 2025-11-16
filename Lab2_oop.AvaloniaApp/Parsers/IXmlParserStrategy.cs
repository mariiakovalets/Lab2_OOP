using System.Collections.Generic;
using Lab2_oop.AvaloniaApp.Models;

namespace Lab2_oop.AvaloniaApp.Parsers;


public interface IXmlParserStrategy
{

    string StrategyName { get; }
    
    List<Student> ParseStudents(string xmlPath, string searchAttribute, string searchValue);
    
    List<string> GetAvailableAttributes(string xmlPath);
}