using System;
using System.Collections.Generic;


//Marker interface
public interface IInventoryItem
{
    int Id { get; }
    string Name { get; }
    int Quantity { get; set; }
}

//ElectronicItem
public class ElectronicItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public string Brand { get; }
    public int WarrantyMonths { get; }

    public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        Brand = brand;
        WarrantyMonths = warrantyMonths;
    }

    public override string ToString()
    {
        return $"[Electronic] ID:{Id} Name:{Name} Brand:{Brand} Qty:{Quantity} Warranty:{WarrantyMonths}mo";
    }
}

//GroceryItem
public class GroceryItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public DateTime ExpiryDate { get; }

    public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        ExpiryDate = expiryDate;
    }

    public override string ToString()
    {
        return $"[Grocery] ID:{Id} Name:{Name} Qty:{Quantity} Expiry:{ExpiryDate:yyyy-MM-dd}";
    }
}

// Custom Exceptions

public class DuplicateItemException : Exception
{
    public DuplicateItemException(string message) : base(message) { }
}

public class ItemNotFoundException : Exception
{
    public ItemNotFoundException(string message) : base(message) { }
}

public class InvalidQuantityException : Exception
{
    public InvalidQuantityException(string message) : base(message) { }
}


//Generic repository constrained to IInventoryItem
public class InventoryRepository<T> where T : IInventoryItem
{
    private readonly Dictionary<int, T> _items = new();

    // Add item - throws DuplicateItemException if id exists
    public void AddItem(T item)
    {
        if (_items.ContainsKey(item.Id))
            throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
        _items[item.Id] = item;
    }

    // Get item by id - throws ItemNotFoundException if missing
    public T GetItemById(int id)
    {
        if (!_items.TryGetValue(id, out var item))
            throw new ItemNotFoundException($"Item with ID {id} not found.");
        return item;
    }

    // Remove item - throws ItemNotFoundException if missing
    public void RemoveItem(int id)
    {
        if (!_items.Remove(id))
            throw new ItemNotFoundException($"Item with ID {id} not found.");
    }

    // Return all items
    public List<T> GetAllItems()
    {
        return new List<T>(_items.Values);
    }

    // Update quantity - throws InvalidQuantityException for negative, ItemNotFoundException if missing
    public void UpdateQuantity(int id, int newQuantity)
    {
        if (newQuantity < 0)
            throw new InvalidQuantityException("Quantity cannot be negative.");

        if (!_items.TryGetValue(id, out var item))
            throw new ItemNotFoundException($"Item with ID {id} not found.");

        item.Quantity = newQuantity;
    }
}

public class WareHouseManager
{
    private readonly InventoryRepository<ElectronicItem> _electronics = new();
    private readonly InventoryRepository<GroceryItem> _groceries = new();

    // f.i SeedData - add 2-3 items of each type
    public void SeedData()
    {
        // Electronics
        _electronics.AddItem(new ElectronicItem(1, "Laptop", 5, "HP", 24));
        _electronics.AddItem(new ElectronicItem(2, "Smartphone", 10, "Samsung", 12));
        _electronics.AddItem(new ElectronicItem(3, "Headphones", 15, "JBL", 6));

        // Groceries
        _groceries.AddItem(new GroceryItem(101, "Butter", 50, DateTime.Now.AddDays(10)));
        _groceries.AddItem(new GroceryItem(102, "Milk", 30, DateTime.Now.AddDays(5)));
        _groceries.AddItem(new GroceryItem(103, "Bread", 20, DateTime.Now.AddDays(3)));
    }

    // Print all items for any repo
    public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
    {
        var items = repo.GetAllItems();
        if (items.Count == 0)
        {
            Console.WriteLine("No items found.");
            return;
        }
        foreach (var item in items)
        {
            Console.WriteLine(item.ToString());
        }
    }

    // Increase stock with exception handling
    public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
    {
        try
        {
            var item = repo.GetItemById(id);
            var newQty = item.Quantity + quantity;
            repo.UpdateQuantity(id, newQty);
            Console.WriteLine($"Increased stock for '{item.Name}' (ID: {id}). New quantity: {newQty}");
        }
        catch (DuplicateItemException dex) // unlikely here but kept for completeness
        {
            Console.WriteLine($"Duplicate Error: {dex.Message}");
        }
        catch (ItemNotFoundException inf)
        {
            Console.WriteLine($"Not Found: {inf.Message}");
        }
        catch (InvalidQuantityException iq)
        {
            Console.WriteLine($"Invalid Quantity: {iq.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error while increasing stock: {ex.Message}");
        }
    }

    // Remove item with exception handling
    public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
    {
        try
        {
            repo.RemoveItem(id);
            Console.WriteLine($"Item with ID {id} removed successfully.");
        }
        catch (ItemNotFoundException inf)
        {
            Console.WriteLine($"Not Found: {inf.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error while removing item: {ex.Message}");
        }
    }

    // Expose repos so Main can run tests
    public InventoryRepository<ElectronicItem> ElectronicsRepo => _electronics;
    public InventoryRepository<GroceryItem> GroceriesRepo => _groceries;
}

public class Program
{
    public static void Main()
    {
        var manager = new WareHouseManager();

        Console.WriteLine("Seeding data...");
        manager.SeedData();

        Console.WriteLine("\n--- Grocery Items ---");
        manager.PrintAllItems(manager.GroceriesRepo);

        Console.WriteLine("\n--- Electronic Items ---");
        manager.PrintAllItems(manager.ElectronicsRepo);

        Console.WriteLine("\n\n--- Testing Exception Scenarios ---");

        //Try to add a duplicate item (duplicate ID)
        Console.WriteLine("\nAttempting to add duplicate electronic item (ID:1) ...");
        try
        {
            manager.ElectronicsRepo.AddItem(new ElectronicItem(1, "Tablet", 3, "Apple", 18)); // ID 1 already exists
        }
        catch (DuplicateItemException dex)
        {
            Console.WriteLine($"DuplicateItemException caught: {dex.Message}");
        }

        //Try to remove a non-existent item
        Console.WriteLine("\nAttempting to remove non-existent electronic item (ID:999) ...");
        try
        {
            manager.ElectronicsRepo.RemoveItem(999);
        }
        catch (ItemNotFoundException inf)
        {
            Console.WriteLine($"ItemNotFoundException caught: {inf.Message}");
        }

        //Try to update with invalid (negative) quantity
        Console.WriteLine("\nAttempting to set negative quantity for electronic item (ID:2) ...");
        try
        {
            manager.ElectronicsRepo.UpdateQuantity(2, -10); // invalid quantity
        }
        catch (InvalidQuantityException iq)
        {
            Console.WriteLine($"InvalidQuantityException caught: {iq.Message}");
        }

        // Show inventory after tests
        Console.WriteLine("\n--- Final Electronic Items ---");
        manager.PrintAllItems(manager.ElectronicsRepo);

        Console.WriteLine("\n--- Final Grocery Items ---");
        manager.PrintAllItems(manager.GroceriesRepo);

        Console.WriteLine("\nProgram finished.");
    }
}

