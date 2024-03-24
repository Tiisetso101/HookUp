using API.Data.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesReposity(DataContext dataContext) : ILikesRepository
    {
        private readonly DataContext _dataContext = dataContext;

        public async Task<UserLike> GetUserLike(int sourceUserId, int targetUserId)
        {
            return await _dataContext.Likes.FindAsync(sourceUserId, targetUserId);
        }

        public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
        {
            var users = _dataContext.users.OrderBy(u => u.UserName).AsQueryable();
            var likes = _dataContext.Likes.AsQueryable();


            if (likesParams.Predicate == "liked")
            {
                likes = likes.Where(like => like.SourceUserID == likesParams.UserId);
                users = likes.Select(like => like.TargetUser);
            }

            if (likesParams.Predicate == "likedBy")
            {
                likes = likes.Where(like => like.TargetUserID == likesParams.UserId);
                users = likes.Select(like => like.SourceUser);
            }

            var likedUsers = users.Select(user => new LikeDto
            {

                UserName = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.isMain).Url,
                City = user.City,
                Id = user.Id
            });

            return await PagedList<LikeDto>.CreateAsync(likedUsers, likesParams.PageNumber, likesParams.PageSize);
        }

        public async Task<User> GetUserWithLikes(int userId)
        {
            return await _dataContext.users
                        .Include(x => x.LikedUsers)
                        .FirstOrDefaultAsync(x => x.Id == userId);
        }
    }
}