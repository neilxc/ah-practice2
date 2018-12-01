using System;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;
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

    public class Command : IRequest<Activity>
    {
      public ActivityData Activity { get; set; }
    }

    public class Handler : IRequestHandler<Command, Activity>
    {
      private readonly DataContext context;
      public Handler(DataContext context)
      {
        this.context = context;
      }

      public async Task<Activity> Handle(Command request, CancellationToken cancellationToken)
      {
        var activity = new Activity
        {
            Title = request.Activity.Title,
            Description = request.Activity.Description,
            Date = request.Activity.Date,
            City = request.Activity.City,
            Venue = request.Activity.Venue,
            GeoCoordinate = new GeoCoordinate{
                Latitude = request.Activity.GeoCoordinate.Latitude,
                Longitude = request.Activity.GeoCoordinate.Longitude
            }
        };

        await context.Activities.AddAsync(activity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return activity;
      }
    }
  }
}