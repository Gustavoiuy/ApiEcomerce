using Domain.Entities;
using Application.Dtos;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace Application.IRepository
{
    /// <summary>
    /// Repositorio específico para Users, hereda de IGenericRepository.
    /// Agrega métodos adicionales si son necesarios.
    /// </summary>
    public interface IUserRepository : IGenericRepository<Users>
    {
        Task<ICollection<ApplicationUser>> GetUsersAsync();
        Task<ApplicationUser?> GetUserAsync(string id);
        Task<bool> IsUniqueUserAsync(string username);
        Task<UserLogginResponseDto> LoginAsync(UserLogginDto userLogginDto);
        Task<UserDataDto> RegisterUserAsync(CreateUserDto createUserDto);
        // Agrega aquí métodos específicos de Users si los necesitas
    }
}