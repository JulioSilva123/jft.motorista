Imports System.Globalization
Imports Xamarin.Forms

Namespace Converters




    ''' <summary>
    ''' Conversor de Valor para Booleano.
    ''' Retorna True se o valor (Decimal) for negativo.
    ''' Usado para acionar Triggers de cor no XAML.
    ''' </summary>
    Public Class SaldoNegativoConverter
        Implements IValueConverter

        ''' <summary>
        ''' Da ViewModel para a View (Leitura)
        ''' </summary>
        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert

            ' Verifica se o valor é um número Decimal válido
            If TypeOf value Is Decimal Then
                Dim valor = DirectCast(value, Decimal)

                ' Regra de Negócio Visual: É negativo?
                Return valor < 0
            End If

            ' Se não for número, assume falso
            Return False
        End Function

        ''' <summary>
        ''' Da View para a ViewModel (Não utilizado neste caso)
        ''' </summary>
        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
            Throw New NotImplementedException("Este conversor é apenas de leitura (OneWay).")
        End Function

    End Class


End Namespace