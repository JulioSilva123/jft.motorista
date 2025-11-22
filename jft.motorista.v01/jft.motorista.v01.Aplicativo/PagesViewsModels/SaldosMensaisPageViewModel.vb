
Imports System.Globalization
Imports System.Reflection
Imports System.Windows.Input
Imports jft.motorista.v01.Aplicativo.Common
Imports jft.motorista.v01.Core.Entities
Imports jft.motorista.v01.Infra.Interfaces
Imports Xamarin.Forms

Namespace PagesViewsModels

    Public Class SaldosMensaisPageViewModel
        Inherits BasePageViewModel

        Private ReadOnly _culturaBR As New CultureInfo("pt-BR")
        Private _idEdicao As Integer = 0

        ' ... (Propriedades Visuais: TituloPagina, Mes, Ano, SaldoFinal mantidas) ...
        Private _tituloPagina As String = "Novo Fechamento"
        Public Property TituloPagina As String
            Get
                Return _tituloPagina
            End Get
            Set(value As String)
                SetProperty(_tituloPagina, value)
            End Set
        End Property

        Private _mes As String
        Public Property Mes As String
            Get
                Return _mes
            End Get
            Set(value As String)
                SetProperty(_mes, value)
            End Set
        End Property

        Private _ano As String
        Public Property Ano As String
            Get
                Return _ano
            End Get
            Set(value As String)
                SetProperty(_ano, value)
            End Set
        End Property

        Private _saldoFinal As String
        Public Property SaldoFinal As String
            Get
                Return _saldoFinal
            End Get
            Set(value As String)
                SetProperty(_saldoFinal, value)
            End Set
        End Property

        Public Property SalvarCommand As ICommand
        Public Property CancelarCommand As ICommand

        Public Sub New(repo As IRepositoryManager, Optional itemEditar As SaldosMensais = Nothing)
            MyBase.New(repo)

            SalvarCommand = New Command(Async Sub() Await SalvarAsync())
            CancelarCommand = New Command(Async Sub() Await Application.Current.MainPage.Navigation.PopAsync())

            Inicializar(itemEditar)
        End Sub

        Private Sub Inicializar(item As SaldosMensais)
            If item Is Nothing Then
                TituloPagina = "Ajuste Manual de Saldo"
                _idEdicao = 0
                Mes = DateTime.Now.Month.ToString()
                Ano = DateTime.Now.Year.ToString()
                SaldoFinal = String.Empty
            Else
                TituloPagina = "Editar Fechamento"
                _idEdicao = item.id_saldomensal
                Mes = item.nr_mes.ToString()
                Ano = item.nr_ano.ToString()
                SaldoFinal = item.vl_saldofinal.ToString("F2", _culturaBR)
            End If
        End Sub

        Private Async Function SalvarAsync() As Task
            If IsBusy Then Return

            Dim m, y As Integer
            Dim s As Decimal

            If Not Integer.TryParse(Mes, m) OrElse m < 1 OrElse m > 12 Then
                Await Application.Current.MainPage.DisplayAlert("Erro", "Mês inválido (1-12).", "OK")
                Return
            End If

            If Not Integer.TryParse(Ano, y) OrElse y < 2000 Then
                Await Application.Current.MainPage.DisplayAlert("Erro", "Ano inválido.", "OK")
                Return
            End If

            If Not Decimal.TryParse(SaldoFinal, NumberStyles.Any, _culturaBR, s) Then
                Await Application.Current.MainPage.DisplayAlert("Erro", "Valor inválido.", "OK")
                Return
            End If

            IsBusy = True
            Dim msgErro As String = String.Empty

            Try
                Dim model As New SaldosMensais With {
                    .id_saldomensal = _idEdicao,
                    .nr_mes = m,
                    .nr_ano = y,
                    .vl_saldofinal = s
                }

                ' Uso do Repositório Plural
                Await Repo.SaldosMensais.SalvarAsync(model)
                Await Application.Current.MainPage.Navigation.PopAsync()

            Catch ex As Exception
                msgErro = ex.Message
            Finally
                IsBusy = False
            End Try

            If Not String.IsNullOrEmpty(msgErro) Then
                Await Application.Current.MainPage.DisplayAlert("Erro", msgErro, "OK")
            End If
        End Function

    End Class
End Namespace