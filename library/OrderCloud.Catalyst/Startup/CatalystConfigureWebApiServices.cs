using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Converters;
using System;

namespace OrderCloud.Catalyst
{
	public static class CatalystConfigureWebApiServices
	{
		public static IServiceCollection ConfigureServices(this IServiceCollection services)
		{
			services.AddControllers()
			.ConfigureApiBehaviorOptions(o =>
			{
				o.SuppressModelStateInvalidFilter = true;
			}).AddNewtonsoftJson(options =>
			{
				options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
			});
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
			services.AddMvc().AddNewtonsoftJson(options => options.SerializerSettings.Converters.Add(new StringEnumConverter()));
			services.AddMvc(o =>
			{
				o.Filters.Add(new ValidateModelAttribute());
				o.EnableEndpointRouting = false;
			});
			services.AddCors(o => o.AddPolicy("integrationcors",
				builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); }));

			return services;
		}

		// TODO - figure out how to remove the dependency on TSettings
		public static IServiceCollection AddOrderCloudUserAuth<TSettings>(this IServiceCollection services)
		{
			services.AddAuthentication()
				.AddScheme<OrderCloudUserAuthOptions, OrderCloudUserAuthHandler<TSettings>>("OrderCloudUser", null);
			return services;
		}

		public static IServiceCollection AddOrderCloudWebhookAuth(this IServiceCollection services, Action<OrderCloudWebhookAuthOptions> configureOptions)
		{
			services.AddAuthentication()
				.AddScheme<OrderCloudWebhookAuthOptions, OrderCloudWebhookAuthHandler>("OrderCloudWebhook", null, configureOptions);
			return services;
		}

	}
}
