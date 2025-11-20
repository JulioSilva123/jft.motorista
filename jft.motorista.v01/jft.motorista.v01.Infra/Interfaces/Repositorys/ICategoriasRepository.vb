Imports jft.motorista.v01.Core.Entities


Namespace Interfaces



    Public Interface ICategoriasRepository
        Inherits ICadastroBaseRepository(Of Categorias)

        ''' <summary>
        ''' Tenta deletar uma categoria, mas impede se ela estiver sendo usada em lançamentos.
        ''' Retorna False se não foi possível apagar.
        ''' </summary>
        Function DeletarComSegurancaAsync(item As Categorias) As Task(Of Boolean)
    End Interface


End Namespace