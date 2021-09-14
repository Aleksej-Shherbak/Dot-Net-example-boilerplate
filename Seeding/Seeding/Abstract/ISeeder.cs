using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Seeding.Seeding.Abstract
{
    public interface ISeeder
    {
        string SeederName { get; }
        Task<bool> SeedAsync(IServiceProvider serviceProvider, IConfiguration configuration);
    }
}