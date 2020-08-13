using BaGet.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BaGet.Scaleway
{
    public static class ScalewayApplicationExtensions
    {
        public static BaGetApplication AddScalewayObjectStorage(this BaGetApplication app)
        {
            app.Services.AddBaGetOptions<ScalewayObjectStorageOptions>(nameof(BaGetOptions.Storage));
            app.Services.AddTransient<ScalewayStorage>();
            app.Services.TryAddTransient<IStorageService>(provider => provider.GetRequiredService<ScalewayStorage>());

            app.Services.AddProvider<IStorageService>((provider, config) =>
            {
                if (!config.HasStorageType("ScalewayObjectStorage")) return null;

                return provider.GetRequiredService<ScalewayStorage>();
            });

            return app;
        }

    }
}
