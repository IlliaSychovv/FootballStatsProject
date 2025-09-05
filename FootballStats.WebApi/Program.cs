using FluentValidation;
using FluentValidation.AspNetCore;
using FootballStats.Application.Interfaces;
using FootballStats.Application.Interfaces.Repositories;
using FootballStats.Application.Interfaces.Services;
using FootballStats.Application.Services;
using FootballStats.Application.Validators;
using FootballStats.Infrastructure.Data;
using FootballStats.Infrastructure.Repositories;
using FootballStats.Infrastructure.Services;
using FootballStats.WebApi.Middleware;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddValidatorsFromAssemblyContaining<CreateMatchValidator>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddHttpContextAccessor(); 
builder.Services.AddSingleton<IExceptionHandler, GlobalExceptionHandler>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IMatchRepository, MatchRepository>();
builder.Services.AddScoped<IMatchService, MatchService>();
builder.Services.AddScoped<IImportService, ImportService>();
builder.Services.AddScoped<IMatchCsvReader, CsvDataReader>();
builder.Services.AddScoped<IExportService, ExportService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();
builder.Services.AddScoped<IUserAuthenticationService, AuthenticationService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(appBuilder =>
{
    appBuilder.Run(async context =>
    {
        var exceptionHandler = context.RequestServices.GetRequiredService<IExceptionHandler>();
        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (exceptionHandlerFeature != null)
        {
            var ex = exceptionHandlerFeature.Error;
            await exceptionHandler.TryHandleAsync(context, ex, CancellationToken.None);
        }
    });
});

app.UseHttpsRedirection();

app.MapControllers();

app.Run(); 