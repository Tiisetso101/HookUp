using API.Data.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;
namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, MemberDTO>()
            .ForMember(dest => dest.PhotoUrl,
            opt => opt.MapFrom(src => src.Photos.FirstOrDefault(x => x.isMain).Url))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            CreateMap<Photo, PhotoDTO>();
            CreateMap<MemberUpdateDTO, User>();
            CreateMap<RegisterDTO, User>();
            CreateMap<Message, MessagesDto>()
              .ForMember(d => d.SenderPhotoUrl, o => o.MapFrom(s => s.Sender.Photos
              .FirstOrDefault(x => x.isMain).Url))
              .ForMember(d => d.RecipientPhotoUrl, o => o.MapFrom(r => r.Recipient.Photos
              .FirstOrDefault(x => x.isMain).Url));
            CreateMap<DateTime, DateTime>().ConvertUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));
            CreateMap<DateTime?, DateTime?>().ConvertUsing(d => d.HasValue? 
           DateTime.SpecifyKind(d.Value, DateTimeKind.Utc) : null );
        }

    }
}