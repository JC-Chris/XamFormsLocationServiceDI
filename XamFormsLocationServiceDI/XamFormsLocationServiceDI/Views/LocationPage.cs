using System;

using Xamarin.Forms;
using XamFormsLocationServiceDI.Services;
using XamFormsLocationServiceDI.Constants;
using XamFormsLocationServiceDI.Models;

namespace XamFormsLocationServiceDI.Views
{
    public class LocationPage : ContentPage
    {
        private ILocationService _location;
        public LocationPage(ILocationService locationService)
        {
            _location = locationService;

            // create the location label
            var lblPosition = new Label {XAlign = TextAlignment.Center, Text = "No position yet"};

            // listen for location messages
            MessagingCenter.Subscribe<ILocationService, GeoPosition>(this, MessagingConstants.LocationUpdateMessage,
                (sender, position) =>
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                    {
                        lblPosition.Text = string.Format("Lat: {0}, Long: {1}, Alt: {2}", position.Latitude, position.Longitude, position.Altitude);
                    }));

            // create the layout
            Content = new StackLayout
            {
                Padding = new Thickness(10, 20),
                Children =
                {
                    lblPosition
                }
            };
        }

        public void StartListeningToLocation()
        {
            // start listening to location changes
            _location.StartMonitoringLocationChanges();
        }

        public void StopListeningToLocation()
        {
            _location.StopMonitoringLocationChanges();
        }
    }
}


