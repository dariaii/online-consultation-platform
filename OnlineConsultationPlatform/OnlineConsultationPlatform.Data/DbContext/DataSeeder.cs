using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using OnlineConsultationPlatform.Data.Entities;

namespace OnlineConsultationPlatform.Data.DbContext
{
    public class DataSeeder
    {
        public static void Initialize(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            if (context.Roles.Any() == false)
            {
                context.Add(new IdentityRole("Teacher") { NormalizedName = "TEACHER" });
                context.Add(new IdentityRole("Mentor") { NormalizedName = "MENTOR" });
                context.Add(new IdentityRole("Admin") { NormalizedName = "ADMIN" });

                context.SaveChanges();
            }

            var adminRole = context.Roles.FirstOrDefault(x => x.Name == "Admin");
            if (adminRole != null && context.UserRoles.Any(x => x.RoleId == adminRole.Id) == false)
            {
                var admin = new ApplicationUser
                {
                    Email = config["AdminCredentials:Email"],
                    UserName = config["AdminCredentials:UserName"],
                    EmailConfirmed = true,
                    IsActive = true
                };

                IdentityResult result = userManager.CreateAsync(admin, config["AdminCredentials:Password"]).GetAwaiter().GetResult();

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(admin, "Admin").Wait();
                }
            }
        }
    }
}
