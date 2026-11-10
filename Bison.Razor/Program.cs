using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using Bison.Core;
using AspNet.Security.OAuth.GitHub;
using Bison.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<BisonDBContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();


builder.Services.AddAuthentication().AddGitHub(options =>
    {
        options.ClientId = builder.Configuration["authentication:github:clientId"]!;
        options.ClientSecret = builder.Configuration["authentication:github:clientSecret"]!;
        options.CallbackPath = "/signin-github";
    });


var dbPath = Environment.GetEnvironmentVariable("BISONDBPATH");

if (string.IsNullOrEmpty(dbPath))
{
    dbPath = Path.Combine(Path.GetTempPath(), "bison.db");
}

builder.Services.AddScoped<IObservationService, ObservationService>();

string connectionString = $"Data Source={dbPath}";
builder.Services.AddDbContext<BisonDBContext>(options => options.UseSqlite(connectionString));
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ITaxonFilterService, TaxonFilterService>();


var app = builder.Build();

using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<BisonDBContext>();

    var userManager = scope.ServiceProvider
        .GetRequiredService<UserManager<IdentityUser>>();

    dbContext.Database.Migrate();

    DbInitializer.SeedDatabase(dbContext);

    var eduard = await userManager.FindByEmailAsync("edka@itu.dk");
    if (eduard == null)
    {
        eduard = new IdentityUser
        {
            UserName = "Eduard",
            Email = "edka@itu.dk",
            EmailConfirmed = true
        };
        var result = await userManager.CreateAsync(eduard, "123456");

        if (!result.Succeeded)
        {
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }

    var eduardAuthorExists = dbContext.Authors
    .Any(a => a.Email == "edka@itu.dk");

    if (!eduardAuthorExists)
    {
        dbContext.Authors.Add(new Author
        {
            Name = "Eduard",
            Email = "edka@itu.dk"
        });

        await dbContext.SaveChangesAsync();
    }

    var debasmita = await userManager.FindByEmailAsync("debloh@itu.dk");
    if (debasmita == null)
    {
        debasmita = new IdentityUser
        {
            UserName = "Debasmita",
            Email = "debloh@itu.dk",
            EmailConfirmed = true
        };
        var result = await userManager.CreateAsync(debasmita, "password");

        if (!result.Succeeded)
        {
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }

    var debasmitaAuthorExists = dbContext.Authors
    .Any(a => a.Email == "debloh@itu.dk");

    if (!debasmitaAuthorExists)
    {
        dbContext.Authors.Add(new Author
        {
            Name = "Debasmita",
            Email = "debloh@itu.dk"
        });

        await dbContext.SaveChangesAsync();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
