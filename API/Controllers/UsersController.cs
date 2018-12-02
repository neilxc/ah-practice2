using System.Threading.Tasks;
using Application.Users;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [AllowAnonymous]
    public class UsersController : BaseController
    {
        [HttpPost]
        public async Task<User> Create(Create.Command command)
        {
            return await Mediator.Send(command);
        }

        [HttpPost("login")]
        public async Task<User> Login(Login.Command command)
        {
            return await Mediator.Send(command);
        }
    }
}