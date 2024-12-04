using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediaInventoryMVC.Data;
using MediaInventoryMVC.Models;
using System;
using System.Linq;

namespace MediaInventoryMVC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransactionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TransactionController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult AddTransaction([FromBody] Transaction transaction)
        {
            // Check if the InventoryId corresponds to a valid inventory item
            var inventory = _context.Inventories.Find(transaction.InventoryId);
            if (inventory == null)
            {
                return NotFound("Inventory item not found.");
            }

            // Ensure the item is available for borrowing
            if (transaction.TransactionType == "Borrow" && !inventory.IsAvailable)
            {
                return BadRequest("Item is not available.");
            }

            // Validate Reason for borrowing
            if (transaction.TransactionType == "Borrow" && string.IsNullOrEmpty(transaction.Reason))
            {
                return BadRequest("Reason for borrow is required.");
            }

            // Update inventory availability based on transaction type
            if (transaction.TransactionType == "Borrow")
            {
                inventory.IsAvailable = false;
            }
            else if (transaction.TransactionType == "Return")
            {
                inventory.IsAvailable = true;
            }

            // Add transaction details
            transaction.TransactionDate = DateTime.UtcNow;
            _context.Transactions.Add(transaction);
            _context.SaveChanges();

            return Ok(new { message = "Transaction recorded successfully." });
        }



        // Get all transactions
        [HttpGet]
        public IActionResult GetAllTransactions()
        {
            var transactions = _context.Transactions
                                       .Include(t => t.Inventory)
                                       .ToList();

            if (!transactions.Any())
            {
                return NotFound("No transactions found.");
            }

            return Ok(transactions);
        }

        // Get transactions for a specific inventory item
        [HttpGet("{inventoryId}")]
        public IActionResult GetTransactionsByInventory(int inventoryId)
        {
            var transactions = _context.Transactions
                                       .Include(t => t.Inventory)
                                       .Where(t => t.InventoryId == inventoryId)
                                       .ToList();

            if (!transactions.Any())
            {
                return NotFound("No transactions found for the specified inventory item.");
            }

            return Ok(transactions);
        }

        // Get transactions filtered by user or date range
        [HttpGet("filter")]
        public IActionResult GetFilteredTransactions(string userName = null, string reason = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Transactions.Include(t => t.Inventory).AsQueryable();

            if (!string.IsNullOrEmpty(userName))
            {
                query = query.Where(t => t.UserName == userName);
            }

            if (!string.IsNullOrEmpty(reason))
            {
                query = query.Where(t => t.Reason.Contains(reason));
            }

            if (startDate.HasValue)
            {
                query = query.Where(t => t.TransactionDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(t => t.TransactionDate <= endDate.Value);
            }

            var transactions = query.ToList();

            if (!transactions.Any())
            {
                return NotFound("No transactions found with the specified filters.");
            }

            return Ok(transactions);
        }
    }
}
