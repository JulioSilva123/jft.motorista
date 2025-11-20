Imports jft.motorista.v01.Core.Entities
Imports jft.motorista.v01.Infra.Interfaces

Namespace Interfaces

    ''' <summary>
    ''' Contrato para gestão de frota.
    ''' </summary>
    Public Interface IVeiculosRepository
        Inherits ICadastroBaseRepository(Of Veiculos)

        ''' <summary>
        ''' Retorna o veículo que está marcado como ATIVO no momento.
        ''' Usado para calcular médias e vincular novos abastecimentos automaticamente.
        ''' </summary>
        Function GetVeiculoAtivoAsync() As Task(Of Veiculos)

        ''' <summary>
        ''' Define qual veículo será o principal.
        ''' Implementa a regra de negócio: "Só pode haver 1 ativo".
        ''' </summary>
        Function DefinirComoAtivoAsync(idVeiculo As Integer) As Task

    End Interface

End Namespace