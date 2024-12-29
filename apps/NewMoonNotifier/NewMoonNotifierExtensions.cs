using Microsoft.Extensions.DependencyInjection;
namespace HomeAssistantApps
{
    public static class NewMoonNotifierExtensions
    {
        ///<summary>Registers all injectable generated types in the serviceCollection</summary>
        public static IServiceCollection AddNewMoonNotifier(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<NewMoonNotifier>();
            return serviceCollection;
        }
    }
}