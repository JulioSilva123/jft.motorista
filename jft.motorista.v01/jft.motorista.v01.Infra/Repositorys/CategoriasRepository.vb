Imports jft.motorista.v01.Core.Entities
Imports jft.motorista.v01.Infra.Common
Imports jft.motorista.v01.Infra.Data
Imports jft.motorista.v01.Infra.Interfaces


Namespace Repositorys





    Public Class CategoriasRepository
        Inherits BaseReporitory
        Implements ICategoriasRepository



        ' Private ReadOnly _context As DbMotorista

        ' Injeção de Dependência do Contexto
        Public Sub New()
            '_context = context
        End Sub

        Public Sub New(context As DbMotorista)
            MyBase.New(context)
        End Sub
        'Private ReadOnly _context As DriverDbContext

        ' --- IMPLEMENTAÇÃO DA BASE ---

        Public Async Function GetItemAsync(id As Integer) As Task(Of Categorias) Implements IBaseRepository(Of Categorias).GetItemAsync
            ' ATUALIZADO: id_categoria
            Return Await _context.Categorias.Where(Function(c) c.id_categoria = id).FirstOrDefaultAsync()
        End Function

        Public Async Function SalvarAsync(item As Categorias) As Task Implements IBaseRepository(Of Categorias).SalvarAsync
            ' ATUALIZADO: id_categoria
            If item.id_categoria <> 0 Then
                Await _context.Connection.UpdateAsync(item)
            Else
                Await _context.Connection.InsertAsync(item)
            End If
        End Function

        Public Async Function DeletarAsync(item As Categorias) As Task Implements IBaseRepository(Of Categorias).DeletarAsync
            Await _context.Connection.DeleteAsync(item)
        End Function

        ' --- IMPLEMENTAÇÃO ESPECÍFICA ---

        Public Async Function GetTodasAsync() As Task(Of List(Of Categorias)) Implements ICategoriasRepository.GetTodasAsync
            Return Await _context.Categorias.ToListAsync()
        End Function

        Public Async Function DeletarComSegurancaAsync(item As Categorias) As Task(Of Boolean) Implements ICategoriasRepository.DeletarComSegurancaAsync
            ' Verifica se existe algum lançamento usando essa categoria
            ' ATUALIZADO: id_categoria
            Dim countUso = Await _context.Lancamentos _
                                         .Where(Function(x) x.id_categoria = item.id_categoria) _
                                         .CountAsync()

            If countUso > 0 Then
                Return False
            End If

            Await DeletarAsync(item)

            Return True
        End Function

    End Class


End Namespace