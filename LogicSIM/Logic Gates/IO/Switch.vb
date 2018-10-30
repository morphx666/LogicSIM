Partial Public Class LogicGates
    Public Class Switch
        Inherits BaseGate

        Public Sub New()
            MyBase.New()

            UI.Size = New Size(28, 28)
            UI.FillColor = Brushes.DarkGreen
            UI.ActiveColor = Brushes.LightGreen

            Output.UI.X = UI.Width
            Output.UI.Y = UI.Height / 2 - Output.UI.Height / 2

            UI.NameOffset = New Point(3, 3)
        End Sub

        Public Overrides Function Clone() As Object
            Return Switch.FromXML(Me.ToXML(), True)
        End Function

        Protected Friend Overrides Sub Evaluate()
            Output.Value = Inputs(0).Value
        End Sub

        Public Overrides ReadOnly Property GateType As IBaseGate.GateTypes
            Get
                Return IBaseGate.GateTypes.Switch
            End Get
        End Property

        Public Shared Function FromXML(xml As XElement, Optional resetID As Boolean = False) As BaseGate
            Dim g As New Switch()
            g.SetBaseFromXML(xml, resetID)
            Return g
        End Function

        Protected Overrides Sub InitializeInputs()
            Name = "SWITCH"
            Inputs.Add(New Pin(Me, Inputs.Count))
        End Sub

        Public Overrides ReadOnly Property Flow As IBaseGate.DataFlow
            Get
                Return IBaseGate.DataFlow.Out
            End Get
        End Property
    End Class
End Class
