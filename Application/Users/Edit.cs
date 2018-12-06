using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Users
{
  public class Edit
  {
    public class UserData
    {
      public string Bio { get; set; }
    }

    public class Command : IRequest<User>
    {
      public UserData User { get; set; }
    }

    public class CommandValidator : AbstractValidator<Command>
    {
      public CommandValidator()
      {
        RuleFor(x => x.User.Bio).NotEmpty().NotNull();
      }
    }

    public class Handler : IRequestHandler<Command, User>
    {
      private readonly DataContext context;
      private readonly IUserAccessor userAccessor;
      public Handler(DataContext context, IUserAccessor userAccessor)
      {
        this.userAccessor = userAccessor;
        this.context = context;
      }

      public async Task<User> Handle(Command request, CancellationToken cancellationToken)
      {
        var user = await context.Users
            .FirstOrDefaultAsync(x => x.UserName == userAccessor
            .GetCurrentUsername(), cancellationToken);

        user.Bio = request.User.Bio ?? user.Bio;

        await context.SaveChangesAsync(cancellationToken);
        
        return new User{
            Username = user.UserName,
            Email = user.Email,
            Bio = user.Bio
        };
      }
    }
  }
}