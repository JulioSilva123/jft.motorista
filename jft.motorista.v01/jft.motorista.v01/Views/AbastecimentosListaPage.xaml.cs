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
	public partial class AbastecimentosListaPage : ContentPage
	{
        public AbastecimentosListaPage()
        {
            InitializeComponent();
            this.BindingContext = new AbastecimentosListaPageViewModel(App.Repo);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is AbastecimentosListaPageViewModel vm)
            {
                vm.CarregarCommand.Execute(null);
            }
        }

        private async void OnNovoClicked(object sender, EventArgs e)
        {
            // Modo Criação
            var vm = new AbastecimentosPageViewModel(App.Repo);
            await Navigation.PushAsync(new AbastecimentosPage(vm));
        }

        private async void OnItemSelected(object sender, SelectionChangedEventArgs e)
        {
            var item = e.CurrentSelection.FirstOrDefault() as AbastecimentosViewModel;
            if (item == null) return;

            // Modo Edição: Passamos o Model original
            var vm = new AbastecimentosPageViewModel(App.Repo, item.Model);
            await Navigation.PushAsync(new AbastecimentosPage(vm));

            ((CollectionView)sender).SelectedItem = null;
        }
    }
}