using System;
using System.Text;
using Autofac;
using TextFileSorter.Configuration;
using TextFileSorter.Sorting;
using TextFileSorter.Validation;

namespace TextFileSorter
{
    internal sealed class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Configuration
            builder.RegisterType<ConfigurationProvider>().AsImplementedInterfaces().SingleInstance();
            builder.Register<IEncodingInfoProvider>(x =>
                {
                    var config = x.Resolve<IConfigurationProvider>();
                    if (config.Encoding.Equals(Encoding.Unicode))
                        return new UnicodeInfoProvider();
                    if (config.Encoding.Equals(Encoding.UTF8))
                        return new Utf8InfoProvider();
                    throw new Exception($"Unsupported encoding type: {config.Encoding}");
                })
                .AsImplementedInterfaces()
                .SingleInstance();
            
            // Sorting
            builder.RegisterType<ChunkFileReader>().AsImplementedInterfaces().SingleInstance().InstancePerDependency();
            builder.RegisterType<ChunkMergeService>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ExternalMergeSorter>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<SortedChunksService>().AsImplementedInterfaces().SingleInstance();
            
            // Validation
            builder.RegisterType<ArgumentsValidator>().AsImplementedInterfaces().SingleInstance();
        }
    }
}