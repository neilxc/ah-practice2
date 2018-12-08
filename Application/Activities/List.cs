using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    public class List
    {
        public class Query : IRequest<ActivitiesEnvelope>
        {
            public Query(string sort, string username, int? limit, int? offset)
            {
                Sort = sort;
                Username = username;
                Limit = limit;
                Offset = offset;
            }

            public string Username { get; set; }
            public string Sort { get; set; }
            public int? Limit { get; }
            public int? Offset { get; }
        }

        public class Handler : IRequestHandler<Query, ActivitiesEnvelope>
        {
            private readonly DataContext context;
            private readonly IMapper mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                this.mapper = mapper;
                this.context = context;
            }
            public async Task<ActivitiesEnvelope> Handle(Query request, CancellationToken cancellationToken)
            {
                var queryable = context.Activities.GetAllData();

                if (!string.IsNullOrEmpty(request.Username))
                {
                    queryable = queryable
                        .Where(a => a.Attendees.Any(x => x.AppUser.UserName == request.Username));
                }

                switch (request.Sort)
                {
                    case "past":
                        queryable = queryable
                            .Where(x => x.Date < DateTime.Now)
                            .OrderByDescending(x => x.Date);
                        break;
                    default:
                        queryable = queryable
                            .Where(x => x.Date > DateTime.Now)
                            .OrderBy(x => x.Date);
                        break;
                }

                var activities = await queryable
                    .Skip(request.Offset ?? 0)
                    .Take(request.Limit ?? 3)
                    .ToListAsync(cancellationToken);

                return new ActivitiesEnvelope
                {
                    Activities = mapper.Map<List<Activity>, List<ActivityDto>>(activities),
                    ActivityCount = queryable.Count()
                };
            }
        }
    }
}