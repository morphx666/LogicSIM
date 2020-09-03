﻿Partial Public Class LogicGates
    Public Class XNORGate
        Inherits BaseGate

        Public Overrides ReadOnly Property GateType As IBaseGate.GateTypes
            Get
                Return IBaseGate.GateTypes.XNOR
            End Get
        End Property

        Protected Friend Overrides Sub Evaluate()
            Dim result As Boolean = Inputs(0).Value
            For i As Integer = 1 To Inputs.Count - 1
                result = result Xor Inputs(i).Value
            Next

            Output.Value = Not result
        End Sub

        Protected Overrides Sub InitializeInputs()
            For i As Integer = 0 To 2 - 1
                Inputs.Add(New Pin(Me, Inputs.Count))
            Next
        End Sub

        Public Shared Function FromXML(xml As XElement, Optional resetID As Boolean = False) As BaseGate
            Dim g As New XNORGate()
            g.SetBaseFromXML(xml, resetID)
            Return g
        End Function

        Public Overrides Function Clone() As Object
            Return XORGate.FromXML(Me.ToXML(), True)
        End Function

        Public Overrides ReadOnly Property Flow As IBaseGate.DataFlow
            Get
                Return IBaseGate.DataFlow.InOut
            End Get
        End Property
    End Class
End Class