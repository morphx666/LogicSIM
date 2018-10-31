Partial Public Class LogicGates
    Public Class ANDGate
        Inherits BaseGate

        Public Overrides ReadOnly Property GateType As IBaseGate.GateTypes
            Get
                Return IBaseGate.GateTypes.AND
            End Get
        End Property

        Protected Friend Overrides Sub Evaluate()
            Dim result As Boolean = Inputs(0).Value
            For i As Integer = 1 To Inputs.Count - 1
                result = result And Inputs(i).Value
            Next

            Output.Value = result
        End Sub

        Protected Overrides Sub InitializeInputs()
            For i As Integer = 0 To 2 - 1
                Inputs.Add(New Pin(Me, Inputs.Count))
            Next
        End Sub

        Protected Friend Overrides Sub Tick()
        End Sub

        Public Shared Function FromXML(xml As XElement, Optional resetID As Boolean = False) As BaseGate
            Dim g As New ANDGate()
            g.SetBaseFromXML(xml, resetID)
            Return g
        End Function

        Private Shared Sub test()

        End Sub

        Public Overrides Function Clone() As Object
            Return ANDGate.FromXML(Me.ToXML(), True)
        End Function

        Public Overrides ReadOnly Property Flow As IBaseGate.DataFlow
            Get
                Return IBaseGate.DataFlow.InOut
            End Get
        End Property
    End Class
End Class