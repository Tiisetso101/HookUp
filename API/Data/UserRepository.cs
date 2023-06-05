using API.Data.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
            
        }

        public async Task<MemberDTO> GetMemberAsync(string username)
        {
           return await _context.users
           .Where(x => x.UserName == username)
           .ProjectTo<MemberDTO>(_mapper.ConfigurationProvider)
           .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<MemberDTO>> GetMembersAsync()
        {
            return await _context.users
           .ProjectTo<MemberDTO>(_mapper.ConfigurationProvider)
           .ToListAsync();
        }

       

        public async Task<IEnumerable<User>> GetUserAsync()
        {
           return await _context.users.Include(p => p.Photos).ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.users.FindAsync(id);
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
          return await _context.users.Include(p => p.Photos).
         SingleOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<bool> SaveAllAsync()
        {
          return await _context.SaveChangesAsync() > 0;
        }

        public void Update(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}