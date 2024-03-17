using API.Data.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        void Update(User user);
        Task<bool> SaveAllAsync();

        Task<IEnumerable<User>> GetUserAsync();

        Task<User> GetUserByIdAsync(int id);

        Task<User> GetUserByUsernameAsync(string username);

        Task<MemberDTO> GetMemberAsync(string username);

        Task<PagedList<MemberDTO>> GetMembersAsync(UserParams userParams);



    }
}