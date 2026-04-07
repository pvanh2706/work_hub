using WorkHub.API.Middlewares;
using WorkHub.Modules.Jira.Infrastructure;
using WorkHub.Modules.Knowledge.Infrastructure;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ── Logging ───────────────────────────────────────────────────────────────────
builder.Host.UseSerilog((ctx, cfg) =>
    cfg.ReadFrom.Configuration(ctx.Configuration));

// ── Controllers ───────────────────────────────────────────────────────────────
builder.Services.AddControllers();

// ── Authentication (JWT) ──────────────────────────────────────────────────────
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });
builder.Services.AddAuthorization();

// ── MediatR ───────────────────────────────────────────────────────────────────
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(
        Assembly.Load("WorkHub.Modules.Knowledge.Application"),
        Assembly.Load("WorkHub.Modules.Tasks.Application"),
        Assembly.Load("WorkHub.Modules.Jira.Application"),
        Assembly.Load("WorkHub.Modules.Organization.Application"),
        Assembly.Load("WorkHub.Modules.Workspace.Application"),
        Assembly.Load("WorkHub.Modules.AI.Application")
    );
});

// ── FluentValidation ──────────────────────────────────────────────────────────
builder.Services.AddValidatorsFromAssemblies([
    Assembly.Load("WorkHub.Modules.Knowledge.Application"),
    Assembly.Load("WorkHub.Modules.Tasks.Application"),
    Assembly.Load("WorkHub.Modules.Jira.Application"),
    Assembly.Load("WorkHub.Modules.Organization.Application"),
    Assembly.Load("WorkHub.Modules.Workspace.Application"),
    Assembly.Load("WorkHub.Modules.AI.Application")
]);

// ── Modules ───────────────────────────────────────────────────────────────────
builder.Services.AddKnowledgeModule(builder.Configuration);
builder.Services.AddJiraModule(builder.Configuration);
// TODO: Uncomment khi implement từng module
// builder.Services.AddOrganizationModule(builder.Configuration);
// builder.Services.AddTasksModule(builder.Configuration);
// builder.Services.AddWorkspaceModule(builder.Configuration);
// builder.Services.AddAIModule(builder.Configuration);

// ── Swagger ───────────────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

// ── Middleware Pipeline ───────────────────────────────────────────────────────
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

// For integration tests
public partial class Program { }
