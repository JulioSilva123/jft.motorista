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
    public partial class DashboardViewModel : ContentPage
    {
        public DashboardViewModel()
        {
            InitializeComponent();
            // No DashboardPage.xaml.cs (Projeto Principal)
            this.BindingContext = new DashboardPageViewModel(App.Repo);
        }
    }
}