using jft.motorista.v01.Aplicativo.PagesViewsModels;
using jft.motorista.v01.Core.EntitiesViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace jft.motorista.v01.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SaldosMensaisListaPage : ContentPage
    {
        public SaldosMensaisListaPage()
        {
            InitializeComponent();
            this.BindingContext = new SaldosMensaisListaPageViewModel(App.Repo);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is SaldosMensaisListaPageViewModel vm) vm.CarregarCommand.Execute(null);
        }

        private async void OnNovoClicked(object sender, EventArgs e)
        {
            var vm = new SaldosMensaisPageViewModel(App.Repo);
            await Navigation.PushAsync(new SaldosMensaisPage(vm));
        }

        private async void OnItemSelected(object sender, SelectionChangedEventArgs e)
        {
            // ATUALIZADO: Cast para SaldosMensaisViewModel
            var item = e.CurrentSelection.FirstOrDefault() as SaldosMensaisViewModel;
            if (item == null) return;

            var vm = new SaldosMensaisPageViewModel(App.Repo, item.Model);
            await Navigation.PushAsync(new SaldosMensaisPage(vm));
            ((CollectionView)sender).SelectedItem = null;
        }
    }
}