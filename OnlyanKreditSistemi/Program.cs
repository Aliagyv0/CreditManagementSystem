using OnlyanKreditSistemi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlyanKreditSistemi.Context;
using OnlyanKreditSistemi.Models;
using OnlyanKreditSistemi.Repository.Interfaces;
using OnlyanKreditSistemi.Services.Interfaces;
using OnlyanKreditSistemi.Hubs;
using OnlyanKreditSistemi.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<INavbarService,NavbarService>();
builder.Services.AddDbContext<KreditDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});
builder.Services.AddScoped(typeof(IRepository<>),typeof(Repository<>));
builder.Services.AddSignalR();

builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
{
    opt.Password.RequireDigit = true;
    opt.Password.RequiredLength = 8;
    opt.Password.RequireLowercase = true;
    opt.Password.RequireUppercase = true;
    opt.Password.RequireNonAlphanumeric = true;
    opt.User.RequireUniqueEmail = true;
    opt.Lockout.MaxFailedAccessAttempts = 5;
    opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
    opt.SignIn.RequireConfirmedEmail = true;


}).AddEntityFrameworkStores<KreditDbContext>().AddDefaultTokenProviders();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseEndpoints(endpoints =>
{

    endpoints.MapAreaControllerRoute(
        name: "admin",
        areaName: "admin",
        pattern: "admin/{controller=Account}/{action=Login}/{id?}"
    );

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}"
    );
});

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<MessageHub>("/messageHub");
    endpoints.MapAreaControllerRoute(
        name: "admin",
        areaName: "admin",
        pattern: "admin/{controller=Account}/{action=Login}/{id?}"
    );

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}"
    );
});

app.MapHub<ChatHub>("/chatHub");
app.Run();
