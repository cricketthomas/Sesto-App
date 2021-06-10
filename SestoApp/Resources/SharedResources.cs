using System;
using SestoApp.Models;
using Xamarin.Forms;

namespace SestoApp.Resources
{

    public static class SharedResources
    {

        public static Color ButtonBkColor
        {
            get { return Color.FromRgb(0xff, 0xa5, 0); }
        }
        public static string HelpText
        {
            get { return "Search for a location using the name and general region or address."; }
        }
        //public static LocationAttributesTypeEnum None
        //{
        //    get { return LocationAttributesTypeEnum.None; }
        //}

        //public static LocationAttributesTypeEnum Queue
        //{
        //    get { return LocationAttributesTypeEnum.Queue; }
        //}



        //public static LocationAttributesTypeEnum Ticketing
        //{
        //    get { return LocationAttributesTypeEnum.Ticketing; }
        //}
       
        //public static LocationAttributesTypeEnum CovidPrecautions
        //{
        //    get { return LocationAttributesTypeEnum.CovidPrecautions; }
        //}
    }
}
