using API.Data.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessagesRepository
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        public MessageRepository(DataContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }
        void IMessagesRepository.AddMessage(Message message)
        {
            _dataContext.Messages.Add(message);
        }

        void IMessagesRepository.DeleteMessage(Message message)
        {
            _dataContext.Messages.Remove(message);
        }

        async Task<Message> IMessagesRepository.GetMessage(int Id)
        {
            return await _dataContext.Messages.FindAsync(Id);
        }

        async Task<PagedList<MessagesDto>> IMessagesRepository.GetMessagesForUser(MessageParams messageParams)
        {
            var query = _dataContext.Messages
            .OrderByDescending(x => x.MessageSent)
            .AsQueryable();



            var message = query.ProjectTo<MessagesDto>(_mapper.ConfigurationProvider);

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.RecipientUsername == messageParams.UserName &&
                u.RecipientDeleted == false),

                "Outbox" => query.Where(u => u.SenderUserName == messageParams.UserName &&
                u.SenderDeleted == false),
                _ => query.Where(u => u.RecipientUsername == messageParams.UserName && u.RecipientDeleted == false && u.DateRead == null)
            };

            return await PagedList<MessagesDto>
            .CreateAsync(message, messageParams.PageNumber, messageParams.PageSize);
        }

        async Task<IEnumerable<MessagesDto>> IMessagesRepository.GetMessageThread(string currentUsername, string recipientUsername)
        {
            var messages = await _dataContext.Messages
            .Include(u => u.Sender).ThenInclude(p => p.Photos)
            .Include(u => u.Recipient).ThenInclude(p => p.Photos)
            .Where(m => m.RecipientUsername == currentUsername &&
            m.RecipientDeleted == false &&
             m.SenderUserName == recipientUsername ||
             m.RecipientUsername == recipientUsername && m.SenderDeleted == false &&
             m.SenderUserName == currentUsername)
             .OrderBy(m => m.MessageSent)
             .ToListAsync();

            var unreadMessages = messages.Where(m => m.DateRead == null
            && m.RecipientUsername == currentUsername).ToList();

            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;

                }
                await _dataContext.SaveChangesAsync();
            }

            return _mapper.Map<IEnumerable<MessagesDto>>(messages);


        }

        async Task<bool> IMessagesRepository.SaveAllAsync()
        {
            return await _dataContext.SaveChangesAsync() > 0;

        }
    }
}