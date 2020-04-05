using Autofac;
using FileGenerator.Generation;
using FileGenerator.IO;
using FileGenerator.Validation;

namespace FileGenerator
{
    internal sealed class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Bootstrapper>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ArgumentsValidator>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<Generator>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<EncodingInfoProviderFactory>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ChunkGenerator>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<FileWriter>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<EntryGenerator>().AsImplementedInterfaces().SingleInstance();
        }
    }
}