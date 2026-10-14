using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var dbPath = Environment.GetEnvironmentVariable("BISONDBPATH");

if (string.IsNullOrEmpty(dbPath))
{
    dbPath = Path.Combine(Path.GetTempPath(), "bison.db");
}

builder.Services.AddScoped<IObservationService, ObservationService>();

string connectionString = $"Data Source={dbPath}";
builder.Services.AddDbContext<BisonDBContext>(options => options.UseSqlite(connectionString));
builder.Services.AddScoped<IPostRepository, PostRepository>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<BisonDBContext>();

    dbContext.Database.Migrate();

    DbInitializer.SeedDatabase(dbContext);
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

app.MapRazorPages();

app.Run();
