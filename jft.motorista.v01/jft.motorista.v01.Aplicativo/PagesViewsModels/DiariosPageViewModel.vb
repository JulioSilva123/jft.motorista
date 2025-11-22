
Imports System.Collections.ObjectModel
Imports System.Globalization
Imports System.Windows.Input
Imports jft.motorista.v01.Aplicativo.Common
Imports jft.motorista.v01.Core.Entities
Imports jft.motorista.v01.Infra.Interfaces
Imports Xamarin.Forms

Namespace PagesViewsModels



    ''' <summary>
    ''' ViewModel para Cadastro e Edição de Diários de Bordo (Jornadas).
    ''' </summary>
    Public Class DiariosPageViewModel
        Inherits BasePageViewModel

        Private ReadOnly _culturaBR As New CultureInfo("pt-BR")
        Private _idEdicao As Integer = 0
        Private _veiculoAtual As Veiculos

        ' ... (Propriedades Visuais e Comandos permanecem iguais) ...

        Private _tituloPagina As String = "Novo Diário"
        Public Property TituloPagina As String
            Get
                Return _tituloPagina
            End Get
            Set(value As String)
                SetProperty(_tituloPagina, value)
            End Set
        End Property

        Private _data As DateTime = DateTime.Now
        Public Property Data As DateTime
            Get
                Return _data
            End Get
            Set(value As DateTime)
                If SetProperty(_data, value) Then
                    CarregarAbastecimentosDoDiaAsync().SafeFireAndForget()
                End If
            End Set
        End Property

        Private _horaInicio As TimeSpan = New TimeSpan(8, 0, 0)
        Public Property HoraInicio As TimeSpan
            Get
                Return _horaInicio
            End Get
            Set(value As TimeSpan)
                SetProperty(_horaInicio, value)
                RecalcularTempo()
            End Set
        End Property

        Private _horaFim As TimeSpan = DateTime.Now.TimeOfDay
        Public Property HoraFim As TimeSpan
            Get
                Return _horaFim
            End Get
            Set(value As TimeSpan)
                SetProperty(_horaFim, value)
                RecalcularTempo()
            End Set
        End Property

        Private _kmInicial As String
        Public Property KmInicial As String
            Get
                Return _kmInicial
            End Get
            Set(value As String)
                If SetProperty(_kmInicial, value) Then RecalcularKmRodado()
            End Set
        End Property

        Private _kmFinal As String
        Public Property KmFinal As String
            Get
                Return _kmFinal
            End Get
            Set(value As String)
                If SetProperty(_kmFinal, value) Then RecalcularKmRodado()
            End Set
        End Property

        Private _kmTrip As String
        Public Property KmTrip As String
            Get
                Return _kmTrip
            End Get
            Set(value As String)
                SetProperty(_kmTrip, value)
            End Set
        End Property

        ' NOVO: Propriedade para o KM do Aplicativo
        Private _kmApp As String
        Public Property KmApp As String
            Get
                Return _kmApp
            End Get
            Set(value As String)
                SetProperty(_kmApp, value)
            End Set
        End Property

        Private _litrosConsumidos As String
        Public Property LitrosConsumidos As String
            Get
                Return _litrosConsumidos
            End Get
            Set(value As String)
                SetProperty(_litrosConsumidos, value)
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

        Public Property ListaAbastecimentos As ObservableCollection(Of Abastecimentos)

        Private _abastecimentoSelecionado As Abastecimentos
        Public Property AbastecimentoSelecionado As Abastecimentos
            Get
                Return _abastecimentoSelecionado
            End Get
            Set(value As Abastecimentos)
                SetProperty(_abastecimentoSelecionado, value)
            End Set
        End Property

        Private _totalRodadoDisplay As String = "0 km"
        Public Property TotalRodadoDisplay As String
            Get
                Return _totalRodadoDisplay
            End Get
            Set(value As String)
                SetProperty(_totalRodadoDisplay, value)
            End Set
        End Property

        Private _tempoDisplay As String = "0h 00min"
        Public Property TempoDisplay As String
            Get
                Return _tempoDisplay
            End Get
            Set(value As String)
                SetProperty(_tempoDisplay, value)
            End Set
        End Property

        Public Property SalvarCommand As ICommand
        Public Property CancelarCommand As ICommand

        Public Sub New(repo As IRepositoryManager, Optional itemEditar As Diarios = Nothing)
            MyBase.New(repo)
            ListaAbastecimentos = New ObservableCollection(Of Abastecimentos)()
            ConfigurarComandos()
            InicializarAsync(itemEditar).SafeFireAndForget()
        End Sub

        Private Sub ConfigurarComandos()
            SalvarCommand = New Command(Async Sub() Await SalvarAsync())
            CancelarCommand = New Command(Async Sub() Await Application.Current.MainPage.Navigation.PopAsync())
        End Sub

        Private Async Function InicializarAsync(item As Diarios) As Task
            IsBusy = True
            Try
                _veiculoAtual = Await Repo.Veiculos.GetVeiculoAtivoAsync()

                If item Is Nothing Then
                    ' --- NOVO ---
                    TituloPagina = "Novo Diário"
                    _idEdicao = 0
                    If _veiculoAtual IsNot Nothing Then
                        KmInicial = _veiculoAtual.nr_km_atual.ToString("0.####", _culturaBR)
                    End If
                    Await CarregarAbastecimentosDoDiaAsync()
                Else
                    ' --- EDIÇÃO ---
                    TituloPagina = "Editar Diário"
                    _idEdicao = item.id_diario
                    Data = item.dt_diario
                    HoraInicio = item.hr_inicio
                    HoraFim = item.hr_fim
                    Observacao = item.te_observacoes

                    KmInicial = If(item.nr_km_inicial > 0, item.nr_km_inicial.ToString("0.####", _culturaBR), String.Empty)
                    KmFinal = If(item.nr_km_final > 0, item.nr_km_final.ToString("0.####", _culturaBR), String.Empty)

                    ' Carrega Trip
                    KmTrip = If(item.nr_km_informado_trip > 0, item.nr_km_informado_trip.ToString("0.####", _culturaBR), String.Empty)

                    ' NOVO: Carrega App
                    KmApp = If(item.nr_km_informado_app > 0, item.nr_km_informado_app.ToString("0.####", _culturaBR), String.Empty)

                    LitrosConsumidos = If(item.qt_litros_consumidos > 0, item.qt_litros_consumidos.ToString("0.###", _culturaBR), String.Empty)

                    If _veiculoAtual Is Nothing OrElse _veiculoAtual.id_veiculo <> item.id_veiculo Then
                        _veiculoAtual = Await Repo.Veiculos.GetItemAsync(item.id_veiculo)
                    End If

                    Await CarregarAbastecimentosDoDiaAsync()
                    If item.id_abastecimento > 0 Then
                        AbastecimentoSelecionado = ListaAbastecimentos.FirstOrDefault(Function(a) a.id_abastecimento = item.id_abastecimento)
                    End If
                End If
                RecalcularKmRodado()
                RecalcularTempo()
            Finally
                IsBusy = False
            End Try
        End Function
        Private Async Function CarregarAbastecimentosDoDiaAsync() As Task
            If _veiculoAtual Is Nothing Then Return
            Dim lista = Await Repo.Abastecimentos.GetPorDataVeiculoAsync(Data, _veiculoAtual.id_veiculo)
            ListaAbastecimentos.Clear()
            For Each abs In lista
                ListaAbastecimentos.Add(abs)
            Next
        End Function

        Private Sub RecalcularKmRodado()
            Dim ini As Decimal = 0
            Dim fim As Decimal = 0
            Decimal.TryParse(KmInicial, NumberStyles.Any, _culturaBR, ini)
            Decimal.TryParse(KmFinal, NumberStyles.Any, _culturaBR, fim)

            If fim > ini AndAlso ini > 0 Then
                TotalRodadoDisplay = $"{fim - ini:0.####} km"
            Else
                TotalRodadoDisplay = "0 km"
            End If
        End Sub

        Private Sub RecalcularTempo()
            Dim diff = HoraFim - HoraInicio
            If diff.TotalMinutes > 0 Then
                TempoDisplay = $"{Math.Floor(diff.TotalHours)}h {diff.Minutes}min"
            Else
                TempoDisplay = "0h 00min"
            End If
        End Sub

        Private Async Function SalvarAsync() As Task
            If IsBusy Then Return

            If _veiculoAtual Is Nothing Then
                Await Application.Current.MainPage.DisplayAlert("Erro", "Nenhum veículo ativo.", "OK")
                Return
            End If

            Dim kmIni, kmFim, kmTripVal, kmAppVal, litCon As Decimal
            Decimal.TryParse(KmInicial, NumberStyles.Any, _culturaBR, kmIni)
            Decimal.TryParse(KmFinal, NumberStyles.Any, _culturaBR, kmFim)
            Decimal.TryParse(KmTrip, NumberStyles.Any, _culturaBR, kmTripVal)
            Decimal.TryParse(KmApp, NumberStyles.Any, _culturaBR, kmAppVal)
            Decimal.TryParse(LitrosConsumidos, NumberStyles.Any, _culturaBR, litCon)

            ' Validação: Pelo menos uma info de distância deve existir
            If kmTripVal <= 0 AndAlso kmAppVal <= 0 AndAlso (kmIni <= 0 OrElse kmFim <= 0) Then
                Await Application.Current.MainPage.DisplayAlert("Atenção", "Informe a KM do Trip, do App ou o Odômetro.", "OK")
                Return
            End If

            IsBusy = True
            Dim mensagemErro As String = String.Empty

            Try
                Dim idAbs As Integer = 0
                If AbastecimentoSelecionado IsNot Nothing Then
                    idAbs = AbastecimentoSelecionado.id_abastecimento
                End If

                Dim model As New Diarios With {
                    .id_diario = _idEdicao,
                    .id_veiculo = _veiculoAtual.id_veiculo,
                    .id_abastecimento = idAbs,
                    .dt_diario = Data,
                    .hr_inicio = HoraInicio,
                    .hr_fim = HoraFim,
                    .nr_km_inicial = kmIni,
                    .nr_km_final = kmFim,
                    .nr_km_informado_trip = kmTripVal,
                    .nr_km_informado_app = kmAppVal, ' NOVO
                    .qt_litros_consumidos = litCon,
                    .te_observacoes = Observacao
                }

                Await Repo.Diarios.SalvarAsync(model)
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

    End Class




End Namespace