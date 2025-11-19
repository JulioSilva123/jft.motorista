Imports SQLite

<Table("SaldosMensais")>
Public Class SaldosMensais

    Public Sub New()

    End Sub

    <PrimaryKey, AutoIncrement>
    Public Property id_saldomensal As Integer

    Public Property nr_mes As Integer

    Public Property nr_ano As Integer

    ' Quanto o motorista tinha no bolso no último segundo desse mês
    Public Property vl_saldofinal As Decimal

End Class