
using ApiEcommerce.Models.Dtos;

public interface IUserRepository
{

    ICollection<Users> GetUsers();
    Users? GetUser(int id);
    bool IsuniqueUser(string username);
    Task<UserLogginResponseDto> Login(UserLogginDto userLogginDto);
    Task<Users> RegisterUser(CreateUserDto createUserDto);

}