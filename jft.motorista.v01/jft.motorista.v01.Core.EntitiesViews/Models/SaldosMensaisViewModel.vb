Imports jft.motorista.v01.Core.Entities
Imports jft.motorista.v01.Core.EntitiesViews.Common
Imports Xamarin.Forms

''' <summary>
''' Wrapper visual para a tabela SaldosMensais.
''' Renomeado para Plural.
''' </summary>
Public Class SaldosMensaisViewModel
    Inherits BaseItemViewModel(Of SaldosMensais)

    Public Sub New(item As SaldosMensais)
        MyBase.New(item)
    End Sub

    Public ReadOnly Property PeriodoFormatado As String
        Get
            Dim nomeMes = New DateTime(Model.nr_ano, Model.nr_mes, 1).ToString("MMMM").ToUpper()
            Return $"{nomeMes} / {Model.nr_ano}"
        End Get
    End Property

    Public ReadOnly Property SaldoFormatado As String
        Get
            Return Model.vl_saldofinal.ToString("C")
        End Get
    End Property

    Public ReadOnly Property CorSaldo As Color
        Get
            Return If(Model.vl_saldofinal >= 0, Color.DarkGreen, Color.Red)
        End Get
    End Property

End Class