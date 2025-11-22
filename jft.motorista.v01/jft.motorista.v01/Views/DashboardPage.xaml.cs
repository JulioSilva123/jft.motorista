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
    public partial class DashboardPage : ContentPage
    {
        public DashboardPage()
        {
            InitializeComponent();
            var repositorio = App.Repo;
            this.BindingContext = new DashboardPageViewModel(repositorio);
        }

        private async void OnNovoLancamentoClicked(object sender, EventArgs e)
        {
            var vm = new LancamentosPageViewModel(App.Repo);
            var page = new LancamentosPage(vm);
            await Navigation.PushAsync(page);
        }

        private async void OnLancamentoSelected(object sender, SelectionChangedEventArgs e)
        {
            var itemSelecionado = e.CurrentSelection.FirstOrDefault() as LancamentosViewModel;
            if (itemSelecionado == null) return;

            var vm = new LancamentosPageViewModel(App.Repo, itemSelecionado.Model);
            var page = new LancamentosPage(vm);
            await Navigation.PushAsync(page);

            ((CollectionView)sender).SelectedItem = null;
        }

        // Navegações do Cabeçalho

        // ALTERADO: Agora abre um menu de opções
        private async void OnConfiguracoesClicked(object sender, EventArgs e)
        {
            string acao = await DisplayActionSheet("Configurações", "Cancelar", null, "Gerir Categorias", "Ajuste Manual de Saldos");

            switch (acao)
            {
                case "Gerir Categorias":
                    await Navigation.PushAsync(new CategoriasListaPage());
                    break;
                case "Ajuste Manual de Saldos":
                    await Navigation.PushAsync(new SaldosMensaisListaPage());
                    break;
            }
        }

        private async void OnMeusVeiculosClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new VeiculosListaPage());
        }

        private async void OnMeusAbastecimentosClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AbastecimentosListaPage());
        }

        private async void OnMeusDiariosClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DiariosListaPage());
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is DashboardPageViewModel vm)
            {
                vm.AtualizarCommand?.Execute(null);
            }
        }


    }
}