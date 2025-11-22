
Imports System.Globalization ' <--- IMPORTANTE
Imports System.Collections.ObjectModel
Imports System.Windows.Input
Imports jft.motorista.v01.Aplicativo.Common
Imports jft.motorista.v01.Core.Entities
Imports jft.motorista.v01.Core.EntitiesViews.Common
Imports jft.motorista.v01.Infra.Interfaces
Imports Xamarin.Forms

Namespace PagesViewsModels

    Public Class LancamentosPageViewModel
        Inherits BasePageViewModel

        'Private ReadOnly _repo As IRepositoryManager 

        ' Armazena o ID se estivermos editando (0 se for novo)
        Private _idEdicao As Integer = 0
        ' Cultura Brasileira para forçar a virgula (R$)
        Private ReadOnly _culturaBR As New CultureInfo("pt-BR")


        ' ============================================================
        ' 1. PROPRIEDADES VISUAIS
        ' ============================================================

        ' Título dinâmico da tela
        Private _tituloPagina As String
        Public Property TituloPagina As String
            Get
                Return _tituloPagina
            End Get
            Set(value As String)
                SetProperty(_tituloPagina, value)
            End Set
        End Property

        '' Valor (Sempre positivo na tela)
        'Private _valor As Decimal
        'Public Property Valor As Decimal
        '    Get
        '        Return _valor
        '    End Get
        '    Set(value As Decimal)
        '        SetProperty(_valor, value)
        '    End Set
        'End Property


        ' IMPORTANTE: O tipo deve ser STRING para aceitar "10," sem converter
        Private _valor As String
        Public Property Valor As String
            Get
                Return _valor
            End Get
            Set(value As String)
                ' SetProperty só notifica se a String for diferente.
                ' "10," é diferente de "10". Então ele aceita e mantém a vírgula.
                SetProperty(_valor, value)
            End Set
        End Property



        Private _data As DateTime
        Public Property Data As DateTime
            Get
                Return _data
            End Get
            Set(value As DateTime)
                SetProperty(_data, value)
            End Set
        End Property

        Private _observacao As String
        Public Property Observacao As String
            Get
                Return _observacao
            End Get
            Set(value As String)
                SetProperty(_observacao, value)
            End Set
        End Property

        ' Lista do Picker
        Public Property ListaCategorias As ObservableCollection(Of Categorias)

        Private _categoriaSelecionada As Categorias
        Public Property CategoriaSelecionada As Categorias
            Get
                Return _categoriaSelecionada
            End Get
            Set(value As Categorias)
                SetProperty(_categoriaSelecionada, value)
            End Set
        End Property

        ' 0 = Receita, 1 = Despesa
        Private _tipoOperacao As Integer = 0
        Public Property TipoOperacao As Integer
            Get
                Return _tipoOperacao
            End Get
            Set(value As Integer)
                If SetProperty(_tipoOperacao, value) Then
                    ' Se o usuário trocou o tipo manualmente, recarregamos as categorias
                    ' Mas cuidado: No carregamento inicial (Edição), evitamos recarregar 2 vezes
                    If Not IsBusy Then
                        CarregarCategoriasAsync().SafeFireAndForget()
                    End If

                    OnPropertyChanged(NameOf(IsReceita))
                    OnPropertyChanged(NameOf(IsDespesa))
                End If
            End Set
        End Property

        Public ReadOnly Property IsReceita As Boolean
            Get
                Return _tipoOperacao = 0
            End Get
        End Property

        Public ReadOnly Property IsDespesa As Boolean
            Get
                Return _tipoOperacao = 1
            End Get
        End Property

        ' ============================================================
        ' 2. COMANDOS
        ' ============================================================
        Public Property SalvarCommand As ICommand
        Public Property MudarTipoCommand As ICommand
        Public Property CancelarCommand As ICommand

        ' ============================================================
        ' 3. CONSTRUTOR (Híbrido: Novo ou Editar)
        ' ============================================================


        Public Sub New(repo As IRepositoryManager, Optional itemEditar As Lancamentos = Nothing)
            ' Repassa o repositório para a classe base (BasePageViewModel)
            MyBase.New(repo)

            ListaCategorias = New ObservableCollection(Of Categorias)()

            ConfigurarComandos()

            ' Inicializa a tela (Modo Criação ou Edição)
            InicializarAsync(itemEditar).SafeFireAndForget()
        End Sub

        Private Sub ConfigurarComandos()
            MudarTipoCommand = New Command(Of String)(Sub(tipoStr)
                                                          TipoOperacao = Integer.Parse(tipoStr)
                                                      End Sub)

            SalvarCommand = New Command(Async Sub() Await SalvarDadosAsync())

            CancelarCommand = New Command(Async Sub()
                                              Await Application.Current.MainPage.Navigation.PopAsync()
                                          End Sub)
        End Sub

        ' ============================================================
        ' 4. LÓGICA DE CARREGAMENTO
        ' ============================================================

        Private Async Function InicializarAsync(item As Lancamentos) As Task
            IsBusy = True
            Try
                If item Is Nothing Then
                    ' --- MODO NOVO ---
                    TituloPagina = "Novo Lançamento"
                    _idEdicao = 0
                    Data = DateTime.Now
                    TipoOperacao = 0 ' Começa como Receita

                    Await CarregarCategoriasAsync()
                Else
                    ' --- MODO EDIÇÃO ---
                    TituloPagina = "Editar Lançamento"
                    _idEdicao = item.id_lancamento
                    Data = item.dt_lancamento
                    Observacao = item.te_observacoes

                    ' CORREÇÃO: Força a cultura PT-BR para garantir que use VÍRGULA
                    ' ToString("N2", _culturaBR) -> "1234,56"
                    'Valor = Math.Abs(item.vl_lancamento).ToString("N2", _culturaBR)

                    ' CORREÇÃO: F2 garante 2 casas decimais sem separador de milhar
                    Valor = Math.Abs(item.vl_lancamento).ToString("F2", _culturaBR)



                    'Valor = Math.Abs(item.vl_lancamento) ' Exibe positivo na tela

                    ' Determina o tipo
                    Dim tipo = 0
                    If item.Categoria IsNot Nothing Then
                        tipo = item.Categoria.cs_tipo
                    Else
                        tipo = If(item.vl_lancamento < 0, 1, 0)
                    End If

                    _tipoOperacao = tipo
                    OnPropertyChanged(NameOf(TipoOperacao))
                    OnPropertyChanged(NameOf(IsReceita))
                    OnPropertyChanged(NameOf(IsDespesa))

                    Await CarregarCategoriasAsync()

                    ' Seleciona a categoria correta no Picker
                    If item.id_categoria > 0 Then
                        Dim catEncontrada = ListaCategorias.FirstOrDefault(Function(c) c.id_categoria = item.id_categoria)
                        CategoriaSelecionada = catEncontrada
                    End If
                End If

            Finally
                IsBusy = False
            End Try
        End Function

        Private Async Function CarregarCategoriasAsync() As Task

            Dim mensagemErro As String = ""

            Try
                ' Usa 'Repo' da classe BasePageViewModel
                Dim todas = Await Repo.Categorias.GetTodasAsync()

                Dim filtradas = todas.Where(Function(c) c.cs_tipo = TipoOperacao).ToList()

                ListaCategorias.Clear()
                For Each cat In filtradas
                    ListaCategorias.Add(cat)
                Next

                ' Seleciona a primeira por padrão se nada estiver selecionado
                If CategoriaSelecionada Is Nothing AndAlso ListaCategorias.Any() Then
                    CategoriaSelecionada = ListaCategorias.First()
                End If
            Catch ex As Exception

                mensagemErro = ex.Message
            Finally
                'IsBusy = False
            End Try

            ' Exibe o erro fora do bloco protegido, se houver
            If Not String.IsNullOrEmpty(mensagemErro) Then
                Await Application.Current.MainPage.DisplayAlert("Erro", mensagemErro, "OK")
            End If


        End Function


        ' ============================================================
        ' 5. SALVAR (Insert ou Update)
        ' ============================================================

        Private Async Function SalvarDadosAsync() As Task
            If IsBusy Then Return


            ' CORREÇÃO: Validação e Conversão de String para Decimal
            Dim valorDecimal As Decimal = 0

            ' CORREÇÃO: Tenta converter usando PT-BR (espera vírgula como decimal)
            ' NumberStyles.Any permite espaços, símbolos de moeda, etc.
            Dim converteu = Decimal.TryParse(Valor, NumberStyles.Any, _culturaBR, valorDecimal)

            If String.IsNullOrEmpty(Valor) OrElse Not converteu OrElse valorDecimal <= 0 Then
                Await Application.Current.MainPage.DisplayAlert("Atenção", "Digite um valor numérico válido (Ex: 50,00).", "OK")
                Return
            End If

            If CategoriaSelecionada Is Nothing Then
                Await Application.Current.MainPage.DisplayAlert("Atenção", "Selecione uma categoria.", "OK")
                Return
            End If

            IsBusy = True

            ' Variável auxiliar para capturar erro sem usar Await no Catch
            Dim mensagemErro As String = String.Empty

            Try
                Dim model As New Lancamentos With {
                    .id_lancamento = _idEdicao,
                    .dt_lancamento = Data,
                    .te_observacoes = Observacao,
                    .id_categoria = CategoriaSelecionada.id_categoria
                }



                ' Ajusta o sinal matemático para o banco
                ' Usa a variável convertida (valorDecimal)
                If TipoOperacao = 1 Then
                    model.vl_lancamento = -Math.Abs(valorDecimal)
                Else
                    model.vl_lancamento = Math.Abs(valorDecimal)
                End If
                ' Salva no Repositório (Repo vem da BasePageViewModel)
                Await Repo.Lancamentos.SalvarAsync(model)

                ' Sucesso: Volta pra tela anterior
                Await Application.Current.MainPage.Navigation.PopAsync()

            Catch ex As Exception
                ' Captura a mensagem para exibir depois
                mensagemErro = ex.Message

                Debug.Print(ex.Source)

            Finally
                IsBusy = False
            End Try

            ' Exibe o erro fora do bloco protegido, se houver
            If Not String.IsNullOrEmpty(mensagemErro) Then
                Await Application.Current.MainPage.DisplayAlert("Erro", mensagemErro, "OK")
            End If

        End Function

    End Class


End Namespace