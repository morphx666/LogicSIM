Public Interface IBaseGate
    Enum GateTypes
        [OR]
        [AND]
        [XOR]
        [NOT]
        NOR
        NAND
        Node
        Component
        Led
        Switch
        Clock
    End Enum

    Enum DataFlow
        [In]
        Out
        InOut
    End Enum

    Property Inputs As List(Of LogicGates.Pin)
    ReadOnly Property Output As LogicGates.Pin
    Property Name As String
    ReadOnly Property GateType As GateTypes
    ReadOnly Property UI As GateUI
    ReadOnly Property Flow As DataFlow

    Sub InitializeInputs()
    Sub Evaluate()
    Sub Tick()
    Sub StartTicking()
    Sub StopTicking()

    Event Ticked()
End Interface
