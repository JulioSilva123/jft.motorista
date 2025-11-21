
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

    ' KM "Digitada" (App Uber/99)
    Public Property nr_km_informado As Decimal

    ' Litros gastos no dia
    Public Property qt_litros_consumidos As Decimal

    Public Property te_observacoes As String

    ' --- Propriedades de Navegação ---

    <Ignore>
    Public Property Abastecimento As Abastecimentos

    ' --- Propriedades Calculadas (Métricas) ---

    ' 1. KM Real (Odômetro)
    <Ignore>
    Public ReadOnly Property KmRodadosCalculado As Decimal
        Get
            If nr_km_final > nr_km_inicial Then
                Return nr_km_final - nr_km_inicial
            End If
            Return 0
        End Get
    End Property

    ' 2. KM Morta
    <Ignore>
    Public ReadOnly Property KmMorta As Decimal
        Get
            Return KmRodadosCalculado - nr_km_informado
        End Get
    End Property

    ' 3. Custo do Dia (Unitário)
    <Ignore>
    Public ReadOnly Property CustoCombustivelReal As Decimal
        Get
            If qt_litros_consumidos > 0 AndAlso Abastecimento IsNot Nothing Then
                Return qt_litros_consumidos * Abastecimento.PrecoPorLitro
            End If
            Return 0
        End Get
    End Property

    ' 4. Custo Acumulado (Extrato Linha a Linha) - NOVO
    ' Mostra o total gasto no mês ATÉ este dia
    <Ignore>
    Public Property CustoAcumulado As Decimal

    ' 5. Custo por KM (FINANCEIRO)
    <Ignore>
    Public ReadOnly Property CustoPorKmInformado As Decimal
        Get
            If nr_km_informado > 0 Then
                Return CustoCombustivelReal / nr_km_informado
            End If
            Return 0
        End Get
    End Property

    ' 6. Custo por KM (REAL/MECÂNICO)
    <Ignore>
    Public ReadOnly Property CustoPorKmReal As Decimal
        Get
            If KmRodadosCalculado > 0 Then
                Return CustoCombustivelReal / KmRodadosCalculado
            End If
            Return 0
        End Get
    End Property

    <Ignore>
    Public ReadOnly Property MediaKmPorLitro As Decimal
        Get
            If qt_litros_consumidos > 0 Then
                Return KmRodadosCalculado / qt_litros_consumidos
            End If
            Return 0
        End Get
    End Property

    <Ignore>
    Public ReadOnly Property MediaKmInformadoPorLitro As Decimal
        Get
            If qt_litros_consumidos > 0 Then
                Return nr_km_informado / qt_litros_consumidos
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