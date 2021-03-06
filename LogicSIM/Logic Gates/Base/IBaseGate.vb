﻿Public Interface IBaseGate
    Enum GateTypes
        [OR]
        [AND]
        [XOR]
        XNOR
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
    Sub Tick(ticksCount As Long, lastTicksCount As Long)
    Sub StartTicking()
    Sub StopTicking()

    Event Ticked(ticksCount As Long)
End Interface
