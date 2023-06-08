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

    [Authorize]
    public class UsersController : BaseApiController
    {
        public IMapper _mapper { get; }
        
        public IUserRepository _repository { get; }
        public UsersController(IUserRepository repository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
        }

        [HttpGet]
        public async Task <ActionResult<IEnumerable<MemberDTO>>> getUsers()
        {
           var users = await _repository.GetMembersAsync();
           return Ok(users);           
        }

        [HttpGet("{username}")]
        public async Task <ActionResult<MemberDTO>> getUser(string username)
        {
            return  await _repository.GetMemberAsync(username);
        }

        
    }
}