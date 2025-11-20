Namespace Interfaces

    ' 1. A Interface MÃE (Comum a tudo)
    Public Interface IBaseRepository(Of T)

        Function GetItemAsync(id As Integer) As Task(Of T)
        Function SalvarAsync(item As T) As Task
        Function DeletarAsync(item As T) As Task

    End Interface



End Namespace