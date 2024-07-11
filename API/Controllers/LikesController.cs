using APi.Interfaces;
using API.Data.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class LikesController : BaseApiController
    {
        private readonly IUnitOfWork _uow;

        public LikesController(IUnitOfWork Uow)
        {
            _uow = Uow;
        }
        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId = User.GetUserID();
            var LikedUser = await _uow.userRepository.GetUserByUsernameAsync(username);
            var sourceUser = await _uow.likesRepository.GetUserWithLikes(sourceUserId);

            if (LikedUser == null)
                return NotFound();

            if (sourceUser.UserName == username)
                return BadRequest("You cannot like yourself");

            var userLike = await _uow.likesRepository.GetUserLike(sourceUserId, LikedUser.Id);


            if (userLike != null)
                return BadRequest("You already like yourself");

            userLike = new UserLike
            {
                SourceUserID = sourceUserId,
                TargetUserID = LikedUser.Id
            };

            sourceUser.LikedUsers.Add(userLike);

            if (await _uow.Complete())
                return Ok();

            return BadRequest("failed to like user");

        }

        [HttpGet]
        public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikes([FromQuery] LikesParams likesParams)
        {
            likesParams.UserId = User.GetUserID();
            var users = await _uow.likesRepository.GetUserLikes(likesParams);

            Response.AddPaginationHeader(new PaginationHeder(users.CurrentPage,
                users.PageSize, users.TotalCount, users.TotlaPages));

            return Ok(users);
        }
    }

           
}