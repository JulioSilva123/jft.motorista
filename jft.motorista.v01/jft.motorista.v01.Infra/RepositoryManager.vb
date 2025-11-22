Imports jft.motorista.v01.Core.Entities
Imports jft.motorista.v01.Infra.Data
Imports jft.motorista.v01.Infra.Interfaces
Imports jft.motorista.v01.Infra.Repositorys

''' <summary>
''' Centraliza o acesso a todos os repositórios do sistema.
''' Equivalente ao padrão Unit of Work (simplificado).
''' </summary>
Public Class RepositoryManager
    Implements IRepositoryManager



    Public Sub New()
        _context = New DbMotorista
    End Sub

    Public Sub New(context As DbMotorista)
        _context = context
    End Sub


    Private ReadOnly _context As DbMotorista




    ' ============================================================
    ' PROPRIEDADES DE ACESSO AOS REPOSITÓRIOS
    ' ============================================================






    Private _abastecimentos As IAbastecimentosRepository
    Public ReadOnly Property Abastecimentos As IAbastecimentosRepository Implements IRepositoryManager.Abastecimentos
        Get
            If _abastecimentos Is Nothing Then _abastecimentos = New AbastecimentosRepository(_context)
            Return _abastecimentos
        End Get
    End Property


    Private _diarios As IDiariosRepository
    Public ReadOnly Property Diarios As IDiariosRepository Implements IRepositoryManager.Diarios
        Get
            If _diarios Is Nothing Then _diarios = New DiariosRepository(_context)
            Return _diarios
        End Get
    End Property



    Private _veiculos As IVeiculosRepository

    Public ReadOnly Property Veiculos As IVeiculosRepository Implements IRepositoryManager.Veiculos
        Get
            If _veiculos Is Nothing Then _veiculos = New VeiculosRepository(_context)
            Return _veiculos
        End Get
    End Property


    ' Variáveis privadas para cache (Lazy Loading)
    Private _lancamentos As LancamentosRepository
    ''' <summary>
    ''' Gerencia toda a lógica de lançamentos, extratos e recálculos.
    ''' </summary>
    Public ReadOnly Property Lancamentos As ILancamentosRepository Implements IRepositoryManager.Lancamentos
        Get
            If _lancamentos Is Nothing Then
                _lancamentos = New LancamentosRepository(_context)
            End If
            Return _lancamentos
        End Get
    End Property
    Private _categorias As CategoriasRepository

    ''' <summary>
    ''' Gerencia o cadastro e proteção de categorias.
    ''' </summary>
    Public ReadOnly Property Categorias As ICategoriasRepository Implements IRepositoryManager.Categorias
        Get
            If _categorias Is Nothing Then
                _categorias = New CategoriasRepository(_context)
            End If
            Return _categorias
        End Get
    End Property
    Private _saldosMensais As SaldosMensaisRepository
    ''' <summary>
    ''' Gerencia os checkpoints de performance (uso interno ou relatórios).
    ''' </summary>
    Public ReadOnly Property SaldosMensais As ISaldosMensaisRepository Implements IRepositoryManager.SaldosMensais
        Get
            If _saldosMensais Is Nothing Then
                _saldosMensais = New SaldosMensaisRepository(_context)
            End If
            Return _saldosMensais
        End Get
    End Property

End Class
