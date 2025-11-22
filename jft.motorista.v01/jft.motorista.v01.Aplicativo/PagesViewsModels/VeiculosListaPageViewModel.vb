
Imports System.Collections.ObjectModel
Imports System.Windows.Input
Imports jft.motorista.v01.Aplicativo.Common
Imports jft.motorista.v01.Core.EntitiesViews
Imports jft.motorista.v01.Infra.Interfaces
Imports Xamarin.Forms

Namespace PagesViewsModels

    Public Class VeiculosListaPageViewModel
        Inherits BasePageViewModel

        Private _lista As ObservableCollection(Of VeiculosViewModel)
        Public Property Lista As ObservableCollection(Of VeiculosViewModel)
            Get
                Return _lista
            End Get
            Set(value As ObservableCollection(Of VeiculosViewModel))
                SetProperty(_lista, value)
            End Set
        End Property

        Public Property CarregarCommand As ICommand
        Public Property DeletarCommand As ICommand
        Public Property AtivarCommand As ICommand

        Public Sub New(repo As IRepositoryManager)
            MyBase.New(repo)
            Lista = New ObservableCollection(Of VeiculosViewModel)()

            CarregarCommand = New Command(Async Sub() Await CarregarDadosAsync())

            DeletarCommand = New Command(Of VeiculosViewModel)(AddressOf DeletarVeiculoAsync)

            ' ATUALIZADO: Adicionado Try/Catch seguro também no Ativar
            AtivarCommand = New Command(Of VeiculosViewModel)(Async Sub(item)
                                                                  If IsBusy Then Return
                                                                  IsBusy = True
                                                                  Dim erro As String = String.Empty

                                                                  Try
                                                                      Await repo.Veiculos.DefinirComoAtivoAsync(item.Model.id_veiculo)
                                                                      Await CarregarDadosAsync()
                                                                  Catch ex As Exception
                                                                      erro = ex.Message
                                                                  Finally
                                                                      IsBusy = False
                                                                  End Try

                                                                  If Not String.IsNullOrEmpty(erro) Then
                                                                      Await Application.Current.MainPage.DisplayAlert("Erro ao Ativar", erro, "OK")
                                                                  End If
                                                              End Sub)

            CarregarDadosAsync().SafeFireAndForget()
        End Sub

        Public Async Function CarregarDadosAsync() As Task
            If IsBusy Then Return
            IsBusy = True

            Try
                Dim dados = Await Repo.Veiculos.GetTodasAsync()

                Lista.Clear()
                For Each item In dados.OrderByDescending(Function(v) v.fl_ativo).ThenBy(Function(v) v.nm_modelo)
                    Lista.Add(New VeiculosViewModel(item))
                Next
            Catch ex As Exception
                System.Diagnostics.Debug.WriteLine($"Erro: {ex.Message}")
            Finally
                IsBusy = False
            End Try
        End Function

        ' ============================================================
        ' DELETAR (CORRIGIDO: Await fora do Catch/Try)
        ' ============================================================
        Private Async Sub DeletarVeiculoAsync(item As VeiculosViewModel)
            If IsBusy Then Return

            ' Validações de UI (fora do Try/Catch é seguro)
            If item.Model.fl_ativo Then
                Await Application.Current.MainPage.DisplayAlert("Atenção", "Não é possível apagar o veículo ativo. Ative outro carro antes.", "OK")
                Return
            End If

            Dim confirm = Await Application.Current.MainPage.DisplayAlert("Excluir", $"Apagar {item.Modelo}?", "Sim", "Não")
            If Not confirm Then Return

            IsBusy = True

            ' Variável para capturar erro
            Dim mensagemErro As String = String.Empty

            Try
                Await Repo.Veiculos.DeletarAsync(item.Model)
                Lista.Remove(item)
            Catch ex As Exception
                ' Captura a mensagem sem usar Await
                mensagemErro = ex.Message
            Finally
                IsBusy = False
            End Try

            ' Exibe erro se houver
            If Not String.IsNullOrEmpty(mensagemErro) Then
                Await Application.Current.MainPage.DisplayAlert("Erro", mensagemErro, "OK")
            End If
        End Sub

    End Class

End Namespace