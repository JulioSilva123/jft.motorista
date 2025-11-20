

Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports jft.motorista.v01.Infra.Interfaces


Namespace Common



    Public Class BasePageViewModel
        Implements INotifyPropertyChanged


        'Inherits BaseViewModel

        ' Acesso protegido ao Repositório (apenas as classes filhas enxergam)
        Protected ReadOnly Repo As IRepositoryManager

        ''' <summary>
        ''' Construtor Obrigatório: Exige que quem herdar passe o Repositório.
        ''' Garante que nenhuma página seja criada sem acesso aos dados.
        ''' </summary>
        Public Sub New(repo As IRepositoryManager)
            If repo Is Nothing Then Throw New ArgumentNullException(NameOf(repo))
            Me.Repo = repo
        End Sub





        ' Evento padrão do MVVM
        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        ' ============================================================
        ' 1. PROPRIEDADES COMUNS (Estado da Tela)
        ' ============================================================

        Private _isBusy As Boolean
        ''' <summary>
        ''' Indica se a tela está processando algo (mostra Loading/Spinner).
        ''' </summary>
        Public Property IsBusy As Boolean
            Get
                Return _isBusy
            End Get
            Set(value As Boolean)
                If SetProperty(_isBusy, value) Then
                    ' Notifica também a propriedade inversa automaticamente
                    OnPropertyChanged(NameOf(IsNotBusy))
                End If
            End Set
        End Property

        ''' <summary>
        ''' Inverso do IsBusy. Útil para a propriedade IsEnabled de botões.
        ''' (Se está ocupado, botão fica desabilitado).
        ''' </summary>
        Public ReadOnly Property IsNotBusy As Boolean
            Get
                Return Not IsBusy
            End Get
        End Property

        Private _title As String = String.Empty
        ''' <summary>
        ''' Título da página (vinculado na NavigationBar).
        ''' </summary>
        Public Property Title As String
            Get
                Return _title
            End Get
            Set(value As String)
                SetProperty(_title, value)
            End Set
        End Property

        ' ============================================================
        ' 2. HELPER DE NOTIFICAÇÃO (O "SetProperty")
        ' ============================================================

        ''' <summary>
        ''' Método mágico que atualiza o valor, verifica se mudou e avisa a tela.
        ''' Retorna True se houve mudança.
        ''' </summary>
        ''' <typeparam name="T">O tipo da propriedade</typeparam>
        ''' <param name="backingStore">A variável privada (ex: _nome)</param>
        ''' <param name="value">O novo valor</param>
        ''' <param name="propertyName">O nome da propriedade (automático)</param>
        ''' <param name="onChanged">Ação opcional para rodar após mudar</param>
        Protected Function SetProperty(Of T)(ByRef backingStore As T,
                                         value As T,
                                         <CallerMemberName> Optional propertyName As String = "",
                                         Optional onChanged As Action = Nothing) As Boolean

            ' Se o valor for igual ao atual, não faz nada (economiza processamento)
            If EqualityComparer(Of T).Default.Equals(backingStore, value) Then
                Return False
            End If

            backingStore = value
            onChanged?.Invoke()

            ' Dispara o evento que a tela escuta
            OnPropertyChanged(propertyName)
            Return True
        End Function

        ''' <summary>
        ''' Dispara o evento de mudança manualmente.
        ''' </summary>
        Protected Sub OnPropertyChanged(<CallerMemberName> Optional propertyName As String = "")
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        End Sub

        ' ============================================================
        ' 3. CICLO DE VIDA (Opcional mas Recomendado)
        ' ============================================================

        ''' <summary>
        ''' Método virtual para inicialização assíncrona (já que construtores não podem ser Async).
        ''' Pode ser chamado no OnAppearing da Page.
        ''' </summary>
        Public Overridable Function InitializeAsync(parameter As Object) As Task
            Return Task.CompletedTask
        End Function
    End Class


End Namespace