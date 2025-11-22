Imports Xamarin.Forms
Imports jft.motorista.v01.Core.Entities
Imports jft.motorista.v01.Core.EntitiesViews.Common


''' <summary>
''' Wrapper visual para um Veículo na lista.
''' Renomeado de VeiculoViewModel para VeiculosViewModel.
''' </summary>
Public Class VeiculosViewModel
        Inherits BaseItemViewModel(Of Veiculos)

        Public Sub New(item As Veiculos)
            MyBase.New(item)
        End Sub

        Public ReadOnly Property Modelo As String
            Get
                Return Model.nm_modelo
            End Get
        End Property

        Public ReadOnly Property Placa As String
            Get
                Return Model.nm_placa
            End Get
        End Property

        Public ReadOnly Property KmDescricao As String
            Get
                Return $"{Model.nr_km_atual:N0} km"
            End Get
        End Property

        ' Cor do status (Verde se for o carro principal, Cinza se não)
        Public ReadOnly Property CorStatus As Color
            Get
                Return If(Model.fl_ativo, Color.FromHex("#4CAF50"), Color.Gray)
            End Get
        End Property

        ' Ícone de Check ou Círculo vazio
        Public ReadOnly Property IconeStatus As String
            Get
                Return If(Model.fl_ativo, "fa-check-circle", "fa-circle")
            End Get
        End Property

        ' Opacidade (Deixa carros inativos um pouco apagados)
        Public ReadOnly Property Opacidade As Double
            Get
                Return If(Model.fl_ativo, 1.0, 0.6)
            End Get
        End Property

    End Class

