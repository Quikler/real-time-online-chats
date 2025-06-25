using Microsoft.AspNetCore.Identity;
using real_time_online_chats.Server.Domain;

namespace real_time_online_chats.Server.Data.Seed.User;

public static class UserSeed
{
    public static async Task SeedDefaultUsersAsync(this IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<UserEntity>>();

        var testUser = await userManager.FindByEmailAsync("test@test.com");
        if (testUser is null)
        {
            testUser = new UserEntity
            {
                UserName = "test@test.com",
                Email = "test@test.com",
                EmailConfirmed = true,
            };

            var createResult = await userManager.CreateAsync(testUser, testUser.Email);
            if (!createResult.Succeeded)
            {
                throw new Exception(string.Join(", ", createResult.Errors.Select(e => e.Description)));
            }
        }

        var testUser2 = await userManager.FindByEmailAsync("test2@test.com");
        if (testUser2 is null)
        {
            testUser2 = new UserEntity
            {
                UserName = "test2@test.com",
                Email = "test2@test.com",
                EmailConfirmed = true,
            };
            var createResult = await userManager.CreateAsync(testUser2, testUser2.Email);
            if (!createResult.Succeeded)
            {
                throw new Exception(string.Join(", ", createResult.Errors.Select(e => e.Description)));
            }
        }
    }
}
