using Data;
using Microsoft.Extensions.DependencyInjection;

namespace Seeding.Seeding
{
    public interface ISeeder
    {
        bool Seed(ServiceCollection services);
    }
}