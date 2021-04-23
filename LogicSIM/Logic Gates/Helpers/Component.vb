Imports System.Reflection

Partial Public Class LogicGates
    Public Class Component
        Inherits BaseGate

        Public Class PinConnection
            Public Property Gate As BaseGate
            Public Property Pin As Pin
            Public Property PinNumber As Integer
            Public Property OutputPinNumber As Integer

            Public Sub New()

            End Sub

            Public Sub New(gate As BaseGate, pinNumber As Integer, Optional outputPinNumber As Integer = -1)
                Me.Gate = gate
                Me.PinNumber = pinNumber
                Me.OutputPinNumber = outputPinNumber
                If pinNumber = -1 Then
                    Me.Pin = gate.Output
                Else
                    While pinNumber >= gate.Inputs.Count
                        gate.Inputs.Add(New Pin(gate, gate.Inputs.Count))
                    End While
                    Me.Pin = gate.Inputs(pinNumber)
                End If
            End Sub

            Public Function ToXML() As XElement
                Return <gatePin>
                           <gate><%= Gate.ID.ToString() %></gate>
                           <pinNumber><%= PinNumber %></pinNumber>
                           <outputPinNumber><%= OutputPinNumber %></outputPinNumber>
                       </gatePin>
            End Function
        End Class

        Private mGates As New List(Of BaseGate)
        Private mCompOutputs As New List(Of PinConnection)
        Private mCompInputs As New List(Of PinConnection)

        Public Property ParentControl As CircuitSurface

        Public Overrides ReadOnly Property GateType As IBaseGate.GateTypes
            Get
                Return IBaseGate.GateTypes.Component
            End Get
        End Property

        Public ReadOnly Property Gates As List(Of BaseGate)
            Get
                Return mGates
            End Get
        End Property

        Public Overloads ReadOnly Property Inputs As List(Of Pin)
            Get
                Dim inPins As New List(Of Pin)
                For Each ci In mCompInputs
                    inPins.Add(ci.Pin)
                Next
                Return inPins
            End Get
        End Property

        Public ReadOnly Property Outputs As List(Of Pin)
            Get
                Dim outPins As New List(Of Pin)
                For Each co In mCompOutputs
                    outPins.Add(co.Pin)
                Next
                Return outPins
            End Get
        End Property

        Public Sub DefineInputPin(gate As BaseGate, pinNumber As Integer)
            mCompInputs.Add(New PinConnection(gate, pinNumber))
        End Sub

        Public Sub RemoveInputPin(pin As Pin)
            For i As Integer = 0 To mCompInputs.Count - 1
                If mCompInputs(i).Pin = pin Then
                    mCompInputs.RemoveAt(i)
                    Exit For
                End If
            Next
        End Sub

        Public Sub DefineOutputPin(gate As BaseGate, pinNumber As Integer)
            mCompOutputs.Add(New PinConnection(gate, pinNumber))
        End Sub

        Public Sub RemoveOutputPin(pin As Pin)
            For i As Integer = 0 To mCompOutputs.Count - 1
                If mCompOutputs(i).Pin = pin Then
                    mCompOutputs.RemoveAt(i)
                    Exit For
                End If
            Next
        End Sub

        Protected Friend Overrides Sub Evaluate()
            SyncLock mGates
                For Each g As BaseGate In mGates.Where(Function(tg) tg.Flow <> IBaseGate.DataFlow.In)
                    Dim v(g.Inputs.Count - 1) As Boolean
                    For i As Integer = 0 To g.Inputs.Count - 1
                        v(i) = g.Inputs(i).Value
                    Next

                    For n As Integer = 0 To 2 ^ g.Inputs.Count - 1
                        For i As Integer = 0 To g.Inputs.Count - 1
                            g.Inputs(i).Value = (n And (i + 1)) <> 0
                        Next
                    Next

                    For i As Integer = 0 To g.Inputs.Count - 1
                        g.Inputs(i).Value = v(i)
                    Next
                Next
            End SyncLock
        End Sub

        Protected Overrides Sub InitializeInputs()
            Name = "Component"
        End Sub

        Protected Friend Overrides Sub Tick(ticksCount As Long, lastTicksCount As Long)
            If ParentControl?.Readonly Then Exit Sub

            For Each g As BaseGate In mGates.Where(Function(tg) tg.GateType() = IBaseGate.GateTypes.Clock)
                g.Tick(ticksCount, lastTicksCount)
            Next
        End Sub

        Public Shared Function FromXML(xml As XElement) As Component
            Dim c As New Component()
            c.SetBaseFromXML(xml.<gate>(0))

            Dim gates = GetAvailableGateTypes()

            For Each gXml In xml.<internals>.<gates>.<gate>
                c.Gates.Add(InstantiateGate(gXml, gates))
            Next

            For Each ipXml In xml.<internals>.<inputPins>.<gatePin>
                c.DefineInputPin(GetGateById(ipXml.<gate>.Value, c), ipXml.<pinNumber>.Value)
            Next

            For Each ipXml In xml.<internals>.<outputPins>.<gatePin>
                c.DefineOutputPin(GetGateById(ipXml.<gate>.Value, c), ipXml.<pinNumber>.Value)
            Next

            For Each gXml In xml.<internals>.<gates>.<gate>
                Dim g As BaseGate = GetGateById(gXml.<id>.Value, c)

                If TypeOf g Is Node Then
                    Dim n = CType(g, Node)
                    For Each opXml In gXml.<internals>.<outputPins>.<gatePin>
                        n.ConnectTo(GetGateById(opXml.<gate>.Value, c), opXml.<pinNumber>.Value, opXml.<outputPinNumber>.Value)
                    Next
                Else
                    For Each ipXml In gXml.<inputPins>.<pin>
                        If ipXml.<connectedTo>.<gate>.Value <> "" Then
                            Stop
                        End If
                    Next

                    Dim opXml = gXml.<outputPin>.<pin>.<connectedTo>
                    If opXml.<gate>.Value <> "" Then
                        g.Output.ConnectTo(GetGateById(opXml.<gate>.Value, c), opXml.<pinNumber>.Value)
                    End If
                End If
            Next

            For Each gXml In xml.<internals>.<gates>.<gate>
                Dim g = GetGateById(gXml.<id>.Value, c)
                Dim pinNumber As Integer = 0

                For Each ipXml In gXml.<inputPins>.<pin>
                    g.Inputs(pinNumber).Value = ipXml.<value>.Value
                    pinNumber += 1
                Next
            Next

            Return c
        End Function

        Private Shared Function GetGateById(id As String, c As Component) As BaseGate
            Dim gId As Guid = Guid.Parse(id)
            For Each g In c.Gates
                If g.ID = gId Then Return g
            Next

            Return Nothing
        End Function

        Private Shared Function InstantiateGate(xml As XElement, gates As List(Of IBaseGate)) As BaseGate
            For Each g In gates
                If g.GetType().FullName.Split("+")(1) = xml.<type>.Value Then
                    Dim gt As BaseGate = CType(g, BaseGate).Clone()
                    gt.SetBaseFromXML(xml)
                    Return gt
                End If
            Next

            Return Nothing
        End Function

        Private Shared Function GetAvailableGateTypes() As List(Of IBaseGate)
            Dim gateTypes As New List(Of IBaseGate)

            Dim gateType As Type = GetType(BaseGate)
            Dim asm As Assembly = Assembly.GetAssembly(Assembly.GetExecutingAssembly.GetType)

            For Each t As Type In GetType(LogicGates).GetNestedTypes()
                If t.BaseType IsNot Nothing AndAlso (t.BaseType Is gateType OrElse t.BaseType.BaseType Is gateType) Then
                    If Not t.IsAbstract Then gateTypes.Add(Activator.CreateInstance(t))
                End If
            Next

            Return gateTypes
        End Function

        Public Overrides Function ToXML() As XElement
            Return <component>
                       <%= MyBase.ToXML() %>
                       <internals>
                           <gates>
                               <%= From g In mGates Select (g.ToXML()) %>
                           </gates>
                           <inputPins>
                               <%= From ip In mCompInputs Select (ip.ToXML()) %>
                           </inputPins>
                           <outputPins>
                               <%= From op In mCompOutputs Select (op.ToXML()) %>
                           </outputPins>
                       </internals>
                   </component>
        End Function

        Public Overrides Function Clone() As Object
            Return Component.FromXML(Me.ToXML())
        End Function

        Public Overrides ReadOnly Property Flow As IBaseGate.DataFlow
            Get
                Return IBaseGate.DataFlow.InOut
            End Get
        End Property

        Public ReadOnly Property OptimalBounds As Rectangle
            Get
                Dim x = mGates.Min(Function(k) k.UI.X)
                Dim y = mGates.Min(Function(k) k.UI.Y)
                Dim r = mGates.Max(Function(k) k.UI.Bounds.Right)
                Dim b = mGates.Max(Function(k) k.UI.Bounds.Bottom)

                Return New Rectangle(x, y, r - x, b - y)
            End Get
        End Property
    End Class
End Class