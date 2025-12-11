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
              .WithOrigins(
                    "https://blue-glacier-01b5d7e00.3.azurestaticapps.net", // Azure Static Web App
                    "http://localhost:5000",                               // local dev (optional)
                    "https://localhost:5000"                               // local HTTPS (optional)
                )
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
