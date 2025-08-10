using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

//Define Immutable Inventory Record with interface
public interface IInventoryEntity
{
    int Id { get; }
}

public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

//Generic Inventory Logger
public class InventoryLogger<T> where T : IInventoryEntity
{
    private List<T> _log = new();
    private readonly string _filePath;

    public InventoryLogger(string filePath)
    {
        _filePath = filePath;
    }

    public void Add(T item)
    {
        _log.Add(item);
    }

    public List<T> GetAll()
    {
        return new List<T>(_log);
    }

    public void SaveToFile()
    {
        try
        {
            using FileStream fs = new(_filePath, FileMode.Create, FileAccess.Write);
            JsonSerializer.Serialize(fs, _log, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving file: {ex.Message}");
        }
    }

    public void LoadFromFile()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                Console.WriteLine("File not found, starting with empty log.");
                _log.Clear();
                return;
            }

            using FileStream fs = new(_filePath, FileMode.Open, FileAccess.Read);
            var items = JsonSerializer.Deserialize<List<T>>(fs);
            _log = items ?? new List<T>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading file: {ex.Message}");
            _log.Clear();
        }
    }
}

// InventoryApp
public class InventoryApp
{
    private readonly InventoryLogger<InventoryItem> _logger;

    public InventoryApp(string filePath)
    {
        _logger = new InventoryLogger<InventoryItem>(filePath);
    }

    public void SeedSampleData()
    {
        _logger.Add(new InventoryItem(1, "Nails", 500, DateTime.Now.AddDays(-10)));
        _logger.Add(new InventoryItem(2, "Saw", 20, DateTime.Now.AddDays(-5)));
        _logger.Add(new InventoryItem(3, "Screwdriver", 80, DateTime.Now.AddDays(-8)));
        _logger.Add(new InventoryItem(4, "Hammer", 40, DateTime.Now.AddDays(-15)));
        _logger.Add(new InventoryItem(5, "Drill", 20, DateTime.Now.AddDays(-12)));
    }

    public void SaveData()
    {
        _logger.SaveToFile();
    }

    public void LoadData()
    {
        _logger.LoadFromFile();
    }

    public void PrintAllItems()
    {
        var items = _logger.GetAll();
        if (items.Count == 0)
        {
            Console.WriteLine("No items found.");
            return;
        }

        foreach (var item in items)
        {
            Console.WriteLine($"Id: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}, Date Added: {item.DateAdded}");
        }
    }
}

//Main Application Flow
class Program
{
    static void Main()
    {
        string filePath = "inventory.json";
        var app = new InventoryApp(filePath);

        // Seed sample data and save
        app.SeedSampleData();
        app.SaveData();

        // Simulate clearing memory / new session by creating a new instance
        app = new InventoryApp(filePath);

        // Load data from file and print
        app.LoadData();
        app.PrintAllItems();
    }
}
