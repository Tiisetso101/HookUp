using System.Text.Json;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(DataContext context)
        {
            if (await context.users.AnyAsync()) return;

            var userData = await File.ReadAllTextAsync("Data/UserSeed.json");

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var users = JsonSerializer.Deserialize<List<User>>(userData);

            foreach (var user in users)
            {

                user.UserName = user.UserName.ToLower();

                context.users.Add(user);

            }

            await context.SaveChangesAsync();
        }
    }
}