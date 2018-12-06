using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Activities;
using Application.Interfaces;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Attendances
{
  public class Add
  {
    public class Command : IRequest<ActivityDto>
    {
      public Command(int id)
      {
        Id = id;
      }
      public int Id { get; set; }
    }

    public class Handler : IRequestHandler<Command, ActivityDto>
    {
      private readonly DataContext context;
      private readonly IUserAccessor userAccessor;
      private readonly IMapper mapper;
      public Handler(DataContext context, IUserAccessor userAccessor, IMapper mapper)
      {
        this.mapper = mapper;
        this.userAccessor = userAccessor;
        this.context = context;
      }

      public async Task<ActivityDto> Handle(Command request, CancellationToken cancellationToken)
      {
        var activity = await context.Activities
            .Include(x => x.Attendees)
            .ThenInclude(x => x.AppUser)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        
        var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == userAccessor.GetCurrentUsername());

        var attendance = await context.ActivityAttendees.FirstOrDefaultAsync(
            x => x.ActivityId == activity.Id && x.AppUserId == user.Id, cancellationToken
        );

        if (attendance == null)
        {
            attendance = new ActivityAttendee
            {
                Activity = activity,
                ActivityId = activity.Id,
                AppUser = user,
                AppUserId = user.Id,
                DateJoined = DateTime.Now,
                IsHost = false
            };

            await context.ActivityAttendees.AddAsync(attendance, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        return mapper.Map<Activity, ActivityDto>(activity);
      }
    }
  }
}