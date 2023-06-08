using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using API.Data;
using API.Data.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    
    
    public class AccountsController: BaseApiController
    {
        private  DataContext _context { get; }
        private readonly ITokenService _tokenService;
        public AccountsController(DataContext context, ITokenService tokenService)
        {
            _tokenService = tokenService;
            _context = context;
            
        }
        [HttpPost("register")]
        public async Task <ActionResult<UserDTO>> Register(RegisterDTO registerDto)
        {
            if(await UserExists(registerDto.username)) 
            return BadRequest("UserName is already taken");

            using var hmac = new HMACSHA512();

            var user = new User{
                UserName = registerDto.username,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.password)),
                PasswordSalt = hmac.Key
            };

            _context.users.Add(user);
            await _context.SaveChangesAsync();
            return new UserDTO
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        } 

        [HttpPost("login")]
        public async Task <ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            var user = await _context.users.SingleOrDefaultAsync(x => 
             x.UserName == loginDTO.userName);

            if(user == null) 
                return Unauthorized("Invalid user name");

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.password));

            for(int i=0; i < computedHash.Length; i++)
            {
                if(computedHash[i] != user.PasswordHash[i])
                {
                    return Unauthorized("Invalid password");
                }
            }
            
            return new UserDTO
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        } 
    

        public async Task <bool> UserExists(string username)
        {
            return await _context.users.AnyAsync(x => x.UserName == username.ToLower());
        }
        
    }
}