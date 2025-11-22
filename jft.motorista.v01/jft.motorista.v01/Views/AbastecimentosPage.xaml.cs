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
	public partial class AbastecimentosPage : ContentPage
	{
        public AbastecimentosPage()
        {
            InitializeComponent();
        }

        public AbastecimentosPage(AbastecimentosPageViewModel viewModel)
        {
            InitializeComponent();
            this.BindingContext = viewModel;
        }
    }
}