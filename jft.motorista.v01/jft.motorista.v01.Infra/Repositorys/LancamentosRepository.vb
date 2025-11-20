

Imports jft.motorista.v01.Core.Entities
Imports jft.motorista.v01.Infra.Common
Imports jft.motorista.v01.Infra.Data
Imports jft.motorista.v01.Infra.Interfaces


Namespace Repositorys

    Public Class LancamentosRepository
        Inherits BaseReporitory
        Implements ILancamentosRepository



        ' Private ReadOnly _context As DbMotorista

        ' Injeção de Dependência do Contexto
        Public Sub New()
            '_context = context
        End Sub

        Public Sub New(context As DbMotorista)
            MyBase.New(context)
        End Sub


        ' --- 1. Busca Unitária (Para Edição) ---
        Public Async Function GetItemAsync(id As Integer) As Task(Of Lancamentos) Implements IBaseRepository(Of Lancamentos).GetItemAsync
            Dim item = Await _context.Lancamentos.Where(Function(x) x.id_lancamento = id).FirstOrDefaultAsync()

            ' Hydration: Busca categoria para exibir ícone corretamente na edição
            If item IsNot Nothing Then
                item.Categoria = Await _context.Categorias.Where(Function(c) c.id_categoria = item.id_categoria).FirstOrDefaultAsync()
            End If
            Return item
        End Function


        ' ============================================================
        ' LEITURA (Extrato com Saldo Linha a Linha)
        ' ============================================================
        Public Async Function GetExtratoDoMesAsync(mes As Integer, ano As Integer) As Task(Of List(Of Lancamentos)) Implements ILancamentosRepository.GetExtratoDoMesAsync

            Dim saldoInicial As Decimal = 0

            ' 1. Lógica para pegar o mês anterior (Checkpoint)
            Dim mesAnterior As Integer = If(mes = 1, 12, mes - 1)
            Dim anoAnterior As Integer = If(mes = 1, ano - 1, ano)

            Dim checkpoint = Await _context.SaldosMensais _
                                           .Where(Function(x) x.nr_mes = mesAnterior AndAlso x.nr_ano = anoAnterior) _
                                           .FirstOrDefaultAsync()
            'Dim checkpoint = Await App.SaldoMensalRepo.GetPorMesAsync(mesAnterior, anoAnterior)


            If checkpoint IsNot Nothing Then
                saldoInicial = checkpoint.vl_saldofinal
            End If

            ' 2. Filtro de Datas
            Dim dataInicio As New DateTime(ano, mes, 1)
            Dim dataFim As DateTime = dataInicio.AddMonths(1).AddTicks(-1)

            ' Busca lançamentos ordenados cronologicamente
            Dim lista = Await _context.Lancamentos _
                                      .Where(Function(l) l.dt_lancamento >= dataInicio AndAlso l.dt_lancamento <= dataFim) _
                                      .OrderBy(Function(l) l.dt_lancamento) _
                                      .ToListAsync()

            ' Cache de categorias para fazer o vínculo
            Dim categorias = Await _context.Categorias.ToListAsync()

            ' 3. Cálculo do Running Balance
            Dim saldoCorrente As Decimal = saldoInicial

            For Each item In lista


                ' Vínculo manual (Hydration)
                ' No VB.NET usamos Function(c) para lambdas
                item.Categoria = categorias.FirstOrDefault(Function(c) c.id_categoria = item.id_categoria)

                ' Determina o tipo (0=Receita, 1=Despesa)
                ' Se a categoria for nula (excluída), assume baseada no sinal do valor
                Dim tipo = If(item.Categoria?.cs_tipo, If(item.vl_lancamento < 0, 1, 0))

                ' --- CORREÇÃO ---
                ' Usamos Math.Abs para garantir a direção correta do cálculo.
                ' Receita: SOMA o valor absoluto.
                ' Despesa: SUBTRAI o valor absoluto.
                If tipo = 0 Then
                    saldoCorrente += Math.Abs(item.vl_lancamento)
                Else
                    saldoCorrente -= Math.Abs(item.vl_lancamento)
                End If

                item.SaldoAposLancamento = saldoCorrente

            Next

            ' Inverte para mostrar o mais recente no topo (Dia 30 -> Dia 1)
            lista.Reverse()

            Return lista

        End Function

        ' ============================================================
        ' ESCRITA (Salvar e Gatilhos)
        ' ============================================================

        Public Async Function SalvarAsync(item As Lancamentos) As Task Implements IBaseRepository(Of Lancamentos).SalvarAsync

            If item.id_lancamento <> 0 Then
                Await _context.Connection.UpdateAsync(item)
            Else
                Await _context.Connection.InsertAsync(item)
            End If

            ' Gatilho: Recalcular Saldos Futuros
            'Await RecalcularCheckpointsAsync()
            Await RecalcularSaldosFuturosAsync(item.dt_lancamento)
        End Function

        Public Async Function DeletarAsync(item As Lancamentos) As Task Implements IBaseRepository(Of Lancamentos).DeletarAsync

            Await _context.Connection.DeleteAsync(item)
            'Await RecalcularCheckpointsAsync()
            Await RecalcularSaldosFuturosAsync(item.dt_lancamento)
        End Function







        ' ============================================================
        ' LÓGICA PRIVADA (Recálculo de Checkpoints)
        ' ============================================================
        Private Async Function RecalcularCheckpointsAsync() As Task

            ' Limpa a tabela auxiliar
            Await _context.Connection.DeleteAllAsync(Of SaldosMensais)()

            ' Pega todos os dados para reconstruir a história
            Dim todos = Await _context.Lancamentos.OrderBy(Function(x) x.dt_lancamento).ToListAsync()
            Dim cats = Await _context.Categorias.ToListAsync()

            If Not todos.Any() Then Return

            Dim saldoAcumulado As Decimal = 0
            Dim fechamentos As New Dictionary(Of String, Decimal)

            For Each item In todos
                Dim cat = cats.FirstOrDefault(Function(c) c.id_categoria = item.id_categoria)

                If cat IsNot Nothing AndAlso cat.cs_tipo = 0 Then
                    saldoAcumulado += item.vl_lancamento
                Else
                    saldoAcumulado -= item.vl_lancamento
                End If

                ' Chave: "2025-11"
                Dim chave As String = $"{item.dt_lancamento.Year}-{item.dt_lancamento.Month}"

                ' No VB, a atribuição de dicionário é igual ao C#
                fechamentos(chave) = saldoAcumulado

            Next

            ' Salva na tabela auxiliar
            Dim listaCheckpoints As New List(Of SaldosMensais)

            For Each kvp In fechamentos

                Dim partes = kvp.Key.Split("-"c)

                listaCheckpoints.Add(New SaldosMensais With {
                    .nr_ano = Integer.Parse(partes(0)),
                    .nr_mes = Integer.Parse(partes(1)),
                    .vl_saldofinal = kvp.Value
                })
            Next

            Await _context.Connection.InsertAllAsync(listaCheckpoints)

        End Function









        Private Async Function RecalcularSaldosFuturosAsync(dataAlteracao As DateTime) As Task


            Dim RepositoryManager As New RepositoryManager

            Dim dataInicio As New DateTime(dataAlteracao.Year, dataAlteracao.Month, 1)

            ' Busca saldo base
            Dim mesAnt = If(dataInicio.Month = 1, 12, dataInicio.Month - 1)
            Dim anoAnt = If(dataInicio.Month = 1, dataInicio.Year - 1, dataInicio.Year)
            Dim saldoAcumulado As Decimal = 0

            Dim checkpointBase = Await RepositoryManager.SaldosMensais.GetPorMesAsync(mesAnt, anoAnt)

            '' Busca na tabela auxiliar o último valor válido (Checkpoint)
            'Dim checkpointBase = Await _context.SaldosMensais _
            '                                   .Where(Function(x) x.nr_mes = mesAnt AndAlso x.nr_ano = anoAnt) _
            '                                   .FirstOrDefaultAsync()


            ' If check IsNot Nothing Then saldoAcumulado = check.SaldoFinal

            'Dim checkpointBase = Await App.SaldoMensalRepo.GetPorMesAsync(mesAnterior, anoAnterior)
            If checkpointBase IsNot Nothing Then
                saldoAcumulado = checkpointBase.vl_saldofinal
            End If

            ' 3. Busca TODOS os lançamentos do dia 1º em diante (Futuro)
            ' Precisamos recalcular tudo pra frente, pois um gasto hoje diminui o saldo de dezembro, janeiro, etc.
            Dim lancamentosFuturos = Await _context.Lancamentos _
                                                   .Where(Function(x) x.dt_lancamento >= dataInicio) _
                                                   .OrderBy(Function(x) x.dt_lancamento) _
                                                   .ToListAsync()

            Dim categorias = Await _context.Categorias.ToListAsync()

            ' Dicionário para guardar os novos fechamentos mensais
            Dim novosFechamentos As New Dictionary(Of String, Decimal)

            For Each item In lancamentosFuturos
                Dim cat = categorias.FirstOrDefault(Function(c) c.id_categoria = item.id_categoria)
                Dim tipo = If(cat?.cs_tipo, If(item.vl_lancamento < 0, 1, 0))

                If tipo = 0 Then
                    saldoAcumulado += Math.Abs(item.vl_lancamento)
                Else
                    saldoAcumulado -= Math.Abs(item.vl_lancamento)
                End If
                novosFechamentos($"{item.dt_lancamento.Year}-{item.dt_lancamento.Month}") = saldoAcumulado
            Next

            For Each kvp In novosFechamentos
                Dim p = kvp.Key.Split("-"c)
                Await RepositoryManager.SaldosMensais.SalvarOuAtualizarAsync(Integer.Parse(p(1)), Integer.Parse(p(0)), kvp.Value)
            Next
            'If checkExistente IsNot Nothing Then
            '    ' UPDATE: Atualiza o valor existente
            '    checkExistente.vl_saldofinal = saldoFinal
            '    Await _context.Connection.UpdateAsync(checkExistente)
            'Else
            '    ' INSERT: Cria um novo fechamento
            '    Dim novoCheck As New SaldosMensais With {
            '            .nr_mes = mesChave,
            '            .nr_ano = anoChave,
            '            .vl_saldofinal = saldoFinal
            '        }
            '    Await _context.Connection.InsertAsync(novoCheck)
            'End If



            Exit Function




            '' 1. Define o dia 1º do mês da alteração como ponto de partida
            '' Ex: Se alterou dia 20/11, começamos a recalcular dia 01/11
            'Dim dataInicio As New DateTime(dataAlteracao.Year, dataAlteracao.Month, 1)

            '' 2. Busca o Saldo Base (O fechamento do mês ANTERIOR ao início)
            'Dim saldoAcumulado As Decimal = 0

            'Dim mesAnterior As Integer = If(dataInicio.Month = 1, 12, dataInicio.Month - 1)
            'Dim anoAnterior As Integer = If(dataInicio.Month = 1, dataInicio.Year - 1, dataInicio.Year)

            '' Busca na tabela auxiliar o último valor válido (Checkpoint)
            'Dim checkpointBase = Await _context.SaldosMensais _
            '                                   .Where(Function(x) x.nr_mes = mesAnterior AndAlso x.nr_ano = anoAnterior) _
            '                                   .FirstOrDefaultAsync()
            ''Dim checkpointBase = Await App.SaldoMensalRepo.GetPorMesAsync(mesAnterior, anoAnterior)
            'If checkpointBase IsNot Nothing Then
            '    saldoAcumulado = checkpointBase.vl_saldofinal
            'End If

            '' 3. Busca TODOS os lançamentos do dia 1º em diante (Futuro)
            '' Precisamos recalcular tudo pra frente, pois um gasto hoje diminui o saldo de dezembro, janeiro, etc.
            'Dim lancamentosFuturos = Await _context.Lancamentos _
            '                                       .Where(Function(x) x.dt_lancamento >= dataInicio) _
            '                                       .OrderBy(Function(x) x.dt_lancamento) _
            '                                       .ToListAsync()

            'Dim categorias = Await _context.Categorias.ToListAsync()

            '' Dicionário para guardar os novos fechamentos mensais
            'Dim novosFechamentos As New Dictionary(Of String, Decimal)

            '' 4. Recalcula o Saldo Linha a Linha
            'For Each item In lancamentosFuturos
            '    ' Vincula categoria para saber se soma ou subtrai
            '    Dim cat = categorias.FirstOrDefault(Function(c) c.id_categoria = item.id_categoria)
            '    Dim tipo = If(cat IsNot Nothing, cat.cs_tipo, If(item.Valor < 0, 1, 0))

            '    If tipo = 0 Then
            '        saldoAcumulado += item.Valor
            '    Else
            '        saldoAcumulado -= item.Valor
            '    End If

            '    ' Atualiza o fechamento deste mês no dicionário
            '    ' A chave garante que pegaremos sempre o ÚLTIMO valor do mês (fechamento)
            '    Dim chave As String = $"{item.dt_lancamento.Year}-{item.dt_lancamento.Month}"
            '    novosFechamentos(chave) = saldoAcumulado
            'Next




            ''' 5. Salva os novos checkpoints usando o Repositório de Saldo
            ''For Each kvp In novosFechamentos
            ''    Dim partes = kvp.Key.Split("-"c)
            ''    Dim ano = Integer.Parse(partes(0))
            ''    Dim mes = Integer.Parse(partes(1))

            ''    ' INTEGRAÇÃO: Delega o salvamento para quem entende disso
            ''    Await App.SaldoMensalRepo.SalvarOuAtualizarAsync(mes, ano, kvp.Value)
            ''Next



            '' 5. Salva os novos checkpoints no Banco
            '' Usamos uma lista para fazer operações de banco
            'For Each kvp In novosFechamentos


            '    Dim partes = kvp.Key.Split("-"c)
            '    Dim anoChave = Integer.Parse(partes(0))
            '    Dim mesChave = Integer.Parse(partes(1))
            '    Dim saldoFinal = kvp.Value

            '    ' Verifica se já existe checkpoint para esse mês
            '    Dim checkExistente = Await _context.SaldosMensais _
            '                                       .Where(Function(x) x.nr_mes = mesChave AndAlso x.nr_mes = anoChave) _
            '                                       .FirstOrDefaultAsync()

            '    If checkExistente IsNot Nothing Then
            '        ' UPDATE: Atualiza o valor existente
            '        checkExistente.vl_saldofinal = saldoFinal
            '        Await _context.Connection.UpdateAsync(checkExistente)
            '    Else
            '        ' INSERT: Cria um novo fechamento
            '        Dim novoCheck As New SaldosMensais With {
            '            .nr_mes = mesChave,
            '            .nr_ano = anoChave,
            '            .vl_saldofinal = saldoFinal
            '        }
            '        Await _context.Connection.InsertAsync(novoCheck)
            '    End If




            'Next

        End Function



    End Class

End Namespace
