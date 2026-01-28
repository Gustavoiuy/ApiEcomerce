using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.Entities;
using Application.Dtos;
using Application.IRepository;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Infrastructure.Data;
using Microsoft.Extensions.Configuration; 
namespace Infrastructure.Repository;

public class UserRepository : GenericRepository<Users>, IUserRepository
{
    public readonly ApplicationDbContext _db;
    private string? secretKey;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    public UserRepository(ApplicationDbContext db, IConfiguration configuration,
                          UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        : base(db)
    {
        _db = db;
        secretKey = configuration.GetValue<string>("ApiSettings:SecretKey");
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<ICollection<ApplicationUser>> GetUsersAsync()
    {
        return await _db.ApplicationUsers.OrderBy(u => u.UserName).ToListAsync();
    }

    public async Task<ApplicationUser?> GetUserAsync(string id)
    {
        return await _db.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<bool> IsUniqueUserAsync(string username)
    {
        return !await _db.LegacyUsers.AnyAsync(u => u.UserName.ToLower().Trim() == username.ToLower().Trim());
    }

    public async Task<UserLogginResponseDto> LoginAsync(UserLogginDto userLoginDto)
    {
        if (string.IsNullOrEmpty(userLoginDto.Username))
        {
            return new UserLogginResponseDto()
            {
                Token = "",
                User = null,
                Message = "El Username es requerido "
            };
        }
        var user = await _db.ApplicationUsers.FirstOrDefaultAsync<ApplicationUser>(u => u.UserName != null && u.UserName.ToLower().Trim() == userLoginDto.Username.ToLower().Trim());
        if (user == null)
        {
            return new UserLogginResponseDto()
            {
                Token = "",
                User = null,
                Message = "Username no encontrado"
            };
        }
        if (userLoginDto.Password == null)
        {
            return new UserLogginResponseDto()
            {
                Token = "",
                User = null,
                Message = "Password requerido"
            };
        }
        bool isValid = await _userManager.CheckPasswordAsync(user, userLoginDto.Password);
        if (!isValid)
        {
            return new UserLogginResponseDto()
            {
                Token = "",
                User = null,
                Message = "Credenciales son incorrectas"
            };
        }
        // JWT
        var handlerToken = new JwtSecurityTokenHandler();
        if (string.IsNullOrWhiteSpace(secretKey))
        {
            throw new InvalidOperationException("SecretKey no esta configurada");
        }
        var roles = await _userManager.GetRolesAsync(user);
        var key = Encoding.UTF8.GetBytes(secretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("id",user.Id.ToString()),
                new Claim("username",user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Role, roles.FirstOrDefault() ?? string.Empty),
            }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = handlerToken.CreateToken(tokenDescriptor);
        return new UserLogginResponseDto()
        {
            Token = handlerToken.WriteToken(token),
            User = user.Adapt<UserDataDto>(),
            Message = "Usuario logueado correctamente"
        };
    }

    public async Task<UserDataDto> RegisterUserAsync(CreateUserDto createUserDto)
    {
        if (string.IsNullOrEmpty(createUserDto.Username))
            throw new ArgumentNullException("El Username es requerido");
        if (createUserDto.Password == null)
            throw new ArgumentNullException("El Password es requerido");
        var user = new ApplicationUser()
        {
            UserName = createUserDto.Username,
            Email = createUserDto.Username,
            NormalizedEmail = createUserDto.Username.ToUpper(),
            Name = createUserDto.Name
        };
        var result = await _userManager.CreateAsync(user, createUserDto.Password);
        if (result.Succeeded)
        {
            var userRole = createUserDto.Role ?? "User";
            var roleExists = await _roleManager.RoleExistsAsync(userRole);
            if (!roleExists)
            {
                var identityRole = new IdentityRole(userRole);
                await _roleManager.CreateAsync(identityRole);
            }
            await _userManager.AddToRoleAsync(user, userRole);
            var createdUser = await _db.ApplicationUsers.FirstOrDefaultAsync(u => u.UserName == createUserDto.Username);
            return createdUser.Adapt<UserDataDto>();
        }
        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        throw new ApplicationException($"No se pudo realizar el registro: {errors}");
    }
}