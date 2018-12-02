using System.Threading.Tasks;
using Application.Activities;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
  public class ActivitiesController : BaseController
  {
    [HttpPost]
    public async Task<IActionResult> Create(Create.Command command)
    {
        var response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActivitiesEnvelope> List()
    {
        return await Mediator.Send(new List.Query());
    }

    [HttpGet("{id}")]
    public async Task<Activity> Details(int id)
    {
        return await Mediator.Send(new Details.Query{Id = id});
    }

    [HttpPut("{id}")]
    public async Task<Activity> Edit(int id, Edit.Command command)
    {
        command.Id = id;
        return await Mediator.Send(command);
    }
  }
}