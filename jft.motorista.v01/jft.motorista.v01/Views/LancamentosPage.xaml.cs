using jft.motorista.v01.Aplicativo.PagesViewsModels;
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
    public partial class LancamentosPage : ContentPage
    {
        public LancamentosPage()
        {
            InitializeComponent();
        }

        // CONSTRUTOR DE INJEÇÃO
        // Recebe a ViewModel já instanciada e configurada (pelo Dashboard)
        public LancamentosPage(LancamentosPageViewModel viewModel) 
        {
            InitializeComponent();

            // Liga a Tela à Lógica
            this.BindingContext = viewModel;
        }


    }
}