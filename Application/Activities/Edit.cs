using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
  public class Edit
  {
    public class ActivityData
    {
      public string Title { get; set; }
      public string Description { get; set; }
      public DateTime? Date { get; set; }
      public string City { get; set; }
      public string Venue { get; set; }
      public GeoCoordinate GeoCoordinate { get; set; }
    }

    public class Command : IRequest<ActivityDto>
    {
      public ActivityData Activity { get; set; }
      public int Id { get; set; }
    }

    public class Handler : IRequestHandler<Command, ActivityDto>
    {
      private readonly DataContext context;
      private readonly IMapper mapper;
      public Handler(DataContext context, IMapper mapper)
      {
        this.mapper = mapper;
        this.context = context;
      }

      public async Task<ActivityDto> Handle(Command request, CancellationToken cancellationToken)
      {
        var activity = await context.Activities
            .Include(x => x.GeoCoordinate)
            .FirstOrDefaultAsync(x => x.Id == request.Id);

        if (activity == null)
          throw new RestException(HttpStatusCode.NotFound, new { Activity = "Not found" });

        activity.Title = request.Activity.Title ?? activity.Title;
        activity.Description = request.Activity.Description ?? activity.Description;
        activity.Date = request.Activity.Date ?? activity.Date;
        activity.City = request.Activity.City ?? activity.City;
        activity.Venue = request.Activity.Venue ?? activity.Venue;

        if (request.Activity.GeoCoordinate != null)
        {
          activity.GeoCoordinate.Latitude = request.Activity.GeoCoordinate.Latitude;
          activity.GeoCoordinate.Longitude = request.Activity.GeoCoordinate.Longitude;
        }

        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<Activity, ActivityDto>(activity);
      }
    }
  }
}