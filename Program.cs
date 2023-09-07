using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("InMemoryDb"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();
// register controllers
builder.Services.AddControllers();
builder.Services.AddSingleton<IAuthorizationHandler, AuthenticatedOrSecretHeaderHandler>();
// register policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ExtensionPolicy", policy =>
    {
        // policy.RequireAuthenticatedUser();
        // policy.RequireClaim("ChromeExtension");
        // header must contain secret 
        policy.Requirements.Add(new AuthenticatedOrSecretHeaderRequirement());
        
    });
});

var app = builder.Build();
using var scoped = app.Services.CreateScope();
// using soc

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    // app.UseHsts();
}

// app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
// register controllers
app.MapControllers();

app.Run();
public class AuthenticatedOrSecretHeaderRequirement : IAuthorizationRequirement { }

public class AuthenticatedOrSecretHeaderHandler : AuthorizationHandler<AuthenticatedOrSecretHeaderRequirement>
{


    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthenticatedOrSecretHeaderRequirement requirement)
    {
        var httpContext = context.Resource as HttpContext;

        // check if 
        if (httpContext!.Request.Headers.ContainsKey("User-Agent") && 
            httpContext.Request.Headers["User-Agent"].ToString().Contains("Mozilla"))
        {
            if (context.User.Identity?.IsAuthenticated ?? false)
            {
                 context.Succeed(requirement);
            }
            return Task.CompletedTask; 
        }
        // Return 401 Unauthorized status code for non-browser requests
        var header = httpContext.Request;
        var secret = header?.Headers["Secret"];
        var exist = secret is not null && secret.ToString() == "VerySecretValue";
        if (!exist)
        {
            httpContext.Response.StatusCode = 401;
            // short circuit request pipeline
            httpContext.Response.CompleteAsync();
        }
        else
        {
            context.Succeed(requirement);
        }
        
        return Task.CompletedTask;
    }
}