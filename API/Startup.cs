using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Profiles;
using Application.Values;
using AutoMapper;
using Domain;
using FluentValidation.AspNetCore;
using Infrastructure;
using Infrastructure.Photos;
using Infrastructure.Security;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Persistence;

namespace API
{
  public class Startup
  {
    private readonly IConfiguration _config;
    public Startup(IConfiguration configuration)
    {
      _config = configuration;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddDbContext<DataContext>(c =>
      {
        c.UseSqlite(_config.GetConnectionString("DefaultConnection"));
      });

      var builder = services.AddIdentityCore<AppUser>();
      builder = new IdentityBuilder(builder.UserType, builder.Services);
      builder.AddEntityFrameworkStores<DataContext>();
      builder.AddSignInManager<SignInManager<AppUser>>();

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["TokenKey"]));

      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(opt =>
        {
          opt.TokenValidationParameters = new TokenValidationParameters
          {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = false,
            ValidateAudience = false,
          };
        });

      services.AddAuthorization(opt =>
      {
        opt.AddPolicy("IsActivityHost", policy => 
        {
          policy.Requirements.Add(new IsHostRequirement());
        });
      });
      services.AddTransient<IAuthorizationHandler, IsHostHandler>();

      services.AddScoped<IJwtGenerator, JwtGenerator>();
      services.AddScoped<IUserAccessor, UserAccessor>();
      services.AddScoped<IProfileReader, ProfileReader>();
      services.Configure<CloudinarySettings>(_config.GetSection("Cloudinary"));
      services.AddScoped<ICloudinaryAccessor, CloudinaryAccessor>();

      services.AddMediatR(typeof(List.Handler).Assembly);
      services.AddAutoMapper();
      services.AddMvc(opt =>
      {
        var policy = new AuthorizationPolicyBuilder()
          .RequireAuthenticatedUser()
          .Build();
        opt.Filters.Add(new AuthorizeFilter(policy));
      })
        .AddFluentValidation(c =>
        {
          c.RegisterValidatorsFromAssemblyContaining(typeof(List));
        })
        .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      app.UseMiddleware<ErrorHandlingMiddleware>();

      if (env.IsDevelopment())
      {
        // app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseHsts();
      }
      app.UseAuthentication();
      app.UseHttpsRedirection();
      app.UseMvc();
    }
  }
}
