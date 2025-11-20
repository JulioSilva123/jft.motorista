Imports jft.motorista.v01.Infra.Repositorys

Namespace Interfaces

    ''' <summary>
    ''' Contrato do Unit of Work.
    ''' Centraliza o acesso a todos os repositórios do sistema.
    ''' As ViewModels devem pedir essa interface no construtor.
    ''' </summary>
    Public Interface IRepositoryManager

        ' Repositório de Movimentações (Lançamentos)
        ReadOnly Property Lancamentos As ILancamentosRepository

        ' Repositório de Cadastros (Categorias)
        ReadOnly Property Categorias As ICategoriasRepository

        ' Repositório Auxiliar (Checkpoints de Saldo)
        ' Importante ter a interface ISaldoMensalRepository criada também
        ReadOnly Property SaldosMensais As ISaldosMensaisRepository




        ReadOnly Property Diarios As IDiariosRepository
        ReadOnly Property Abastecimentos As IAbastecimentosRepository
        ReadOnly Property Veiculos As IVeiculosRepository





    End Interface

End Namespace