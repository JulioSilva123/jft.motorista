Imports SQLite





<Table("Categorias")>
Public Class Categorias


    Public Sub New()

    End Sub

    <PrimaryKey, AutoIncrement>
    Public Property id_categoria As Integer

    Public Property nm_categoria As String

    ' 0 = Receita, 1 = Despesa
    Public Property cs_tipo As Integer

    Public Property te_icone As String

    Public Property te_CorHex As String

End Class
