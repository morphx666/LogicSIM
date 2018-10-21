﻿Partial Public Class LogicGates
    Public MustInherit Class BaseGate
        Implements IBaseGate, ICloneable

        Private mOutput As Pin
        Private mInputs As New List(Of Pin)
        Private mName As String = ""
        Private mInternalID As Guid
        Private mUI As GateUI

        Protected MustOverride Sub InitializeInputs() Implements IBaseGate.InitializeInputs
        Protected Friend MustOverride Sub Evaluate() Implements IBaseGate.Evaluate
        Public MustOverride Function Clone() As Object Implements ICloneable.Clone
        Public MustOverride ReadOnly Property GateType As IBaseGate.GateTypes Implements IBaseGate.GateType
        Public MustOverride ReadOnly Property Flow As IBaseGate.DataFlow Implements IBaseGate.Flow

        Public Sub New()
            mUI = New GateUI()
            mInternalID = Guid.NewGuid()
            mOutput = New Pin(Me)

            mUI.NameOffset = New Point(mUI.Width / 2 - 20, mUI.Height / 2 - 10)

            InitializeInputs()
        End Sub

        Public Property ID As Guid
            Get
                Return mInternalID
            End Get
            Protected Set(value As Guid)
                mInternalID = value
            End Set
        End Property

        Public Property Name As String Implements IBaseGate.Name
            Get
                Return mName
            End Get
            Set(value As String)
                mName = value
            End Set
        End Property

        Public ReadOnly Property UI As GateUI Implements IBaseGate.UI
            Get
                Return mUI
            End Get
        End Property

        Public Overridable Property Inputs As List(Of Pin) Implements IBaseGate.Inputs
            Get
                Return mInputs
            End Get
            Set(value As List(Of Pin))
                mInputs = value
            End Set
        End Property

        Public Overridable ReadOnly Property Output As Pin Implements IBaseGate.Output
            Get
                Return mOutput
            End Get
        End Property

        Public Overrides Function ToString() As String
            Dim r As String = Me.GetType().Name
            If r <> Name Then
                r += String.Format(" ({0}) ", Name)
            Else
                r += " "
            End If

            For i As Integer = 0 To Inputs.Count - 1
                r += String.Format("I{0}={1}, ", i + 1, Inputs(i).Value)
            Next

            r += String.Format("O{0}={1}", 0, Output.Value)

            Return r
        End Function

        Public Shared Operator =(g1 As BaseGate, g2 As BaseGate) As Boolean
            If g1 Is Nothing AndAlso g2 IsNot Nothing Then Return False
            If g1 IsNot Nothing AndAlso g2 Is Nothing Then Return False
            If g1 Is Nothing AndAlso g2 Is Nothing Then Return True

            Return g1.ID = g2.ID
        End Operator

        Public Shared Operator <>(g1 As BaseGate, g2 As BaseGate) As Boolean
            Return Not (g1 = g2)
        End Operator

        Protected Friend Sub SetBaseFromXML(xml As XElement)
            mInternalID = Guid.Parse(xml.<id>.Value)
            mName = xml.<name>.Value
            mUI = GateUI.FromXML(xml.<ui>(0))

            ' FIXME: We need to send the output first, so that we don't confuse the stupid code used to evaluate the Input Pins' UI
            mOutput = Pin.FromXML(xml.<outputPin>.<pin>(0), Me)

            mInputs.Clear()
            For Each ipXml In xml.<inputPins>.<pin>
                mInputs.Add(Pin.FromXML(ipXml, Me))
            Next
        End Sub

        Public Overridable Function ToXML() As XElement
            Return <gate>
                       <id><%= mInternalID %></id>
                       <type><%= Me.GetType().Name %></type>
                       <name><%= Name %></name>
                       <inputPins>
                           <%= From ip In Inputs Select (ip.ToXML()) %>
                       </inputPins>
                       <outputPin><%= Output.ToXML() %></outputPin>
                       <%= mUI.ToXML() %>
                   </gate>
        End Function

        Public Shared Function XMLToFont(xml As XElement) As Font
            Dim fs As FontStyle = FontStyle.Regular
            [Enum].TryParse(xml.<style>.Value, fs)

            Return New Font(xml.<family>.Value, xml.<size>.Value, fs, GraphicsUnit.Point)
        End Function

        Public Shared Function ParseString(Of T)(value As String) As T
            Dim type As Type = GetType(T)

            Dim removeText = Function(text As String)
                                 Dim newText As String = ""
                                 For i As Integer = 0 To value.Length - 1
                                     If value(i) = "." OrElse value(i) = "," OrElse value(i) = "-" OrElse IsNumeric(value(i)) Then
                                         newText += value(i)
                                     End If
                                 Next
                                 Return newText
                             End Function

            Select Case type
                Case GetType(Point), GetType(Rectangle), GetType(Padding), GetType(Size)
                    value = removeText(value)
                Case GetType(Boolean)
                    If value Is Nothing Then value = "False"
                Case Else
                    Throw New Exception(String.Format("Unsupported Type: {0}", type.FullName))
            End Select

            Return System.ComponentModel.TypeDescriptor.GetConverter(type).ConvertFromInvariantString(value)
        End Function

        Public Shared Function GetGateConnectedToInput(parentComponent As Component, inputPin As LogicGates.Pin) As LogicGates.BaseGate
            For Each gt In parentComponent.Gates
                If gt.GateType = IBaseGate.GateTypes.Node Then
                    For Each o In CType(gt, Node).Outputs
                        If o IsNot Nothing AndAlso o.Pin = inputPin Then Return gt
                    Next
                Else
                    If gt.Output.ConnectedToPin = inputPin Then Return gt
                End If
            Next

            Return Nothing
        End Function
    End Class
End Class