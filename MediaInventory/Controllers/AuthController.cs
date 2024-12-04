using Microsoft.AspNetCore.Mvc;
using MediaInventoryMVC.Data;
using MediaInventoryMVC.Models;
using BCrypt.Net;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly JwtHelper _jwtHelper;

    public AuthController(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _jwtHelper = new JwtHelper(configuration);
    }

    [HttpPost("signup")]
    public IActionResult SignUp([FromBody] User user)
    {
        if (_context.Users.Any(u => u.Username == user.Username))
            return BadRequest("User already exists.");

        // Hash the password before storing
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
        _context.Users.Add(user);
        _context.SaveChanges();
        return Ok("User registered successfully.");
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] User loginRequest)
    {
        var user = _context.Users.SingleOrDefault(u => u.Username == loginRequest.Username);
        if (user == null)
        {
            Console.WriteLine($"User not found: {loginRequest.Username}");
            return Unauthorized("Invalid username or password.");
        }

        Console.WriteLine($"Database PasswordHash: {user.PasswordHash}");
        Console.WriteLine($"Input Password: {loginRequest.PasswordHash}");

        if (!BCrypt.Net.BCrypt.Verify(loginRequest.PasswordHash, user.PasswordHash))
        {
            Console.WriteLine("Password verification failed.");
            return Unauthorized("Invalid username or password.");
        }

        var token = _jwtHelper.GenerateJwtToken(user);
        return Ok(new { Token = token });
    }

}
