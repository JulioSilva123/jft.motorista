
Imports jft.motorista.v01.Core.Entities

Imports jft.motorista.v01.Core.EntitiesViews.Common

''' <summary>
''' Wrapper visual para item de Abastecimento.
''' </summary>
Public Class AbastecimentosViewModel
    Inherits BaseItemViewModel(Of Abastecimentos)

    Public Sub New(item As Abastecimentos)
        MyBase.New(item)
    End Sub

    Public ReadOnly Property DataFormatada As String
        Get
            Return Model.dt_abastecimento.ToString("dd/MM/yyyy")
        End Get
    End Property

    Public ReadOnly Property KmDescricao As String
        Get
            Return $"{Model.nr_km_odometro:N0} km"
        End Get
    End Property

    Public ReadOnly Property ValorTotalFormatado As String
        Get
            Return Model.vl_total_pago.ToString("C")
        End Get
    End Property

    Public ReadOnly Property Detalhes As String
        Get
            Return $"{Model.qt_litros:N2} L • {Model.PrecoPorLitro:C2}/L"
        End Get
    End Property

    ' Nome do Veículo (Se hidratado)
    Public ReadOnly Property NomeVeiculo As String
        Get
            Return If(Model.Veiculo?.nm_modelo, "Carro Desconhecido")
        End Get
    End Property

End Class
