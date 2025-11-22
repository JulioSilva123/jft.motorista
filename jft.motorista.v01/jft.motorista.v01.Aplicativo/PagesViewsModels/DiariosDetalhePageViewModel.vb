
Imports System.Windows.Input
Imports jft.motorista.v01.Aplicativo.Common
Imports jft.motorista.v01.Core.Entities
Imports jft.motorista.v01.Infra.Interfaces
Imports Xamarin.Forms

Namespace PagesViewsModels

    ''' <summary>
    ''' ViewModel para exibir o relatório detalhado de um dia de trabalho.
    ''' Renomeado para DiariosDetalhePageViewModel para seguir o padrão de Páginas.
    ''' </summary>
    Public Class DiariosDetalhePageViewModel
        Inherits BasePageViewModel

        Private _model As Diarios

        ' ... (Propriedades existentes) ...

        Public ReadOnly Property TituloPagina As String
            Get
                Return $"Resumo: {_model.dt_diario:dd/MM}"
            End Get
        End Property

        Public ReadOnly Property DataExtenso As String
            Get
                Return _model.dt_diario.ToString("dddd, dd 'de' MMMM 'de' yyyy").ToUpper()
            End Get
        End Property

        Public ReadOnly Property VeiculoInfo As String
            Get
                Return $"Veículo ID: {_model.id_veiculo}"
            End Get
        End Property

        Public ReadOnly Property Horario As String
            Get
                Return $"{_model.hr_inicio:hh\:mm} às {_model.hr_fim:hh\:mm}"
            End Get
        End Property

        Public ReadOnly Property TempoTotal As String
            Get
                Dim t = _model.TempoTrabalhado
                Return $"{Math.Floor(t.TotalHours)}h {t.Minutes}min"
            End Get
        End Property

        ' --- Métricas de Quilometragem ---
        Public ReadOnly Property KmReal As String
            Get
                ' ATUALIZADO: Usa Trip
                Return $"{_model.KmRodadosCalculadoTrip:N0} km"
            End Get
        End Property

        Public ReadOnly Property KmTrip As String
            Get
                Return $"{Math.Abs(_model.nr_km_informado_trip):0.####} km"
            End Get
        End Property

        Public ReadOnly Property KmApp As String
            Get
                Return $"{Math.Abs(_model.nr_km_informado_app):0.####} km"
            End Get
        End Property

        Public ReadOnly Property KmMorta As String
            Get
                Return $"{_model.KmMorta:0.####} km"
            End Get
        End Property

        Public ReadOnly Property KmMortaCor As Color
            Get
                ' ATUALIZADO: Usa Trip para o cálculo de porcentagem
                If _model.KmRodadosCalculadoTrip > 0 AndAlso (_model.KmMorta / _model.KmRodadosCalculadoTrip) > 0.2 Then
                    Return Color.Red
                End If
                Return Color.Gray
            End Get
        End Property

        Public ReadOnly Property TemAbastecimento As Boolean
            Get
                Return _model.id_abastecimento > 0 OrElse _model.qt_litros_consumidos > 0
            End Get
        End Property

        Public ReadOnly Property LitrosConsumidos As String
            Get
                Return $"{_model.qt_litros_consumidos:0.###} L"
            End Get
        End Property

        Public ReadOnly Property PrecoPorLitro As String
            Get
                If _model.Abastecimento IsNot Nothing Then
                    Return $"{_model.Abastecimento.PrecoPorLitro:C3} / L"
                End If
                Return "-"
            End Get
        End Property

        Public ReadOnly Property CustoTotalCombustivel As String
            Get
                Return _model.CustoCombustivelReal.ToString("C")
            End Get
        End Property

        Public ReadOnly Property CustoPorKmReal As String
            Get
                Return _model.CustoPorKmReal.ToString("C") & " / km"
            End Get
        End Property

        Public ReadOnly Property KmPorLitro As String
            Get
                If _model.MediaKmInformadoPorLitro > 0 Then
                    Return $"{_model.MediaKmInformadoPorLitro:N2} km/L"
                End If
                Return "-"
            End Get
        End Property

        ' --- DADOS DO ABASTECIMENTO VINCULADO ---

        Public ReadOnly Property TemAbastecimentoVinculado As Boolean
            Get
                Return _model.Abastecimento IsNot Nothing
            End Get
        End Property

        Public ReadOnly Property AbsTipoCombustivel As String
            Get
                If _model.Abastecimento Is Nothing Then Return "-"
                Select Case _model.Abastecimento.cs_tipo_combustivel
                    Case 0 : Return "Gasolina"
                    Case 1 : Return "Etanol"
                    Case 2 : Return "Diesel"
                    Case 3 : Return "GNV"
                    Case Else : Return "Outro"
                End Select
            End Get
        End Property

        ' NOVO: Data do Abastecimento
        Public ReadOnly Property AbsData As String
            Get
                Return _model.Abastecimento?.dt_abastecimento.ToString("dd/MM/yy")
            End Get
        End Property

        Public ReadOnly Property AbsTotalPago As String
            Get
                Return _model.Abastecimento?.vl_total_pago.ToString("C")
            End Get
        End Property

        Public ReadOnly Property AbsLitros As String
            Get
                Return $"{_model.Abastecimento?.qt_litros:N2} L"
            End Get
        End Property

        Public ReadOnly Property AbsOdometro As String
            Get
                Return $"{_model.Abastecimento?.nr_km_odometro:N0} km"
            End Get
        End Property

        ' ============================================================
        ' COMANDOS
        ' ============================================================
        Public Property EditarCommand As ICommand
        Public Property VoltarCommand As ICommand

        Public Sub New(repo As IRepositoryManager, diario As Diarios)
            MyBase.New(repo)
            _model = diario

            EditarCommand = New Command(AddressOf Editar)
            VoltarCommand = New Command(Async Sub() Await Application.Current.MainPage.Navigation.PopAsync())

            If _model.Abastecimento Is Nothing AndAlso _model.id_abastecimento > 0 Then
                CarregarDadosExtrasAsync().SafeFireAndForget()
            End If
        End Sub

        Private Async Function CarregarDadosExtrasAsync() As Task
            _model.Abastecimento = Await Repo.Abastecimentos.GetItemAsync(_model.id_abastecimento)
            OnPropertyChanged("")
        End Function

        Private Async Sub Editar()
            MessagingCenter.Send(Me, "NavegarParaEdicao", _model)
        End Sub

    End Class

End Namespace