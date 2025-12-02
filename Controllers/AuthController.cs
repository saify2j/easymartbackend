using EasyMart.API.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace EasyMart.API.Controllers;
[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly IJwtTokenService _jwt;

    public AuthController(IConfiguration config, IJwtTokenService jwt)
    {
        _config = config;
        _jwt = jwt;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(EasyMart.API.Application.DTOs.Auth.LoginRequest request)
    {
        using var conn = new SqlConnection(_config.GetConnectionString("Default"));
        await conn.OpenAsync();

        var cmd = new SqlCommand(@"
            SELECT u.UserId, u.Username, u.PasswordHash, u.PasswordSalt,
                   STRING_AGG(r.Name, ',') AS Roles
            FROM Users u
            JOIN UserRoles ur ON u.UserId = ur.UserId
            JOIN Roles r ON r.RoleId = ur.RoleId
            WHERE u.Username = @Username AND u.IsActive = 1
            GROUP BY u.UserId, u.Username, u.PasswordHash, u.PasswordSalt",
            conn
        );

        cmd.Parameters.AddWithValue("@Username", request.Username);

        using var reader = await cmd.ExecuteReaderAsync();

        if (!reader.Read())
            return Unauthorized("Invalid credentials");

        var hash = (byte[])reader["PasswordHash"];
        var salt = (byte[])reader["PasswordSalt"];

        if (!VerifyPassword(request.Password, hash, salt))
            return Unauthorized("Invalid credentials");

        var roles = reader["Roles"].ToString()!.Split(',');

        var token = _jwt.GenerateToken(
            (int)reader["UserId"],
            reader["Username"].ToString()!,
            roles
        );

        return Ok(new { token });
    }
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(EasyMart.API.Application.DTOs.Auth.RegisterRequest request)
    {
        // Generate random salt
        var salt = RandomNumberGenerator.GetBytes(16);

        // Hash password using PBKDF2 (simple + standard)
        var hash = HashPassword(request.Password, salt);

        using var conn = new SqlConnection(_config.GetConnectionString("Default"));
        await conn.OpenAsync();

        var cmd = new SqlCommand(@"
        INSERT INTO Users (Username, Email, PasswordHash, PasswordSalt)
        VALUES (@Username, @Email, @Hash, @Salt);
        SELECT SCOPE_IDENTITY();
    ", conn);

        cmd.Parameters.AddWithValue("@Username", request.Username);
        cmd.Parameters.AddWithValue("@Email", request.Email);
        cmd.Parameters.AddWithValue("@Hash", hash);
        cmd.Parameters.AddWithValue("@Salt", salt);

        var userId = Convert.ToInt32(await cmd.ExecuteScalarAsync());

        // Assign default role (User)
        var roleCmd = new SqlCommand(@"
        INSERT INTO UserRoles (UserId, RoleId)
        SELECT @UserId, RoleId FROM Roles WHERE Name = 'User'
    ", conn);

        roleCmd.Parameters.AddWithValue("@UserId", userId);
        await roleCmd.ExecuteNonQueryAsync();

        return Ok("User registered successfully");
    }
    private static bool VerifyPassword(string password, byte[] hash, byte[] salt)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(
            password, salt, 100_000, HashAlgorithmName.SHA256
        );
        return pbkdf2.GetBytes(32).SequenceEqual(hash);
    }
    private static byte[] HashPassword(string password, byte[] salt)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(
            password,
            salt,
            100_000,
            HashAlgorithmName.SHA256
        );

        return pbkdf2.GetBytes(32); // 256-bit hash
    }
}
