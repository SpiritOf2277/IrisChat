using Microsoft.EntityFrameworkCore;
using IrisChat.Data;
using IrisChat.Data.Entities;
using Microsoft.AspNetCore.Identity;
using IrisChat.Data.Repositorys;
using IrisChat.Data.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Регистрация репозиториев
builder.Services.AddScoped<IRepository<ForumThread>, ForumThreadRepository>();
builder.Services.AddScoped<IRepository<Category>, CategoryRepository>();
builder.Services.AddScoped<IRepository<Post>, PostRepository>();
builder.Services.AddScoped<UserRepository>();

var connectionString = builder.Configuration.GetConnectionString("IrisChatConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Конфигурация для Identity
builder.Services.Configure<IdentityOptions>(options =>
{
    // Настройка пароля
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;

    // Настройка блокировки пользователя
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // Настройка пользователя
    options.User.RequireUniqueEmail = true;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    // Настройка куков
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Создание ролей и администратора
using (var scope = app.Services.CreateScope()) {
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

    string[] roleNames = { "Admin", "Moderator", "User" };

    foreach (var roleName in roleNames) {
        if (!await roleManager.RoleExistsAsync(roleName)) {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // Создание администратора
    var adminEmail = "admin@example.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null) {
        var user = new User
        {
            UserName = adminEmail,
            Email = adminEmail,
            DisplayName = "Admin"
        };

        var createUserResult = await userManager.CreateAsync(user, "AdminPassword123!");
        if (createUserResult.Succeeded) {
            await userManager.AddToRoleAsync(user, "Admin");
        }
    }
}

if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
