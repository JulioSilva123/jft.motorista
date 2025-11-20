
Imports jft.motorista.v01.Core.Entities
Imports jft.motorista.v01.Infra.Common
Imports jft.motorista.v01.Infra.Data
Imports jft.motorista.v01.Infra.Interfaces

Namespace Repositorys

    Public Class AbastecimentosRepository
        Inherits BaseReporitory
        Implements IAbastecimentosRepository

        ' Injeção de Dependência do Contexto
        Public Sub New()
            '_context = context
        End Sub

        Public Sub New(context As DbMotorista)
            MyBase.New(context)
        End Sub

        ' --- CRUD DA BASE ---

        Public Async Function GetItemAsync(id As Integer) As Task(Of Abastecimentos) Implements IBaseRepository(Of Abastecimentos).GetItemAsync
            Dim item = Await _context.Connection.Table(Of Abastecimentos) _
                                .Where(Function(x) x.id_abastecimento = id) _
                                .FirstOrDefaultAsync()

            ' HYDRATION: Preenche o veículo relacionado
            If item IsNot Nothing Then
                item.Veiculo = Await _context.Connection.Table(Of Veiculos) _
                                    .Where(Function(v) v.id_veiculo = item.id_veiculo) _
                                    .FirstOrDefaultAsync()
            End If

            Return item
        End Function

        Public Async Function SalvarAsync(item As Abastecimentos) As Task Implements IBaseRepository(Of Abastecimentos).SalvarAsync
            If item.id_abastecimento <> 0 Then
                Await _context.Connection.UpdateAsync(item)
            Else
                Await _context.Connection.InsertAsync(item)
            End If

            ' DICA DE REGRA DE NEGÓCIO FUTURA:
            ' Aqui poderíamos atualizar automaticamente o nr_km_atual do Veículo 
            ' se o km do abastecimento for maior que o atual.
        End Function

        Public Async Function DeletarAsync(item As Abastecimentos) As Task Implements IBaseRepository(Of Abastecimentos).DeletarAsync
            Await _context.Connection.DeleteAsync(item)
        End Function

        ' --- MÉTODOS ESPECÍFICOS ---

        Public Async Function GetHistoricoPorVeiculoAsync(idVeiculo As Integer, Optional quantidade As Integer = 20) As Task(Of List(Of Abastecimentos)) Implements IAbastecimentosRepository.GetHistoricoPorVeiculoAsync
            ' Busca os últimos X abastecimentos deste carro
            ' Nota: Não fazemos hydration aqui para performance, pois já sabemos o ID do veículo
            Return Await _context.Connection.Table(Of Abastecimentos) _
                                .Where(Function(a) a.id_veiculo = idVeiculo) _
                                .OrderByDescending(Function(a) a.dt_abastecimento) _
                                .Take(quantidade) _
                                .ToListAsync()
        End Function

        Public Async Function GetUltimoDoVeiculoAsync(idVeiculo As Integer) As Task(Of Abastecimentos) Implements IAbastecimentosRepository.GetUltimoDoVeiculoAsync
            ' Pega apenas 1 registro mais recente
            Return Await _context.Connection.Table(Of Abastecimentos) _
                                .Where(Function(a) a.id_veiculo = idVeiculo) _
                                .OrderByDescending(Function(a) a.dt_abastecimento) _
                                .FirstOrDefaultAsync()
        End Function

    End Class

End Namespace