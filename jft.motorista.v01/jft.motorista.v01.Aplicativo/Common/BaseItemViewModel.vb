
Namespace Common



    Public MustInherit Class BaseItemViewModel(Of T)
        Inherits BaseViewModel

        Public ReadOnly Property Model As T

        Public Sub New(item As T)
            If item Is Nothing Then Throw New ArgumentNullException(NameOf(item))
            Model = item
        End Sub


    End Class


End Namespace