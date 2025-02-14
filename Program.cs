using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using HousingSManagement.Data;
using HousingSManagement.Models;

var builder = WebApplication.CreateBuilder(args);

// ✅ Configure Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ Configure Identity with Role Support
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireUppercase = false;
})
    .AddRoles<IdentityRole>() // Enables Role-Based Authentication
    .AddEntityFrameworkStores<ApplicationDbContext>();

// ✅ Add Razor Pages & Controllers
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ✅ Apply Migrations Automatically & Seed Roles/Admin User
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<ApplicationDbContext>();

    try
    {
        dbContext.Database.Migrate(); // Apply pending migrations
        await SeedRolesAndAdmin(services); // ✅ Seeding roles & admin user
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error during migration: {ex.Message}");
    }
}

// ✅ Configure Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// ✅ Map Controllers & Identity UI
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();

// ✅ Seed Roles & Admin User Function
async Task SeedRolesAndAdmin(IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    string[] roleNames = { "Admin", "Resident", "Security" };

    // ✅ Ensure Roles Exist
    foreach (var role in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            var result = await roleManager.CreateAsync(new IdentityRole(role));
            if (result.Succeeded)
            {
                Console.WriteLine($"✅ Role '{role}' created successfully!");
            }
            else
            {
                Console.WriteLine($"❌ Failed to create role '{role}'.");
            }
        }
    }

    // ✅ Ensure Admin User Exists
    string adminEmail = "admin@housing.com";
    string adminPassword = "Admin@123"; // ✅ Change this in production

    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        var newAdmin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true, // Admin should have confirmed email
            FullName = "System Admin"
        };

        var result = await userManager.CreateAsync(newAdmin, adminPassword);
        if (result.Succeeded)
        {
            Console.WriteLine("✅ Admin user created successfully!");
            adminUser = newAdmin; // Assign newly created user
        }
        else
        {
            Console.WriteLine("❌ Failed to create admin user.");
            return; // Exit if user creation failed
        }
    }
    else
    {
        Console.WriteLine("⚠️ Admin user already exists.");
    }

    // ✅ Ensure Admin is Assigned Role
    if (!await userManager.IsInRoleAsync(adminUser!, "Admin"))
    {
        await userManager.AddToRoleAsync(adminUser!, "Admin");
        Console.WriteLine("✅ Admin role assigned successfully!");
    }
    else
    {
        Console.WriteLine("⚠️ Admin user is already in role 'Admin'.");
    }
}
