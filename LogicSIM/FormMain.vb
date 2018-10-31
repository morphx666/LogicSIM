Imports LogicSIM.LogicGates

Public Class FormMain
    Private circuit As Component
    Private testFile As String = IO.Path.Combine(My.Application.Info.DirectoryPath, "LogicSIM_Sample.xml")
    Private Const nisInfo = "No item selected"

    Private Sub FormMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If circuit IsNot Nothing Then
            circuit.StopTicking()
            IO.File.WriteAllText(testFile, circuit.ToXML().ToString())
        End If
    End Sub

    Private Sub FormMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If IO.File.Exists(testFile) AndAlso circuit Is Nothing Then
            Dim xDoc = XDocument.Parse(IO.File.ReadAllText(testFile))
            circuit = Component.FromXML(xDoc.FirstNode())
        End If
        If circuit Is Nothing Then GenTestProject()

        CircuitSurfaceContainer.Circuit = circuit

        AddHandler CircuitSurfaceContainer.GatesSelectedChanged, Sub()
                                                                     If CircuitSurfaceContainer.SelectedGates.Count = 0 Then
                                                                         If CircuitSurfaceContainer.SelectedPin Is Nothing Then
                                                                             PropertyGridGateEditor.SelectedObject = circuit
                                                                         Else
                                                                             PropertyGridGateEditor.SelectedObject = CircuitSurfaceContainer.SelectedPin
                                                                         End If
                                                                     Else
                                                                         PropertyGridGateEditor.SelectedObjects = CircuitSurfaceContainer.SelectedGates.ToArray()
                                                                     End If

                                                                     Dim txt As String = ""
                                                                     For Each so In PropertyGridGateEditor.SelectedObjects
                                                                         If TypeOf so Is BaseGate Then
                                                                             Dim g As BaseGate = CType(so, BaseGate)
                                                                             txt += $"{g.GateType} [{g.Name}], "
                                                                         ElseIf TypeOf so Is Pin Then
                                                                             Dim p As Pin = CType(so, Pin)
                                                                             'txt += $"PIN [{p.Name}] {p.ParentGate.GateType}, "
                                                                             txt += $"PIN {p.ToString()}, "
                                                                         End If
                                                                     Next
                                                                     txt = If(txt.Length = 0, nisInfo, txt.Substring(0, txt.Length - 2))
                                                                     LabelSelectedItem.Text = txt
                                                                 End Sub

        AddGatesToUI()
        LabelSelectedItem.Text = nisInfo
    End Sub

    Private Sub GenTestProject() ' Full adder: https://en.wikipedia.org/wiki/Adder_(electronics)#Full_adder
        circuit = New Component() With {.Name = "Full Adder"}

        Dim xor1 As New XORGate() With {.Name = "XOR 1"}
        Dim xor2 As New XORGate() With {.Name = "XOR 2"}

        Dim and1 As New ANDGate() With {.Name = "AND 1"}
        Dim and2 As New ANDGate() With {.Name = "AND 2"}

        Dim or1 As New ORGate() With {.Name = "OR"}

        Dim led1 As New Led() With {.Name = "S"}
        Dim led2 As New Led() With {.Name = "C"}

        Dim sw1 As New Switch() With {.Name = "A"}
        Dim sw2 As New Switch() With {.Name = "B"}
        Dim sw3 As New Switch() With {.Name = "C"}

        Dim node1 As New Node() With {.Name = "1"}
        node1.ConnectTo(xor1, 0, 1)
        node1.ConnectTo(and2, 0, 2)

        Dim node2 As New Node() With {.Name = "2"}
        node2.ConnectTo(xor1, 1, 1)
        node2.ConnectTo(and2, 1, 2)

        Dim node3 As New Node() With {.Name = "3"}
        node3.ConnectTo(xor2, 0, 1)
        node3.ConnectTo(and1, 0, 2)

        Dim node4 As New Node() With {.Name = "4"}
        node4.ConnectTo(xor2, 1, 1)
        node4.ConnectTo(and1, 1, 2)

        xor1.Output.ConnectTo(node3, 0)
        and1.Output.ConnectTo(or1, 0)
        and2.Output.ConnectTo(or1, 1)

        xor2.Output.ConnectTo(led1, 0)
        or1.Output.ConnectTo(led2, 0)

        sw1.Output.ConnectTo(node1, 0)
        sw2.Output.ConnectTo(node2, 0)
        sw3.Output.ConnectTo(node4, 0)

        circuit.Gates.Add(xor1)
        circuit.Gates.Add(xor2)
        circuit.Gates.Add(and1)
        circuit.Gates.Add(and2)
        circuit.Gates.Add(or1)
        circuit.Gates.Add(node1)
        circuit.Gates.Add(node2)
        circuit.Gates.Add(node3)
        circuit.Gates.Add(node4)
        circuit.Gates.Add(led1)
        circuit.Gates.Add(led2)
        circuit.Gates.Add(sw1)
        circuit.Gates.Add(sw2)
        circuit.Gates.Add(sw3)

        circuit.DefineInputPin(sw1, 0)
        circuit.DefineInputPin(sw2, 1)
        circuit.DefineInputPin(sw3, 2)

        circuit.DefineOutputPin(xor2, 0)
        circuit.DefineOutputPin(or1, 1)

        circuit.Inputs(0).Value = False ' A
        circuit.Inputs(1).Value = False ' B
        circuit.Inputs(2).Value = False ' C

        xor1.UI.Location = New Point(300, 20)
        xor2.UI.Location = New Point(550, 40)
        and1.UI.Location = New Point(550, 180)
        and2.UI.Location = New Point(550, 270)
        or1.UI.Location = New Point(750, 230)

        led1.UI.Location = New Point(910, xor2.UI.Y + led1.UI.Height - 2 + 140)
        led1.UI.Angle = 90
        led2.UI.Location = New Point(910, or1.UI.Y + led2.UI.Height - 2)

        node1.UI.Location = New Point(220, 40 - 5)
        node2.UI.Location = New Point(180, 80 - 5)
        node3.UI.Location = New Point(490, 60 - 5)
        node4.UI.Location = New Point(450, 100 - 5)

        sw1.UI.Location = New Point(60, node1.UI.Y - 10)
        sw2.UI.Location = New Point(60, node2.UI.Y - 10)
        sw3.UI.Location = New Point(60, node4.UI.Y + 10)

        node1.Name = "1"
        node2.Name = "2"
        node3.Name = "3"
        node4.Name = "4"
    End Sub

    Private Sub AddGatesToUI()
        Dim gatesContainer = New Component() With {.Name = "Gates Container"}
        Dim gates As New List(Of BaseGate)

        For Each gateType In GetAvailableGates()
            If gateType.Item2 <> GetType(Component) Then
                Dim g As BaseGate = Activator.CreateInstance(gateType.Item2)
                gates.Add(g)
                If gateType.Item2 = GetType(Switch) OrElse gateType.Item2 = GetType(Node) Then
                    g.UI.NameOffset = New Point((g.UI.Width - g.Name.Length * 9) / 2, g.UI.Height)
                End If
            End If
        Next
        gates = (From g In gates Order By g.UI.Bounds.Height, g.Flow, g.Name Select g).ToList()
        gates.ForEach(Sub(g) gatesContainer.Gates.Add(g))

        Dim p As Point = New Point(0, CircuitSurfaceGatePicker.Snap.Height + 10)
        gatesContainer.Gates.ForEach(Sub(g)
                                         p.X = (CircuitSurfaceGatePicker.Width - g.UI.Width) / 2
                                         g.UI.Location = p
                                         p.Y += g.UI.Height + If(g.UI.NameOffset.Y > g.UI.Height / 2, g.UI.NameOffset.Y, 0) + CircuitSurfaceGatePicker.Snap.Height * 2
                                     End Sub)

        CircuitSurfaceGatePicker.Circuit = gatesContainer
        CircuitSurfaceGatePicker.Readonly = True
        CircuitSurfaceGatePicker.MultiSelect = False

        AddHandler CircuitSurfaceContainer.MouseUp, Sub(o As Object, e As MouseEventArgs)
                                                        If CircuitSurfaceGatePicker.SelectedGates.Any() Then
                                                            If CircuitSurfaceContainer.SelectedGates.Count = 0 Then
                                                                Dim g As BaseGate = CircuitSurfaceGatePicker.SelectedGates(0).Clone()
                                                                circuit.Gates.Add(g)
                                                                g.UI.Location = CircuitSurfaceContainer.GateRenderer.TransformPoint(e.Location)
                                                                If CircuitSurfaceContainer.SnapToGrid Then
                                                                    g.UI.X -= g.UI.X Mod CircuitSurfaceContainer.Snap.Width
                                                                    g.UI.Y -= g.UI.Y Mod CircuitSurfaceContainer.Snap.Height
                                                                    If g.GateType = IBaseGate.GateTypes.Led OrElse g.GateType = IBaseGate.GateTypes.Node OrElse g.GateType = IBaseGate.GateTypes.Switch Then
                                                                        g.UI.X -= 5
                                                                        g.UI.Y -= 5
                                                                        If g.GateType = IBaseGate.GateTypes.Node Then g.Name = ""
                                                                    End If
                                                                End If
                                                                CircuitSurfaceContainer.SelectedGates.Add(g)
                                                            End If
                                                            CircuitSurfaceGatePicker.SelectedGates.Clear()
                                                        End If
                                                    End Sub
    End Sub
End Class
