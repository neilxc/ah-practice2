using System.Threading.Tasks;
using Application.Activities;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
  public class ActivitiesController : BaseController
  {
    [HttpPost]
    public async Task<IActionResult> Create(Create.Command command)
    {
        var response = await Mediator.Send(command);

        return CreatedAtRoute("GetActivity", new {id = response.Id}, response);
    }

    [HttpGet]
    public async Task<ActivitiesEnvelope> List(string sort, string username, int? limit, int? offset)
    {
        return await Mediator.Send(new List.Query(sort, username, limit, offset));
    }

    [HttpGet("{id}", Name = "GetActivity")]
    public async Task<ActivityDto> Details(int id)
    {
        return await Mediator.Send(new Details.Query{Id = id});
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "IsActivityHost")]
    public async Task<ActivityDto> Edit(int id, Edit.Command command)
    {
        command.Id = id;
        return await Mediator.Send(command);
    }
  }
}