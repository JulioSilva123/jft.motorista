Imports jft.motorista.v01.Core.Entities
Imports jft.motorista.v01.Infra.Interfaces


Namespace Interfaces

    ''' <summary>
    ''' Contrato para manipulação dos Checkpoints de Saldo (Performance).
    ''' </summary>
    Public Interface ISaldosMensaisRepository
        Inherits IBaseRepository(Of SaldosMensais)

        ''' <summary>
        ''' Busca o saldo fechado de um mês específico.
        ''' Usado para iniciar o cálculo do extrato do mês seguinte.
        ''' </summary>
        Function GetPorMesAsync(mes As Integer, ano As Integer) As Task(Of SaldosMensais)

        ''' <summary>
        ''' Método inteligente que decide se cria um novo registro ou atualiza o existente (Upsert).
        ''' Vital para o "Efeito Dominó" de recálculo.
        ''' </summary>
        Function SalvarOuAtualizarAsync(mes As Integer, ano As Integer, novoSaldo As Decimal) As Task

        ''' <summary>
        ''' Limpa a tabela auxiliar para forçar um recálculo total do zero.
        ''' </summary>
        Function DeletarTudoAsync() As Task




        Function GetHistoricoCompletoAsync() As Task(Of List(Of SaldosMensais))


    End Interface

End Namespace