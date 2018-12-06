using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
  public class Details
  {
    public class Query : IRequest<ActivityDto>
    {
      public int Id { get; set; }
    }

    public class Handler : IRequestHandler<Query, ActivityDto>
    {
      private readonly DataContext context;
      private readonly IMapper mapper;
      public Handler(DataContext context, IMapper mapper)
      {
        this.mapper = mapper;
        this.context = context;
      }
      public async Task<ActivityDto> Handle(Query request, CancellationToken cancellationToken)
      {
        var activity = await context.Activities
          .GetAllData()
          .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        return mapper.Map<Activity, ActivityDto>(activity);
      }
    }
  }
}