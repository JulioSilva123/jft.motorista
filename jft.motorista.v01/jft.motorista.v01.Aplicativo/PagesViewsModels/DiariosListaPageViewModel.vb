
Imports System.Collections.ObjectModel
Imports System.Windows.Input
Imports jft.motorista.v01.Aplicativo.Common
Imports jft.motorista.v01.Core.EntitiesViews
Imports jft.motorista.v01.Infra.Interfaces
Imports Xamarin.Forms

Namespace PagesViewsModels

    Public Class DiariosListaPageViewModel
        Inherits BasePageViewModel

        Private _dataRef As DateTime = DateTime.Now

        Private _lista As ObservableCollection(Of DiariosViewModel)
        Public Property Lista As ObservableCollection(Of DiariosViewModel)
            Get
                Return _lista
            End Get
            Set(value As ObservableCollection(Of DiariosViewModel))
                SetProperty(_lista, value)
            End Set
        End Property

        Private _mesTexto As String
        Public Property MesTexto As String
            Get
                Return _mesTexto
            End Get
            Set(value As String)
                SetProperty(_mesTexto, value)
            End Set
        End Property

        Private _totalKmMes As Decimal
        Public Property TotalKmMes As Decimal
            Get
                Return _totalKmMes
            End Get
            Set(value As Decimal)
                SetProperty(_totalKmMes, value)
            End Set
        End Property

        Public Property CarregarCommand As ICommand
        Public Property ProximoMesCommand As ICommand
        Public Property MesAnteriorCommand As ICommand
        Public Property DeletarCommand As ICommand

        Public Sub New(repo As IRepositoryManager)
            MyBase.New(repo)
            Lista = New ObservableCollection(Of DiariosViewModel)()

            ConfigurarComandos()
            CarregarDadosAsync().SafeFireAndForget()
        End Sub

        Private Sub ConfigurarComandos()
            CarregarCommand = New Command(Async Sub() Await CarregarDadosAsync())

            ProximoMesCommand = New Command(Async Sub()
                                                _dataRef = _dataRef.AddMonths(1)
                                                Await CarregarDadosAsync()
                                            End Sub)

            MesAnteriorCommand = New Command(Async Sub()
                                                 _dataRef = _dataRef.AddMonths(-1)
                                                 Await CarregarDadosAsync()
                                             End Sub)

            DeletarCommand = New Command(Of DiariosViewModel)(AddressOf DeletarDiarioAsync)
        End Sub

        Public Async Function CarregarDadosAsync() As Task
            If IsBusy Then Return
            IsBusy = True

            Try
                MesTexto = _dataRef.ToString("MMMM yyyy").ToUpper()

                Dim resultado = Await Repo.Diarios.GetResumoCustosMensalAsync(_dataRef.Month, _dataRef.Year)
                Dim dados = resultado.Lista

                Lista.Clear()
                Dim somaKm As Decimal = 0

                For Each item In dados
                    Lista.Add(New DiariosViewModel(item))

                    ' ATUALIZADO: Soma o Trip para ser consistente com a exibição da lista
                    somaKm += item.nr_km_informado_trip
                Next

                TotalKmMes = somaKm

            Catch ex As Exception
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar diários: {ex.Message}")
            Finally
                IsBusy = False
            End Try
        End Function

        Private Async Sub DeletarDiarioAsync(item As DiariosViewModel)
            If IsBusy Then Return

            Dim confirm = Await Application.Current.MainPage.DisplayAlert("Excluir", "Apagar este diário de bordo?", "Sim", "Não")
            If Not confirm Then Return

            IsBusy = True
            Dim erro As String = String.Empty

            Try
                Await Repo.Diarios.DeletarAsync(item.Model)
                Lista.Remove(item)

                ' ATUALIZADO: Subtrai o Trip do total
                TotalKmMes -= item.Model.nr_km_informado_trip
            Catch ex As Exception
                erro = ex.Message
            Finally
                IsBusy = False
            End Try

            If Not String.IsNullOrEmpty(erro) Then
                Await Application.Current.MainPage.DisplayAlert("Erro", erro, "OK")
            End If
        End Sub

    End Class

End Namespace