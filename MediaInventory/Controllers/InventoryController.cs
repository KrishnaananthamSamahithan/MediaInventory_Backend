using Microsoft.AspNetCore.Mvc;
using MediaInventoryMVC.Models;
using MediaInventoryMVC.Data;
using Microsoft.AspNetCore.Authorization;


[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "User")]

public class InventoryController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public InventoryController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public IActionResult AddInventory([FromBody] Inventory inventory)
    {
        if (inventory == null)
            return BadRequest("Ïnvalid inventory data.");

        _context.Inventories.Add(inventory);
        _context.SaveChanges();

        return CreatedAtAction(nameof(GetInventoryById), new { id = inventory.Id }, inventory);

    }

    [HttpGet]
    public IActionResult GetAllInventory()
    {
        var inventories = _context.Inventories.ToList();
        if (inventories == null || inventories.Count == 0)
        {
            return NotFound("No inventory items found.");
        }

        return Ok(inventories);
    }



    [HttpGet("{id}")]
    public IActionResult GetInventoryById(int id)
    {
        var inventory = _context.Inventories.Find(id);
        if (inventory == null)
        {
            return NotFound("Inventory item not found.");
        }

        return Ok(inventory);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateInventory(int id, [FromBody] Inventory inventory)
    {
        if (id != inventory.Id)
        {
            return BadRequest("Inventory ID mismatch.");
        }

        var existingInventory = _context.Inventories.Find(id);
        if (existingInventory == null)
        {
            return NotFound("Inventory item not found.");
        }

        // Update the existing inventory item
        existingInventory.Name = inventory.Name;
        existingInventory.Description = inventory.Description;
        existingInventory.IsAvailable = inventory.IsAvailable;

        _context.Inventories.Update(existingInventory);
        _context.SaveChanges();

        return Ok(new { message = "Item updated successfully" });
    }


    [HttpDelete("{id}")]
    public IActionResult DeleteInventory(int id)
    {
        var inventory = _context.Inventories.Find(id);
        if (inventory == null)
        {
            return NotFound("Inventory item not found.");
        }

        _context.Inventories.Remove(inventory);
        _context.SaveChanges();

        return Ok(new { message = "Item deleted successfully" });
    }



}

