using System.Threading.Tasks;
using Application.Photos;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class PhotosController : BaseController
    {
        [HttpPost]
        public async Task<IActionResult> Add([FromForm]Add.Command command)
        {
            var response = await Mediator.Send(command);

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await Mediator.Send(new Delete.Command(id));

            return Ok();
        }
    }
}