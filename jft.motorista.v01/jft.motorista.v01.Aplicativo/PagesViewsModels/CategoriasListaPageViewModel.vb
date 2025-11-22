Imports System.Collections.ObjectModel
Imports System.Windows.Input
Imports jft.motorista.v01.Aplicativo.Common
Imports jft.motorista.v01.Core.Entities
Imports jft.motorista.v01.Core.EntitiesViews
Imports jft.motorista.v01.Core.EntitiesViews.Common
Imports jft.motorista.v01.Infra.Interfaces
Imports Xamarin.Forms

Namespace PagesViewsModels

    ''' <summary>
    ''' ViewModel para a tela de listagem de categorias.
    ''' </summary>
    Public Class CategoriasListaPageViewModel
        Inherits BasePageViewModel

        Private _lista As ObservableCollection(Of CategoriasViewModel)
        Public Property Lista As ObservableCollection(Of CategoriasViewModel)
            Get
                Return _lista
            End Get
            Set(value As ObservableCollection(Of CategoriasViewModel))
                SetProperty(_lista, value)
            End Set
        End Property

        ' Comandos
        Public Property CarregarCommand As ICommand
        Public Property DeletarCommand As ICommand

        Public Sub New(repo As IRepositoryManager)
            MyBase.New(repo)
            Lista = New ObservableCollection(Of CategoriasViewModel)()

            CarregarCommand = New Command(Async Sub() Await CarregarDadosAsync())

            DeletarCommand = New Command(Of CategoriasViewModel)(AddressOf DeletarCategoriaAsync)

            CarregarDadosAsync().SafeFireAndForget()
        End Sub

        Public Async Function CarregarDadosAsync() As Task
            If IsBusy Then Return
            IsBusy = True

            Try
                Dim dados = Await Repo.Categorias.GetTodasAsync()

                Lista.Clear()
                ' Ordena: Primeiro Receitas (0), depois Despesas (1), depois Nome
                For Each item In dados.OrderBy(Function(c) c.cs_tipo).ThenBy(Function(c) c.nm_categoria)
                    Lista.Add(New CategoriasViewModel(item))
                Next

            Catch ex As Exception
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar categorias: {ex.Message}")
            Finally
                IsBusy = False
            End Try
        End Function

        ' ============================================================
        ' DELETAR (CORRIGIDO: Await fora do Catch/Try)
        ' ============================================================
        Private Async Sub DeletarCategoriaAsync(item As CategoriasViewModel)
            If IsBusy Then Return

            ' Confirmação é UI, pode ficar fora do Try
            Dim confirm = Await Application.Current.MainPage.DisplayAlert("Excluir", $"Deseja apagar a categoria '{item.Nome}'?", "Sim", "Não")
            If Not confirm Then Return

            IsBusy = True

            ' Variáveis para armazenar mensagens e exibir DEPOIS do bloco protegido
            Dim mensagemErro As String = String.Empty
            Dim mensagemAviso As String = String.Empty

            Try
                ' Tenta deletar com segurança (verifica se tem lançamentos vinculados)
                Dim sucesso = Await Repo.Categorias.DeletarComSegurancaAsync(item.Model)

                If sucesso Then
                    Lista.Remove(item)
                Else
                    ' Não mostra o alert aqui dentro para evitar travamento em algumas versões
                    mensagemAviso = "Esta categoria já está sendo usada em lançamentos. Apague os lançamentos antes."
                End If

            Catch ex As Exception
                ' Captura o erro para mostrar depois
                mensagemErro = ex.Message
            Finally
                IsBusy = False
            End Try

            ' --- EXIBIÇÃO DE MENSAGENS (Seguro) ---

            If Not String.IsNullOrEmpty(mensagemErro) Then
                Await Application.Current.MainPage.DisplayAlert("Erro", mensagemErro, "OK")
            End If

            If Not String.IsNullOrEmpty(mensagemAviso) Then
                Await Application.Current.MainPage.DisplayAlert("Impossível Excluir", mensagemAviso, "OK")
            End If
        End Sub

    End Class


End Namespace