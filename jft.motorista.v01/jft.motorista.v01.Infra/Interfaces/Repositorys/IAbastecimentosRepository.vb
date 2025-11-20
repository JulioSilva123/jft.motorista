Imports jft.motorista.v01.Core.Entities
Imports jft.motorista.v01.Infra.Interfaces

Namespace Interfaces

    ''' <summary>
    ''' Contrato para manipulação de dados de Abastecimento.
    ''' </summary>
    Public Interface IAbastecimentosRepository
        Inherits IMovimentacaoBaseRepository(Of Abastecimentos)

        ''' <summary>
        ''' Busca o histórico de abastecimentos de um veículo específico, ordenado do mais recente para o mais antigo.
        ''' </summary>
        Function GetHistoricoPorVeiculoAsync(idVeiculo As Integer, Optional quantidade As Integer = 20) As Task(Of List(Of Abastecimentos))

        ''' <summary>
        ''' Retorna o último abastecimento registrado para o veículo.
        ''' Útil para sugerir a KM anterior ou calcular médias parciais.
        ''' </summary>
        Function GetUltimoDoVeiculoAsync(idVeiculo As Integer) As Task(Of Abastecimentos)

    End Interface

End Namespace