
Imports jft.motorista.v01.Core.Entities
Imports jft.motorista.v01.Infra.Common
Imports jft.motorista.v01.Infra.Data
Imports jft.motorista.v01.Infra.Interfaces

Namespace Repositorys

    Public Class VeiculosRepository
        Inherits BaseReporitory
        Implements IVeiculosRepository


        ' Injeção de Dependência do Contexto
        Public Sub New()
            '_context = context
        End Sub

        Public Sub New(context As DbMotorista)
            MyBase.New(context)
        End Sub


        ' --- CRUD Básico (ICadastroBaseRepository) ---

        Public Async Function GetItemAsync(id As Integer) As Task(Of Veiculos) Implements IBaseRepository(Of Veiculos).GetItemAsync
            Return Await _context.Connection.Table(Of Veiculos).Where(Function(x) x.id_veiculo = id).FirstOrDefaultAsync()
        End Function

        Public Async Function GetTodasAsync() As Task(Of List(Of Veiculos)) Implements ICadastroBaseRepository(Of Veiculos).GetTodasAsync
            ' Retorna ordenado: O ativo primeiro, depois por nome
            Return Await _context.Connection.Table(Of Veiculos) _
                                .OrderByDescending(Function(v) v.fl_ativo) _
                                .ThenBy(Function(v) v.nm_modelo) _
                                .ToListAsync()
        End Function

        Public Async Function SalvarAsync(item As Veiculos) As Task Implements IBaseRepository(Of Veiculos).SalvarAsync

            ' REGRA DE NEGÓCIO: Exclusividade
            ' Se estamos salvando este carro como ATIVO, precisamos desativar os outros antes.
            If item.fl_ativo Then
                Await DesativarTodosExcetoAsync(item.id_veiculo)
            Else
                ' REGRA DE SEGURANÇA:
                ' Se este é o único carro do banco, ele OBRIGATORIAMENTE tem que ser ativo.
                ' Evita que o sistema fique sem carro ativo.
                Dim qtd = Await _context.Connection.Table(Of Veiculos).CountAsync()
                If qtd = 0 Then item.fl_ativo = True
            End If

            If item.id_veiculo <> 0 Then
                Await _context.Connection.UpdateAsync(item)
            Else
                Await _context.Connection.InsertAsync(item)
            End If
        End Function

        Public Async Function DeletarAsync(item As Veiculos) As Task Implements IBaseRepository(Of Veiculos).DeletarAsync
            ' REGRA: Não deixar apagar o carro ativo se houver outros (ou forçar ativar outro antes)
            ' Por enquanto, deletamos simples.
            Await _context.Connection.DeleteAsync(item)
        End Function

        ' --- Métodos Específicos ---

        Public Async Function GetVeiculoAtivoAsync() As Task(Of Veiculos) Implements IVeiculosRepository.GetVeiculoAtivoAsync
            Return Await _context.Connection.Table(Of Veiculos).Where(Function(v) v.fl_ativo).FirstOrDefaultAsync()
        End Function

        Public Async Function DefinirComoAtivoAsync(idVeiculo As Integer) As Task Implements IVeiculosRepository.DefinirComoAtivoAsync
            ' 1. Desativa todo mundo
            Await DesativarTodosExcetoAsync(idVeiculo)

            ' 2. Ativa o escolhido
            Dim alvo = Await GetItemAsync(idVeiculo)
            If alvo IsNot Nothing Then
                alvo.fl_ativo = True
                Await _context.Connection.UpdateAsync(alvo)
            End If
        End Function

        ' --- Helpers Privados ---

        Private Async Function DesativarTodosExcetoAsync(idVeiculoExcecao As Integer) As Task
            ' Busca todos os ativos que não sejam o atual
            Dim ativos = Await _context.Connection.Table(Of Veiculos) _
                                    .Where(Function(v) v.fl_ativo AndAlso v.id_veiculo <> idVeiculoExcecao) _
                                    .ToListAsync()

            ' Desativa um por um
            For Each v In ativos
                v.fl_ativo = False
                Await _context.Connection.UpdateAsync(v)
            Next
        End Function

    End Class

End Namespace