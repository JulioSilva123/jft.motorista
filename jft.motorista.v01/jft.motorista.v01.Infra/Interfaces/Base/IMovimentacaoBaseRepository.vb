Namespace Interfaces


    ' 2B. A Interface para MOVIMENTAÇÃO (Tabelas Grandes: Lançamentos, Logs, Histórico)
    ' Herda da Mãe, mas NÃO TEM GetTodasAsync. 
    ' Podemos forçar um padrão de busca por data, por exemplo.
    Public Interface IMovimentacaoBaseRepository(Of T)
        Inherits IBaseRepository(Of T)

        ' Opcional: Você pode obrigar que toda movimentação tenha busca por período
        ' Function GetPorPeriodoAsync(inicio As Date, fim As Date) As Task(Of List(Of T))
    End Interface


End Namespace