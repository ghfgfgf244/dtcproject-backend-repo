using dtc.Application.Interfaces;
using dtc.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace dtc.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}
