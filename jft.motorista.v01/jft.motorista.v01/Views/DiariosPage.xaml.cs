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
	public partial class DiariosPage : ContentPage
	{
        public DiariosPage()
        {
            InitializeComponent();
        }

        public DiariosPage(DiariosPageViewModel viewModel)
        {
            InitializeComponent();
            this.BindingContext = viewModel;
        }
    }
}