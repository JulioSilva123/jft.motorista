
Imports Xamarin.Forms
Imports System.Windows.Input
Imports jft.motorista.v01.Core.Entities
Imports jft.motorista.v01.Core.EntitiesViews.Common

Public Class LancamentosViewModel
    Inherits BaseItemViewModel(Of Lancamentos)

    ' Construtor repassa o item para a base
    Public Sub New(item As Lancamentos)
        MyBase.New(item)
    End Sub

    ' --- Propriedades Visuais ---

    Public ReadOnly Property Titulo As String
        Get
            Return If(Model.Categoria?.nm_categoria, "Sem Categoria")
        End Get
    End Property

    Public ReadOnly Property Icone As String
        Get
            Return If(Model.Categoria?.te_icone, "fa-question-circle")
        End Get
    End Property


    ' ============================================================
    ' A propriedade "Me.Model" já existe e é do tipo "Lancamentos"
    ' ============================================================
    Public ReadOnly Property ValorFormatado As String
        Get
            Dim val = Math.Abs(Model.vl_lancamento)
            Return If(IsDespesa, $"- {val:C}", $"{val:C}")
        End Get
    End Property
    'Public ReadOnly Property ValorFormatado As String
    '    Get
    '        ' Acessa Me.Model direto
    '        Return If(IsDespesa, $"- {Math.Abs(Model.vl_lancamento):C}", $"{Model.vl_lancamento:C}")
    '        'Return If(IsDespesa, $"- {Math.Abs(Model.Valor):C}", $"{Model.Valor:C}")
    '    End Get
    'End Property

    ' ... (Resto das propriedades visuais: CorValor, DataFormatada, etc) ...


    Public ReadOnly Property CorValor As Color
        Get
            Return If(IsDespesa, Color.Red, Color.DarkGreen)
        End Get
    End Property


    Public ReadOnly Property DataFormatada As String
        Get
            Return Model.dt_lancamento.ToString("dd MMM • HH:mm").ToUpper()
        End Get
    End Property

    Public ReadOnly Property SaldoLinha As String
        Get
            Return $"Saldo: {Model.SaldoAposLancamento:C}"
        End Get
    End Property


    ' --- Lógica & Comandos ---
    ' Lógica Privada
    Private ReadOnly Property IsDespesa As Boolean
        Get
            If Model.Categoria IsNot Nothing Then
                Return Model.Categoria.cs_tipo = 1
            End If
            Return Model.vl_lancamento < 0
        End Get
    End Property



    'Public ReadOnly Property DeletarCommand As ICommand
    '    Get
    '        Return New Command(Async Sub()
    '                               Dim confirm = Await Application.Current.MainPage.DisplayAlert("Excluir", "Apagar este item?", "Sim", "Não")
    '                               If confirm Then
    '                                   Await App.Repo.Lancamentos.DeletarAsync(Me.Model)
    '                                   MessagingCenter.Send(Me, "LancamentoDeletado")
    '                               End If
    '                           End Sub)
    '    End Get
    'End Property
End Class
