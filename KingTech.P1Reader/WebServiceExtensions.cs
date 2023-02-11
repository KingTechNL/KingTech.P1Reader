namespace KingTech.P1Reader;

/// <summary>
/// Extension methods for WebApplicationBuilder registration and configuration.
/// </summary>
public static class WebServiceExtensions
{
    /// <summary>
    /// Load the given configuration/settings type using the ConfigurationManager and register it to the DI container.
    /// </summary>
    /// <typeparam name="TConfiguration">Type of configuration to load.</typeparam>
    /// <param name="builder">WebApplicationBuilder containing DI container and ConfigurationManager</param>
    /// <param name="optional">If set to true, this settings object is optional and doesnt throw an error when not existing.</param>
    public static TConfiguration Configure<TConfiguration>(this WebApplicationBuilder builder, bool optional = false) where TConfiguration : class
    {
        var config = builder.Configuration.GetSection(typeof(TConfiguration).Name);
        if (!optional && config?.Get<TConfiguration>() == null)
            throw new Exception($"No {typeof(TConfiguration).Name} found in settings file!");
        builder.Services.Configure<TConfiguration>(config);

        if (config?.Get<TConfiguration>() != null)
        {
            var configuration = config.Get<TConfiguration>();
            builder.Services.AddSingleton(configuration);
            return configuration;
        }

        return default;
    }

    /// <summary>
    /// Load the given configuration/settings type using the ConfigurationManager and register it to the DI container.
    /// </summary>
    /// <typeparam name="TConfiguration">Type of configuration to load.</typeparam>
    /// <param name="builder">WebApplicationBuilder containing DI container and ConfigurationManager</param>
    /// <param name="defaultValue">Default value to use if no configuration could be loaded.</param>
    public static TConfiguration Configure<TConfiguration>(this WebApplicationBuilder builder, TConfiguration defaultValue) where TConfiguration : class
    {
        var config = builder.Configuration.GetSection(typeof(TConfiguration).Name);
        if (config?.Get<TConfiguration>() != null)
        {
            var configuration = config.Get<TConfiguration>();
            builder.Services.Configure<TConfiguration>(config);
            builder.Services.AddSingleton(configuration);
            return configuration;
        }
        else
        {
            builder.Services.AddSingleton(defaultValue);
            return defaultValue;
        }
    }
}