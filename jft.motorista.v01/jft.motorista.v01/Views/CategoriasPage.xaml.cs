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
    public partial class CategoriasPage : ContentPage
    {
        public CategoriasPage()
        {
            InitializeComponent();
        }

        // CONSTRUTOR DE INJEÇÃO
        // Recebe a ViewModel pronta (Nova ou Edição) vinda da lista
        public CategoriasPage(CategoriasPageViewModel viewModel)
        {
            InitializeComponent();
            this.BindingContext = viewModel;
        }
    }
}