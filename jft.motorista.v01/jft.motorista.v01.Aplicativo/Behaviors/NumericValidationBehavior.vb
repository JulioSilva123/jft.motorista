
Imports System.Text.RegularExpressions
Imports Xamarin.Forms

Namespace Behaviors

    ''' <summary>
    ''' Behavior que impede a digitação de caracteres não numéricos em um Entry.
    ''' Aceita apenas números, vírgula e ponto.
    ''' </summary>
    Public Class NumericValidationBehavior
        Inherits Behavior(Of Entry)

        ''' <summary>
        ''' Chamado quando o Behavior é adicionado ao controle (Entry).
        ''' </summary>
        Protected Overrides Sub OnAttachedTo(entry As Entry)
            AddHandler entry.TextChanged, AddressOf OnEntryTextChanged
            MyBase.OnAttachedTo(entry)
        End Sub

        ''' <summary>
        ''' Chamado quando o Behavior é removido (limpeza de memória).
        ''' </summary>
        Protected Overrides Sub OnDetachingFrom(entry As Entry)
            RemoveHandler entry.TextChanged, AddressOf OnEntryTextChanged
            MyBase.OnDetachingFrom(entry)
        End Sub

        ''' <summary>
        ''' Lógica de interceptação da digitação.
        ''' </summary>
        Private Sub OnEntryTextChanged(sender As Object, args As TextChangedEventArgs)

            ' Se o campo estiver vazio, permite (para poder apagar tudo)
            If String.IsNullOrWhiteSpace(args.NewTextValue) Then Return

            ' REGEX:
            ' ^       : Começo da linha
            ' [0-9]* : Zero ou mais dígitos
            ' ([.,]?  : Um grupo opcional que contém Ponto OU Vírgula
            ' [0-9]*)?: Seguido de mais dígitos opcionais
            ' $       : Fim da linha
            Dim isValid As Boolean = Regex.IsMatch(args.NewTextValue, "^[0-9]*([.,]?[0-9]*)?$")

            ' Se não for válido (ex: digitou letra 'a' ou duas vírgulas '10,,5'), 
            ' volta ao texto anterior imediatamente.
            If Not isValid Then

                Dim entry = DirectCast(sender, Entry)

                Debug.Print(args.NewTextValue)
                Debug.Print(args.OldTextValue)


                entry.Text = args.OldTextValue

            End If

        End Sub

    End Class

End Namespace