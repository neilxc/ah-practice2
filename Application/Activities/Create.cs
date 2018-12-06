using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
  public class Create
  {
    public class ActivityData
    {
      public string Title { get; set; }
      public string Description { get; set; }
      public DateTime Date { get; set; }
      public string City { get; set; }
      public string Venue { get; set; }
      public GeoCoordinate GeoCoordinate { get; set; }
    }

    public class Command : IRequest<ActivityDto>
    {
      public ActivityData Activity { get; set; }
    }

    public class GeoCoordinateValidator : AbstractValidator<GeoCoordinate>
    {
      public GeoCoordinateValidator()
      {
        RuleFor(x => x.Latitude).NotEmpty().NotNull();
        RuleFor(x => x.Longitude).NotEmpty().NotNull();
      }
    }
    public class CommandValidator : AbstractValidator<Command>
    {
      public CommandValidator()
      {
        RuleFor(x => x.Activity.Title).NotEmpty().NotNull();
        RuleFor(x => x.Activity.Description).NotEmpty().NotNull();
        RuleFor(x => x.Activity.Date).NotEmpty().NotNull();
        RuleFor(x => x.Activity.City).NotEmpty().NotNull();
        RuleFor(x => x.Activity.Venue).NotEmpty().NotNull();
        RuleFor(x => x.Activity.GeoCoordinate).NotNull().SetValidator(new GeoCoordinateValidator());
      }
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
        var activity = mapper.Map<ActivityData, Activity>(request.Activity);

        await context.Activities.AddAsync(activity, cancellationToken);

        var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == userAccessor.GetCurrentUsername());

        var attendee = new ActivityAttendee
        {
          AppUser = user,
          Activity = activity,
          DateJoined = DateTime.Now,
          IsHost = true,
          AppUserId = user.Id,
          ActivityId = activity.Id
        };

        await context.ActivityAttendees.AddAsync(attendee, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<Activity, ActivityDto>(activity);
      }
    }
  }
}