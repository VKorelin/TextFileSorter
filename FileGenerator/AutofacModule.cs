using System;
using System.Text;
using Autofac;
using FileGenerator.Configuration;
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
            builder.RegisterType<ChunkInfoBuilder>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ChunkGenerator>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<FileWriter>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<FilePathProvider>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<RandomNumberGenerator>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<RandomStringGenerator>().AsImplementedInterfaces().SingleInstance();
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
        }
    }
}