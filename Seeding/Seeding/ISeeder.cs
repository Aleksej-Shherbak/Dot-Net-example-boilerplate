using System.Threading.Tasks;
using Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Seeding.Seeding
{
    public interface ISeeder
    {
        Task<bool> SeedAsync(ServiceProvider serviceProvider, IConfiguration configuration);
    }
}