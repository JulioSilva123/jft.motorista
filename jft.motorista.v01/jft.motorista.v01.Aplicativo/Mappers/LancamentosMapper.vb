Imports System.Collections.ObjectModel
Imports System.Runtime.CompilerServices
Imports jft.motorista.v01.Core.Entities
Imports jft.motorista.v01.Core.EntitiesViews

Namespace Mappers


    ''' <summary>
    ''' Módulo de Extensão responsável por converter Models em ViewModels e vice-versa.
    ''' Evita poluição de código dentro das ViewModels principais.
    ''' </summary>
    Public Module LancamentosMapper

        ' ============================================================
        ' 1. MODEL -> VIEWMODEL (Leitura)
        ' ============================================================

        ''' <summary>
        ''' Converte um único Model em sua representação visual (ViewModel).
        ''' Uso: Dim vm = meuModel.ToViewModel()
        ''' </summary>
        <Extension()>
        Public Function ToViewModel(model As Lancamentos) As LancamentosViewModel
            If model Is Nothing Then
                Return Nothing
            End If

            ' Chama o construtor do Wrapper que criamos antes
            Return New LancamentosViewModel(model)
        End Function

        ''' <summary>
        ''' Converte uma LISTA inteira de Models para uma ObservableCollection de ViewModels.
        ''' Ideal para preencher listas de tela (CollectionView/ListView).
        ''' </summary>
        <Extension()>
        Public Function ToViewModels(models As IEnumerable(Of Lancamentos)) As ObservableCollection(Of LancamentosViewModel)
            If models Is Nothing Then
                Return New ObservableCollection(Of LancamentosViewModel)()
            End If

            ' Usa LINQ para projetar cada item e cria a coleção observável
            Dim listaConvertida = models.Select(Function(m) m.ToViewModel())

            Return New ObservableCollection(Of LancamentosViewModel)(listaConvertida)

        End Function

        ' ============================================================
        ' 2. VIEWMODEL -> MODEL (Escrita/Persistência)
        ' ============================================================

        ''' <summary>
        ''' Extrai o Model original de dentro da ViewModel.
        ''' Útil quando você tem o item da tela e quer salvar no repositório.
        ''' </summary>
        <Extension()>
        Public Function ToModel(vm As LancamentosViewModel) As Lancamentos
            If vm Is Nothing Then
                Return Nothing
            End If
            Return vm.Model
        End Function

    End Module


End Namespace