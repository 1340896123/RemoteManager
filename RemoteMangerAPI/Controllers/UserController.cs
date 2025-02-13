

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using RemoteMangerAPI.Entitys;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static RemoteMangerAPI.RemoteMangerDBContext;

namespace RemoteMangerAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    public readonly IConfiguration _configuration;
    public readonly ILogger<UserController> _logger;
    public readonly RemoteMangerDBContext db;
    public readonly IMemoryCache cache;

    public UserController(IConfiguration _configuration, RemoteMangerDBContext db, IMemoryCache cache, ILogger<UserController> logger)
    {
        this._configuration = _configuration;
        this._logger = logger;
        this.db = db;
        this.cache = cache;
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register(User user)
    {
        if (await db.Users.AnyAsync(u => u.Username == user.Username))
        {
            return BadRequest("User already exists.");
        }
        user.Id = Guid.NewGuid().ToString("N");
        await db.Users.AddAsync(user);
        await db.SaveChangesAsync();
        return Ok("User registered successfully.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(User user)
    {

        var tempuser = await db.Users.FirstOrDefaultAsync(u => u.Username == user.Username && u.Password == user.Password);
        if (tempuser == null)
        {
            return Unauthorized("Invalid username or password.");
        }

        var token = GenerateJwtToken(tempuser);
        return Ok(new { Token = token });
    }

    private string GenerateJwtToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
                new Claim(JwtRegisteredClaimNames.Jti, user.Id),
                new Claim(JwtRegisteredClaimNames.Name, user.Username),
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim("UserId", user.Id)
            };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(24),
            signingCredentials: credentials);

        var token22 = new JwtSecurityTokenHandler().WriteToken(token); Response.Headers.Add("token", token22);
        return token22;
    }

    [Authorize]
    [HttpPost("rdp-accounts")]
    public IActionResult CreateRdpAccount(RDPAccount request)
    {
        var rdpAccount = new RDPAccount
        {
            Id = Guid.NewGuid().ToString("N"),
            Name = request.Name,
            Pwd = request.Pwd,
            Host = request.Host,
            Account = request.Account,
            UserId = User.FindFirst("UserId").Value,
        };
     
        db.RDPAccounts.Add(rdpAccount);
        db.SaveChanges();
        return Ok(rdpAccount);
    }

    [Authorize]
    [HttpGet("rdp-accounts")]
    public IActionResult GetRdpAccounts()
    {
        return Ok(db.RDPAccounts);
    }

    [Authorize]
    [HttpGet("rdp-accounts/{id}")]
    public IActionResult GetRdpAccount(string id)
    {
        var rdpAccount = db.RDPAccounts.FirstOrDefault(a => a.Id == id);
        if (rdpAccount == null)
        {
            return NotFound("RDP account not found.");
        }

        return Ok(rdpAccount);
    }

    [Authorize]
    [HttpPut("rdp-accounts/{id}")]
    public IActionResult UpdateRdpAccount(string id, RDPAccount request)
    {
        //var rdpAccount = db.RDPAccounts.FirstOrDefault(a => a.Id == id);
        //if (rdpAccount == null)
        //{
        //    return NotFound("RDP account not found.");
        //}

        //rdpAccount.Name = request.Name;
        //rdpAccount.Pwd = request.Pwd;
        //rdpAccount.Host = request.Host;
        //rdpAccount.Account = request.Account;
        request.Id = id;
        db.Update(request);
        db.SaveChanges();
        var rdpAccount = db.RDPAccounts.FirstOrDefault(a => a.Id == id);
        return Ok(rdpAccount);
    }

    [Authorize]
    [HttpDelete("rdp-accounts/{id}")]
    public IActionResult DeleteRdpAccount(string id)
    {
        var rdpAccount = db.RDPAccounts.FirstOrDefault(a => a.Id == id);
        if (rdpAccount == null)
        {
            return NotFound("RDP account not found.");
        }

        db.Remove(rdpAccount);
        db.SaveChanges();
        return Ok("RDP account deleted successfully.");
    }
}