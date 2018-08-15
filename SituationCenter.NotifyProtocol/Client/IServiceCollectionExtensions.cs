using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace SituationCenter.NotifyProtocol.Client
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddHubNotificator(
            this IServiceCollection serviceCollection,
            NotifyHubSettings configs)
        {
            serviceCollection.AddHttpClient(HttpNotificator.HttpClientName, client =>
            {
                client.BaseAddress = new Uri(configs.Url);
                client.DefaultRequestHeaders.Add("Authorization", configs.AccessToken);
            });
            serviceCollection.AddTransient<INotificator, HttpNotificator>();
            return serviceCollection;
        }

        public static IServiceCollection AddHubNotificator(
            this IServiceCollection serviceCollection,
            IConfigurationSection configurationSection)
            => AddHubNotificator(serviceCollection, configurationSection.Get<NotifyHubSettings>());

        public static IServiceCollection AddHubNotificator(
            this IServiceCollection serviceCollection)
        {
            serviceCollection.AddHttpClient(HttpNotificator.HttpClientName, (serviceProvider, client) =>
            {
                var configs = serviceProvider.GetService<IOptions<NotifyHubSettings>>().Value;
                client.BaseAddress = new Uri(configs.Url);
                client.DefaultRequestHeaders.Add("Authorization", configs.AccessToken);
            });
            serviceCollection.AddTransient<INotificator, HttpNotificator>();
            return serviceCollection;
        }
    }
}
