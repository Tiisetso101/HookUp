using System.Globalization;
using APi.Interfaces;
using API.Data.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class MessageHub: Hub
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IHubContext<PresenceHub> _presenceHub;

        public MessageHub(IUnitOfWork uow, IMapper mapper, IHubContext<PresenceHub> presenceHub)
        {
            _uow = uow;
            _mapper = mapper;
            _presenceHub = presenceHub;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext.Request.Query["user"];
            var groupName = GetGroupName(Context.User.GetUsername(),otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName); 
            var group = await AddToGroup(groupName);

            await Clients.Group(groupName).SendAsync("UpdatedGroup",group);
           
           var messages = await _uow.messagesRepository.GetMessageThread(Context.User.GetUsername(), otherUser);

           if(_uow.HasChanges()) await _uow.Complete();

           await Clients.Caller.SendAsync("RecieveMessageThread", messages); 
        }

        public override  async Task OnDisconnectedAsync(Exception ex)
        {
              var group = await RemoveFromMessageGroup();
               await Clients.Group(group.Name).SendAsync("UpdatedGroup");
              await  base.OnDisconnectedAsync(ex);
        }

        public async Task SendMessage(CreateMessageDto messagesDto)
        {
             var username = Context.User.GetUsername();

            if (username == messagesDto.RecipientUsername)
                throw new HubException("You cannot send messages to yourself");

            var sender = await _uow.userRepository.GetUserByUsernameAsync(username);

            var recipient = await _uow.userRepository.GetUserByUsernameAsync(messagesDto.RecipientUsername);

            if (recipient == null)
                throw new HubException("User not found");

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUserName = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = messagesDto.Content
            };
            
            var groupName = GetGroupName(sender.UserName, recipient.UserName);

            var group = await _uow.messagesRepository.GetMessageGroup(groupName);

            if(group.connections.Any(x => x.Username == username))
            {
                message.DateRead = DateTime.UtcNow;
            }
            else
            {
                var connections = await PresenceTracker.GetConnectionsForUser(recipient.UserName);
                if(connections != null)
                {
                    await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageRecieved", 
                    new {username = sender.UserName, knownAs = sender.KnownAs});
                }
            }

            _uow.messagesRepository.AddMessage(message);

            if (await _uow.Complete())
            {
                await Clients.Group(groupName).SendAsync("newMessage", _mapper.Map<MessagesDto>(message));
            }
                
        }

        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller} - {other}" : $"{other} - {caller}";

        }

        private async Task<Group> AddToGroup(string groupName)
        {
            var group = await _uow.messagesRepository.GetMessageGroup(groupName);
            var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());

            if(group == null)
            {
                group = new Group(groupName);
                _uow.messagesRepository.AddGroup(group);
            }

            group.connections.Add(connection);
            if( await _uow.Complete())
                     return group;

            throw new HubException("failed to add group");
        }

        private async Task <Group> RemoveFromMessageGroup()
        {
           var group = await _uow.messagesRepository.GetgroupForConnection(Context.ConnectionId);
           var connection = group.connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            _uow.messagesRepository.RemoveConnection(connection);
           if(await _uow.Complete()) 
                    return group ;

            throw new HubException("Could not remove group");
        }

    }
}