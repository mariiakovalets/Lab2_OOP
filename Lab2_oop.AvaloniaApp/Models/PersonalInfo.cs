namespace Lab2_oop.AvaloniaApp.Models;

public class PersonalInfo
{
    public string FullName { get; set; } = string.Empty;
    
    public string Faculty { get; set; } = string.Empty;

    public string Department { get; set; } = string.Empty;
    
    public override string ToString()
    {
        return $"{FullName} | {Faculty}, {Department}";
    }
}