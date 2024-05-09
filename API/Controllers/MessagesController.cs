using API.Data.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class MessagesController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessagesRepository _messagesRepository;
        private readonly IMapper _mapper;

        public MessagesController(IUserRepository userRepository, IMessagesRepository messagesRepository
        , IMapper mapper)
        {
            _userRepository = userRepository;
            _messagesRepository = messagesRepository;
            _mapper = mapper;
        }
        [HttpPost]
        public async Task<ActionResult<MessagesDto>> CreateMessage(CreateMessageDto messageDto)
        {
            var username = User.GetUsername();

            if (username == messageDto.RecipientUsername)
                return BadRequest("You cannot send messages to yourself");

            var sender = await _userRepository.GetUserByUsernameAsync(username);

            var recipient = await _userRepository.GetUserByUsernameAsync(messageDto.RecipientUsername);

            if (recipient == null)
                return NotFound();

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUserName = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = messageDto.Content
            };

            _messagesRepository.AddMessage(message);

            if (await _messagesRepository.SaveAllAsync())
                return Ok(_mapper.Map<MessagesDto>(message));

            return BadRequest("Message not sent");

        }
        [HttpGet]
        public async Task<ActionResult<PagedList<MessagesDto>>> GetMessagesForUser([FromQuery]
        MessageParams messageParams)
        {
            messageParams.UserName = User.GetUsername();

            var messages = await _messagesRepository.GetMessagesForUser(messageParams);

            Response.AddPaginationHeader(new PaginationHeder(messages.CurrentPage, messages.PageSize,
            messages.TotalCount, messages.TotlaPages));

            return messages;
        }
        [HttpGet("thread/{unsername}")]

        public async Task<ActionResult<IEnumerable<MessagesDto>>> GetMessagesThread(string unsername)
        {
            var currentUsername = User.GetUsername();

            return Ok(await _messagesRepository.GetMessageThread(currentUsername, unsername));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.GetUsername();

            var message = await _messagesRepository.GetMessage(id);

            if (message.SenderUserName != username && message.RecipientUsername != username)
                return Unauthorized();

            if (message.SenderUserName == username) message.SenderDeleted = true;
            if (message.RecipientUsername == username) message.RecipientDeleted = true;

            if (message.SenderDeleted && message.RecipientDeleted)
            {
                _messagesRepository.DeleteMessage(message);
            }

            if (await _messagesRepository.SaveAllAsync())
            {
                return Ok();
            }
            return BadRequest("Problem deleting message");

        }
    }
}