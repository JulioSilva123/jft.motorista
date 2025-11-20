using jft.motorista.v01.Infra;
using jft.motorista.v01.Infra.Data;
using jft.motorista.v01.Infra.Interfaces;
using jft.motorista.v01.Views;
using System;
using System.Globalization;
using System.Threading;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace jft.motorista.v01
{
    public partial class App : Application
    {
        public App()
        {


            // ============================================================
            // 1. FORÇAR CULTURA PT-BR (GLOBAL)
            // ============================================================
            // Isso garante que Vírgula seja decimal e Ponto seja milhar
            // em todo o sistema, independente do idioma do celular.

            var culture = new CultureInfo("pt-BR");

            // Aplica na Thread Principal (UI)
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            // Aplica em todas as Tasks (Async/Await) que forem criadas depois
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            // ============================================================


            InitializeComponent();

            //MainPage = new MainPage();
            // Define a página inicial dentro de uma NavigationPage
            // Isso habilita a barra de título azul no topo
            MainPage = new NavigationPage(new DashboardPage());
        }


        // Variável privada estática para segurar a instância única
        private static IRepositoryManager _repo;

        // ============================================================
        // COMPOSITION ROOT (Ponto Único de Verdade)
        // ============================================================

        /// <summary>
        /// Acesso global ao Gerenciador de Repositórios (Unit of Work).
        /// Usa o padrão Singleton para garantir que só exista uma conexão de banco.
        /// </summary>
        public static IRepositoryManager Repo
        {
            get
            {
                if (_repo == null)
                {
                    // 1. Cria a Infraestrutura (Banco SQLite)
                    var dbContext = new DbMotorista();

                    // 2. Cria a Unidade de Trabalho (Repositórios) injetando a Infra
                    _repo = new RepositoryManager(dbContext);
                }

                return _repo;
            }
        }

        // ============================================================
        // CICLO DE VIDA DO APP
        // ============================================================

        protected override void OnStart()
        {
            // Opcional: Você pode forçar a criação do banco aqui se quiser
            // var db = Repo; 

            var init = Repo;

        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
