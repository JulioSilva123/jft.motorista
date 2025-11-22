
Imports System.Collections.ObjectModel
Imports System.Windows.Input
Imports jft.motorista.v01.Aplicativo.Common
Imports jft.motorista.v01.Core.EntitiesViews
Imports jft.motorista.v01.Infra.Interfaces
Imports Xamarin.Forms




Namespace PagesViewsModels

    Public Class AbastecimentosListaPageViewModel
        Inherits BasePageViewModel

        Private _lista As ObservableCollection(Of AbastecimentosViewModel)
        Public Property Lista As ObservableCollection(Of AbastecimentosViewModel)
            Get
                Return _lista
            End Get
            Set(value As ObservableCollection(Of AbastecimentosViewModel))
                SetProperty(_lista, value)
            End Set
        End Property

        Private _titulo As String = "Histórico"
        Public Property Titulo As String
            Get
                Return _titulo
            End Get
            Set(value As String)
                SetProperty(_titulo, value)
            End Set
        End Property

        Public Property CarregarCommand As ICommand
        Public Property DeletarCommand As ICommand

        Public Sub New(repo As IRepositoryManager)
            MyBase.New(repo)
            Lista = New ObservableCollection(Of AbastecimentosViewModel)()

            CarregarCommand = New Command(Async Sub() Await CarregarDadosAsync())

            DeletarCommand = New Command(Of AbastecimentosViewModel)(AddressOf DeletarItemAsync)

            CarregarDadosAsync().SafeFireAndForget()
        End Sub

        Public Async Function CarregarDadosAsync() As Task
            If IsBusy Then Return
            IsBusy = True

            Try
                ' 1. Pega o carro ativo
                Dim veiculo = Await Repo.Veiculos.GetVeiculoAtivoAsync()

                If veiculo Is Nothing Then
                    Titulo = "Nenhum Veículo Ativo"
                    Lista.Clear()
                Else
                    Titulo = $"Histórico: {veiculo.nm_modelo}"

                    ' 2. Pega histórico deste carro
                    Dim dados = Await Repo.Abastecimentos.GetHistoricoPorVeiculoAsync(veiculo.id_veiculo)

                    Lista.Clear()
                    For Each item In dados
                        ' Hydration manual para exibir nome (se precisar)
                        item.Veiculo = veiculo
                        Lista.Add(New AbastecimentosViewModel(item))
                    Next
                End If

            Catch ex As Exception
                System.Diagnostics.Debug.WriteLine($"Erro: {ex.Message}")
            Finally
                IsBusy = False
            End Try
        End Function

        Private Async Sub DeletarItemAsync(item As AbastecimentosViewModel)
            If IsBusy Then Return

            Dim confirm = Await Application.Current.MainPage.DisplayAlert("Excluir", "Apagar este abastecimento?", "Sim", "Não")
            If Not confirm Then Return

            IsBusy = True
            Dim erro As String = String.Empty

            Try
                Await Repo.Abastecimentos.DeletarAsync(item.Model)
                Lista.Remove(item)
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