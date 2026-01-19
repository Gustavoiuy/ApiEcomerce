
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiEcommerce.Models.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ApiEcommerce.Repository;

public class UserRepository : IUserRepository
{
    public readonly ApplicationDbContext _db;
    private string? secretKey;

    public UserRepository(ApplicationDbContext db, IConfiguration configuration)

    {
        _db = db;
        secretKey = configuration.GetValue<string>("ApiSettings:SecretKey");
    }


    public Users? GetUser(int id)
    {
        return _db.Users.FirstOrDefault(u => u.Id == id);
    }

    public ICollection<Users> GetUsers()
    {
        return _db.Users.OrderBy(u => u.UserName).ToList();
    }

    public bool IsuniqueUser(string username)
    {
        return !_db.Users.Any(u => u.UserName.ToLower().Trim() == username.ToLower().Trim());
    }

    public async Task<UserLogginResponseDto> Login(UserLogginDto userLogginDto)
    {
        if (string.IsNullOrEmpty(userLogginDto.Username))
        {
            return new UserLogginResponseDto()
            {
                Token = "",
                User = null,
                Message = "El Username es requerido "
            };
        }
        var user = await _db.Users.FirstOrDefaultAsync<Users>(u => u.UserName.ToLower().Trim() == userLogginDto.Username.ToLower().Trim());
        if (user == null)
        {
            return new UserLogginResponseDto()
            {
                Token = "",
                User = null,
                Message = "Username no encontrado"
            };
        }
        if (!BCrypt.Net.BCrypt.Verify(userLogginDto.Password, user.Password))
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
        var key = Encoding.UTF8.GetBytes(secretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
          {
        new Claim("id",user.Id.ToString()),
        new Claim("username",user.UserName),
        new Claim(ClaimTypes.Role,user.Role ?? string.Empty),
      }
          ),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = handlerToken.CreateToken(tokenDescriptor);
        return new UserLogginResponseDto()
        {
            Token = handlerToken.WriteToken(token),
            User = new RegisterUserDto()
            {
                Username = user.UserName,
                Name = user.Name,
                Role = user.Role,
                Password = user.Password ?? ""
            },
            Message = "Usuario logueado correctamente"
        };
    }

    public async Task<Users> RegisterUser(CreateUserDto createUserDto)
    {
        var encriptedPassword = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);
        var user = new Users()
        {
            UserName = createUserDto.Username ?? "No username",
            Name = createUserDto.Name,
            Role = createUserDto.Role,
            Password = encriptedPassword
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }
}