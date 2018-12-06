using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Security
{
  public class IsHostRequirement : IAuthorizationRequirement
  {

  }

  public class IsHostHandler : AuthorizationHandler<IsHostRequirement>
  {
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly DataContext _context;
    public IsHostHandler(IHttpContextAccessor httpContextAccessor, DataContext context)
    {
      _context = context;
      this.httpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsHostRequirement requirement)
    {
      if (context.Resource is AuthorizationFilterContext authContext)
      {
        var currentUsername = httpContextAccessor.HttpContext.User?.Claims?
          .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

        var routeInfo = authContext.RouteData.Values["id"].ToString();
        var activityId = int.Parse(routeInfo);

        var activityFromDb = _context.Activities
            .Include(a => a.Attendees)
            .ThenInclude(x => x.AppUser)
            .FirstOrDefaultAsync(x => x.Id == activityId).Result;

        var host = activityFromDb.Attendees.FirstOrDefault(x => x.IsHost);

        if (host?.AppUser?.UserName == currentUsername)
        {
          context.Succeed(requirement);
        }
      }
      else
      {
        context.Fail();
      }

      return Task.CompletedTask;
    }
  }
}