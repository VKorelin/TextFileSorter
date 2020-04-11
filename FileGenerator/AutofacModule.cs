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
            // Configuration
            builder.RegisterType<ConfigurationProvider>().AsImplementedInterfaces().SingleInstance();
            
            // Generation
            builder.RegisterType<ChunkInfoBuilder>().AsImplementedInterfaces().InstancePerDependency().ExternallyOwned();
            builder.RegisterType<RandomNumberGenerator>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<RandomStringGenerator>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<Generator>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ChunkGenerator>().AsImplementedInterfaces().SingleInstance();
            
            // IO
            builder.RegisterType<FileWriter>().AsImplementedInterfaces().InstancePerDependency().ExternallyOwned();
            builder.RegisterType<FileNameProvider>().AsImplementedInterfaces().SingleInstance();
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
            
            // Validation
            builder.RegisterType<ArgumentsValidator>().AsImplementedInterfaces().SingleInstance();
            
            builder.RegisterType<Bootstrapper>().AsImplementedInterfaces().SingleInstance();
        }
    }
}