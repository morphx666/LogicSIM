Partial Public Class LogicGates
    Public Class NOTGate
        Inherits BaseGate

        Public Overrides ReadOnly Property GateType As IBaseGate.GateTypes
            Get
                Return IBaseGate.GateTypes.NOT
            End Get
        End Property

        Protected Friend Overrides Sub Evaluate()
            Output.Value = Not Inputs(0).Value
        End Sub

        Protected Overrides Sub InitializeInputs()
            Inputs.Add(New Pin(Me, Inputs.Count))
        End Sub

        Public Shared Function FromXML(xml As XElement, Optional resetID As Boolean = False) As BaseGate
            Dim g As New NOTGate()
            g.SetBaseFromXML(xml, resetID)
            Return g
        End Function

        Public Overrides Function Clone() As Object
            Return NOTGate.FromXML(Me.ToXML(), True)
        End Function

        Public Overrides ReadOnly Property Flow As IBaseGate.DataFlow
            Get
                Return IBaseGate.DataFlow.InOut
            End Get
        End Property
    End Class
End Class