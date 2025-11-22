
Imports System.Collections.ObjectModel
Imports System.Windows.Input
Imports jft.motorista.v01.Aplicativo.Common
Imports jft.motorista.v01.Core.EntitiesViews
Imports jft.motorista.v01.Infra.Interfaces
Imports Xamarin.Forms

Namespace PagesViewsModels

    Public Class SaldosMensaisListaPageViewModel
        Inherits BasePageViewModel

        Private _lista As ObservableCollection(Of SaldosMensaisViewModel)
        Public Property Lista As ObservableCollection(Of SaldosMensaisViewModel)
            Get
                Return _lista
            End Get
            Set(value As ObservableCollection(Of SaldosMensaisViewModel))
                SetProperty(_lista, value)
            End Set
        End Property

        Public Property CarregarCommand As ICommand
        Public Property DeletarCommand As ICommand

        Public Sub New(repo As IRepositoryManager)
            MyBase.New(repo)
            Lista = New ObservableCollection(Of SaldosMensaisViewModel)()
            CarregarCommand = New Command(Async Sub() Await CarregarDadosAsync())
            DeletarCommand = New Command(Of SaldosMensaisViewModel)(AddressOf DeletarItemAsync)

            CarregarDadosAsync().SafeFireAndForget()
        End Sub

        Public Async Function CarregarDadosAsync() As Task
            If IsBusy Then Return
            IsBusy = True
            Try
                ' Uso do Repositório Plural
                Dim dados = Await Repo.SaldosMensais.GetHistoricoCompletoAsync()
                Lista.Clear()
                For Each item In dados
                    Lista.Add(New SaldosMensaisViewModel(item))
                Next
            Finally
                IsBusy = False
            End Try
        End Function

        ' ============================================================
        ' DELETAR (CORRIGIDO: Await fora do Catch)
        ' ============================================================
        Private Async Sub DeletarItemAsync(item As SaldosMensaisViewModel)
            ' Validação de UI (Pode usar Await aqui fora)
            Dim confirm = Await Application.Current.MainPage.DisplayAlert("Excluir", "Apagar este fechamento mensal forçará o recálculo automático. Continuar?", "Sim", "Não")
            If Not confirm Then Return

            IsBusy = True
            Dim mensagemErro As String = String.Empty

            Try
                Await Repo.SaldosMensais.DeletarAsync(item.Model)
                Lista.Remove(item)
            Catch ex As Exception
                ' Captura o erro sem travar a thread com Await
                mensagemErro = ex.Message
            Finally
                IsBusy = False
            End Try

            ' Exibe o erro de forma segura fora do bloco protegido
            If Not String.IsNullOrEmpty(mensagemErro) Then
                Await Application.Current.MainPage.DisplayAlert("Erro", mensagemErro, "OK")
            End If
        End Sub
    End Class
End Namespace