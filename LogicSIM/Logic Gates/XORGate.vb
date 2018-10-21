﻿Partial Public Class LogicGates
    Public Class XORGate
        Inherits BaseGate

        Public Overrides ReadOnly Property GateType As IBaseGate.GateTypes
            Get
                Return IBaseGate.GateTypes.XOR
            End Get
        End Property

        Protected Friend Overrides Sub Evaluate()
            Dim result As Boolean = Inputs(0).Value
            For i As Integer = 1 To Inputs.Count - 1
                result = result Xor Inputs(i).Value
            Next

            Output.Value = result
        End Sub

        Protected Overrides Sub InitializeInputs()
            Name = "XOR"
            For i As Integer = 0 To 2 - 1
                Inputs.Add(New Pin(Me))
            Next
        End Sub

        Public Shared Function FromXML(xml As XElement) As BaseGate
            Dim g As New XORGate()
            g.SetBaseFromXML(xml)
            Return g
        End Function

        Public Overrides Function Clone() As Object
            Return XORGate.FromXML(Me.ToXML())
        End Function

        Public Overrides ReadOnly Property Flow As IBaseGate.DataFlow
            Get
                Return IBaseGate.DataFlow.InOut
            End Get
        End Property
    End Class
End Class