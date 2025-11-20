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
    public partial class VeiculosListaPage : ContentPage
    {
        public VeiculosListaPage()
        {
            InitializeComponent();
            this.BindingContext = new VeiculosListaPageViewModel(App.Repo);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is VeiculosListaPageViewModel vm)
            {
                vm.CarregarCommand.Execute(null);
            }
        }

        private async void OnNovoVeiculoClicked(object sender, EventArgs e)
        {
            var vm = new VeiculosPageViewModel(App.Repo);
            await Navigation.PushAsync(new VeiculosPage(vm));
        }

        private async void OnVeiculoSelected(object sender, SelectionChangedEventArgs e)
        {
            // ATUALIZADO: Cast para VeiculosViewModel
            var item = e.CurrentSelection.FirstOrDefault() as VeiculosViewModel;
            if (item == null) return;

            var vm = new VeiculosPageViewModel(App.Repo, item.Model);
            await Navigation.PushAsync(new VeiculosPage(vm));

            ((CollectionView)sender).SelectedItem = null;
        }
    }
}