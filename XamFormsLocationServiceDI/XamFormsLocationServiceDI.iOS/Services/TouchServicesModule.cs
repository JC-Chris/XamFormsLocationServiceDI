using System;
using Ninject.Modules;
using XamFormsLocationServiceDI.Services;

namespace XamFormsLocationServiceDI.iOS.Services
{
    public class TouchServicesModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ILocationService>().To<TouchLocationService>().InSingletonScope();
        }
    }
}

