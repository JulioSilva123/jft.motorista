using jft.motorista.v01.Aplicativo.PagesViewsModels;
using jft.motorista.v01.Core.Entities;
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
	public partial class DiariosDetalhePage : ContentPage
	{
        public DiariosDetalhePage(Diarios diario)
        {
            InitializeComponent();

            // ATUALIZADO: Usa DiariosDetalhePageViewModel
            var vm = new DiariosDetalhePageViewModel(App.Repo, diario);
            this.BindingContext = vm;

            // ATUALIZADO: Tipo genérico no Subscribe
            MessagingCenter.Subscribe<DiariosDetalhePageViewModel, Diarios>(this, "NavegarParaEdicao", async (sender, model) =>
            {
                var vmEdicao = new DiariosPageViewModel(App.Repo, model);
                await Navigation.PushAsync(new DiariosPage(vmEdicao));
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            // ATUALIZADO: Unsubscribe com o novo tipo
            MessagingCenter.Unsubscribe<DiariosDetalhePageViewModel, Diarios>(this, "NavegarParaEdicao");
        }
    }
}