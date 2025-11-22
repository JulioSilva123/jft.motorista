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
	public partial class DiariosListaPage : ContentPage
	{
        public DiariosListaPage()
        {
            InitializeComponent();
            this.BindingContext = new DiariosListaPageViewModel(App.Repo);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is DiariosListaPageViewModel vm)
            {
                vm.CarregarCommand.Execute(null);
            }
        }

        private async void OnNovoDiarioClicked(object sender, EventArgs e)
        {
            var vm = new DiariosPageViewModel(App.Repo);
            await Navigation.PushAsync(new DiariosPage(vm));
        }

        private async void OnDiarioSelected(object sender, SelectionChangedEventArgs e)
        {
            var item = e.CurrentSelection.FirstOrDefault() as DiariosViewModel;
            if (item == null) return;

            // ATUALIZADO: Chama DiariosDetalhePage (Plural)
            await Navigation.PushAsync(new DiariosDetalhePage(item.Model));

            ((CollectionView)sender).SelectedItem = null;
        }
    }
}