
Imports jft.motorista.v01.Core.Entities
Imports jft.motorista.v01.Infra.Common
Imports jft.motorista.v01.Infra.Data
Imports jft.motorista.v01.Infra.Interfaces

Namespace Repositorys

    ' RENOMEADO: De DiarioRepository para DiariosRepository
    Public Class DiariosRepository
        Inherits BaseReporitory
        Implements IDiariosRepository



        ' Injeção de Dependência do Contexto
        Public Sub New()
            '_context = context
        End Sub

        Public Sub New(context As DbMotorista)
            MyBase.New(context)
        End Sub

        ' --- CRUD Básico ---
        Public Async Function GetItemAsync(id As Integer) As Task(Of Diarios) Implements IBaseRepository(Of Diarios).GetItemAsync
            Return Await _context.Connection.Table(Of Diarios).Where(Function(x) x.id_diario = id).FirstOrDefaultAsync()
        End Function

        Public Async Function SalvarAsync(item As Diarios) As Task Implements IBaseRepository(Of Diarios).SalvarAsync
            If item.id_diario <> 0 Then
                Await _context.Connection.UpdateAsync(item)
            Else
                Await _context.Connection.InsertAsync(item)
            End If
        End Function

        Public Async Function DeletarAsync(item As Diarios) As Task Implements IBaseRepository(Of Diarios).DeletarAsync
            Await _context.Connection.DeleteAsync(item)
        End Function

        ' --- LÓGICA DE CUSTOS (EXTRATO) ---
        Public Async Function GetResumoCustosMensalAsync(mes As Integer, ano As Integer) As Task(Of (Lista As List(Of Diarios), TotalCusto As Decimal)) Implements IDiariosRepository.GetResumoCustosMensalAsync

            Dim dataInicio As New DateTime(ano, mes, 1)
            Dim dataFim = dataInicio.AddMonths(1).AddTicks(-1)

            ' Busca Ordenada
            Dim diarios = Await _context.Connection.Table(Of Diarios) _
                                        .Where(Function(d) d.dt_diario >= dataInicio AndAlso d.dt_diario <= dataFim) _
                                        .OrderBy(Function(d) d.dt_diario) _
                                        .ToListAsync()

            Dim abastecimentos = Await _context.Connection.Table(Of Abastecimentos).ToListAsync()
            Dim totalAcumulado As Decimal = 0

            ' Cálculo Linha a Linha
            For Each diario In diarios
                If diario.id_abastecimento > 0 Then
                    diario.Abastecimento = abastecimentos.FirstOrDefault(Function(a) a.id_abastecimento = diario.id_abastecimento)
                End If

                totalAcumulado += diario.CustoCombustivelReal
                diario.CustoAcumulado = totalAcumulado
            Next

            diarios.Reverse()
            Return (diarios, totalAcumulado)
        End Function

    End Class

End Namespace