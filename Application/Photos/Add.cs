using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos
{
    public class Add
    {
        public class PhotoData
        {
            public IFormFile File { get; set; }
        }

        public class Command : IRequest<PhotoDto>
        {
            public PhotoData Photo { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Photo.File).NotNull();
            }
        }

        public class Handler : IRequestHandler<Command, PhotoDto>
        {
            private readonly DataContext context;
            private readonly IUserAccessor userAccessor;
            private readonly IMapper mapper;
            private readonly ICloudinaryAccessor cloudinaryAccessor;
            public Handler(DataContext context, 
                IUserAccessor userAccessor, 
                IMapper mapper, 
                ICloudinaryAccessor cloudinaryAccessor)
            {
                this.cloudinaryAccessor = cloudinaryAccessor;
                this.mapper = mapper;
                this.userAccessor = userAccessor;
                this.context = context;
            }

            public async Task<PhotoDto> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await context.Users.Include(p => p.UserPhotos)
                    .FirstOrDefaultAsync(x => x.UserName == userAccessor.GetCurrentUsername());
                
                var photo = cloudinaryAccessor.AddPhotoForUser(request.Photo.File);

                if (!user.UserPhotos.Any(x => x.IsMain))
                    photo.IsMain = true;
                
                user.UserPhotos.Add(photo);

                await context.SaveChangesAsync(cancellationToken);

                var photoDto = mapper.Map<UserPhoto, PhotoDto>(photo);

                return photoDto;
            }
        }
    }
}