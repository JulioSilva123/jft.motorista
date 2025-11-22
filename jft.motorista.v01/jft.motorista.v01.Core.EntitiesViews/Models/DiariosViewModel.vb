
Imports jft.motorista.v01.Core.Entities

Imports jft.motorista.v01.Core.EntitiesViews.Common

''' <summary>
''' Wrapper visual para exibir um dia de trabalho na lista.
''' Renomeado para Plural (DiariosViewModel).
''' </summary>
Public Class DiariosViewModel
    Inherits BaseItemViewModel(Of Diarios)

    Public Sub New(item As Diarios)
        MyBase.New(item)
    End Sub

    Public ReadOnly Property DiaMes As String
        Get
            Return Model.dt_diario.ToString("dd MMM").ToUpper()
        End Get
    End Property

    Public ReadOnly Property DiaSemana As String
        Get
            Return Model.dt_diario.ToString("dddd")
        End Get
    End Property

    Public ReadOnly Property HorarioFormatado As String
        Get
            Return $"{Model.hr_inicio:hh\:mm} - {Model.hr_fim:hh\:mm}"
        End Get
    End Property

    Public ReadOnly Property TempoTrabalhadoFormatado As String
        Get
            Dim t = Model.TempoTrabalhado
            Return $"{Math.Floor(t.TotalHours)}h {t.Minutes}min"
        End Get
    End Property

    ' ATUALIZADO: Agora mostra explicitamente o KM do Trip
    Public ReadOnly Property KmRodadoFormatado As String
        Get
            Return $"{Model.nr_km_informado_trip:N0} km"
        End Get
    End Property

    Public ReadOnly Property TemKmMorta As Boolean
        Get
            ' Se a KM Morta for significativa (> 10km), marcamos (Opcional)
            Return Model.KmMorta > 10
        End Get
    End Property

    Public ReadOnly Property KmPorLitro As String
        Get
            ' Rendimento Mecânico (Trip / Litros)
            If Model.qt_litros_consumidos > 0 Then
                Dim media = Model.nr_km_informado_trip / Model.qt_litros_consumidos
                Return $"{media:N2} km/L"
            End If
            Return "-"
        End Get
    End Property

End Class

