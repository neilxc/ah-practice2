using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Application.Interfaces;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Persistence;

namespace Application.Users
{
  public class Login
  {
    public class UserData
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class Command : IRequest<User>
    {
      public UserData User { get; set; }
    }

    public class CommandValidator : AbstractValidator<Command>
    {
      public CommandValidator()
      {
          RuleFor(x => x.User.Email).EmailAddress().NotEmpty().NotNull();
          RuleFor(x => x.User.Password).NotEmpty().NotNull();
      }
    }

    public class Handler : IRequestHandler<Command, User>
    {
      private readonly DataContext context;
      private readonly UserManager<AppUser> userManager;
      private readonly SignInManager<AppUser> signInManager;
      private readonly IJwtGenerator jwtGenerator;

      public Handler(DataContext context, 
        UserManager<AppUser> userManager, 
        SignInManager<AppUser> signInManager, 
        IJwtGenerator jwtGenerator)
      {
        this.context = context;
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.jwtGenerator = jwtGenerator;
      }

      public async Task<User> Handle(Command request, CancellationToken cancellationToken)
      {
        var user = await userManager.FindByEmailAsync(request.User.Email);

        if (user == null)
            throw new RestException(HttpStatusCode.Unauthorized, new {Email = "Not found"});
        
        var result = await signInManager.CheckPasswordSignInAsync(user, request.User.Password, false);

        if (result.Succeeded)
        {
            return new User
            {
                Bio = user.Bio,
                Email = user.Email,
                Image = null,
                Token = jwtGenerator.CreateToken(user),
                Username = user.UserName
            };
        }

        throw new RestException(HttpStatusCode.Unauthorized);
      }
    }
  }
}