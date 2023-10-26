using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Management.Persistence;

public static class PersistanceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection @this, IConfiguration configuration)
    {
        @this.AddDbContext<ManagementDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });
        return @this;
    }
}
