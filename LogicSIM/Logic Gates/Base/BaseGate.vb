Imports System.ComponentModel
Imports System.Threading

Partial Public Class LogicGates
    Public MustInherit Class BaseGate
        Implements IBaseGate, ICloneable

        Private mOutput As Pin
        Private mInputs As New List(Of Pin)
        Private mName As String = ""
        Private mInternalID As Guid
        Private mUI As GateUI
        Public Event Ticked(ticksCount As Long) Implements IBaseGate.Ticked
        Protected MustOverride Sub InitializeInputs() Implements IBaseGate.InitializeInputs
        Protected Friend MustOverride Sub Evaluate() Implements IBaseGate.Evaluate
        Public MustOverride Function Clone() As Object Implements ICloneable.Clone
        <BrowsableAttribute(False)> Public MustOverride ReadOnly Property GateType As IBaseGate.GateTypes Implements IBaseGate.GateType
        <BrowsableAttribute(False)> Public MustOverride ReadOnly Property Flow As IBaseGate.DataFlow Implements IBaseGate.Flow

        Private cts As CancellationTokenSource
        Private ct As CancellationToken

        Public Sub New()
            mUI = New GateUI()
            mInternalID = Guid.NewGuid()
            mOutput = New Pin(Me, 0)

            mUI.NameOffset = New Point(mUI.Width / 2 - 20, mUI.Height / 2 - 10)

            mName = Me.GetType().ToString().Split("+")(1).Replace("Gate", "")

            InitializeInputs()
        End Sub

        Protected Friend Overridable Sub Tick(ticksCount As Long, lastTicksCount As Long) Implements IBaseGate.Tick
        End Sub

        Public Overridable Sub StartTicking() Implements IBaseGate.StartTicking
            If cts IsNot Nothing Then Exit Sub
            cts = New CancellationTokenSource()
            ct = cts.Token

            Task.Run(Sub()
                         Dim curTicks As Long
                         Dim lastTicks As Long
                         Do
                             curTicks = Now.Ticks
                             Tick(curTicks, lastTicks)
                             Thread.Sleep(10)
                             RaiseEvent Ticked(curTicks)
                             lastTicks = Now.Ticks
                         Loop
                     End Sub, ct)
        End Sub

        Public Overridable Sub StopTicking() Implements IBaseGate.StopTicking
            If cts IsNot Nothing Then
                cts.Cancel()
                ct = Nothing
                cts.Dispose()
                cts = Nothing
            End If
        End Sub

        Public Sub New(name As String)
            MyBase.New()
            mName = name
        End Sub

        <BrowsableAttribute(False)> Public Property ID As Guid
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

        Public Overridable Sub SetBaseFromXML(xml As XElement, Optional resetID As Boolean = False)
            mInternalID = If(resetID, Guid.NewGuid, Guid.Parse(xml.<id>.Value))
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

        Public Shared Function GetGateConnectedToInput(parentComponent As Component, inputPin As Pin) As BaseGate
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

    Public Shared Function GetAvailableGates() As List(Of Tuple(Of String, Type))
        Return GetType(BaseGate).Assembly.GetTypes().
                Where(Function(t) t.IsClass AndAlso t.IsSubclassOf(GetType(BaseGate)) AndAlso Not t.IsAbstract).
                Select(Function(t) New Tuple(Of String, Type)(t.Name, t)).ToList()
    End Function

    'Public Shared Function GetProperties(obj As Object) As List(Of Tuple(Of MemberInfo, Double, Double, String, String))
    '    Dim properties As New List(Of Tuple(Of MemberInfo, Double, Double, String, String))

    '    Dim objType As Type = obj.GetType()
    '    Dim asm As Assembly = Assembly.GetAssembly(objType)

    '    For Each t As Type In asm.GetExportedTypes().Where(Function(testType As Type) testType Is objType)
    '        For Each mi As MemberInfo In t.GetMembers.Where(Function(member As MemberInfo) member.MemberType = MemberTypes.Property Or
    '                                                                                       member.MemberType = MemberTypes.Field)

    '            Dim caObj() As Object = mi.GetCustomAttributes(GetType(BaseGate.EffectProperyAttribute), False)
    '            If caObj.Length = 0 Then
    '                properties.Add(New Tuple(Of MemberInfo, Double, Double, String, String)(mi, 0, 0, "", ""))
    '            Else
    '                Dim filterAttr As BaseGate.EffectProperyAttribute = CType(caObj(0), BaseGate.EffectProperyAttribute)
    '                properties.Add(New Tuple(Of MemberInfo, Double, Double, String, String)(mi, filterAttr.Minimum, filterAttr.Maximum, filterAttr.Unit, filterAttr.Description))
    '            End If
    '        Next
    '    Next

    '    Return properties
    'End Function
End Class