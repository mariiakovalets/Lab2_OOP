using System.Collections.Generic;
using Lab2_oop.AvaloniaApp.Models;

namespace Lab2_oop.AvaloniaApp.Parsers;

/// <summary>
/// Інтерфейс стратегії парсингу XML файлів
/// Реалізується трьома способами: LINQ to XML, DOM API, SAX API
/// </summary>
public interface IXmlParserStrategy
{
    /// <summary>
    /// Назва стратегії (для відображення в UI)
    /// </summary>
    string StrategyName { get; }
    
    /// <summary>
    /// Парсить XML файл та шукає студентів за заданим критерієм
    /// </summary>
    /// <param name="xmlPath">Шлях до XML файлу</param>
    /// <param name="searchAttribute">Атрибут/поле для пошуку (year, FullName, Faculty, Subject тощо)</param>
    /// <param name="searchValue">Значення для пошуку</param>
    /// <returns>Список знайдених студентів</returns>
    List<Student> ParseStudents(string xmlPath, string searchAttribute, string searchValue);
    
    /// <summary>
    /// Отримує список всіх доступних атрибутів з XML файлу
    /// Для динамічної підгрузки ComboBox
    /// </summary>
    /// <param name="xmlPath">Шлях до XML файлу</param>
    /// <returns>Список назв атрибутів</returns>
    List<string> GetAvailableAttributes(string xmlPath);
}