Namespace Common


    ''' <summary>
    ''' Classe base para itens de lista.
    ''' Garante que todo item tenha um Model tipado e funcionalidades comuns.
    ''' </summary>
    ''' <typeparam name="T">O tipo do Model (ex: Lancamentos, Categorias)</typeparam>
    Public MustInherit Class BaseItemViewModel(Of T)
        Inherits BaseViewModel

        ' O Model Tipado (Genérico)
        Public ReadOnly Property Model As T

        Public Sub New(item As T)
            If item Is Nothing Then Throw New ArgumentNullException(NameOf(item))
            Model = item
        End Sub

        ' ============================================================
        ' FUNCIONALIDADES COMUNS A QUALQUER ITEM DE LISTA
        ' ============================================================

        ' Exemplo: Controle de seleção (para mudar a cor de fundo quando clica)
        Private _isSelected As Boolean
        Public Property IsSelected As Boolean
            Get
                Return _isSelected
            End Get
            Set(value As Boolean)
                SetProperty(_isSelected, value)
            End Set
        End Property

    End Class

End Namespace