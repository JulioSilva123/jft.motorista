


Imports jft.motorista.v01.Infra.Data




Namespace Common


    Public MustInherit Class BaseReporitory





        Friend ReadOnly _context As DbMotorista

        ' Injeção de Dependência do Contexto
        Public Sub New()
            _context = New DbMotorista
        End Sub

        Public Sub New(context As DbMotorista)
            _context = context
        End Sub





    End Class


End Namespace