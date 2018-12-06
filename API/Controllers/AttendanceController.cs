using System.Threading.Tasks;
using Application.Activities;
using Application.Attendances;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/activities")]
    public class AttendanceController : BaseController
    {
        [HttpPost("{id}/attend")]
        public async Task<ActivityDto> Add(int id)
        {
            return await Mediator.Send(new Add.Command(id));
        }

        [HttpDelete("{id}/attend")]
        public async Task<Unit> Delete(int id)
        {
            return await Mediator.Send(new Delete.Command(id));
        }
    }
}