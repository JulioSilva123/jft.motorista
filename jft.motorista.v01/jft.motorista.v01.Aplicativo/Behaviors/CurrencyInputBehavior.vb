
Imports Xamarin.Forms

Namespace Behaviors

    ''' <summary>
    ''' Behavior para entrada monetária (R$).
    ''' Garante que apenas números e uma única vírgula sejam aceitos.
    ''' Não altera a posição do cursor.
    ''' </summary>
    Public Class CurrencyInputBehavior
        Inherits Behavior(Of Entry)

        ' Flag para evitar loops infinitos (Reentrância)
        Private _isValidating As Boolean = False

        Protected Overrides Sub OnAttachedTo(entry As Entry)
            AddHandler entry.TextChanged, AddressOf OnEntryTextChanged
            MyBase.OnAttachedTo(entry)
        End Sub

        Protected Overrides Sub OnDetachingFrom(entry As Entry)
            RemoveHandler entry.TextChanged, AddressOf OnEntryTextChanged
            MyBase.OnDetachingFrom(entry)
        End Sub

        Private Sub OnEntryTextChanged(sender As Object, args As TextChangedEventArgs)

            ' Se já estamos validando, sai para não criar loop
            If _isValidating Then Return

            ' Se o texto não mudou de verdade (mudança de foco/binding nulo), sai
            If args.NewTextValue = args.OldTextValue Then Return

            If String.IsNullOrEmpty(args.NewTextValue) Then Return

            _isValidating = True
            Try
                Dim entry = DirectCast(sender, Entry)
                Dim texto = args.NewTextValue
                Dim valido = True

                ' 1. Regra: Apenas Números, Vírgula e Ponto
                For Each c In texto
                    If Not Char.IsDigit(c) AndAlso c <> ","c AndAlso c <> "."c Then
                        valido = False
                        Exit For
                    End If
                Next

                ' 2. Regra: Máximo 1 separador (vírgula ou ponto)
                Dim qtdSeparadores = texto.Count(Function(c) c = ","c OrElse c = "."c)
                If qtdSeparadores > 1 Then
                    valido = False
                End If

                ' SE INVÁLIDO: Reverte e sai
                If Not valido Then
                    entry.Text = args.OldTextValue
                    Return ' O set acima dispara o evento de novo, mas o finally libera a flag
                End If

                ' 3. Auto-Correção: Troca Ponto por Vírgula (para teclados US/Numéricos)
                ' Isso ajuda muito no Android onde o teclado numérico só tem ponto
                If texto.Contains(".") Then
                    Dim textoCorrigido = texto.Replace(".", ",")

                    ' Só aplica se for diferente para evitar loop desnecessário
                    If textoCorrigido <> texto Then
                        entry.Text = textoCorrigido
                    End If
                End If

            Finally
                _isValidating = False
            End Try
        End Sub

    End Class

End Namespace