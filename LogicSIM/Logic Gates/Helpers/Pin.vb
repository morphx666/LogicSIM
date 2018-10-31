Imports System.ComponentModel

Partial Public Class LogicGates
    <TypeConverter(GetType(PinConverter))> Public Class Pin
        Implements ICloneable

        Private mID As Guid

        Private mValue As Boolean
        Private mParentGate As BaseGate
        Private mPinNumber As Integer

        Private mConnectedToGate As BaseGate
        Private mConnectedToPin As Pin
        Private mConnectedToPinNumber As Integer = -1

        <ReadOnlyAttribute(False)> Public Property ConnectedFromGate As BaseGate

        Private mUI As GateUI

        Private mName As String

        Public Sub New(parentGate As BaseGate, pinNumber As Integer)
            mID = Guid.NewGuid()
            mParentGate = parentGate
            mName = "PIN"
            mPinNumber = pinNumber
            mUI = New GateUI With {
                .Size = New Size(10, 10)
            }

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
                        mParentGate.Inputs(0).UI.X = -mUI.Width
                        mParentGate.Inputs(0).UI.Y = mParentGate.UI.Height / 2 - mParentGate.Inputs(0).UI.Height / 2 + 1
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

        <BrowsableAttribute(False)> Public ReadOnly Property ID As Guid
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

        <BrowsableAttribute(False)> Public ReadOnly Property ParentGate As LogicGates.BaseGate
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

        <ReadOnlyAttribute(False)> Public ReadOnly Property ConnectedToGate As BaseGate
            Get
                Return mConnectedToGate
            End Get
        End Property

        <ReadOnlyAttribute(False)> Public ReadOnly Property ConnectedToPin As Pin
            Get
                Return mConnectedToPin
            End Get
        End Property

        <ReadOnlyAttribute(False)> Public ReadOnly Property ConnectedToPinNumber As Integer
            Get
                Return mConnectedToPinNumber
            End Get
        End Property

        <ReadOnlyAttribute(False)> Public ReadOnly Property PinNumber As Integer
            Get
                Return mPinNumber
            End Get
        End Property

        Public Sub ConnectTo(gate As BaseGate, pinNumber As Integer)
            'If mConnectedToPinNumber <> -1 Then Throw New Exception("Pin already connected")
            Disconnect()
            mConnectedToGate = gate
            mConnectedToPin = gate.Inputs(pinNumber)
            mConnectedToPinNumber = pinNumber
            mPinNumber = mPinNumber

            gate.Inputs(pinNumber).ConnectedFromGate = mParentGate
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
            mValue = False
            If mConnectedToPin IsNot Nothing Then mConnectedToPin.Value = False

            mConnectedToGate = Nothing
            mConnectedToPin = Nothing
            mConnectedToPinNumber = -1
        End Sub

        <ReadOnlyAttribute(True)> Public Property Value As Boolean
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

        Public Overrides Function ToString() As String
            If mConnectedToPin Is Nothing Then
                If ConnectedFromGate Is Nothing Then
                    Return $"[{mName}] {mParentGate.GateType}({mPinNumber})"
                Else
                    Return $"[{mName}] {mParentGate.GateType}({mPinNumber})<-{ConnectedFromGate.GateType}"
                End If
            Else
                Return $"[{mName}] {mParentGate.GateType}({mPinNumber})->{mConnectedToPin.ParentGate.GateType}({mConnectedToPin.PinNumber})"
            End If
        End Function

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
            Dim p As New Pin(parent, xml.<pinNumber>.Value) With {
                .Name = xml.<name>.Value,
                .UI = GateUI.FromXML(xml.<ui>(0))
            }

            p.mValue = Boolean.Parse(xml.<value>.Value)
            p.mConnectedToPinNumber = xml.<connectedTo>.<pinNumber>.Value

            Return p
        End Function

        Public Function ToXML() As XElement
            Return <pin>
                       <name><%= Name %></name>
                       <parentGate><%= mParentGate.ID %></parentGate>
                       <connectedTo>
                           <gate><%= If(mConnectedToPinNumber <> -1, mConnectedToGate.ID, "") %></gate>
                           <pinNumber><%= mConnectedToPinNumber %></pinNumber>
                       </connectedTo>
                       <value><%= mValue %></value>
                       <pinNumber><%= mPinNumber %></pinNumber>
                       <value><%= mValue %></value>
                       <%= mUI.ToXML() %>
                   </pin>
        End Function

        Public Function Clone() As Object Implements ICloneable.Clone
            Return Pin.FromXML(Me.ToXML(), mParentGate)
        End Function
    End Class

    Public Class PinConverter
        Inherits ExpandableObjectConverter

        Private selectedPin As Pin

        Public Overrides Function CanConvertTo(ByVal context As ITypeDescriptorContext, ByVal destinationType As Type) As Boolean
            If destinationType Is GetType(Pin) Then
                Return True
            Else
                Return MyBase.CanConvertTo(context, destinationType)
            End If
        End Function

        Public Overloads Overrides Function ConvertTo(ByVal context As ITypeDescriptorContext, ByVal culture As Globalization.CultureInfo, ByVal value As Object, ByVal destinationType As System.Type) As Object
            If destinationType Is GetType(String) AndAlso TypeOf value Is Pin Then
                selectedPin = CType(value, Pin)
                Return selectedPin.ToString()
            End If

            Return MyBase.ConvertTo(context, culture, value, destinationType)
        End Function

        Public Overloads Overrides Function ConvertFrom(ByVal context As ITypeDescriptorContext, ByVal culture As Globalization.CultureInfo, ByVal value As Object) As Object
            If TypeOf value Is String Then
                Dim description As String = CType(value, String)
                'selectedPin.Description = description.Replace(selectedPin.Time.ToString() + " ", "")
                Return selectedPin
            End If

            Return MyBase.ConvertFrom(context, culture, value)
        End Function

        Public Overrides Function CanConvertFrom(ByVal context As ITypeDescriptorContext, ByVal sourceType As System.Type) As Boolean
            If sourceType Is GetType(String) Then
                Return True
            Else
                Return MyBase.CanConvertFrom(context, sourceType)
            End If
        End Function
    End Class
End Class