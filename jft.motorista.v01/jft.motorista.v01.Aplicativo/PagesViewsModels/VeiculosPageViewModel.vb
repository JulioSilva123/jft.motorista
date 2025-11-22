
Imports System.Globalization
Imports System.Windows.Input
Imports jft.motorista.v01.Aplicativo.Common
Imports jft.motorista.v01.Core.Entities
Imports jft.motorista.v01.Infra.Interfaces
Imports Xamarin.Forms




Namespace PagesViewsModels

    ''' <summary>
    ''' ViewModel para Cadastro e Edição de Veículos.
    ''' Gerencia modelo, placa, odômetro inicial e média de consumo.
    ''' </summary>
    Public Class VeiculosPageViewModel
        Inherits BasePageViewModel

        Private _idEdicao As Integer = 0
        Private ReadOnly _culturaBR As New CultureInfo("pt-BR")

        ' ============================================================
        ' 1. PROPRIEDADES VISUAIS
        ' ============================================================

        Private _tituloPagina As String
        Public Property TituloPagina As String
            Get
                Return _tituloPagina
            End Get
            Set(value As String)
                SetProperty(_tituloPagina, value)
            End Set
        End Property

        ' Nome do Modelo (Ex: Chevrolet Onix)
        Private _modelo As String
        Public Property Modelo As String
            Get
                Return _modelo
            End Get
            Set(value As String)
                SetProperty(_modelo, value)
            End Set
        End Property

        ' Placa (Ex: ABC-1234)
        Private _placa As String
        Public Property Placa As String
            Get
                Return _placa
            End Get
            Set(value As String)
                ' Opcional: Forçar maiúsculas
                SetProperty(_placa, value?.ToUpper())
            End Set
        End Property

        ' KM Atual (String para evitar bugs de digitação)
        Private _kmAtual As String
        Public Property KmAtual As String
            Get
                Return _kmAtual
            End Get
            Set(value As String)
                SetProperty(_kmAtual, value)
            End Set
        End Property

        ' Média de Consumo (String)
        Private _mediaConsumo As String
        Public Property MediaConsumo As String
            Get
                Return _mediaConsumo
            End Get
            Set(value As String)
                SetProperty(_mediaConsumo, value)
            End Set
        End Property

        ' Define se é o carro principal
        Private _isAtivo As Boolean
        Public Property IsAtivo As Boolean
            Get
                Return _isAtivo
            End Get
            Set(value As Boolean)
                SetProperty(_isAtivo, value)
            End Set
        End Property

        ' ============================================================
        ' 2. COMANDOS
        ' ============================================================

        Public Property SalvarCommand As ICommand
        Public Property CancelarCommand As ICommand

        ' ============================================================
        ' 3. CONSTRUTOR
        ' ============================================================

        Public Sub New(repo As IRepositoryManager, Optional itemEditar As Veiculos = Nothing)
            MyBase.New(repo)

            ConfigurarComandos()
            InicializarAsync(itemEditar).SafeFireAndForget()
        End Sub

        Private Sub ConfigurarComandos()
            SalvarCommand = New Command(Async Sub() Await SalvarAsync())

            CancelarCommand = New Command(Async Sub()
                                              Await Application.Current.MainPage.Navigation.PopAsync()
                                          End Sub)
        End Sub

        ' ============================================================
        ' 4. CARREGAMENTO
        ' ============================================================

        Private Async Function InicializarAsync(item As Veiculos) As Task
            IsBusy = True
            Try
                If item Is Nothing Then
                    ' --- MODO NOVO ---
                    TituloPagina = "Novo Veículo"
                    _idEdicao = 0
                    Modelo = String.Empty
                    Placa = String.Empty
                    KmAtual = String.Empty
                    MediaConsumo = String.Empty

                    ' Se for o primeiro carro do sistema, sugerimos marcar como ativo
                    Dim qtd = Await Repo.Veiculos.GetTodasAsync()
                    IsAtivo = (qtd.Count = 0)
                Else
                    ' --- MODO EDIÇÃO ---
                    TituloPagina = "Editar Veículo"
                    _idEdicao = item.id_veiculo
                    Modelo = item.nm_modelo
                    Placa = item.nm_placa
                    IsAtivo = item.fl_ativo

                    ' Formatação Numérica PT-BR
                    ' N0 para KM (sem decimais geralmente) ou N1
                    KmAtual = item.nr_km_atual.ToString("N0", _culturaBR)
                    MediaConsumo = item.nr_media_consumo.ToString("N2", _culturaBR)
                End If
            Finally
                IsBusy = False
            End Try
        End Function

        ' ============================================================
        ' 5. SALVAR
        ' ============================================================

        Private Async Function SalvarAsync() As Task
            If IsBusy Then Return

            ' --- VALIDAÇÕES ---
            If String.IsNullOrWhiteSpace(Modelo) Then
                Await Application.Current.MainPage.DisplayAlert("Atenção", "Digite o modelo do veículo.", "OK")
                Return
            End If

            If String.IsNullOrWhiteSpace(Placa) Then
                Await Application.Current.MainPage.DisplayAlert("Atenção", "Digite a placa do veículo.", "OK")
                Return
            End If

            ' Conversão Numérica Segura
            Dim kmDecimal As Decimal = 0
            Dim mediaDecimal As Decimal = 0

            Dim kmValido = Decimal.TryParse(KmAtual, NumberStyles.Any, _culturaBR, kmDecimal)
            Dim mediaValida = Decimal.TryParse(MediaConsumo, NumberStyles.Any, _culturaBR, mediaDecimal)

            If Not kmValido OrElse kmDecimal < 0 Then
                Await Application.Current.MainPage.DisplayAlert("Atenção", "KM Atual inválido.", "OK")
                Return
            End If

            ' Média é opcional, mas se digitada deve ser válida
            If Not String.IsNullOrEmpty(MediaConsumo) AndAlso (Not mediaValida OrElse mediaDecimal < 0) Then
                Await Application.Current.MainPage.DisplayAlert("Atenção", "Média de consumo inválida.", "OK")
                Return
            End If

            IsBusy = True
            Dim mensagemErro As String = String.Empty

            Try
                Dim model As New Veiculos With {
                    .id_veiculo = _idEdicao,
                    .nm_modelo = Modelo,
                    .nm_placa = Placa,
                    .nr_km_atual = kmDecimal,
                    .nr_media_consumo = mediaDecimal,
                    .fl_ativo = IsAtivo
                }

                ' O Repositório cuida de desativar outros carros se IsAtivo = True
                Await Repo.Veiculos.SalvarAsync(model)

                Await Application.Current.MainPage.Navigation.PopAsync()

            Catch ex As Exception
                mensagemErro = ex.Message
            Finally
                IsBusy = False
            End Try

            If Not String.IsNullOrEmpty(mensagemErro) Then
                Await Application.Current.MainPage.DisplayAlert("Erro", mensagemErro, "OK")
            End If
        End Function

    End Class

End Namespace