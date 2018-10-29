Partial Public Class LogicGates
    Public Class Node
        Inherits BaseGate

        Private Const ToRad As Double = Math.PI / 180

        Private mOutputs As List(Of Component.PinConnection)
        Private mOutputsUIs As List(Of GateUI)

        Public Sub New()
            MyBase.New()
        End Sub

        Public Overrides ReadOnly Property GateType As IBaseGate.GateTypes
            Get
                Return IBaseGate.GateTypes.Node
            End Get
        End Property

        Public ReadOnly Property Input As Pin
            Get
                Return Inputs(0)
            End Get
        End Property

        Public ReadOnly Property Outputs As List(Of Component.PinConnection)
            Get
                Return mOutputs
            End Get
        End Property

        Public ReadOnly Property OutputsUIs As List(Of GateUI)
            Get
                Return mOutputsUIs
            End Get
        End Property

        Public Sub ConnectTo(gate As BaseGate, pinNumber As Integer, Optional outputPinNumber As Integer = -1)
            If outputPinNumber = -1 Then
                For i As Integer = 0 To 3 - 1
                    If mOutputs(i) Is Nothing Then
                        outputPinNumber = i
                        Exit For
                    End If
                Next
            End If

            mOutputs(outputPinNumber) = New Component.PinConnection(gate, pinNumber, outputPinNumber)
        End Sub

        Public Sub ConnectTo(gate As BaseGate, pin As Pin)
            If gate.Output = pin Then
                gate.Output.ConnectTo(Me, 0)
            Else
                For i As Integer = 0 To gate.Inputs.Count - 1
                    If gate.Inputs(i) = pin Then
                        ConnectTo(gate, i)
                        Exit For
                    End If
                Next
            End If
        End Sub

        Public Sub Disconnect(ouputPinConnection As Component.PinConnection)
            mOutputs(mOutputs.IndexOf(ouputPinConnection)) = Nothing
        End Sub

        Private Sub PositionOutputs()
            Dim a = 180
            Dim s = 90 ' 360 / (mOutputsUIs.Count + 1)
            For i = 0 To mOutputsUIs.Count - 1
                a -= s
                PositionPin(mOutputsUIs(i), a)
            Next
        End Sub

        Private Sub PositionPin(pinUI As GateUI, a As Double)
            pinUI.X = UI.Width / 2 + UI.Width * Math.Cos(a * ToRad) - pinUI.Width / 2
            pinUI.Y = UI.Height / 2 + UI.Height * Math.Sin(-a * ToRad) - pinUI.Height / 2
        End Sub

        Protected Friend Overrides Sub Evaluate()
            If Output.Value <> Input.Value Then
                Output.Value = Input.Value

                For Each o In mOutputs
                    If o IsNot Nothing Then o.Pin.Value = Output.Value
                Next
            End If
        End Sub

        Protected Overrides Sub InitializeInputs()
            Name = "Node"

            UI.Size = New Size(10, 10)
            UI.NameOffset = New Point(5, 5)
            UI.FillColor = Brushes.Blue

            Inputs.Add(New Pin(Me, Inputs.Count))
            Inputs(0).UI.Size = New Size(10, 10)

            mOutputs = New List(Of Component.PinConnection)()
            mOutputsUIs = New List(Of GateUI)

            For i As Integer = 0 To 3 - 1
                mOutputsUIs.Add(New GateUI())
                mOutputsUIs.Last().Size = Input.UI.Size()
                mOutputs.Add(Nothing)
            Next

            PositionPin(Input.UI, 180)
            PositionOutputs()
        End Sub

        Public Shared Function FromXML(xml As XElement) As BaseGate
            Dim g As New Node()
            g.SetBaseFromXML(xml)

            Return g
        End Function

        Public Overrides Function ToXML() As XElement
            Dim xml = MyBase.ToXML()

            xml.Add(<internals>
                        <outputPins>
                            <%= From op In mOutputs Select (op?.ToXML()) %>
                        </outputPins>
                    </internals>)

            Return xml
        End Function

        Public Overrides Function Clone() As Object
            Return Node.FromXML(Me.ToXML())
        End Function

        Public Overrides ReadOnly Property Flow As IBaseGate.DataFlow
            Get
                Return IBaseGate.DataFlow.InOut
            End Get
        End Property
    End Class
End Class