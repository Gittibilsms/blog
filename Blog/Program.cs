using Blog.Data;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

// ---- CORS ----
const string AllowFrontendOrigin = "AllowFrontendOrigin";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowFrontendOrigin,
        policy =>
        {
            // React / frontend origin
            policy
                .WithOrigins("http://localhost:5000") // or https://localhost:5000 if your frontend uses https
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

// existing services...
builder.Services.AddControllers();


builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSession(); // simple session auth

var app = builder.Build();
// enable CORS before auth / endpoints
app.UseCors(AllowFrontendOrigin);
app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Admin}/{action=Login}/{id?}");

app.Run();
