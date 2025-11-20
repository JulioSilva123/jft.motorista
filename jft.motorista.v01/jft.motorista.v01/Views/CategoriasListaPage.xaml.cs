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
    public partial class CategoriasListaPage : ContentPage
    {
        public CategoriasListaPage()
        {
            InitializeComponent();

            // ATUALIZADO: Usa CategoriasListaPageViewModel
            this.BindingContext = new CategoriasListaPageViewModel(App.Repo);
        }

        // Ao abrir a tela, recarrega a lista
        protected override void OnAppearing()
        {
            base.OnAppearing();
            // ATUALIZADO: Cast com o novo nome
            if (BindingContext is CategoriasListaPageViewModel vm)
            {
                vm.CarregarCommand.Execute(null);
            }
        }

        // Botão "Nova" da Toolbar -> Vai para a tela de edição (CategoriasPage)
        private async void OnNovaCategoriaClicked(object sender, EventArgs e)
        {
            // Modo Criação (apenas Repo)
            var vm = new CategoriasPageViewModel(App.Repo);
            await Navigation.PushAsync(new CategoriasPage(vm));
        }

        // Clique na Lista -> Vai para a tela de edição (CategoriasPage)
        private async void OnCategoriaSelected(object sender, SelectionChangedEventArgs e)
        {
            var item = e.CurrentSelection.FirstOrDefault() as CategoriasViewModel;
            if (item == null) return;

            // Modo Edição (Repo + Model Original)
            var vm = new CategoriasPageViewModel(App.Repo, item.Model);
            await Navigation.PushAsync(new CategoriasPage(vm));

            // Limpa seleção
            ((CollectionView)sender).SelectedItem = null;
        }
    }
}