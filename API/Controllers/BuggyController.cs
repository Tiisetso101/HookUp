using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BuggyController : BaseApiController
    {
        public readonly DataContext _context;
        public BuggyController(DataContext context)
        {
            _context = context;

        }
        [Authorize]
        [HttpGet("auth")]
        public ActionResult<string> GetSecret()
        {
            return "this is a secret";
        }

        [HttpGet("not-found")]
        public ActionResult<User> GetNotFound()
        {
            var thing = _context.Users.Find(-1);
            if (thing == null) return NotFound();



            return thing;
        }

        [HttpGet("server-error")]
        public ActionResult<string> GetServerError()
        {
            var thing = _context.Users.Find(-1);
            var thingToReturn = thing.ToString();

            return thingToReturn;
        }

        [HttpGet("bad-request")]
        public ActionResult<string> GetBadRequest()
        {
            return BadRequest("bad request returned");
        }
    }
}