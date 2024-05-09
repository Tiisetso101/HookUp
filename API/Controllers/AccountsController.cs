using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using API.Data;
using API.Data.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{


    public class AccountsController : BaseApiController
    {
        private DataContext _context { get; }
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        public AccountsController(DataContext context, ITokenService tokenService, IMapper mapper)
        {
            _mapper = mapper;
            _tokenService = tokenService;
            _context = context;

        }
        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDto)
        {
            if (await UserExists(registerDto.username))
                return BadRequest("UserName is already taken");

            var user = _mapper.Map<User>(registerDto);

            using var hmac = new HMACSHA512();


            user.UserName = registerDto.username;



            _context.users.Add(user);
            await _context.SaveChangesAsync();
            return new UserDTO
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            var user = await _context.users.Include(p => p.Photos)
            .SingleOrDefaultAsync(x =>
             x.UserName == loginDTO.userName);

            if (user == null)
                return Unauthorized("Invalid user name");



            return new UserDTO
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.isMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender

            };
        }


        public async Task<bool> UserExists(string username)
        {
            return await _context.users.AnyAsync(x => x.UserName == username.ToLower());
        }

    }
}