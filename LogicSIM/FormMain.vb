Imports LogicSIM.LogicGates

Public Class FormMain
    Dim circuit As Component

    Dim testFile As String = "E:\Information Resources\LogicSIM.xml"

    Private Sub FormMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        'IO.File.WriteAllText(testFile, circuit.ToXML().ToString())
    End Sub

    Private Sub FormMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        GenTestProject()
        'GenSimpleTestProject()

        If IO.File.Exists(testFile) AndAlso circuit Is Nothing Then
            Dim xDoc = XDocument.Parse(IO.File.ReadAllText(testFile))
            'circuit = Component.FromXML(xDoc.FirstNode())
        End If

        CircuitSurface1.Circuit = circuit
    End Sub

    Private Sub GenSimpleTestProject()
        circuit = New Component()

        Dim n As New Node()
        n.UI.Location = New Point(200, 200)
        n.ConnectTo(New ORGate(), 0)

        circuit.Gates.Add(n)
    End Sub

    Private Sub GenTestProject()
        circuit = New Component()

        Dim xor1 As New XORGate()
        Dim xor2 As New XORGate()

        Dim and1 As New ANDGate()
        Dim and2 As New ANDGate()

        Dim or1 As New ORGate()

        Dim led1 As New Led()
        Dim led2 As New Led()

        Dim sw1 As New Switch()
        Dim sw2 As New Switch()
        Dim sw3 As New Switch()

        Dim node1 As New Node()
        node1.ConnectTo(xor1, 0, 1)
        node1.ConnectTo(and2, 0, 2)

        Dim node2 As New Node()
        node2.ConnectTo(xor1, 1, 1)
        node2.ConnectTo(and2, 1, 2)

        Dim node3 As New Node()
        node3.ConnectTo(xor2, 0, 1)
        node3.ConnectTo(and1, 0, 2)

        Dim node4 As New Node()
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
        circuit.DefineInputPin(sw2, 0)
        circuit.DefineInputPin(sw3, 0)

        circuit.DefineOutputPin(xor2)
        circuit.DefineOutputPin(or1)

        circuit.Inputs(0).Value = False ' A
        circuit.Inputs(1).Value = False ' B
        circuit.Inputs(2).Value = False ' C

        'Debug.WriteLine("S = {0}", If(circuit.Outputs(0).Value, 1, 0))
        'Debug.WriteLine("C = {0}", If(circuit.Outputs(1).Value, 1, 0))

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

        led1.Name = "S"
        led2.Name = "C"

        node1.Name = "1"
        node2.Name = "2"
        node3.Name = "3"
        node4.Name = "4"

        sw1.Name = "A"
        sw2.Name = "B"
        sw3.Name = "C"

        'xor2.UI.Angle = 45
    End Sub
End Class
