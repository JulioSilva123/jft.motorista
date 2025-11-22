

Imports System.Collections.ObjectModel
Imports System.Windows.Input
Imports jft.motorista.v01.Aplicativo.Common
Imports jft.motorista.v01.Aplicativo.Mappers
Imports jft.motorista.v01.Core.EntitiesViews
Imports jft.motorista.v01.Core.EntitiesViews.Common
Imports jft.motorista.v01.Infra.Interfaces
Imports Xamarin.Forms

Namespace PagesViewsModels


    ''' <summary>
    ''' ViewModel principal do Dashboard.
    ''' Responsável por orquestrar a visualização do Saldo, Extrato e Navegação Temporal.
    ''' </summary>
    Public Class DashboardPageViewModel
        Inherits BasePageViewModel

        ' Controle interno da data visualizada (Mês/Ano)
        Private _dataRef As DateTime

        ' ============================================================
        ' 1. PROPRIEDADES VISUAIS (Data Binding)
        ' ============================================================

        ' Lista de itens formatados para a tela (Wrappers)
        Private _extrato As ObservableCollection(Of LancamentosViewModel)
        Public Property Extrato As ObservableCollection(Of LancamentosViewModel)
            Get
                Return _extrato
            End Get
            Set(value As ObservableCollection(Of LancamentosViewModel))
                SetProperty(_extrato, value)
            End Set
        End Property

        ' Título do Cabeçalho (Ex: "NOVEMBRO 2025")
        Private _mesTexto As String
        Public Property MesTexto As String
            Get
                Return _mesTexto
            End Get
            Set(value As String)
                SetProperty(_mesTexto, value)
            End Set
        End Property

        ' Saldo Principal (Acumulado Total)
        Private _saldoAtual As Decimal
        Public Property SaldoAtual As Decimal
            Get
                Return _saldoAtual
            End Get
            Set(value As Decimal)
                SetProperty(_saldoAtual, value)
            End Set
        End Property

        ' Saldo do Dia (Destaque para meta diária)
        Private _saldoDia As Decimal
        Public Property SaldoDia As Decimal
            Get
                Return _saldoDia
            End Get
            Set(value As Decimal)
                SetProperty(_saldoDia, value)
            End Set
        End Property

        ' Resumo: Total de Entradas do Mês
        Private _totalEntradas As Decimal
        Public Property TotalEntradas As Decimal
            Get
                Return _totalEntradas
            End Get
            Set(value As Decimal)
                SetProperty(_totalEntradas, value)
            End Set
        End Property

        ' Resumo: Total de Saídas do Mês
        Private _totalSaidas As Decimal
        Public Property TotalSaidas As Decimal
            Get
                Return _totalSaidas
            End Get
            Set(value As Decimal)
                SetProperty(_totalSaidas, value)
            End Set
        End Property

        ' ============================================================
        ' 2. COMANDOS (Ações)
        ' ============================================================

        Public Property ProximoMesCommand As ICommand
        Public Property MesAnteriorCommand As ICommand
        Public Property NovoLancamentoCommand As ICommand
        Public Property AtualizarCommand As ICommand

        ' ============================================================
        ' 3. CONSTRUTOR
        ' ============================================================

        ''' <summary>
        ''' Construtor com Injeção de Dependência.
        ''' Recebe o Repositório (Interface) e repassa para a classe base.
        ''' </summary>
        Public Sub New(repo As IRepositoryManager)
            ' Repassa o repositório para a classe base (BasePageViewModel)
            MyBase.New(repo)

            ' Inicializa propriedades
            Extrato = New ObservableCollection(Of LancamentosViewModel)()
            _dataRef = DateTime.Now

            Debug.Print(_dataRef)

            ConfigurarComandos()

            ' Escuta mensagens de exclusão vindas dos itens da lista para atualizar o saldo
            MessagingCenter.Subscribe(Of LancamentosViewModel)(Me, "LancamentoDeletado", AddressOf AoDeletarItem)

            ' Carrega os dados iniciais sem travar o construtor
            CarregarDadosAsync().SafeFireAndForget()
        End Sub

        Private Sub ConfigurarComandos()

            ProximoMesCommand = New Command(Async Sub()
                                                _dataRef = _dataRef.AddMonths(1)
                                                Await CarregarDadosAsync()
                                            End Sub)

            MesAnteriorCommand = New Command(Async Sub()
                                                 _dataRef = _dataRef.AddMonths(-1)
                                                 Await CarregarDadosAsync()
                                             End Sub)

            ' Comando vital para o OnAppearing da View (Refresh ao voltar da tela de cadastro)
            AtualizarCommand = New Command(Async Sub()
                                               Await CarregarDadosAsync()
                                           End Sub)

            NovoLancamentoCommand = New Command(Async Sub()
                                                    ' A navegação geralmente é tratada pela View ou NavigationService
                                                End Sub)

        End Sub

        ' ============================================================
        ' 4. LÓGICA PRINCIPAL (O Maestro)
        ' ============================================================

        Private Async Function CarregarDadosAsync() As Task
            If IsBusy Then Return
            IsBusy = True

            Try
                ' 1. Atualiza Título
                MesTexto = _dataRef.ToString("MMMM yyyy").ToUpper()

                ' 2. Busca Dados no Banco (Usando a propriedade 'Repo' da base)
                Dim models = Await Repo.Lancamentos.GetExtratoDoMesAsync(_dataRef.Month, _dataRef.Year)

                ' 3. Converte Model -> ViewModel (Mapper) e atualiza a lista
                Extrato = models.ToViewModels()

                ' 4. Calcula Totais do Mês (para os Cards) e Saldo do Dia
                Dim entradas As Decimal = 0
                Dim saidas As Decimal = 0

                Dim hoje = DateTime.Today
                Dim somaDia As Decimal = 0

                ' Verifica se estamos visualizando o mês/ano atual
                Dim ehMesAtual = (_dataRef.Month = hoje.Month AndAlso _dataRef.Year = hoje.Year)

                For Each item In models
                    ' Verifica se é despesa (pela categoria ou valor negativo)
                    Dim ehDespesa As Boolean = False

                    If item.Categoria IsNot Nothing Then
                        ehDespesa = (item.Categoria.cs_tipo = 1)
                    Else
                        ehDespesa = (item.vl_lancamento < 0)
                    End If

                    ' Normaliza o valor (Positivo Absoluto)
                    Dim valorAbsoluto = Math.Abs(item.vl_lancamento)

                    If ehDespesa Then
                        saidas += valorAbsoluto
                    Else
                        entradas += valorAbsoluto
                    End If

                    ' Cálculo do Saldo do Dia (apenas se for o mês atual e o dia bater)
                    ' CORREÇÃO: Usamos a mesma lógica de sinal (Receita soma, Despesa subtrai)
                    ' para garantir que o valor esteja certo mesmo se o banco tiver inconsistência.
                    If ehMesAtual AndAlso item.dt_lancamento.Day = hoje.Day Then
                        If ehDespesa Then
                            somaDia -= valorAbsoluto
                        Else
                            somaDia += valorAbsoluto
                        End If
                    End If
                Next

                TotalEntradas = entradas
                TotalSaidas = saidas
                SaldoDia = If(ehMesAtual, somaDia, 0D)

                ' 5. Define o Saldo Atual (Acumulado Total)
                If models.Any() Then
                    ' Se tem lançamentos, o saldo atual é o do topo da lista (mais recente)
                    SaldoAtual = models.First().SaldoAposLancamento
                Else
                    ' Se lista vazia, busca o saldo FECHADO do mês anterior (Checkpoint)
                    Dim mAnt = If(_dataRef.Month = 1, 12, _dataRef.Month - 1)
                    Dim yAnt = If(_dataRef.Month = 1, _dataRef.Year - 1, _dataRef.Year)

                    Dim checkpoint = Await Repo.SaldosMensais.GetPorMesAsync(mAnt, yAnt)

                    SaldoAtual = If(checkpoint IsNot Nothing, checkpoint.vl_saldofinal, 0D)
                End If

            Catch ex As Exception
                System.Diagnostics.Debug.WriteLine($"Erro no Dashboard: {ex.Message}")
            Finally
                IsBusy = False
            End Try
        End Function

        ' Handler do evento de delete (Recarrega tudo para atualizar saldos)
        Private Sub AoDeletarItem(sender As LancamentosViewModel)
            CarregarDadosAsync().SafeFireAndForget()
        End Sub

    End Class


    ' ============================================================
    ' 6. HELPER PARA ASYNC NO CONSTRUTOR
    ' ============================================================
    ' Coloque isso num arquivo separado "TaskExtensions.vb" ou deixe aqui no final

    Public Module TaskExtensions
        <System.Runtime.CompilerServices.Extension>
        Public Sub SafeFireAndForget(ByVal task As Task)
            task.ContinueWith(Sub(t)
                                  If t.IsFaulted Then
                                      System.Diagnostics.Debug.WriteLine($"Erro Async Background: {t.Exception}")
                                  End If
                              End Sub)
        End Sub
    End Module

End Namespace