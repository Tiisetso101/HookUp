using API.Data.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers
{


    public class UsersController : BaseApiController
    {
        public IMapper _mapper { get; }
        public IPhotoService _photoService { get; }

        public IUserRepository _repository { get; }
        public UsersController(IUserRepository repository, IMapper mapper, IPhotoService photoService)
        {
            _photoService = photoService;
            _mapper = mapper;
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDTO>>> getUsers()
        {
            var users = await _repository.GetMembersAsync();
            return Ok(users);
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDTO>> getUser(string username)
        {
            return await _repository.GetMemberAsync(username);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDTO memberUpdateDTO)
        {
            var username = User.GetUsername();
            var user = await _repository.GetUserByUsernameAsync(username);
            if (user == null)
            {
                return NotFound();
            }

            _mapper.Map(memberUpdateDTO, user);

            if (await _repository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update user");
        }
        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDTO>> addPhoto(IFormFile file)
        {
            var user = await _repository.GetUserByUsernameAsync(User.GetUsername());
            if (user == null) return NotFound();

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if (user.Photos.Count == 1) photo.isMain = true;

            user.Photos.Add(photo);

            if (await _repository.SaveAllAsync())
            {


                return CreatedAtAction(nameof(getUser), new { username = user.UserName },
                _mapper.Map<PhotoDTO>(photo));

            }

            return BadRequest("Problem saving photo");

        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _repository.GetUserByUsernameAsync(User.GetUsername());

            if (user == null) return NotFound();

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo == null) return NotFound();

            if (photo.isMain) return BadRequest("Photo is already the main");

            var currentMain = user.Photos.FirstOrDefault(x => x.isMain);

            if (currentMain != null) currentMain.isMain = false;

            photo.isMain = true;

            if (await _repository.SaveAllAsync()) return NoContent();

            return BadRequest("Problem setting main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _repository.GetUserByUsernameAsync(User.GetUsername());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo == null) return NotFound();

            if (photo.isMain) return BadRequest("You cannot delete this!");

            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);

            if (await _repository.SaveAllAsync()) return Ok();

            return BadRequest("There was a problem deleting photo");



        }


    }
}