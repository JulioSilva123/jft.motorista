
Imports Xamarin.Forms

Namespace Behaviors

    ''' <summary>
    ''' Behavior para entrada monetária (R$).
    ''' Aceita números, vírgula e sinal de menos (negativo) no início.
    ''' </summary>
    Public Class CurrencyInputBehavior
        Inherits Behavior(Of Entry)

        ' Flag para evitar loops infinitos
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

            If _isValidating Then Return
            If args.NewTextValue = args.OldTextValue Then Return

            If String.IsNullOrEmpty(args.NewTextValue) Then Return

            _isValidating = True
            Try
                Dim entry = DirectCast(sender, Entry)
                Dim texto = args.NewTextValue
                Dim valido = True

                ' 1. Auto-Correção: Troca Ponto por Vírgula
                If texto.Contains(".") Then
                    texto = texto.Replace(".", ",")
                    entry.Text = texto
                    ' O set dispara o evento de novo, mas o _isValidating protege
                    Return
                End If

                ' 2. Regra: Caracteres Permitidos (Números, Vírgula, Menos)
                For i As Integer = 0 To texto.Length - 1
                    Dim c = texto(i)

                    If Char.IsDigit(c) Then Continue For

                    If c = ","c Then Continue For

                    ' Permite sinal de Menos APENAS na primeira posição
                    If c = "-"c AndAlso i = 0 Then Continue For

                    ' Se chegou aqui, é inválido
                    valido = False
                    Exit For
                Next

                ' 3. Regra: Máximo 1 vírgula
                Dim qtdVirgulas = texto.Count(Function(c) c = ","c)
                If qtdVirgulas > 1 Then
                    valido = False
                End If

                ' 4. Regra: Máximo 1 sinal de menos
                Dim qtdMenos = texto.Count(Function(c) c = "-"c)
                If qtdMenos > 1 Then
                    valido = False
                End If

                ' SE INVÁLIDO: Reverte
                If Not valido Then
                    entry.Text = args.OldTextValue
                End If

            Finally
                _isValidating = False
            End Try
        End Sub

    End Class

End Namespace