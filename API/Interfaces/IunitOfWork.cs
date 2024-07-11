using API.Interfaces;

namespace APi.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository userRepository{ get; }

        IMessagesRepository messagesRepository{ get; }

        ILikesRepository likesRepository{ get; }

        Task<bool> Complete();

        bool HasChanges();

        
    }
}