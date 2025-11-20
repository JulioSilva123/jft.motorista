Namespace Interfaces



    ' 2A. A Interface para CADASTROS (Tabelas Pequenas: Categorias, Configurações, Carros)
    ' Herda da Mãe e ADICIONA o poder de pegar tudo
    Public Interface ICadastroBaseRepository(Of T)
        Inherits IBaseRepository(Of T)

        ' Seguro de usar aqui porque sabemos que a tabela é pequena
        Function GetTodasAsync() As Task(Of List(Of T))

    End Interface




End Namespace