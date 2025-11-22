Imports System.IO
Imports System.Runtime.CompilerServices
Imports jft.motorista.v01.Core.Entities
Imports SQLite
Imports Xamarin.Essentials ' Necessário para guardar a versão atual


Namespace Data



    Public Class DbMotorista

        Public Sub New()
            ' 1. Definição do Caminho do Banco
            Dim dbPath As String =
                Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.LocalApplicationData), Constantes.DatabaseFilename)

            '"GCDriver.db3"

            ' 2. Configurações de Performance e Acesso
            ' Em VB.NET usamos "Or" para combinar as Flags bit a bit
            Dim flags As SQLiteOpenFlags = SQLiteOpenFlags.ReadWrite Or
                                       SQLiteOpenFlags.Create Or
                                       SQLiteOpenFlags.SharedCache

            flags = Constantes.Flags




            Connection = New SQLiteAsyncConnection(dbPath, flags)

            ' 3. Inicialização (Criação de Tabelas)
            ' Rodamos em background (SafeFireAndForget) para não travar o construtor
            Initialize().SafeFireAndForget()

        End Sub

        ' Defina qual é a versão ATUAL do seu banco de dados aqui
        ' Sempre que você alterar uma entidade (adicionar campo), aumente esse número.
        Private Const VERSAO_ATUAL_DB As Integer = 2
        Public Property Connection As SQLiteAsyncConnection  '{ get; private set; }


        ' ============================================================
        ' PROPRIEDADES DE ACESSO (Estilo Entity Framework / DbSet)
        ' ============================================================
        ' Em VB.NET usamos ReadOnly Property para simular o getter (=>) do C#

        Public ReadOnly Property Lancamentos As AsyncTableQuery(Of Lancamentos)
            Get
                Return Connection.Table(Of Lancamentos)()
            End Get
        End Property

        Public ReadOnly Property Categorias As AsyncTableQuery(Of Categorias)
            Get
                Return Connection.Table(Of Categorias)()
            End Get
        End Property

        Public ReadOnly Property SaldosMensais As AsyncTableQuery(Of SaldosMensais)
            Get
                Return Connection.Table(Of SaldosMensais)()
            End Get
        End Property

        Public ReadOnly Property Veiculos As AsyncTableQuery(Of Veiculos)
            Get
                Return Connection.Table(Of Veiculos)()
            End Get
        End Property

        Public ReadOnly Property Diarios As AsyncTableQuery(Of Diarios)
            Get
                Return Connection.Table(Of Diarios)()
            End Get
        End Property

        Public ReadOnly Property Abastecimentos As AsyncTableQuery(Of Abastecimentos)
            Get
                Return Connection.Table(Of Abastecimentos)()
            End Get
        End Property


        ' ============================================================
        ' CONFIGURAÇÃO E SEED (Carga Inicial)
        ' ============================================================

        Private Async Function Initialize() As Task
            ' CreateFlags.AllImplicit: Ajusta a tabela automaticamente se a classe mudar
            'Await Connection.CreateTableAsync(Of Categorias)(CreateFlags.AllImplicit)
            'Await Connection.CreateTableAsync(Of Lancamentos)(CreateFlags.AllImplicit)
            'Await Connection.CreateTableAsync(Of SaldosMensais)(CreateFlags.AllImplicit)


            Await Connection.CreateTableAsync(Of Categorias)()
            Await Connection.CreateTableAsync(Of Lancamentos)()
            Await Connection.CreateTableAsync(Of SaldosMensais)()
            Await Connection.CreateTableAsync(Of Veiculos)()
            Await Connection.CreateTableAsync(Of Diarios)()
            Await Connection.CreateTableAsync(Of Abastecimentos)()

            ' 2. Carga de Dados Padrão
            Await SeedCategoriasAsync()

            ' 3. MIGRAÇÃO MANUAL (Para garantir atualizações complexas)
            Await VerificarMigracoesAsync()
        End Function


        Private Async Function VerificarMigracoesAsync() As Task
            ' Pega a versão que está instalada no celular do usuário (Padrão 0 se nunca instalou)
            Dim versaoInstalada = Preferences.Get("DB_VERSION", 0)

            If versaoInstalada < VERSAO_ATUAL_DB Then

                ' --- MIGRAÇÃO PARA VERSÃO 2 (Exemplo: Adicionamos id_lancamento em Abastecimentos) ---
                If versaoInstalada < 2 Then
                    Try
                        ' Tenta adicionar a coluna manualmente caso o CreateTableAsync tenha falhado
                        ' O SQLite não dá erro se a coluna já existir usando "ADD COLUMN" em algumas versões,
                        ' mas para garantir, colocamos num Try/Catch vazio ou verificamos antes.

                        ' Exemplo de comando SQL direto para garantir a estrutura:
                        ' Await Connection.ExecuteAsync("ALTER TABLE Abastecimentos ADD COLUMN id_lancamento INTEGER DEFAULT 0")

                        ' Como usamos CreateFlags.AllImplicit acima, ele JÁ DEVE ter criado.
                        ' Mas se você renomeou uma coluna (Ex: "Valor" para "vl_lancamento"), 
                        ' o SQLite cria a nova e deixa a velha. Aqui você poderia migrar os dados:
                        ' Await Connection.ExecuteAsync("UPDATE Lancamentos SET vl_lancamento = Valor WHERE vl_lancamento IS NULL")

                    Catch ex As Exception
                        ' Logar erro de migração silencioso
                        Console.WriteLine($"Erro na Migração V2: {ex.Message}")
                    End Try
                End If

                ' --- FUTURA MIGRAÇÃO PARA VERSÃO 3 ---
                ' If versaoInstalada < 3 Then ...

                ' Atualiza a versão no celular para não rodar de novo
                Preferences.Set("DB_VERSION", VERSAO_ATUAL_DB)
            End If
        End Function
        Private Async Function SeedCategoriasAsync() As Task
            ' Verifica se a tabela de categorias está vazia
            If Await Categorias.CountAsync() = 0 Then

                ' A sintaxe de inicialização de lista em VB.NET usa "From" e "With"
                Dim padroes As New List(Of Categorias) From {
                New Categorias With {.nm_categoria = "Uber", .cs_tipo = 0, .te_icone = "fa-uber", .te_CorHex = "#000000"},
                New Categorias With {.nm_categoria = "99", .cs_tipo = 0, .te_icone = "fa-taxi", .te_CorHex = "#FFCC00"},
                New Categorias With {.nm_categoria = "InDriver", .cs_tipo = 0, .te_icone = "fa-car", .te_CorHex = "#00FF00"},
                New Categorias With {.nm_categoria = "Particular", .cs_tipo = 0, .te_icone = "fa-user", .te_CorHex = "#0000FF"},
                New Categorias With {.nm_categoria = "Combustível", .cs_tipo = 1, .te_icone = "fa-gas-pump", .te_CorHex = "#FF0000"},
                New Categorias With {.nm_categoria = "Alimentação", .cs_tipo = 1, .te_icone = "fa-utensils", .te_CorHex = "#FFA500"},
                New Categorias With {.nm_categoria = "Manutenção", .cs_tipo = 1, .te_icone = "fa-wrench", .te_CorHex = "#808080"},
                New Categorias With {.nm_categoria = "Lava-jato", .cs_tipo = 1, .te_icone = "fa-water", .te_CorHex = "#00BFFF"},
                New Categorias With {.nm_categoria = "Seguro/IPVA", .cs_tipo = 1, .te_icone = "fa-file-invoice-dollar", .te_CorHex = "#8B0000"}
            }

                Await Connection.InsertAllAsync(padroes)
            End If
        End Function


    End Class


    ' ============================================================
    ' MÓDULO DE EXTENSÃO (HELPER)
    ' ============================================================
    ' Em VB.NET, métodos de extensão devem ficar dentro de um Module
    Public Module TaskExtensions

        <Extension()>
        Public Sub SafeFireAndForget(ByVal task As Task)
            task.ContinueWith(Sub(t)
                                  If t.IsFaulted Then
                                      ' Em produção, logar o erro aqui
                                      System.Diagnostics.Debug.WriteLine($"[DB ERROR] Inicialização falhou: {t.Exception}")
                                  End If
                              End Sub)
        End Sub
    End Module


End Namespace