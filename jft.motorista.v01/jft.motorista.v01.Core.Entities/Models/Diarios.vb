
Imports SQLite

''' <summary>
''' Tabela para registrar o período de trabalho (Diário de Bordo).
''' O motorista registra o odômetro ao sair de casa e ao chegar.
''' Essencial para calcular a "KM Morta" (sem passageiro) e o rendimento diário.
''' </summary>
<Table("Diarios")>
Public Class Diarios

    <PrimaryKey, AutoIncrement>
    Public Property id_diario As Integer

    <Indexed>
    Public Property id_veiculo As Integer

    <Indexed>
    Public Property id_abastecimento As Integer

    Public Property dt_diario As DateTime

    Public Property hr_inicio As TimeSpan

    Public Property hr_fim As TimeSpan

    ' Odômetro Real
    Public Property nr_km_inicial As Decimal
    Public Property nr_km_final As Decimal

    ' Trip (Painel)
    Public Property nr_km_informado_trip As Decimal

    ' App (Uber/99)
    Public Property nr_km_informado_app As Decimal

    Public Property qt_litros_consumidos As Decimal

    Public Property te_observacoes As String

    ' --- Propriedades de Navegação ---

    <Ignore>
    Public Property Abastecimento As Abastecimentos

    ' --- Propriedades Calculadas (Métricas de Distância) ---

    ' 1. KM Calculado pelo Odômetro (Final - Inicial)
    <Ignore>
    Public ReadOnly Property KmRodadosCalculadoOdometro As Decimal
        Get
            If nr_km_final > nr_km_inicial Then
                Return nr_km_final - nr_km_inicial
            End If
            Return 0
        End Get
    End Property

    ' 2. KM do Trip (Painel)
    <Ignore>
    Public ReadOnly Property KmRodadosCalculadoTrip As Decimal
        Get
            Return nr_km_informado_trip
        End Get
    End Property

    ' 3. KM do App (Uber/99)
    <Ignore>
    Public ReadOnly Property KmRodadosCalculadoApp As Decimal
        Get
            Return nr_km_informado_app
        End Get
    End Property

    ' 4. KM REAL CONSOLIDADO
    ' Prioridade: Odômetro (se válido) > Trip.
    <Ignore>
    Public ReadOnly Property KmRealFisico As Decimal
        Get
            If KmRodadosCalculadoOdometro > 0 Then
                Return KmRodadosCalculadoOdometro
            End If
            Return KmRodadosCalculadoTrip
        End Get
    End Property

    ' --- Métricas Derivadas ---

    ' KM Morta
    <Ignore>
    Public ReadOnly Property KmMorta As Decimal
        Get
            Return KmRealFisico - KmRodadosCalculadoApp
        End Get
    End Property

    <Ignore>
    Public ReadOnly Property CustoCombustivelReal As Decimal
        Get
            If qt_litros_consumidos > 0 AndAlso Abastecimento IsNot Nothing Then
                Return qt_litros_consumidos * Abastecimento.PrecoPorLitro
            End If
            Return 0
        End Get
    End Property

    <Ignore>
    Public Property CustoAcumulado As Decimal

    ' Custo por KM Pago (Eficiência Financeira)
    <Ignore>
    Public ReadOnly Property CustoPorKmInformado As Decimal
        Get
            If nr_km_informado_app > 0 Then
                Return CustoCombustivelReal / nr_km_informado_app
            End If
            Return 0
        End Get
    End Property

    ' Custo por KM Físico (Eficiência Mecânica)
    <Ignore>
    Public ReadOnly Property CustoPorKmReal As Decimal
        Get
            If KmRealFisico > 0 Then
                Return CustoCombustivelReal / KmRealFisico
            End If
            Return 0
        End Get
    End Property

    ' Média Mecânica (Físico / Litros)
    <Ignore>
    Public ReadOnly Property MediaKmPorLitro As Decimal
        Get
            If qt_litros_consumidos > 0 Then
                Return KmRealFisico / qt_litros_consumidos
            End If
            Return 0
        End Get
    End Property

    ' Rendimento (Trip / Litros) - CORRIGIDO
    ' Agora calcula a média usando o TRIP, que é mais preciso que o App para consumo
    <Ignore>
    Public ReadOnly Property MediaKmInformadoPorLitro As Decimal
        Get
            If qt_litros_consumidos > 0 Then
                ' ATUALIZADO: Usa Trip
                Return nr_km_informado_trip / qt_litros_consumidos
            End If
            Return 0
        End Get
    End Property

    <Ignore>
    Public ReadOnly Property TempoTrabalhado As TimeSpan
        Get
            Return hr_fim - hr_inicio
        End Get
    End Property

End Class