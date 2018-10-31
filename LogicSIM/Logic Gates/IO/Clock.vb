Partial Public Class LogicGates
    Public Class Clock
        Inherits BaseGate

        Public Property Phase As Double = 0
        Public Property Frequency As Double = 2 ' 2 Hz
        Public Property DutyCycle As Double = 0.5 ' 50%

        Private lastTickCount As Long

        Public Sub New()
            MyBase.New()

            UI.Size = New Size(100, 62)
            UI.FillColor = Color.DarkGreen
            UI.ActiveColor = Color.LightGreen

            Output.UI.X = UI.Width
            Output.UI.Y = UI.Height / 2 - Output.UI.Height / 2

            UI.NameOffset = New Point(26, 3)

            lastTickCount = Now.Ticks
        End Sub

        Protected Friend Overrides Sub Tick()
            Dim curTicks As Long = Now.Ticks
            If curTicks - lastTickCount > If(Output.Value, DutyCycle, 1 - DutyCycle) * 10000000 / Frequency + Phase Then
                lastTickCount = curTicks
                Output.Value = (Not Output.Value) And (Not Inputs(0).Value)
            End If
        End Sub

        Protected Friend Overrides Sub Evaluate()
        End Sub

        Public Overrides ReadOnly Property GateType As IBaseGate.GateTypes
            Get
                Return IBaseGate.GateTypes.Clock
            End Get
        End Property

        Public Shared Function FromXML(xml As XElement, Optional resetID As Boolean = False) As BaseGate
            Dim g As New Clock()
            g.SetBaseFromXML(xml, resetID)
            Return g
        End Function

        Public Overrides Function ToXML() As XElement
            Dim xml = MyBase.ToXML()

            xml.Add(<internals>
                        <phase><%= Phase %></phase>
                        <frequency><%= Frequency %></frequency>
                        <dutyCycle><%= DutyCycle %></dutyCycle>
                    </internals>)

            Return xml
        End Function

        Public Overrides Sub SetBaseFromXML(xml As XElement, Optional resetID As Boolean = False)
            MyBase.SetBaseFromXML(xml, resetID)
            Phase = Double.Parse(xml.<internals>.<phase>.Value)
            Frequency = Double.Parse(xml.<internals>.<frequency>.Value)
            DutyCycle = Double.Parse(xml.<internals>.<dutyCycle>.Value)
        End Sub

        Protected Overrides Sub InitializeInputs()
            Name = "CLOCK"
            Inputs.Add(New Pin(Me, Inputs.Count)) ' Disable clock = 1
            Inputs.Last().UI.Y = Output.UI.Y
        End Sub

        Public Overrides Function Clone() As Object
            Return Clock.FromXML(Me.ToXML(), True)
        End Function

        Public Overrides ReadOnly Property Flow As IBaseGate.DataFlow
            Get
                Return IBaseGate.DataFlow.InOut
            End Get
        End Property
    End Class
End Class
