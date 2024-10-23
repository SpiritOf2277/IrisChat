using Microsoft.EntityFrameworkCore;
using IrisChat.Data;
using IrisChat.Data.Entities;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

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

// Создание ролей и администратора при запуске приложения
using (var scope = app.Services.CreateScope()) {
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

    // Список ролей, которые ты хочешь создать
    string[] roleNames = { "Admin", "Moderator", "User" };

    foreach (var roleName in roleNames) {
        if (!await roleManager.RoleExistsAsync(roleName)) {
            // Создание роли, если она еще не существует
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // Создание пользователя-администратора, если его еще нет
    var adminEmail = "admin@example.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null) {
        var user = new User
        {
            UserName = adminEmail,
            Email = adminEmail,
            DisplayName = "Admin"
        };

        var createUserResult = await userManager.CreateAsync(user, "AdminPassword123!"); // Установи безопасный пароль
        if (createUserResult.Succeeded) {
            // Назначение роли Admin новому пользователю
            await userManager.AddToRoleAsync(user, "Admin");
        }
    }
}

builder.Services.AddControllersWithViews();

var app = builder.Build();

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
