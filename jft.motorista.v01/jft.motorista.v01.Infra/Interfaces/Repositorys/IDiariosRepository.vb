Imports jft.motorista.v01.Core.Entities

Namespace Interfaces



    ' RENOMEADO: De IDiarioRepository para IDiariosRepository (Plural igual a tabela)
    Public Interface IDiariosRepository
        Inherits IMovimentacaoBaseRepository(Of Diarios)

        ''' <summary>
        ''' Retorna os diários do mês com o CustoCombustivelReal já calculado.
        ''' Também retorna o somatório total desse custo no mês.
        ''' </summary>
        Function GetResumoCustosMensalAsync(mes As Integer, ano As Integer) As Task(Of (Lista As List(Of Diarios), TotalCusto As Decimal))

    End Interface



End Namespace