Imports Xamarin.Forms
Imports System.Windows.Input
Imports jft.motorista.v01.Core.Entities
Imports jft.motorista.v01.Core.EntitiesViews.Common


Public Class CategoriasViewModel
        Inherits BaseItemViewModel(Of Categorias)

        Public Sub New(item As Categorias)
            MyBase.New(item)
        End Sub

        Public ReadOnly Property Nome As String
            Get
                Return Model.nm_categoria
            End Get
        End Property

        Public ReadOnly Property Icone As String
            Get
                Return If(String.IsNullOrEmpty(Model.te_icone), "fa-tag", Model.te_icone)
            End Get
        End Property

        ' Cor Hexadecimal convertida para Color do Xamarin
        Public ReadOnly Property Cor As Color
            Get
            If String.IsNullOrEmpty(Model.te_CorHex) Then
                Return Color.Gray
            End If
            Return Color.FromHex(Model.te_CorHex)
        End Get
        End Property

        ' Texto descritivo do Tipo
        Public ReadOnly Property TipoTexto As String
            Get
                Return If(Model.cs_tipo = 0, "Receita", "Despesa")
            End Get
        End Property

        ' Cor do Texto do Tipo
        Public ReadOnly Property CorTipo As Color
            Get
                Return If(Model.cs_tipo = 0, Color.Green, Color.Red)
            End Get
        End Property

    End Class

