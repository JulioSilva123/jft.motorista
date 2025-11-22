
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

            ' HYDRATION: Preenche os objetos relacionados
            If item IsNot Nothing Then
                ' 1. Busca o Veículo
                item.Veiculo = Await _context.Connection.Table(Of Veiculos) _
                                    .Where(Function(v) v.id_veiculo = item.id_veiculo) _
                                    .FirstOrDefaultAsync()

                ' 2. Busca o Lançamento Financeiro (NOVO)
                If item.id_lancamento > 0 Then
                    item.Lancamento = Await _context.Connection.Table(Of Lancamentos) _
                                            .Where(Function(l) l.id_lancamento = item.id_lancamento) _
                                            .FirstOrDefaultAsync()
                End If
            End If

            Return item
        End Function

        Public Async Function SalvarAsync(item As Abastecimentos) As Task Implements IBaseRepository(Of Abastecimentos).SalvarAsync
            If item.id_abastecimento <> 0 Then
                Await _context.Connection.UpdateAsync(item)
            Else
                Await _context.Connection.InsertAsync(item)
            End If

            ' Recalcula KM e Média do Carro
            Await AtualizarDadosDoVeiculoAsync(item.id_veiculo)
        End Function

        Public Async Function DeletarAsync(item As Abastecimentos) As Task Implements IBaseRepository(Of Abastecimentos).DeletarAsync
            ' REGRA DE INTEGRIDADE: 
            ' Se apagar o abastecimento, devemos apagar o lançamento financeiro vinculado?
            ' Geralmente SIM, para não sobrar "lixo" no caixa.

            If item.id_lancamento > 0 Then
                Dim lancamento = Await _context.Connection.Table(Of Lancamentos).Where(Function(l) l.id_lancamento = item.id_lancamento).FirstOrDefaultAsync()
                If lancamento IsNot Nothing Then
                    Await _context.Connection.DeleteAsync(lancamento)
                    ' Nota: Idealmente, chamaríamos o Repo.Lancamentos.DeletarAsync para recalcular o saldo, 
                    ' mas aqui estamos acessando direto o contexto para evitar dependência circular.
                    ' O saldo será recalculado na próxima abertura ou podemos forçar.
                End If
            End If

            Await _context.Connection.DeleteAsync(item)

            ' Recalcula KM do carro
            Await AtualizarDadosDoVeiculoAsync(item.id_veiculo)
        End Function

        ' ... (Resto dos métodos GetHistoricoPorVeiculoAsync, GetUltimoDoVeiculoAsync, AtualizarDadosDoVeiculoAsync permanecem iguais) ...
        ' IMPLEMENTAÇÃO DO NOVO MÉTODO
        Public Async Function GetPorDataVeiculoAsync(data As DateTime, idVeiculo As Integer) As Task(Of List(Of Abastecimentos)) Implements IAbastecimentosRepository.GetPorDataVeiculoAsync

            ' Filtra o dia inteiro (00:00 até 23:59)
            Dim inicioDia = data.Date
            Dim fimDia = data.Date.AddDays(1).AddTicks(-1)

            Return Await _context.Connection.Table(Of Abastecimentos) _
                                .Where(Function(a) a.id_veiculo = idVeiculo AndAlso
                                                   a.dt_abastecimento >= inicioDia AndAlso
                                                   a.dt_abastecimento <= fimDia) _
                                .ToListAsync()
        End Function
        Public Async Function GetHistoricoPorVeiculoAsync(idVeiculo As Integer, Optional quantidade As Integer = 20) As Task(Of List(Of Abastecimentos)) Implements IAbastecimentosRepository.GetHistoricoPorVeiculoAsync
            Return Await _context.Connection.Table(Of Abastecimentos) _
                                .Where(Function(a) a.id_veiculo = idVeiculo) _
                                .OrderByDescending(Function(a) a.dt_abastecimento) _
                                .Take(quantidade) _
                                .ToListAsync()
        End Function

        Public Async Function GetUltimoDoVeiculoAsync(idVeiculo As Integer) As Task(Of Abastecimentos) Implements IAbastecimentosRepository.GetUltimoDoVeiculoAsync
            Return Await _context.Connection.Table(Of Abastecimentos) _
                                .Where(Function(a) a.id_veiculo = idVeiculo) _
                                .OrderByDescending(Function(a) a.nr_km_odometro) _
                                .FirstOrDefaultAsync()
        End Function

        Private Async Function AtualizarDadosDoVeiculoAsync(idVeiculo As Integer) As Task
            Dim veiculo = Await _context.Connection.Table(Of Veiculos).Where(Function(v) v.id_veiculo = idVeiculo).FirstOrDefaultAsync()
            If veiculo Is Nothing Then Return

            Dim ultimo = Await _context.Connection.Table(Of Abastecimentos).Where(Function(a) a.id_veiculo = idVeiculo).OrderByDescending(Function(a) a.nr_km_odometro).FirstOrDefaultAsync()

            If ultimo IsNot Nothing Then
                veiculo.nr_km_atual = ultimo.nr_km_odometro
                Dim penultimo = Await _context.Connection.Table(Of Abastecimentos).Where(Function(a) a.id_veiculo = idVeiculo AndAlso a.nr_km_odometro < ultimo.nr_km_odometro).OrderByDescending(Function(a) a.nr_km_odometro).FirstOrDefaultAsync()

                If penultimo IsNot Nothing Then
                    Dim distancia = ultimo.nr_km_odometro - penultimo.nr_km_odometro
                    Dim litros = ultimo.qt_litros
                    If litros > 0 AndAlso distancia > 0 Then
                        veiculo.nr_media_consumo = distancia / litros
                    End If
                End If
            Else
                veiculo.nr_media_consumo = 0
            End If

            Await _context.Connection.UpdateAsync(veiculo)
        End Function


    End Class

End Namespace