Imports LogicSIM.LogicGates

Public Class FormMain
    Dim circuit As Component

    Dim testFile As String = IO.Path.Combine(My.Application.Info.DirectoryPath, "LogicSIM_Sample.xml")

    Private Sub FormMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        IO.File.WriteAllText(testFile, circuit.ToXML().ToString())
    End Sub

    Private Sub FormMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'GenTestProject()

        If IO.File.Exists(testFile) AndAlso circuit Is Nothing Then
            Dim xDoc = XDocument.Parse(IO.File.ReadAllText(testFile))
            circuit = Component.FromXML(xDoc.FirstNode())
        End If

        CircuitSurfaceContainer.Circuit = circuit

        AddGatesToUI()
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

        For Each gateType In LogicGates.GetAvailableGates()
            If gateType.Item2 <> GetType(Component) AndAlso gateType.Item2 <> GetType(Node) Then
                gatesContainer.Gates.Add(Activator.CreateInstance(gateType.Item2))
                If gateType.Item2 = GetType(Switch) Then gatesContainer.Gates.Last().UI.NameOffset = New Point(0, gatesContainer.Gates.Last().UI.Height)
            End If
        Next

        Dim p As Point = New Point(0, CircuitSurfaceGatePicker.Snap.Height)
        gatesContainer.Gates.ForEach(Sub(g)
                                         p.X = (CircuitSurfaceGatePicker.Width - g.UI.Width) / 2
                                         g.UI.Location = p
                                         p.Y += g.UI.Height + If(g.UI.NameOffset.Y > g.UI.Height / 2, g.UI.NameOffset.Y, 0) + CircuitSurfaceGatePicker.Snap.Height * 2
                                     End Sub)

        CircuitSurfaceGatePicker.Circuit = gatesContainer
        CircuitSurfaceGatePicker.Readonly = True
        CircuitSurfaceGatePicker.MultiSelect = False
    End Sub
End Class
