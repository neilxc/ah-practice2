using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Activities;
using Application.Errors;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Attendances
{
  public class Delete
  {

    public class Command : IRequest
    {
      public Command(int id)
      {
        Id = id;
      }
      public int Id { get; }
    }

    public class Handler : IRequestHandler<Command>
    {
      private readonly DataContext context;
      private readonly IUserAccessor userAccessor;
      public Handler(DataContext context, IUserAccessor userAccessor)
      {
        this.userAccessor = userAccessor;
        this.context = context;
      }

      public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
      {
        var activity = context.Activities.GetAllData()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (activity == null)
          throw new RestException(HttpStatusCode.NotFound, new { Activity = "Not Found" });

        var user = context.Users.FirstOrDefaultAsync(x => x.UserName == 
            userAccessor.GetCurrentUsername(), cancellationToken);
        
        var attendance = await context.ActivityAttendees.FirstOrDefaultAsync(
            x => x.ActivityId == activity.Id && x.AppUserId == user.Id, cancellationToken
        );

        if (attendance != null)
        {
            context.ActivityAttendees.Remove(attendance);
            await context.SaveChangesAsync(cancellationToken);
        }

        return Unit.Value;
      }
    }
  }
}