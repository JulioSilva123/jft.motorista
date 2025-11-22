
Imports System.Collections.ObjectModel
Imports System.Windows.Input
Imports jft.motorista.v01.Aplicativo.Common
Imports jft.motorista.v01.Core.Entities
Imports jft.motorista.v01.Infra.Interfaces
Imports Xamarin.Forms





Namespace PagesViewsModels


    ''' <summary>
    ''' ViewModel responsável por Criar ou Editar uma Categoria.
    ''' (Antiga NovaCategoriaPageViewModel)
    ''' </summary>
    Public Class CategoriasPageViewModel
        Inherits BasePageViewModel

        Private _idEdicao As Integer = 0

        ' ============================================================
        ' PROPRIEDADES
        ' ============================================================

        Private _tituloPagina As String
        Public Property TituloPagina As String
            Get
                Return _tituloPagina
            End Get
            Set(value As String)
                SetProperty(_tituloPagina, value)
            End Set
        End Property

        Private _nome As String
        Public Property Nome As String
            Get
                Return _nome
            End Get
            Set(value As String)
                SetProperty(_nome, value)
            End Set
        End Property

        ' 0=Receita, 1=Despesa
        Private _tipoSelecionado As Integer = 1
        Public Property TipoSelecionado As Integer
            Get
                Return _tipoSelecionado
            End Get
            Set(value As Integer)
                SetProperty(_tipoSelecionado, value)
                OnPropertyChanged(NameOf(IsReceita))
                OnPropertyChanged(NameOf(IsDespesa))
            End Set
        End Property

        Public ReadOnly Property IsReceita As Boolean
            Get
                Return _tipoSelecionado = 0
            End Get
        End Property

        Public ReadOnly Property IsDespesa As Boolean
            Get
                Return _tipoSelecionado = 1
            End Get
        End Property

        ' Seleção de Ícone
        Public Property ListaIcones As ObservableCollection(Of String)
        Private _iconeSelecionado As String
        Public Property IconeSelecionado As String
            Get
                Return _iconeSelecionado
            End Get
            Set(value As String)
                SetProperty(_iconeSelecionado, value)
            End Set
        End Property

        ' Seleção de Cor
        Public Property ListaCores As ObservableCollection(Of String)
        Private _corSelecionada As String
        Public Property CorSelecionada As String
            Get
                Return _corSelecionada
            End Get
            Set(value As String)
                SetProperty(_corSelecionada, value)
            End Set
        End Property

        ' ============================================================
        ' COMANDOS
        ' ============================================================
        Public Property SalvarCommand As ICommand
        Public Property MudarTipoCommand As ICommand
        Public Property CancelarCommand As ICommand

        ' ============================================================
        ' CONSTRUTOR
        ' ============================================================
        Public Sub New(repo As IRepositoryManager, Optional itemEditar As Categorias = Nothing)
            MyBase.New(repo)

            CarregarListasAuxiliares()
            ConfigurarComandos()
            Inicializar(itemEditar)
        End Sub

        Private Sub CarregarListasAuxiliares()
            ' FontAwesome Icons úteis para motoristas
            ListaIcones = New ObservableCollection(Of String) From {
                "fa-car", "fa-gas-pump", "fa-wrench", "fa-utensils",
                "fa-mobile-alt", "fa-money-bill-wave", "fa-university",
                "fa-soap", "fa-parking", "fa-coffee", "fa-tag", "fa-file-invoice-dollar"
            }

            ' Paleta de Cores Material Design
            ListaCores = New ObservableCollection(Of String) From {
                "#F44336", "#E91E63", "#9C27B0", "#673AB7",
                "#3F51B5", "#2196F3", "#03A9F4", "#00BCD4",
                "#009688", "#4CAF50", "#8BC34A", "#CDDC39",
                "#FFEB3B", "#FFC107", "#FF9800", "#FF5722",
                "#795548", "#9E9E9E", "#607D8B", "#000000"
            }
        End Sub

        Private Sub ConfigurarComandos()
            MudarTipoCommand = New Command(Of String)(Sub(t)
                                                          TipoSelecionado = Integer.Parse(t)
                                                      End Sub)

            SalvarCommand = New Command(Async Sub() Await SalvarAsync())

            CancelarCommand = New Command(Async Sub()
                                              Await Application.Current.MainPage.Navigation.PopAsync()
                                          End Sub)
        End Sub

        Private Sub Inicializar(item As Categorias)
            If item Is Nothing Then
                TituloPagina = "Nova Categoria"
                _idEdicao = 0
                TipoSelecionado = 1 ' Padrão Despesa
                IconeSelecionado = ListaIcones.First()
                CorSelecionada = ListaCores.First()
            Else
                TituloPagina = "Editar Categoria"
                ' Campos atualizados do Model
                _idEdicao = item.id_categoria
                Nome = item.nm_categoria
                TipoSelecionado = item.cs_tipo
                IconeSelecionado = item.te_icone
                CorSelecionada = item.te_CorHex
            End If
        End Sub

        ' ============================================================
        ' LÓGICA DE SALVAR (CORRIGIDA)
        ' ============================================================
        Private Async Function SalvarAsync() As Task
            If IsBusy Then Return

            ' Validações fora do Try/Catch (OK usar Await aqui)
            If String.IsNullOrWhiteSpace(Nome) Then
                Await Application.Current.MainPage.DisplayAlert("Atenção", "Digite o nome da categoria.", "OK")
                Return
            End If

            IsBusy = True

            ' Variável auxiliar para capturar erro
            Dim mensagemErro As String = String.Empty

            Try
                Dim model As New Categorias With {
                    .id_categoria = _idEdicao,
                    .nm_categoria = Nome,
                    .cs_tipo = TipoSelecionado,
                    .te_icone = IconeSelecionado,
                    .te_CorHex = CorSelecionada
                }

                Await Repo.Categorias.SalvarAsync(model)
                Await Application.Current.MainPage.Navigation.PopAsync()

            Catch ex As Exception
                ' Captura apenas a mensagem, sem Await
                mensagemErro = ex.Message
            Finally
                IsBusy = False
            End Try

            ' Exibe o erro fora do bloco protegido
            If Not String.IsNullOrEmpty(mensagemErro) Then
                Await Application.Current.MainPage.DisplayAlert("Erro", mensagemErro, "OK")
            End If
        End Function

    End Class

End Namespace