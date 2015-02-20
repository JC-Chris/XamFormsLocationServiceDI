using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using XamFormsLocationServiceDI.Constants;
using XamFormsLocationServiceDI.Models;
using XamFormsLocationServiceDI.Services;
using Ninject;
using System.Threading.Tasks;
using Ninject.Modules;
using XamFormsLocationServiceDI.Views;

namespace XamFormsLocationServiceDI
{
    public class App : Application
    {
        public StandardKernel Kernel
        { get; private set; }

        private LocationPage _locPage;

        public App(List<INinjectModule> modules)
        {
            // local modules already passed in, add shared
            modules.Add(new SharedServicesModule());
            InitIoC(modules);

            // The root page of your application
            _locPage = Kernel.Get<LocationPage>();
            MainPage = _locPage;
        }

        private void InitIoC(List<INinjectModule> modules)
        {
            Kernel = new StandardKernel(modules.ToArray());
        }

        protected override void OnStart()
        {
            _locPage.StartListeningToLocation();
        }

        protected override void OnSleep()
        {
            _locPage.StopListeningToLocation();
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
