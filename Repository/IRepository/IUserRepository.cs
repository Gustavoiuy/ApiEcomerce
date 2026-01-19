
using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;

public interface IUserRepository
{

    ICollection<ApplicationUser> GetUsers();
    ApplicationUser? GetUser(string id);
    bool IsuniqueUser(string username);
    Task<UserLogginResponseDto> Login(UserLogginDto userLogginDto);
    Task<UserDataDto> RegisterUser(CreateUserDto createUserDto);

}