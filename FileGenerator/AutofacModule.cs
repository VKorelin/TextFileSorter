using Autofac;
using FileGenerator.Generation;
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
            builder.RegisterType<EncodingFactory>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<RandomStringGenerator>().AsImplementedInterfaces().SingleInstance();
        }
    }
}