
Imports System.Globalization
Imports System.Windows.Input
Imports jft.motorista.v01.Aplicativo.Common
Imports jft.motorista.v01.Core.Entities
Imports jft.motorista.v01.Infra.Interfaces
Imports Xamarin.Forms

Namespace PagesViewsModels

    ''' <summary>
    ''' ViewModel para Cadastro de Abastecimentos.
    ''' Renomeada para usar o plural (Nome da Tabela).
    ''' </summary>
    Public Class AbastecimentosPageViewModel
        Inherits BasePageViewModel


        Private ReadOnly _culturaBR As New CultureInfo("pt-BR")

        ' Estado
        Private _veiculoAtual As Veiculos
        Private _idEdicao As Integer = 0
        Private _idLancamentoVinculado As Integer = 0

        ' ============================================================
        ' PROPRIEDADES
        ' ============================================================

        Private _tituloPagina As String = "Novo Abastecimento"
        Public Property TituloPagina As String
            Get
                Return _tituloPagina
            End Get
            Set(value As String)
                SetProperty(_tituloPagina, value)
            End Set
        End Property

        ' Controla a visibilidade do Switch "Gerar Despesa"
        ' Só mostramos ao criar. Na edição, a atualização é automática se já existir vínculo.
        Private _isNovo As Boolean = True
        Public Property IsNovo As Boolean
            Get
                Return _isNovo
            End Get
            Set(value As Boolean)
                SetProperty(_isNovo, value)
            End Set
        End Property

        Private _nomeVeiculo As String = "Carregando..."
        Public Property NomeVeiculo As String
            Get
                Return _nomeVeiculo
            End Get
            Set(value As String)
                SetProperty(_nomeVeiculo, value)
            End Set
        End Property

        Private _data As DateTime = DateTime.Now
        Public Property Data As DateTime
            Get
                Return _data
            End Get
            Set(value As DateTime)
                SetProperty(_data, value)
            End Set
        End Property

        Private _kmOdometro As String
        Public Property KmOdometro As String
            Get
                Return _kmOdometro
            End Get
            Set(value As String)
                SetProperty(_kmOdometro, value)
            End Set
        End Property

        Private _valorTotal As String
        Public Property ValorTotal As String
            Get
                Return _valorTotal
            End Get
            Set(value As String)
                If SetProperty(_valorTotal, value) Then RecalcularPrecoPorLitro()
            End Set
        End Property

        Private _litros As String
        Public Property Litros As String
            Get
                Return _litros
            End Get
            Set(value As String)
                If SetProperty(_litros, value) Then RecalcularPrecoPorLitro()
            End Set
        End Property

        Private _precoPorLitroDisplay As String = "R$ 0,00 / L"
        Public Property PrecoPorLitroDisplay As String
            Get
                Return _precoPorLitroDisplay
            End Get
            Set(value As String)
                SetProperty(_precoPorLitroDisplay, value)
            End Set
        End Property

        Private _tipoCombustivel As Integer = 0
        Public Property TipoCombustivel As Integer
            Get
                Return _tipoCombustivel
            End Get
            Set(value As Integer)
                SetProperty(_tipoCombustivel, value)
            End Set
        End Property

        Private _gerarDespesa As Boolean = True
        Public Property GerarDespesa As Boolean
            Get
                Return _gerarDespesa
            End Get
            Set(value As Boolean)
                SetProperty(_gerarDespesa, value)
            End Set
        End Property

        ' ============================================================
        ' COMANDOS
        ' ============================================================
        Public Property SalvarCommand As ICommand
        Public Property CancelarCommand As ICommand

        ' ============================================================
        ' CONSTRUTOR (Híbrido Novo/Editar)
        ' ============================================================
        Public Sub New(repo As IRepositoryManager, Optional itemEditar As Abastecimentos = Nothing)
            MyBase.New(repo)

            ConfigurarComandos()
            InicializarAsync(itemEditar).SafeFireAndForget()
        End Sub

        Private Sub ConfigurarComandos()
            SalvarCommand = New Command(Async Sub() Await SalvarAsync())
            CancelarCommand = New Command(Async Sub() Await Application.Current.MainPage.Navigation.PopAsync())
        End Sub

        ' ============================================================
        ' INICIALIZAÇÃO
        ' ============================================================
        Private Async Function InicializarAsync(item As Abastecimentos) As Task
            IsBusy = True
            Try
                If item Is Nothing Then
                    ' --- MODO NOVO ---
                    IsNovo = True
                    TituloPagina = "Novo Abastecimento"
                    _idEdicao = 0
                    _idLancamentoVinculado = 0

                    ' Carrega o veículo ativo padrão
                    Await CarregarVeiculoAtivoAsync()
                Else
                    ' --- MODO EDIÇÃO ---
                    IsNovo = False
                    TituloPagina = "Editar Abastecimento"
                    _idEdicao = item.id_abastecimento
                    _idLancamentoVinculado = item.id_lancamento

                    ' Carrega dados do registro
                    Data = item.dt_abastecimento
                    TipoCombustivel = item.cs_tipo_combustivel

                    ' CORREÇÃO: Use F2 para dinheiro (2 casas fixas) e 0.#### para KMs/Litros
                    ValorTotal = item.vl_total_pago.ToString("F2", _culturaBR)

                    ' Litros com até 3 casas exatas (sem milhar)
                    Litros = item.qt_litros.ToString("0.###", _culturaBR)

                    ' KM sem arredondar e sem milhar (para facilitar edição)
                    KmOdometro = item.nr_km_odometro.ToString("0.####", _culturaBR)


                    ' Carrega o veículo original deste abastecimento
                    Dim v = Await Repo.Veiculos.GetItemAsync(item.id_veiculo)
                    If v IsNot Nothing Then
                        _veiculoAtual = v
                        NomeVeiculo = $"{v.nm_modelo} ({v.nm_placa})"
                    Else
                        NomeVeiculo = "Veículo Desconhecido"
                    End If
                End If
            Finally
                IsBusy = False
            End Try
        End Function

        Private Async Function CarregarVeiculoAtivoAsync() As Task
            _veiculoAtual = Await Repo.Veiculos.GetVeiculoAtivoAsync()

            If _veiculoAtual Is Nothing Then
                NomeVeiculo = "Nenhum Veículo Ativo!"
                Await Application.Current.MainPage.DisplayAlert("Atenção", "Ative um veículo antes de abastecer.", "OK")
                Await Application.Current.MainPage.Navigation.PopAsync()
            Else
                NomeVeiculo = $"{_veiculoAtual.nm_modelo} ({_veiculoAtual.nm_placa})"
                KmOdometro = _veiculoAtual.nr_km_atual.ToString("N0", _culturaBR)
            End If
        End Function

        Private Sub RecalcularPrecoPorLitro()
            Dim val As Decimal = 0
            Dim lit As Decimal = 0
            Dim okVal = Decimal.TryParse(ValorTotal, NumberStyles.Any, _culturaBR, val)
            Dim okLit = Decimal.TryParse(Litros, NumberStyles.Any, _culturaBR, lit)

            If okVal AndAlso okLit AndAlso lit > 0 Then
                PrecoPorLitroDisplay = $"{(val / lit).ToString("C3", _culturaBR)} / L"
            Else
                PrecoPorLitroDisplay = "R$ 0,00 / L"
            End If
        End Sub

        ' ============================================================
        ' SALVAR
        ' ============================================================
        Private Async Function SalvarAsync() As Task
            If IsBusy Then Return

            Dim valDec, litDec, kmDec As Decimal
            Decimal.TryParse(ValorTotal, NumberStyles.Any, _culturaBR, valDec)
            Decimal.TryParse(Litros, NumberStyles.Any, _culturaBR, litDec)
            Decimal.TryParse(KmOdometro, NumberStyles.Any, _culturaBR, kmDec)

            If valDec <= 0 OrElse litDec <= 0 OrElse kmDec <= 0 Then
                Await Application.Current.MainPage.DisplayAlert("Erro", "Verifique os valores.", "OK")
                Return
            End If

            If _veiculoAtual IsNot Nothing AndAlso kmDec < _veiculoAtual.nr_km_atual AndAlso _idEdicao = 0 Then
                ' Validação de KM menor apenas se for NOVO registro. 
                ' Se for edição antiga, o KM pode ser menor que o atual do carro hoje.
                If Not Await Application.Current.MainPage.DisplayAlert("Atenção", $"KM informado ({kmDec}) é menor que o atual do carro ({_veiculoAtual.nr_km_atual}). Confirmar?", "Sim", "Não") Then Return
            End If

            IsBusy = True
            Dim mensagemErro As String = String.Empty

            Try
                ' 1. Lógica de Lançamento Financeiro
                Dim idLancamentoParaSalvar As Integer = _idLancamentoVinculado

                If _idEdicao = 0 Then
                    ' INSERT: Cria financeiro se o usuário pediu
                    If GerarDespesa Then
                        idLancamentoParaSalvar = Await GerarLancamentoFinanceiroAsync(valDec, litDec, Data)
                    End If
                Else
                    ' UPDATE: Se já existe vínculo, atualiza o financeiro também
                    If _idLancamentoVinculado > 0 Then
                        Await AtualizarLancamentoFinanceiroAsync(_idLancamentoVinculado, valDec, litDec, Data)
                    End If
                End If

                ' 2. Salva o Abastecimento
                Dim model As New Abastecimentos With {
                    .id_abastecimento = _idEdicao,
                    .id_veiculo = _veiculoAtual.id_veiculo,
                    .id_lancamento = idLancamentoParaSalvar,
                    .dt_abastecimento = Data,
                    .vl_total_pago = valDec,
                    .qt_litros = litDec,
                    .nr_km_odometro = kmDec,
                    .cs_tipo_combustivel = TipoCombustivel
                }

                Await Repo.Abastecimentos.SalvarAsync(model)
                Await Application.Current.MainPage.Navigation.PopAsync()

            Catch ex As Exception
                mensagemErro = ex.Message
            Finally
                IsBusy = False
            End Try

            If Not String.IsNullOrEmpty(mensagemErro) Then
                Await Application.Current.MainPage.DisplayAlert("Erro", mensagemErro, "OK")
            End If
        End Function

        ' --- Helpers Financeiros ---

        Private Async Function GerarLancamentoFinanceiroAsync(valor As Decimal, litros As Decimal, data As DateTime) As Task(Of Integer)
            Dim catCombustivel = (Await Repo.Categorias.GetTodasAsync()).FirstOrDefault(Function(c) c.nm_categoria.ToLower().Contains("combustível") OrElse c.nm_categoria.ToLower().Contains("gasolina") OrElse c.cs_tipo = 1)

            If catCombustivel IsNot Nothing Then
                Dim novo As New Lancamentos With {
                    .dt_lancamento = data,
                    .vl_lancamento = -Math.Abs(valor),
                    .id_categoria = catCombustivel.id_categoria,
                    .te_observacoes = $"Abastecimento Automático ({litros:N1} L)"
                }
                Await Repo.Lancamentos.SalvarAsync(novo)
                Return novo.id_lancamento
            End If
            Return 0
        End Function

        Private Async Function AtualizarLancamentoFinanceiroAsync(idLancamento As Integer, valor As Decimal, litros As Decimal, data As DateTime) As Task
            Dim lancamento = Await Repo.Lancamentos.GetItemAsync(idLancamento)
            If lancamento IsNot Nothing Then
                lancamento.dt_lancamento = data
                lancamento.vl_lancamento = -Math.Abs(valor)
                lancamento.te_observacoes = $"Abastecimento Automático ({litros:N1} L)"
                Await Repo.Lancamentos.SalvarAsync(lancamento)
            End If
        End Function

    End Class

End Namespace