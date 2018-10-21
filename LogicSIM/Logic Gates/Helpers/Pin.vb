Partial Public Class LogicGates
    Public Class Pin
        Implements ICloneable

        Private mID As Guid

        Private mValue As Boolean
        Private mParentGate As BaseGate
        Private mPinNumber As Integer

        Private mConnectedToGate As BaseGate
        Private mConnectedToPin As Pin
        Private mConnectedToPinNumber As Integer = -1

        Private mUI As GateUI

        Private mName As String

        Public Sub New(parentGate As BaseGate)
            mID = Guid.NewGuid()
            mParentGate = parentGate
            mName = "PIN"
            mUI = New GateUI()
            mUI.Size = New Size(10, 10)

            SetupUI()
        End Sub

        Protected Sub SetupUI()
            ' FIXME: this code sucks... it's so stupid that it requires the Output pin(s) to be set first, otherwise
            ' the Add/Remove code will break when loading from XML
            Select Case mParentGate.GateType
                Case IBaseGate.GateTypes.Node, IBaseGate.GateTypes.Led, IBaseGate.GateTypes.Switch
                Case IBaseGate.GateTypes.Component
                Case Else
                    mParentGate.Inputs.Add(Me)

                    If mParentGate.Inputs.Count = 1 Then
                        mParentGate.Inputs(0).UI.Y = mParentGate.UI.Height / 2
                    Else
                        Dim stp = mParentGate.UI.Height / mParentGate.Inputs.Count
                        For i As Integer = 0 To mParentGate.Inputs.Count - 1
                            mParentGate.Inputs(i).UI.X = -mUI.Width
                            mParentGate.Inputs(i).UI.Y = i * stp + stp / 2 - mUI.Height / 2
                        Next
                    End If

                    mParentGate.Inputs.Remove(Me)

                    If mParentGate.Output IsNot Nothing Then
                        mParentGate.Output.UI.Location = New Point(mParentGate.UI.Width, mParentGate.UI.Height / 2 - mUI.Height / 2)
                    End If
            End Select
        End Sub

        Public ReadOnly Property ID As Guid
            Get
                Return mID
            End Get
        End Property

        Public Property Name As String
            Get
                Return mName
            End Get
            Set(value As String)
                mName = value
            End Set
        End Property

        Public ReadOnly Property ParentGate As LogicGates.BaseGate
            Get
                Return mParentGate
            End Get
        End Property

        Public Property UI As GateUI
            Get
                Return mUI
            End Get
            Protected Set(value As GateUI)
                mUI = value
            End Set
        End Property

        Public ReadOnly Property ConnectedToGate As BaseGate
            Get
                Return mConnectedToGate
            End Get
        End Property

        Public ReadOnly Property ConnectedToPin As Pin
            Get
                Return mConnectedToPin
            End Get
        End Property

        Public ReadOnly Property ConnectedToPinNumber As Integer
            Get
                Return mConnectedToPinNumber
            End Get
        End Property

        Public Sub ConnectTo(gate As BaseGate, pinNumber As Integer)
            If mConnectedToPinNumber <> -1 Then Throw New Exception("Pin already connected")
            Disconnect()
            mConnectedToGate = gate
            mConnectedToPin = gate.Inputs(pinNumber)
            mConnectedToPinNumber = pinNumber
        End Sub

        Public Sub ConnectTo(gate As BaseGate, inputPin As Pin)
            For i As Integer = 0 To gate.Inputs.Count - 1
                If gate.Inputs(i) = inputPin Then
                    ConnectTo(gate, i)
                    Exit For
                End If
            Next
        End Sub

        Public Sub Disconnect()
            mConnectedToGate = Nothing
            mConnectedToPin = Nothing
            mConnectedToPinNumber = -1
        End Sub

        Public Property Value As Boolean
            Get
                Return mValue
            End Get
            Set(value As Boolean)
                If mValue <> value Then
                    mValue = value
                    mParentGate.Evaluate()

                    If mConnectedToPinNumber <> -1 Then
                        mConnectedToPin.Value = value
                        mConnectedToGate.Evaluate()
                    End If
                End If
            End Set
        End Property

        Public Shared Operator =(p1 As Pin, p2 As Pin) As Boolean
            If p1 Is Nothing AndAlso p2 IsNot Nothing Then Return False
            If p1 IsNot Nothing AndAlso p2 Is Nothing Then Return False
            If p1 Is Nothing AndAlso p2 Is Nothing Then Return True

            Return p1.ID = p2.ID
        End Operator

        Public Shared Operator <>(p1 As Pin, p2 As Pin) As Boolean
            Return Not (p1 = p2)
        End Operator

        Public Shared Function FromXML(xml As XElement, parent As BaseGate) As Pin
            Dim p As New Pin(parent)
            p.Name = xml.<name>.Value
            p.UI = GateUI.FromXML(xml.<ui>(0))
            Return p
        End Function

        Public Function ToXML() As XElement
            Return <pin>
                       <name><%= Name %></name>
                       <parentGate><%= mParentGate.ID.ToString() %></parentGate>
                       <connectedTo>
                           <gate><%= If(mConnectedToPinNumber <> -1, mConnectedToGate.ID.ToString(), "") %></gate>
                           <pinNumber><%= If(mConnectedToPinNumber <> -1, mConnectedToPinNumber.ToString(), -1) %></pinNumber>
                       </connectedTo>
                       <value><%= mValue.ToString() %></value>
                       <%= mUI.ToXML() %>
                   </pin>
        End Function

        Public Function Clone() As Object Implements ICloneable.Clone
            Return Pin.FromXML(Me.ToXML(), mParentGate)
        End Function
    End Class
End Class