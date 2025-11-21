

Imports SQLite



''' <summary>
''' Tabela específica para registrar as paradas no posto de gasolina.
''' </summary>
<Table("Abastecimentos")>
Public Class Abastecimentos


    <PrimaryKey, AutoIncrement>
    Public Property id_abastecimento As Integer

    <Indexed>
    Public Property id_veiculo As Integer

    ' Vínculo com o financeiro (Perfeito!)
    <Indexed>
    Public Property id_lancamento As Integer

    Public Property dt_abastecimento As DateTime

    Public Property vl_total_pago As Decimal

    Public Property qt_litros As Decimal

    Public Property nr_km_odometro As Decimal

    Public Property cs_tipo_combustivel As Integer

    ' --- Propriedades de Navegação (Hydration) ---

    <Ignore>
    Public Property Veiculo As Veiculos

    ' NOVO: Permite acessar o objeto financeiro vinculado diretamente
    <Ignore>
    Public Property Lancamento As Lancamentos

    ' --- Propriedades Calculadas ---

    <Ignore>
    Public ReadOnly Property PrecoPorLitro As Decimal
        Get
            If qt_litros <= 0 Then Return 0
            Return vl_total_pago / qt_litros
        End Get
    End Property

End Class
