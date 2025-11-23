 using System.Collections.Generic;
using Lab2_oop.AvaloniaApp.Models;

namespace Lab2_oop.AvaloniaApp.Saver;

public interface ISaver
{
    string FormatName { get; }
    
    string FileExtension { get; }
    
    bool Save(List<Student> students, string outputPath, string? xslPath = null);
}
