using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using XamFormsLocationServiceDI.Models;
using XamFormsLocationServiceDI.Services;

namespace XamFormsLocationServiceDI.Droid.Services
{
    public class DroidLocationService : ILocationService
    {
        public Task<GeoPosition> GetCurrentLocationAsync()
        {
            throw new NotImplementedException();
        }
    }
}