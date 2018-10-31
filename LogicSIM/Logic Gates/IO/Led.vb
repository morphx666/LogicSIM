Partial Public Class LogicGates
    Public Class Led
        Inherits BaseGate

        Public Sub New()
            MyBase.New()

            UI.Size = New Size(28, 28)
            UI.FillColor = Color.DarkGreen
            UI.ActiveColor = Color.LightGreen

            Inputs(0).UI.X = -UI.Width / 2
            Inputs(0).UI.Y = UI.Height / 2 - Inputs(0).UI.Height / 2

            UI.NameOffset = New Point(0, UI.Height)
        End Sub

        Public Overrides Function Clone() As Object
            Return Led.FromXML(Me.ToXML(), True)
        End Function

        Protected Friend Overrides Sub Evaluate()
            Output.Value = Inputs(0).Value
        End Sub

        Public Overrides ReadOnly Property GateType As IBaseGate.GateTypes
            Get
                Return IBaseGate.GateTypes.Led
            End Get
        End Property

        Public Shared Function FromXML(xml As XElement, resetID As Boolean) As BaseGate
            Dim g As New Led()
            g.SetBaseFromXML(xml, resetID)
            Return g
        End Function

        Protected Overrides Sub InitializeInputs()
            Name = "LED"
            Inputs.Add(New Pin(Me, Inputs.Count))
        End Sub

        Public Overrides ReadOnly Property Flow As IBaseGate.DataFlow
            Get
                Return IBaseGate.DataFlow.In
            End Get
        End Property
    End Class
End Class
