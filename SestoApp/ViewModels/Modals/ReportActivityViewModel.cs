using System;
using System.Threading.Tasks;
using SestoApp.Models;
using SestoApp.Services;
using Xamarin.Forms;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows.Input;
using SestoApp.Views.Modals;

namespace SestoApp.ViewModels.Modals
{

    public class ReportActivityViewModel : BaseViewModel
    {
        private readonly ILogger _logger;
        public PlacesTextModel PlaceModel { get; set; }
        public string ShortAddress { get; set; }



        private bool showForm = true;
        public bool ShowForm
        {
            get
            {
                return showForm;
            }
            set
            {
                if (value != showForm)
                {
                    showForm = value;
                    OnPropertyChanged();
                }
            }

        }


        #region FormInputs
        private IList<LocationAttributesTypeEnum> locationAttributes = new List<LocationAttributesTypeEnum>();
        public IList<LocationAttributesTypeEnum> LocationAttributes
        {
            get
            {
                return locationAttributes;
            }
            set
            {
                locationAttributes = value;
                Debug.WriteLine($"Long Line, {locationAttributes.Count}");

                OnPropertyChanged();
            }
        }


        private bool lineCheck;
        public bool LineCheck
        {
            get
            {
                return lineCheck;
            }
            set
            {
                if (value != lineCheck)
                {
                    lineCheck = value;
                    updateAttributes(lineCheck, LocationAttributesTypeEnum.Queue);
                    OnPropertyChanged();
                }
            }

        }


        private bool longLineCheck;
        public bool LongLineCheck
        {
            get
            {
                return longLineCheck;
            }
            set
            {
                if (value != longLineCheck)
                {
                    longLineCheck = value;
                    updateAttributes(longLineCheck, LocationAttributesTypeEnum.LongQueue);
                    OnPropertyChanged();
                }
            }

        }


        private bool ticketingCheck;
        public bool TicketingCheck
        {
            get
            {
                return ticketingCheck;
            }
            set
            {
                if (value != ticketingCheck)
                {
                    ticketingCheck = value;
                    updateAttributes(ticketingCheck, LocationAttributesTypeEnum.Ticketing);
                    OnPropertyChanged();
                }
            }

        }

        private bool covidPrecautionsCheck;
        public bool CovidPrecautionsCheck
        {
            get
            {
                return covidPrecautionsCheck;
            }
            set
            {
                if (value != covidPrecautionsCheck)
                {
                    covidPrecautionsCheck = value;
                    updateAttributes(covidPrecautionsCheck, LocationAttributesTypeEnum.CovidPrecautions);
                    OnPropertyChanged();
                }
            }

        }



        private bool capacityLimitsCheck;
        public bool CapacityLimitsCheck
        {
            get
            {
                return capacityLimitsCheck;
            }
            set
            {
                if (value != capacityLimitsCheck)
                {
                    capacityLimitsCheck = value;
                    updateAttributes(capacityLimitsCheck, LocationAttributesTypeEnum.CapacityLimits);
                    OnPropertyChanged();
                }
            }

        }


        private bool reducedHoursCheck;
        public bool ReducedHoursCheck
        {
            get
            {
                return covidPrecautionsCheck;
            }
            set
            {
                if (value != reducedHoursCheck)
                {
                    reducedHoursCheck = value;
                    updateAttributes(reducedHoursCheck, LocationAttributesTypeEnum.ReducedHours);
                    OnPropertyChanged();
                }
            }

        }

        private bool busyCheck;
        public bool BusyCheck
        {
            get
            {
                return busyCheck;
            }
            set
            {
                if (value != busyCheck)
                {
                    busyCheck = value;
                    updateAttributes(busyCheck, LocationAttributesTypeEnum.Busy);
                    OnPropertyChanged();
                }
            }

        }


        private bool notBusyCheck;
        public bool NotBusyCheck
        {
            get
            {
                return notBusyCheck;
            }
            set
            {
                if (value != notBusyCheck)
                {
                    notBusyCheck = value;
                    updateAttributes(notBusyCheck, LocationAttributesTypeEnum.NotBusy);
                    OnPropertyChanged();
                }
            }

        }



        private int waitTime { get; set; }
        public int WaitTime
        {
            get
            {
                return waitTime;
            }
            set
            {
                waitTime = value;
                OnPropertyChanged();
            }
        }

        private int headCount { get; set; }
        public int HeadCount
        {
            get
            {
                return headCount;
            }
            set
            {
                headCount = value;
                OnPropertyChanged();
            }
        }



        private void updateAttributes(bool checkbox, LocationAttributesTypeEnum chechboxType)
        {
            if (checkbox == true)
                locationAttributes.Add(chechboxType);
            if (checkbox == false)
                locationAttributes.RemoveAt(locationAttributes.IndexOf(chechboxType));
        }
        #endregion
        public ICommand SubmitActivityButton => new Command(async (object sender) =>
        {
            IsBusy = true;
            ShowForm = false;


            try
            {

                var attributes = new List<ActivityAttributeModel>();
                foreach (LocationAttributesTypeEnum attributeType in LocationAttributes)
                {
                    attributes.Add(new ActivityAttributeModel
                    {
                        AttributeTypeId = (int)attributeType
                    });
                }

                var activity = new PlaceActivityModel
                {
                    ActivityAttributes = attributes,
                    HeadCount = HeadCount,
                    WaitTime = WaitTime,
                    PlaceId = PlaceModel.PlaceId
                };
                await DataService.CreateActivityReport(activity);

                //await Task.Delay(3000);
                //await App.Current.MainPage.Navigation.PopModalAsync();
                IsSuccess = true;
                MessagingCenter.Send(this, "AnimateSuccessIcon");
                Debug.WriteLine($"show form ={ShowForm}");

            }
            catch (Exception Ex)
            {
                IsBusy = false;
                ShowForm = true;
                IsSuccess = false;

                await Application.Current.MainPage.DisplayAlert("We already got your status update", "Looks like you've submitted a report within the last 5 minutes. Please try adding a status report in a few minutes.", "OK");

            }
            finally
            {

                IsBusy = false;
                await Task.Delay(3000000);
                var modalStack = (Application.Current.MainPage.Navigation.ModalStack);
                bool shouldPopModal = modalStack.Any(p => p is ReportActivityPage);
                if (shouldPopModal)
                {
                    await App.Current.MainPage.Navigation.PopModalAsync();
                }
            }

            return;
        });


        public ReportActivityViewModel(PlacesTextModel placesTextModel)
        {
            PlaceModel = placesTextModel;
            Title = "Situation Report";
            ShortAddress = PlaceModel.FormattedAddress.Split(",")[0];
        }


    }
}