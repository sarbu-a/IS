using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using PCShop.Models; 
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PCShop.Data
{
    public static class RoleSeeder
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            
            string[] roles = { "Manager", "Senior", "Junior", "Client" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            
            await CreateTestUserAsync(userManager, "manager@pcshop.ro", "Parola!123", "Manager");
            await CreateTestUserAsync(userManager, "senior@pcshop.ro", "Parola!123", "Senior");
            await CreateTestUserAsync(userManager, "junior@pcshop.ro", "Parola!123", "Junior");
            await CreateTestUserAsync(userManager, "client@pcshop.ro", "Parola!123", "Client");

            
            if (!context.Categories.Any())
            {
                
                context.Categories.Add(new Category { Name = "Componente PC" });
                await context.SaveChangesAsync();
            }

           
            var categoryId = context.Categories.First().Id;

            
            if (!context.Products.Any())
            {
                context.Products.AddRange(
                    new Product 
                    { 
                        Name = "Placă Video RTX 4060", 
                        ComponentType = "Placă Video",
                        Description = "O placă video excelentă pentru gaming la 1080p.",
                        Price = 1600m,
                        Rating = 4.8,
                        IsAssembledSystem = false,
                        CategoryId = categoryId,
                        HasPromotion = false
                    },
                    new Product 
                    { 
                        Name = "Sistem Desktop Gaming ASUS", 
                        ComponentType = "Sistem Complet",
                        Description = "Sistem pre-asamblat cu i5 și RTX 4060.",
                        Price = 4500m,
                        Rating = 4.5,
                        IsAssembledSystem = true,
                        CategoryId = categoryId,
                        HasPromotion = true
                    }
                );
                await context.SaveChangesAsync();
            }
        }

        private static async Task CreateTestUserAsync(UserManager<IdentityUser> userManager, string email, string password, string role)
        {
            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new IdentityUser { UserName = email, Email = email };
                var result = await userManager.CreateAsync(user, password);
                
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }
    }
}