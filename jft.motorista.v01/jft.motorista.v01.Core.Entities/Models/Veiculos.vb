Imports SQLite


''' <summary>
''' Representa o carro do motorista.
''' Fundamental para cálculos de consumo e alertas de manutenção.
''' </summary>
<Table("Veiculos")>
Public Class Veiculos

    <PrimaryKey, AutoIncrement>
    Public Property id_veiculo As Integer

    Public Property nm_modelo As String

    Public Property nm_placa As String

    ' ATUALIZADO: Odômetro atual (Número)
    Public Property nr_km_atual As Decimal

    ' ATUALIZADO: Média de consumo (Km/L)
    Public Property nr_media_consumo As Decimal

    Public Property fl_ativo As Boolean

    ' --- Propriedades Auxiliares ---

    <Ignore>
    Public ReadOnly Property DescricaoCompleta As String
        Get
            Return $"{nm_modelo} ({nm_placa})"
        End Get
    End Property


End Class
