using Microsoft.EntityFrameworkCore;
using ElegantTrinkets2.Data;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorPages();
builder.Services.AddControllers();

// Register ApplicationDbContext and configure MySQL connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8, 0, 21))));

// Register UnitOfWork and repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Add authentication services
builder.Services.AddAuthentication("MyCookieAuth").AddCookie("MyCookieAuth", options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Configure authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// Define route mappings
app.MapGet("/", () => Results.Redirect("/Products"));
app.MapControllers();
app.MapRazorPages();

app.Run();
