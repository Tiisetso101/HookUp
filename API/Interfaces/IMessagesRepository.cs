using API.Data.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IMessagesRepository
    {
        void AddMessage(Message message);

        void DeleteMessage(Message message);

        Task<Message> GetMessage(int Id);

        Task<PagedList<MessagesDto>> GetMessagesForUser(MessageParams messageParams);

        Task<IEnumerable<MessagesDto>> GetMessageThread(string currentUsername, string recipientUsername);

        Task<bool> SaveAllAsync();
    }
}