using System.Diagnostics;
using SestoApp.Models;
using SestoApp.ViewModels;
using SestoApp.ViewModels.Modals;
using Xamarin.Forms;


namespace SestoApp.Views.Modals
{
    public partial class ReportActivityPage : ContentPage
    {
        ReportActivityViewModel viewModel;


        public ReportActivityPage(ReportActivityViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = this.viewModel = viewModel;
            MessagingCenter.Subscribe<ReportActivityViewModel>(this, "AnimateSuccessIcon", async (sender) =>
            {
                TransformImage();
                Debug.WriteLine("AnimateSuccessIcon message recieved.");
            });
        }

        public ReportActivityPage()
        {
            InitializeComponent();

            viewModel = new ReportActivityViewModel(placesTextModel: viewModel.PlaceModel);
            BindingContext = viewModel;
        }


        async void TransformImage()
        {
            //SuccessImage.Opacity = 0;
            await SuccessImage.ScaleTo(2, 100);
            await SuccessImage.ScaleTo(1.6, 100);
            await SuccessImage.ScaleTo(1.5, 100);
            await SuccessImage.ScaleTo(1.6, 100);
            //await SuccessImage.FadeTo(1, 4000);
        }




        async void CloseModal_Clicked(System.Object sender, System.EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

    }
}
