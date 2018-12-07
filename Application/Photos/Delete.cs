using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos
{
    public class Delete
    {
        public class Command : IRequest
        {
            public Command(int id)
            {
                Id = id;
            }

            public int Id { get; set; }
        }

        public class Handler : AsyncRequestHandler<Command>
        {
            private readonly DataContext context;
            private readonly IUserAccessor userAccessor;
            private readonly ICloudinaryAccessor cloudinaryAccessor;
            public Handler(DataContext context, IUserAccessor userAccessor, ICloudinaryAccessor cloudinaryAccessor)
            {
                this.cloudinaryAccessor = cloudinaryAccessor;
                this.userAccessor = userAccessor;
                this.context = context;
            }

            protected override async Task Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await context.Users
                    .Include(p => p.UserPhotos)
                    .FirstOrDefaultAsync(u => u.UserName == userAccessor.GetCurrentUsername());

                if (user.UserPhotos.All(p => p.Id != request.Id))
                    throw new RestException(HttpStatusCode.Unauthorized);

                var photoFromDb = await context.UserPhotos
                    .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

                if (photoFromDb.IsMain)
                    throw new RestException(HttpStatusCode.BadRequest, new { Photo = "Cannot delete main photo" });

                cloudinaryAccessor.DeletePhotoForUser(photoFromDb);
            }
        }
    }
}