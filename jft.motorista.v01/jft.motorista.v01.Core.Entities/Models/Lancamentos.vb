Imports SQLite


<Table("Lancamentos")>
Public Class Lancamentos


    Public Sub New()

    End Sub


    <PrimaryKey, AutoIncrement>
    Public Property id_lancamento As Integer

    Public Property vl_lancamento As Decimal

    Public Property dt_lancamento As DateTime

    Public Property te_observacoes As String

    <Indexed>
    Public Property id_categoria As Integer




    ' ============================================================
    ' PROPRIEDADES AUXILIARES (Não salvas no Banco)
    ' ============================================================

    <Ignore>
    Public Property Categoria As Categorias

    '<Ignore>
    'Public ReadOnly Property nm_categoria As String
    '    Get
    '        ' Operador condicional If(objeto, valor_se_nulo)
    '        Return If(Categoria?.nm_categoria, "...")
    '    End Get
    'End Property

    ' Campo calculado em tempo de execução (Running Balance)
    <Ignore>
    Public Property SaldoAposLancamento As Decimal

    '<Ignore>
    'Public ReadOnly Property Saldo As String
    '    Get
    '        Return $"Saldo: {SaldoAposLancamento:C}"
    '    End Get
    'End Property

    '<Ignore>
    'Public ReadOnly Property Valor As String
    '    Get
    '        If Categoria Is Nothing Then
    '            Return $"{vl_lancamento:C}"
    '        End If

    '        ' Se Tipo = 1 (Despesa), adiciona o sinal de menos visualmente
    '        Return If(Categoria.cs_tipo = 1, $"- {vl_lancamento:C}", $"{vl_lancamento:C}")
    '    End Get
    'End Property

    '<Ignore>
    'Public ReadOnly Property CorTexto As String
    '    Get
    '        If Categoria Is Nothing Then
    '            Return "#000000" ' Preto padrão
    '        End If

    '        ' Vermelho (#FF0000) para Despesa, Verde (#008000) para Receita
    '        Return If(Categoria.cs_tipo = 1, "#FF0000", "#008000")
    '    End Get
    'End Property

End Class
