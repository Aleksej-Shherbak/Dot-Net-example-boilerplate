using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Seeding.Seeding.Abstract
{
    public class SeederBase : ISeeder
    {
        public virtual string SeederName => GetType().Name;

        public virtual Task<bool> SeedAsync(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            return Task.FromResult(true);
        }
    }
}