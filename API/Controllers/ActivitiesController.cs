using System.Threading.Tasks;
using Application.Activities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class ActivitiesController : ControllerBase
  {
    private readonly IMediator mediator;
    public ActivitiesController(IMediator mediator)
    {
      this.mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(Create.Command command)
    {
        var response = await mediator.Send(command);

        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var activities = await mediator.Send(new List.Query());

        return Ok(activities);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Details(int id)
    {
        var activity = await mediator.Send(new Details.Query{Id = id});

        return Ok(activity);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Edit(int id, Edit.Command command)
    {
        command.Id = id;
        var activity = await mediator.Send(command);

        return Ok(activity);
    }
  }
}