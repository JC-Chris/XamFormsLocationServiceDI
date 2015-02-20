using System;
using Ninject.Modules;

namespace XamFormsLocationServiceDI
{
    public class SharedServicesModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ISettingsService>().To<DebugSettingsService>().InSingletonScope();
        }
    }
}

