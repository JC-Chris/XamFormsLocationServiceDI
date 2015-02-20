using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Foundation;
using UIKit;
using XamFormsLocationServiceDI.Models;
using XamFormsLocationServiceDI.Services;
using CoreLocation;
using UIKit;
using Xamarin.Forms;
using XamFormsLocationServiceDI.Constants;

namespace XamFormsLocationServiceDI.iOS.Services
{
    public class TouchLocationService : ILocationService
    {
        private int[] _delays;
        private int _curDelay;
        private CLLocationManager _locationManager;
        private int _locationTests;
        private bool _listenForLocationChanges;
        private CLLocation _lastReported;

        public TouchLocationService(ISettingsService settings)
        {
            // create the location manager
            _locationManager = new CLLocationManager();

            //iOS 8 requires you to manually request authorization now - Note the Info.plist file has a new key called requestWhenInUseAuthorization added to.
            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                Device.BeginInvokeOnMainThread(() => 
                    _locationManager.RequestWhenInUseAuthorization());

            }

            // setup the location updated callback
            _locationManager.LocationsUpdated += (sender, args) =>
            {
                // now that we have a result, turn off updating to conserve battery
                _locationManager.StopUpdatingLocation();

                // increment the location tests count
                _locationTests++;

                Console.WriteLine(string.Format("Location test count now at {0}", _locationTests));

                // get the position
                var lastLocation = args.Locations[args.Locations.Length - 1];
                var position = new GeoPosition
                {
                    Latitude = lastLocation.Coordinate.Latitude,
                    Longitude = lastLocation.Coordinate.Longitude,
                    Altitude = lastLocation.Altitude
                };

                // check if the distance trigger has been met
                double delta = DistanceTrigger + 100;
                if (_lastReported != null)
                    delta = lastLocation.DistanceFrom(_lastReported);
                if (delta > DistanceTrigger)
                {
                    Console.WriteLine(string.Format("Location Update reported at {0}", DateTime.Now.ToString("h:mm s")));

                    // update the last reported and reduce the delay
                    _lastReported = lastLocation;
                    ReduceDelay();

                    // send the message
                    MessagingCenter.Send<ILocationService, GeoPosition>(this, MessagingConstants.LocationUpdateMessage,
                        position);
                }
                else
                {
                    Console.WriteLine("Distance change of {0:f0} is less than trigger of {1:f0}", delta, DistanceTrigger);
                    // if 2 consecutive polls without update, increase delay
                    if (_locationTests > 1)
                        IncreaseDelay();
                }
            };

            // initialize the location settings
            SetupDelays(settings);
            SetupAccuracy(settings);
            SetupDistanceTrigger(settings);
        }

        private void SetupDelays(ISettingsService settings)
        {
            var delayStrings = settings.GetSetting(SettingsConstants.LocationDelaysKey).Split(new char[] {
                ','
            });
            var delayInts = new List<int>();
            foreach (var d in delayStrings)
            {
                int delay;
                if (int.TryParse(d, out delay))
                    delayInts.Add(delay);
            }
            if (delayInts.Count > 0)
                _delays = delayInts.ToArray();
            else
                // worst case just do 1 minute
                _delays = new int[] { 60 };
        }

        private void SetupAccuracy(ISettingsService settings)
        {
            var acc = settings.GetSetting(SettingsConstants.LocationAccuracyKey);
            double setAcc;
            if (!string.IsNullOrWhiteSpace(acc))
            {
                if (double.TryParse(acc, out setAcc))
                    Accuracy = setAcc;
                else
                    // worst case just do 1 kilometer
                    Accuracy = 1000;
            }
        }

        private void SetupDistanceTrigger(ISettingsService settings)
        {
            var dis = settings.GetSetting(SettingsConstants.LocationDistanceTriggerKey);
            double setDis;
            if (!string.IsNullOrWhiteSpace(dis))
            {
                if (double.TryParse(dis, out setDis))
                    DistanceTrigger = setDis;
                else
                    // worst case just do 1 kilometer
                    DistanceTrigger = 1000;
            }
        }

        public double Accuracy
        {
            get { return _locationManager.DesiredAccuracy; }
            set { _locationManager.DesiredAccuracy = value; }
        }

        public double DistanceTrigger
        {
            get { return _locationManager.DistanceFilter; }
            set { _locationManager.DistanceFilter = value; }
        }

        public void StartMonitoringLocationChanges()
        {
            if (!CLLocationManager.LocationServicesEnabled)
                throw new NotSupportedException("Location Services is not enabled");

            _listenForLocationChanges = true;
            _locationTests = 0;
            _curDelay = _delays[0];

            Task.Run(async () =>
            {
                while (_listenForLocationChanges)
                {
                    // turn on updating -- the update trigger will turn it off
                    _locationManager.StartUpdatingLocation();

                    // wait for the specified delay
                    await Task.Delay(_curDelay * 1000);
                }
            });
        }

        public void StopMonitoringLocationChanges()
        {
            _listenForLocationChanges = false;
        }

        private void ReduceDelay()
        {
            _locationTests = 0; // reset the counter
            int curPosition;
            for (curPosition = 0; curPosition < _delays.Length; curPosition++)
            {
                if (_curDelay == _delays[curPosition])
                    break;
            }

            if (curPosition > 0)
            {
                _curDelay = _delays[curPosition - 1];
                Console.WriteLine(string.Format("Reduced delay to {0} seconds", _curDelay));
            }
        }

        private void IncreaseDelay()
        {
            _locationTests = 0; // reset the counter
            int curPosition;
            for (curPosition = 0; curPosition < _delays.Length; curPosition++)
            {
                if (_curDelay == _delays[curPosition])
                    break;
            }

            if (curPosition < _delays.Length - 1)
            {                
                _curDelay = _delays[curPosition + 1];
                Console.WriteLine(string.Format("Increased delay to {0} seconds", _curDelay));
            }
        }
    }
}