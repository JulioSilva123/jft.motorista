

Imports jft.motorista.v01.Core.Entities
Imports jft.motorista.v01.Infra.Interfaces


Namespace Interfaces



    Public Interface ILancamentosRepository
        Inherits IMovimentacaoBaseRepository(Of Lancamentos)

        ' Busca filtrada por Mês/Ano para garantir performance
        Function GetExtratoDoMesAsync(mes As Integer, ano As Integer) As Task(Of List(Of Lancamentos))
    End Interface


End Namespace