using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Lab2_oop.AvaloniaApp.Models;

namespace Lab2_oop.AvaloniaApp.Saver;
public static class XmlGenerator
{
    public static XDocument CreateXmlDocument(List<Student> students)
    {
        return new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            new XElement("Students",
                students.Select(student => 
                    new XElement("Student",
                        student.Year.HasValue ? new XAttribute("year", student.Year.Value) : null,
                        !string.IsNullOrEmpty(student.Faculty) ? new XAttribute("faculty", student.Faculty) : null,
                        !string.IsNullOrEmpty(student.Department) ? new XAttribute("department", student.Department) : null,
                        new XAttribute("averageGrade", student.AverageGrade.ToString("F2", CultureInfo.InvariantCulture)),
                        
                        new XElement("PersonalInfo",
                            new XElement("FullName", student.FullName),
                            new XElement("Faculty", student.Faculty ?? ""),
                            new XElement("Department", student.Department ?? "")
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
    }
    
    public static void SaveToFile(XDocument document, string outputPath)
    {
        document.Save(outputPath);
    }
}