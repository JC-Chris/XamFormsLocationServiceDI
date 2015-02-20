using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XamFormsLocationServiceDI.Models;

namespace XamFormsLocationServiceDI.Services
{
    public interface ILocationService
    {
        double Accuracy { get; set; }
        double DistanceTrigger { get; set; }
        void StartMonitoringLocationChanges();
        void StopMonitoringLocationChanges();
    }
}
