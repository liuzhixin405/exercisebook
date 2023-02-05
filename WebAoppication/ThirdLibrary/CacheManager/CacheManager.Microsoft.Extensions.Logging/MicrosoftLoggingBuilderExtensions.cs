﻿using System;
using System.Linq;
using CacheManager.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using static CacheManager.Core.Utility.Guard;

namespace CacheManager.Core
{
    /// <summary>
    /// Extensions for the configuration builder for logging.
    /// </summary>
    public static class MicrosoftLoggingBuilderExtensions
    {
        /// <summary>
        /// Enables logging for the cache manager instance.
        /// Using this extension will create a NEW instance of <c>Microsoft.Extensions.Logging.ILoggerFactory</c>.
        /// <para>
        /// If you use the standard Micorosft AspNetCore DI, you might want to use the other extensions which make CacheManager use
        /// the already injected/shared instance of <see cref="Microsoft.Extensions.Logging.ILoggerFactory"/>.
        /// </para>
        /// <para>
        /// Use the <paramref name="factory"/> to configure the logger factory and add <see cref="ILoggerProvider"/>s as needed.
        /// </para>
        /// </summary>
        /// <param name="part">The builder part.</param>
        /// <param name="factory">The logger factory used for configuring logging.</param>
        /// <returns>The builder.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "not owning it")]
        public static ConfigurationBuilderCachePart WithMicrosoftLogging(this ConfigurationBuilderCachePart part, Action<ILoggerFactory> factory)
        {
            NotNull(part, nameof(part));
            NotNull(factory, nameof(factory));
            var externalFactory = new LoggerFactory();
            factory(externalFactory);
            return part.WithLogging(typeof(MicrosoftLoggerFactoryAdapter), new Func<ILoggerFactory>(() => externalFactory));
        }

        /// <summary>
        /// Enables logging for the cache manager instance using an existion <c>Microsoft.Extensions.Logging.ILoggerFactory</c> as target.
        /// </summary>
        /// <param name="part">The builder part.</param>
        /// <param name="loggerFactory">The logger factory which should be used.</param>
        /// <returns>The builder.</returns>
        public static ConfigurationBuilderCachePart WithMicrosoftLogging(this ConfigurationBuilderCachePart part, ILoggerFactory loggerFactory)
        {
            NotNull(part, nameof(part));
            NotNull(loggerFactory, nameof(loggerFactory));
            return part.WithLogging(typeof(MicrosoftLoggerFactoryAdapter), new Func<ILoggerFactory>(() => loggerFactory));
        }

        /// <summary>
        /// Configures cache manager logging to use a <c>Microsoft.Extensions.Logging.ILoggerFactory</c>
        /// which gets resolved from the <paramref name="serviceCollection"/>.
        /// </summary>
        /// <param name="part">The builder part.</param>
        /// <param name="serviceCollection">The services collection.</param>
        /// <returns>The builder.</returns>
        public static ConfigurationBuilderCachePart WithMicrosoftLogging(this ConfigurationBuilderCachePart part, IServiceCollection serviceCollection)
        {
            NotNull(part, nameof(part));
            NotNull(serviceCollection, nameof(serviceCollection));

            return part.WithLogging(typeof(MicrosoftLoggerFactoryAdapter), new Func<ILoggerFactory>(() => GetLoggerFactory(serviceCollection)));
        }

        private static ILoggerFactory GetLoggerFactory(IServiceCollection serviceCollection)
        {
            var factory = serviceCollection.BuildServiceProvider().GetRequiredService<ILoggerFactory>();
            EnsureNotNull(factory, "No instance of ILoggerFactory found in {0}.", nameof(serviceCollection));
            return factory;
        }
    }
}
