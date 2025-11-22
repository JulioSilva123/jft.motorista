Imports jft.motorista.v01.Core.Entities
Imports jft.motorista.v01.Infra.Common
Imports jft.motorista.v01.Infra.Data
Imports jft.motorista.v01.Infra.Interfaces

Namespace Repositorys



    Public Class SaldosMensaisRepository
        Inherits BaseReporitory
        Implements ISaldosMensaisRepository


        ' Private ReadOnly _context As DbMotorista

        ' Injeção de Dependência do Contexto
        Public Sub New()
            '_context = context
        End Sub

        Public Sub New(context As DbMotorista)
            MyBase.New(context)
        End Sub


        ''' <summary>
        ''' Busca o checkpoint de um mês específico. Retorna Nothing se não existir.
        ''' </summary>
        Public Async Function GetPorMesAsync(mes As Integer, ano As Integer) As Task(Of SaldosMensais) Implements ISaldosMensaisRepository.GetPorMesAsync
            Return Await _context.SaldosMensais _
                                 .Where(Function(s) s.nr_mes = mes AndAlso s.nr_ano = ano) _
                                 .FirstOrDefaultAsync()
        End Function

        ''' <summary>
        ''' Atualiza o saldo de um mês existente ou cria um novo se não houver.
        ''' </summary>
        Public Async Function SalvarOuAtualizarAsync(mes As Integer, ano As Integer, novoSaldo As Decimal) As Task Implements ISaldosMensaisRepository.SalvarOuAtualizarAsync
            ' Verifica se já existe registro para este mês
            Dim existente = Await GetPorMesAsync(mes, ano)

            If existente IsNot Nothing Then
                ' UPDATE
                existente.vl_saldofinal = novoSaldo
                Await _context.Connection.UpdateAsync(existente)
            Else
                ' INSERT
                Dim novo As New SaldosMensais With {
                    .nr_mes = mes,
                    .nr_ano = ano,
                    .vl_saldofinal = novoSaldo
                }
                Await _context.Connection.InsertAsync(novo)
            End If
        End Function




        ''' <summary>
        ''' Limpa todos os saldos (útil para resets ou recálculos totais)
        ''' </summary>
        Public Async Function DeletarTudoAsync() As Task Implements ISaldosMensaisRepository.DeletarTudoAsync
            Await _context.Connection.DeleteAllAsync(Of SaldosMensais)()
        End Function

        Public Async Function GetItemAsync(id As Integer) As Task(Of SaldosMensais) Implements IBaseRepository(Of SaldosMensais).GetItemAsync
            Return Await _context.SaldosMensais.Where(Function(s) s.id_saldomensal = id).FirstOrDefaultAsync()
        End Function

        Public Async Function SalvarAsync(item As SaldosMensais) As Task Implements IBaseRepository(Of SaldosMensais).SalvarAsync
            If item.id_saldomensal <> 0 Then
                Await _context.Connection.UpdateAsync(item)
            Else
                Await _context.Connection.InsertAsync(item)
            End If
        End Function

        Public Async Function DeletarAsync(item As SaldosMensais) As Task Implements IBaseRepository(Of SaldosMensais).DeletarAsync
            Await _context.Connection.DeleteAsync(item)
        End Function

        Public Async Function GetHistoricoCompletoAsync() As Task(Of List(Of SaldosMensais)) Implements ISaldosMensaisRepository.GetHistoricoCompletoAsync
            Return Await _context.SaldosMensais _
                                 .OrderByDescending(Function(s) s.nr_ano) _
                                 .ThenByDescending(Function(s) s.nr_mes) _
                                 .ToListAsync()
        End Function
    End Class


End Namespace