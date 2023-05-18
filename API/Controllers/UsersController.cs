using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace API.Controllers
{
  
    public class UsersController : BaseApiController
    {
        private DataContext _context { get; }
        public UsersController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task <ActionResult<IEnumerable<User>>> getUsers()
        {
            var users = await _context.users.ToListAsync();
            return users;
        }

        [HttpGet("{id}")]
        public async Task <ActionResult<User>> getUser(int id)
        {
            var user = await _context.users.FindAsync(id);
            return user;
        }

        
    }
}