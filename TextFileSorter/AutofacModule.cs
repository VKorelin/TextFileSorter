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
            
            // Sorting
            builder.RegisterType<ChunkFileReader>().AsImplementedInterfaces().SingleInstance().InstancePerDependency();
            builder.RegisterType<ChunkMergeService>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ExternalMergeSorter>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<SortedChunksService>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<FilesMerger>().AsImplementedInterfaces().SingleInstance();
            
            // Validation
            builder.RegisterType<ArgumentsValidator>().AsImplementedInterfaces().SingleInstance();
        }
    }
}