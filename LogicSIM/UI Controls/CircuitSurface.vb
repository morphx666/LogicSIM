﻿Imports LogicSIM.LogicGates

Public Class CircuitSurface
    Public Class PanningXY
        Public Property X
        Public Property Y
    End Class

    Private mCircuit As Component
    Private mGateRenderer As GateRenderer
    Private mZoom As Double = 1.0
    Private ReadOnly mPanning As New PanningXY()

    Private overGate As BaseGate
    Private ReadOnly mSelectedGates As New List(Of BaseGate)
    Private mSelPin As Pin
    Private selPinUI As GateUI

    Private overPin As Pin
    Private overPinBounds As Rectangle

    Private isLeftMouseDown As Boolean
    Private isRightMouseDown As Boolean
    Private isCtrlDown As Boolean

    Private mouseOrigin As Point
    Private mousePosSnaped As Point
    Private mousePos As Point

    Private selRect As Rectangle = Rectangle.Empty

    Private ReadOnly txtNameEditor As TextBox

    Public Property SnapToGrid As Boolean = True
    Public Property Snap As New Size(10, 10)
    Public Property MultiSelect As Boolean = True
    Public Property [Readonly] As Boolean = False

    Public Event GatesSelectedChanged(sender As Object, e As EventArgs)

    Private Sub CircuitSurface_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        Me.SetStyle(ControlStyles.ResizeRedraw, True)
        Me.SetStyle(ControlStyles.UserPaint, True)
        Me.SetStyle(ControlStyles.Selectable, True)

        AddHandler Me.KeyUp, Sub(s1 As Object, e1 As KeyEventArgs) isCtrlDown = ((e1.Modifiers And Keys.Control) = Keys.Control)
        AddHandler Me.KeyDown, Sub(s1 As Object, e1 As KeyEventArgs) isCtrlDown = ((e1.Modifiers And Keys.Control) = Keys.Control)

        AddHandler Me.SizeChanged, Sub()
                                       If mGateRenderer IsNot Nothing Then
                                           mGateRenderer.SetGridResolution()
                                           mGateRenderer.UpgardeGrid()
                                       End If
                                   End Sub

        AddHandler Me.MouseWheel, Sub(s1 As Object, e1 As MouseEventArgs)
                                      If e1.Delta > 0 Then
                                          mZoom += e1.Delta / 1500
                                      Else
                                          mZoom -= Math.Abs(e1.Delta) / 1500
                                      End If
                                  End Sub
    End Sub

    Public Property Circuit As Component
        Get
            Return mCircuit
        End Get
        Set(value As Component)
            If mCircuit IsNot Nothing Then mCircuit.StopTicking()

            mCircuit = value
            mGateRenderer = New GateRenderer(Me, mCircuit)
            mGateRenderer.UpgardeGrid()

            Dim lastTicked As Long
            If mCircuit IsNot Nothing Then
                AddHandler mCircuit.Ticked, Sub(ticksCount As Long)
                                                If ticksCount - lastTicked > 100000 * 15 Then
                                                    Try
                                                        mCircuit.Evaluate()
                                                    Catch ' FIXME: Fix race condition
                                                    End Try
                                                    Me.Invalidate()
                                                    lastTicked = ticksCount
                                                End If
                                            End Sub
                mCircuit.StartTicking()
                mCircuit.ParentControl = Me
            End If
        End Set
    End Property

    Public ReadOnly Property Panning As PanningXY
        Get
            Return mPanning
        End Get
    End Property

    Public Property Zoom As Double
        Get
            Return mZoom
        End Get
        Set(value As Double)
            mZoom = value
        End Set
    End Property

    Public ReadOnly Property GateRenderer As GateRenderer
        Get
            Return mGateRenderer
        End Get
    End Property

    Public ReadOnly Property SelectedGates As List(Of BaseGate)
        Get
            Return mSelectedGates
        End Get
    End Property

    Public ReadOnly Property SelectedPin As Pin
        Get
            Return mSelPin
        End Get
    End Property

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        Dim g As Graphics = e.Graphics
        Dim r As Rectangle = Me.DisplayRectangle
        r.Width -= 1
        r.Height -= 1

        If mCircuit Is Nothing Then Exit Sub

        g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias

        g.ScaleTransform(mZoom, mZoom)
        g.TranslateTransform(mPanning.X, mPanning.Y)
        If selRect.Width > 0 AndAlso selRect.Height > 0 Then
            Using b As New SolidBrush(Color.FromArgb(64, Color.DimGray))
                g.FillRoundedRectangle(b, selRect, 10, RectangleEdgeFilter.All)
            End Using
            Using p As New Pen(Brushes.DarkGray, 2)
                g.DrawRoundedRectangle(p, selRect, 10, RectangleEdgeFilter.All)
            End Using
        End If

        For Each gt In mCircuit.Gates
            mGateRenderer.ApplyRotation(g, gt.UI, mZoom, mPanning)

            If gt.UI.Path Is Nothing Then
                Select Case gt.GateType
                    Case IBaseGate.GateTypes.AND : gt.UI.Path = mGateRenderer.DrawANDGate(gt)
                    Case IBaseGate.GateTypes.NAND : gt.UI.Path = mGateRenderer.DrawNANDGate(gt)
                    Case IBaseGate.GateTypes.OR : gt.UI.Path = mGateRenderer.DrawORGate(gt)
                    Case IBaseGate.GateTypes.NOR : gt.UI.Path = mGateRenderer.DrawNORGate(gt)
                    Case IBaseGate.GateTypes.XOR : gt.UI.Path = mGateRenderer.DrawXORGate(gt)
                    Case IBaseGate.GateTypes.XNOR : gt.UI.Path = mGateRenderer.DrawXNORGate(gt)
                    Case IBaseGate.GateTypes.NOT : gt.UI.Path = mGateRenderer.DrawNOTGate(gt)
                    Case IBaseGate.GateTypes.Node : gt.UI.Path = mGateRenderer.DrawNode(gt)
                    Case IBaseGate.GateTypes.Led : mGateRenderer.DrawLed(g, gt)
                    Case IBaseGate.GateTypes.Switch : mGateRenderer.DrawSwitch(g, gt)
                    Case IBaseGate.GateTypes.Clock : mGateRenderer.DrawClock(g, gt)
                    Case IBaseGate.GateTypes.Component : Continue For
                End Select
            End If

            If gt.UI.Path IsNot Nothing Then
                If gt.GateType = IBaseGate.GateTypes.Node Then
                    Using p As New SolidBrush(mGateRenderer.GetWireColor(gt.Output))
                        g.FillPath(p, gt.UI.Path)
                    End Using
                Else
                    g.FillPath(gt.UI.FillColorBrush, gt.UI.Path)
                    g.DrawPath(gt.UI.BorderColorPen, gt.UI.Path)
                End If
            End If

            Dim pinUI As GateUI
            If gt.GateType = IBaseGate.GateTypes.Node Then
                If Not [Readonly] Then
                    Dim n = CType(gt, Node)
                    If mSelectedGates.Contains(n) OrElse overGate = n Then
                        pinUI = n.Input.UI
                        g.FillRectangle(Brushes.Blue, New Rectangle(gt.UI.Location + pinUI.Location, pinUI.Size))

                        For Each op In n.OutputsUIs
                            pinUI = If(mSelPin = op, selPinUI, op.UI)
                            g.FillRectangle(Brushes.MediumVioletRed, New Rectangle(gt.UI.Location + pinUI.Location, pinUI.Size))
                        Next
                        For Each ip In gt.Inputs
                            pinUI = If(mSelPin = ip, selPinUI, ip.UI)
                            g.FillRectangle(Brushes.MediumVioletRed, New Rectangle(gt.UI.Location + pinUI.Location, pinUI.Size))
                        Next
                    End If
                End If
            Else
                If gt.Flow <> IBaseGate.DataFlow.Out Then
                    For Each ip In gt.Inputs
                        pinUI = If(mSelPin = ip, selPinUI, ip.UI)
                        g.FillRectangle(Brushes.MediumVioletRed, New Rectangle(gt.UI.Location + pinUI.Location, pinUI.Size))
                    Next
                End If
                If gt.Flow <> IBaseGate.DataFlow.In Then
                    pinUI = If(mSelPin = gt.Output, selPinUI, gt.Output.UI)
                    g.FillRectangle(Brushes.Orange, New Rectangle(gt.UI.Location + pinUI.Location, pinUI.Size))
                End If
            End If

            g.ResetTransform()
            g.ScaleTransform(mZoom, mZoom)
            g.TranslateTransform(mPanning.X, mPanning.Y)
            g.DrawString(gt.Name, gt.UI.Font, gt.UI.ForeColorBrush, gt.UI.Location + gt.UI.NameOffset)
        Next

        mGateRenderer.DrawWiresAStar(g, r, mSelPin, selPinUI)
        'gr.DrawWires(g, r, selPin, selPinUI)

        'For Each gt In mCircuit.Gates.Where(Function(k) k.GateType = IBaseGate.GateTypes.Node)
        '    If gt.UI.Path Is Nothing Then gt.UI.Path = gr.DrawNode(gt)
        '    g.FillPath(gt.UI.FillColor, gt.UI.Path)
        'Next

#If DEBUG Then
        ' This is for debugging purposes
        'DrawGrid(g)
#End If

        'mCircuit.Gates.Where(Function(k) k.GateType = IBaseGate.GateTypes.Node).ToList().ForEach(Sub(gt) g.FillPath(gt.UI.FillColor, gt.UI.Path))

        If mSelPin IsNot Nothing Then
            g.FillRectangle(Brushes.Blue, New Rectangle(mSelPin.ParentGate.UI.Location + mSelPin.UI.Location, mSelPin.UI.Size))

            If overPin IsNot Nothing Then
                Dim opb = overPinBounds
                opb.Inflate(10, 10)
                DrawSelection(g, opb)
            End If
        ElseIf overPin IsNot Nothing Then
            mGateRenderer.ApplyRotation(g, overPin.ParentGate.UI, mZoom, mPanning)
            g.FillRectangle(Brushes.Red, overPinBounds)
            g.ResetTransform()
        Else
            If overGate IsNot Nothing Then
                Dim overBounds = overGate.UI.Bounds
                overBounds.Inflate(10, 10)

                mGateRenderer.ApplyRotation(g, overGate.UI, mZoom, mPanning)
                DrawSelection(g, overBounds)
                g.ResetTransform()
            End If

            For Each gt In mSelectedGates
                Dim selBounds = gt.UI.Bounds
                selBounds.Inflate(10, 10)

                mGateRenderer.ApplyRotation(g, gt.UI, mZoom, mPanning)
                DrawSelection(g, selBounds)
                g.ResetTransform()
            Next
        End If
    End Sub

    Private Sub DrawSelection(g As Graphics, r As Rectangle)
        Using b As New SolidBrush(Color.FromArgb(64, Color.DodgerBlue))
            g.FillRoundedRectangle(b, r, 10, RectangleEdgeFilter.All)
        End Using
        Using p As New Pen(Brushes.DarkGray, 2)
            g.DrawRoundedRectangle(p, r, 10, RectangleEdgeFilter.All)
        End Using
    End Sub

    Private Sub DrawGrid(g As Graphics)
        Dim r As Rectangle
        Using p As New SolidBrush(Color.FromArgb(128, Color.LightGreen))
            For x As Integer = 0 To mGateRenderer.GridSize.Width - 1
                For y As Integer = 0 To mGateRenderer.GridSize.Height - 1
                    r = New Rectangle(x * mGateRenderer.GridResolution.Width, y * mGateRenderer.GridResolution.Height, mGateRenderer.GridResolution.Width + 1, mGateRenderer.GridResolution.Height + 1)
                    If mGateRenderer.Grid(x, y) <> 1 Then g.FillRectangle(p, r)
                    'g.DrawRectangle(Pens.Green, r)
                Next
            Next
        End Using
    End Sub

    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        Dim isShiftDown As Boolean
        If (keyData And Keys.Shift) = Keys.Shift Then
            keyData = keyData Xor Keys.Shift
            isShiftDown = True
        End If

        If txtNameEditor Is Nothing Then
            Select Case keyData
                Case Keys.Delete
                    mSelectedGates.ForEach(Sub(gt)
                                               gt.Inputs.ForEach(Sub(i)
                                                                     i.ConnectedFromGate?.Output.Disconnect()
                                                                     i.Disconnect()
                                                                 End Sub)
                                               If gt.GateType = IBaseGate.GateTypes.Node Then
                                                   CType(gt, Node).Outputs.ForEach(Sub(o) o?.Pin.Disconnect())
                                               End If
                                               gt.Output.Disconnect()
                                               For i As Integer = 0 To mCircuit.Inputs.Count - 1
                                                   If mCircuit.Inputs(i).ParentGate = gt Then
                                                       mCircuit.RemoveInputPin(mCircuit.Inputs(i))
                                                       Exit For
                                                   End If
                                               Next
                                               For i As Integer = 0 To mCircuit.Outputs.Count - 1
                                                   If mCircuit.Outputs(i).ParentGate = gt Then
                                                       mCircuit.RemoveOutputPin(mCircuit.Outputs(i))
                                                       Exit For
                                                   End If
                                               Next
                                               mCircuit.Gates.Remove(gt)
                                           End Sub)
                    mSelectedGates.Clear()
                    overGate = Nothing
                    overPin = Nothing
                    mSelPin = Nothing
                    mCircuit.Evaluate()
                    mGateRenderer.UpgardeGrid()

                Case Keys.Tab
                    mSelectedGates.ForEach(Sub(gt) gt.UI.Angle += 15 * If(isShiftDown, -1, 1))
                    mGateRenderer.UpgardeGrid()
                    Return True

            End Select
        End If

        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    Private Sub CircuitSurface_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown
        If mCircuit Is Nothing Then Exit Sub

        'If txtNameEditor IsNot Nothing Then RemoveTextbox(True)

        Dim p As Point = e.Location
        p.X /= mZoom
        p.Y /= mZoom
        p.X -= mPanning.X
        p.Y -= mPanning.Y

        selRect = New Rectangle(mouseOrigin, New Size())
        isLeftMouseDown = (e.Button = Windows.Forms.MouseButtons.Left)
        isRightMouseDown = (e.Button = Windows.Forms.MouseButtons.Right)

        mouseOrigin = mGateRenderer.TransformPoint(p)

        If SnapToGrid Then
            mouseOrigin.X -= mouseOrigin.X Mod Snap.Width
            mouseOrigin.Y -= mouseOrigin.Y Mod Snap.Height
        End If

        If isLeftMouseDown Then
            If overPin IsNot Nothing Then
                mSelPin = overPin
                selPinUI = mSelPin.UI.Clone()
                mSelPin.UI.Location = mGateRenderer.TransformPoint(mSelPin.ParentGate.UI.Location + mSelPin.UI.Location, mSelPin.ParentGate) - mSelPin.ParentGate.UI.Location
                overPin = Nothing
                mSelectedGates.Clear()
            Else
                mSelPin = Nothing
                If overGate Is Nothing Then
                    mSelectedGates.Clear()
                Else
                    If Not isCtrlDown AndAlso Not mSelectedGates.Contains(overGate) Then mSelectedGates.Clear()
                    If isCtrlDown AndAlso mSelectedGates.Contains(overGate) Then
                        mSelectedGates.Remove(overGate)
                        Exit Sub
                    End If
                    If Not mSelectedGates.Contains(overGate) Then
                        If Not MultiSelect Then mSelectedGates.Clear()
                        mSelectedGates.Add(overGate)
                    End If
                End If
            End If
        End If

        RaiseEvent GatesSelectedChanged(Me, New EventArgs())
    End Sub

    Private Sub CircuitSurface_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
        If mCircuit Is Nothing Then Exit Sub

        Dim p As Point = e.Location
        p.X /= mZoom
        p.Y /= mZoom
        p.X -= mPanning.X
        p.Y -= mPanning.Y

        mousePosSnaped = mGateRenderer.TransformPoint(p)
        mousePos = mousePosSnaped

        If SnapToGrid Then
            mousePosSnaped.X -= mousePosSnaped.X Mod Snap.Width
            mousePosSnaped.Y -= mousePosSnaped.Y Mod Snap.Height
        End If

        Dim deltaX As Integer = (mousePosSnaped.X - mouseOrigin.X)
        Dim deltaY As Integer = (mousePosSnaped.Y - mouseOrigin.Y)

        If isLeftMouseDown Then
            If mSelectedGates.Count > 0 AndAlso Not [Readonly] Then
                For Each gt In mSelectedGates
                    gt.UI.Path = Nothing
                    gt.UI.X += deltaX
                    gt.UI.Y += deltaY
                Next
                mouseOrigin += (mousePosSnaped - mouseOrigin)
                mGateRenderer.UpgardeGrid()
            ElseIf overGate Is Nothing Then
                If mSelPin Is Nothing Then
                    If MultiSelect Then
                        Dim p1 As Point = New Point(Math.Min(mouseOrigin.X, mousePosSnaped.X), Math.Min(mouseOrigin.Y, mousePosSnaped.Y))
                        Dim p2 As Point = New Point(Math.Max(mouseOrigin.X, mousePosSnaped.X), Math.Max(mouseOrigin.Y, mousePosSnaped.Y))
                        selRect = New Rectangle(p1, New Size(p2.X - p1.X, p2.Y - p1.Y))
                    End If
                Else
                    mSelPin.UI.Bounds = New Rectangle(mSelPin.UI.Bounds.X + deltaX,
                                                      mSelPin.UI.Bounds.Y + deltaY,
                                                      mSelPin.UI.Bounds.Width,
                                                      mSelPin.UI.Bounds.Height)
                    mouseOrigin += (mousePosSnaped - mouseOrigin)
                    mGateRenderer.UpgardeGrid()
                End If
            End If
        ElseIf isRightMouseDown Then
            mPanning.X += deltaX
            mPanning.Y += deltaY
            mouseOrigin += (mousePosSnaped - mouseOrigin)
            Exit Sub
        End If

        Dim lastMousePos As Point = mousePosSnaped
        For Each gt In mCircuit.Gates
            mousePosSnaped = If(gt.UI.Angle <> 0, mGateRenderer.TransformPoint(mousePos, gt), mousePos)
            If Not isLeftMouseDown AndAlso gt.UI.Bounds.Contains(mousePosSnaped) Then
                overGate = gt
                overPin = Nothing
                Exit Sub
            ElseIf Not [Readonly] Then
                Dim pb As Rectangle

                If gt.Flow <> IBaseGate.DataFlow.In Then
                    Dim outputs As New List(Of Pin)
                    If gt.GateType = IBaseGate.GateTypes.Node Then
                        Dim n As Node = CType(gt, Node)
                        For i As Integer = 0 To n.Outputs.Count - 1
                            outputs.Add(n.OutputsUIs(i))
                        Next
                    Else
                        outputs.Add(gt.Output)
                    End If
                    For Each o In outputs
                        pb = If(mSelPin = o,
                                New Rectangle(gt.UI.Location + selPinUI.Location, selPinUI.Size),
                                New Rectangle(gt.UI.Location + o.UI.Location, o.UI.Size))
                        If gt.UI.Angle <> 0 Then pb.Location = mGateRenderer.TransformPoint(pb.Location, gt)

                        If pb.Contains(p) Then
                            overPin = o
                            overPinBounds = New Rectangle(overPin.ParentGate.UI.Location + overPin.UI.Location, overPin.UI.Size)
                            overGate = Nothing
                            Exit Sub
                        End If
                    Next
                End If

                If gt.Flow <> IBaseGate.DataFlow.Out Then
                    For Each ip In gt.Inputs
                        pb = If(mSelPin = ip,
                                New Rectangle(gt.UI.Location + selPinUI.Location, selPinUI.Size),
                                New Rectangle(gt.UI.Location + ip.UI.Location, ip.UI.Size))
                        If gt.UI.Angle <> 0 Then pb.Location = mGateRenderer.TransformPoint(pb.Location, gt)

                        If pb.Contains(p) Then
                            overPin = ip
                            overPinBounds = New Rectangle(overPin.ParentGate.UI.Location + overPin.UI.Location, overPin.UI.Size)
                            overGate = Nothing
                            Exit Sub
                        End If
                    Next
                End If
            End If
        Next

        If overGate IsNot Nothing Then
            If Not isLeftMouseDown Then overGate = Nothing
        ElseIf overPin IsNot Nothing Then
            overPin = Nothing
        End If
    End Sub

    Private Sub CircuitSurface_MouseUp(sender As Object, e As MouseEventArgs) Handles Me.MouseUp
        If mCircuit Is Nothing Then Exit Sub
        If isLeftMouseDown Then isLeftMouseDown = Not (e.Button = Windows.Forms.MouseButtons.Left)
        If isRightMouseDown Then
            isRightMouseDown = Not (e.Button = Windows.Forms.MouseButtons.Right)
            If Not isRightMouseDown Then Exit Sub
        End If

        If SnapToGrid Then
            mousePosSnaped.X -= mousePosSnaped.X Mod Snap.Width
            mousePosSnaped.Y -= mousePosSnaped.Y Mod Snap.Height
        End If

        If selRect.Width > 0 Then
            mSelectedGates.Clear()

            For Each gt In mCircuit.Gates
                Dim p1 As Point = selRect.Location
                Dim p2 As Point = New Point(selRect.Right, selRect.Bottom)
                p2.X -= p1.X
                p2.Y -= p1.Y
                Dim testRect As Rectangle = New Rectangle(p1, p2)
                If testRect.IntersectsWith(gt.UI.Bounds) Then mSelectedGates.Add(gt)
            Next
            selRect = Rectangle.Empty
        ElseIf mSelPin IsNot Nothing Then
            If overPin Is Nothing Then
                If mSelPin.ConnectedToPinNumber = -1 Then
                    ' Disconnect Input Pin
                    Dim pc As Component.PinConnection = Nothing

                    For Each gt In mCircuit.Gates
                        If gt.GateType = IBaseGate.GateTypes.Node Then
                            Dim node = CType(gt, Node)
                            For Each o In node.Outputs
                                If o Is Nothing Then Continue For
                                If o.Pin = mSelPin Then
                                    pc = o
                                    Exit For
                                End If
                            Next
                            If pc IsNot Nothing Then
                                node.Disconnect(pc)
                                Exit For
                            End If
                        ElseIf gt.Output.ConnectedToPin = mSelPin Then
                            If gt.Output IsNot Nothing Then gt.Output.Disconnect()
                            Exit For
                        End If
                    Next

                    '' It's a wire
                    'Dim addNode As Boolean = False
                    'Dim p As New Point(mousePos.X / mGateRenderer.GridResolution.Width, mousePos.Y / mGateRenderer.GridResolution.Height)
                    'For x As Integer = -1 To 1
                    '    For y As Integer = -1 To 1
                    '        If mGateRenderer.Grid(p.X + x, p.Y + y) = 128 Then
                    '            addNode = True
                    '            Exit For
                    '        End If
                    '    Next
                    'Next

                    'If addNode Then
                    '    ' Add a node to the wire
                    '    Dim n As New Node()
                    '    mCircuit.Gates.Add(n)
                    '    n.UI.Location = mousePos
                    '    If selPin.ParentGate.Output = selPin Then
                    '        selPin.ConnectTo(n, 0)
                    '    Else
                    '        For i As Integer = 0 To selPin.ParentGate.Inputs.Count
                    '            If selPin.ParentGate.Inputs(i) = selPin Then
                    '                n.ConnectTo(selPin.ParentGate, i, 0)
                    '                Exit For
                    '            End If
                    '        Next
                    '    End If

                    '    Me.Invalidate()
                    '    MsgBox("Creating nodes is not yet supported...")
                    'End If
                Else
                    ' Disconnect Output Pin
                    mSelPin.ParentGate.Output.Disconnect()
                End If
            ElseIf mSelPin <> overPin Then
                If mSelPin = mSelPin.ParentGate.Output Then
                    ' Connect Output Pin
                    If BaseGate.GetGateConnectedToInput(mCircuit, overPin) Is Nothing Then
                        If overPin.ParentGate.GateType = IBaseGate.GateTypes.Node Then
                            Dim node = CType(overPin.ParentGate, Node)
                            If BaseGate.GetGateConnectedToInput(mCircuit, node.Input) Is Nothing Then
                                node.ConnectTo(mSelPin.ParentGate, mSelPin)
                            Else
                                ' TODO: Notify user that nodes do not support multiple connections from outputs
                                ' as they only have one input.
                            End If
                        Else
                            mSelPin.Disconnect()
                            mSelPin.ConnectTo(overPin.ParentGate, overPin)
                        End If
                    Else
                        ' TODO: Notify user that inputs only support one connection.
                        ' Or, alternatively, add a node.
                    End If
                Else
                    If overPin.ConnectedToPinNumber = -1 Then
                        ' Connect Output Pin
                        Dim gt = BaseGate.GetGateConnectedToInput(mCircuit, mSelPin)
                        If gt Is Nothing Then
                            If overPin.ParentGate.GateType = IBaseGate.GateTypes.Node Then
                                CType(overPin.ParentGate, Node).ConnectTo(mSelPin.ParentGate, mSelPin.PinNumber, overPin.PinNumber)
                            ElseIf mSelPin.ParentGate.GateType = IBaseGate.GateTypes.Node Then
                                CType(mSelPin.ParentGate, Node).ConnectTo(overPin.ParentGate, overPin.PinNumber, mSelPin.PinNumber)
                            Else
                                If overPin.ParentGate.Output = overPin Then
                                    ' Connect from Input pin to Output pin
                                    overPin.Disconnect()
                                    overPin.ConnectTo(mSelPin.ParentGate, mSelPin)
                                Else
                                    ' TODO: Notify user that inputs cannot be connected to inputs.
                                    ' Or, alternatively, add a node.
                                End If
                            End If
                        Else
                            If gt.GateType = IBaseGate.GateTypes.Node Then
                                Dim node As Node = CType(gt, Node)
                                For Each o In node.Outputs
                                    If o.Pin = mSelPin Then
                                        node.Disconnect(o)
                                        node.ConnectTo(overPin.ParentGate, overPin)
                                        Exit For
                                    End If
                                Next
                            Else
                                gt.Output.Disconnect()
                                gt.Output.ConnectTo(overPin.ParentGate, overPin)
                            End If
                        End If
                    Else
                        ' Connect Input Pin to Output Pin
                        overPin.Disconnect()
                        overPin.ConnectTo(mSelPin.ParentGate, mSelPin)
                    End If
                End If
                mCircuit.Evaluate()
            Else
                mSelPin = overPin
                mSelectedGates.Clear()
            End If

            Dim isDone As Boolean
            Do
                isDone = True
                For Each gt In mCircuit.Gates.Where(Function(k) k.GateType = IBaseGate.GateTypes.Node)
                    Dim node = CType(gt, Node)
                    Dim hasInput As Boolean = False

                    For Each gt2 In mCircuit.Gates
                        If gt2.Output.ConnectedToPin = node.Input Then
                            hasInput = True
                            Exit For
                        End If
                    Next

                    If node.Outputs.Count = 1 AndAlso hasInput Then
                        Dim g = BaseGate.GetGateConnectedToInput(mCircuit, node.Input)
                        g.Output.Disconnect()
                        g.Output.ConnectTo(node.Outputs(0).Pin.ParentGate, node.Outputs(0).PinNumber)
                        mCircuit.Gates.Remove(node)
                        isDone = False
                        Exit For
                    ElseIf node.Outputs.Count = 0 AndAlso Not hasInput Then
                        mCircuit.Gates.Remove(node)
                        isDone = False
                        Exit For
                    End If
                Next
            Loop Until isDone

            mSelPin.UI.Location = selPinUI.Location
            overPin = Nothing
            'mSelPin = Nothing
        End If

        RaiseEvent GatesSelectedChanged(Me, New EventArgs())
    End Sub

    Private Sub CircuitSurface_Click(sender As Object, e As EventArgs) Handles Me.Click
        If mCircuit Is Nothing OrElse [Readonly] Then Exit Sub

        If overGate?.GateType = IBaseGate.GateTypes.Switch Then overGate.Inputs(0).Value = Not overGate.Inputs(0).Value
    End Sub

    'Private Sub CircuitSurface_DoubleClick(sender As Object, e As EventArgs) Handles Me.DoubleClick
    '    If mCircuit Is Nothing OrElse [Readonly] Then Exit Sub

    '    If overGate IsNot Nothing Then
    '        Dim w As Integer = TextRenderer.MeasureText(overGate.Name, overGate.UI.Font).Width
    '        txtNameEditor = New TextBox With {
    '            .Location = New Point(overGate.UI.X + overGate.UI.NameOffset.X - 1, overGate.UI.Y + overGate.UI.NameOffset.Y - 1),
    '            .Width = If(w = 0, 80, w),
    '            .Tag = overGate,
    '            .Text = overGate.Name,
    '            .Visible = True
    '        }
    '        Me.Controls.Add(txtNameEditor)
    '        txtNameEditor.Focus()

    '        AddHandler txtNameEditor.Validated, Sub() RemoveTextbox(True)
    '        AddHandler txtNameEditor.KeyDown, Sub(o1 As Object, e1 As KeyEventArgs)
    '                                              If e1.KeyCode = Keys.Escape Then RemoveTextbox(False)
    '                                              If e1.KeyCode = Keys.Enter Then RemoveTextbox(True)
    '                                              If (e1.Modifiers And Keys.Control) = Keys.Control Then
    '                                                  Dim gUI As GateUI = CType(txtNameEditor.Tag, BaseGate).UI
    '                                                  Dim k As Integer = If((e1.Modifiers And Keys.Shift) = Keys.Shift, 4, 2)
    '                                                  Select Case e1.KeyCode
    '                                                      Case Keys.Up : gUI.NameOffset = New Point(gUI.NameOffset.X, gUI.NameOffset.Y - k)
    '                                                      Case Keys.Down : gUI.NameOffset = New Point(gUI.NameOffset.X, gUI.NameOffset.Y + k)
    '                                                      Case Keys.Left : gUI.NameOffset = New Point(gUI.NameOffset.X - k, gUI.NameOffset.Y)
    '                                                      Case Keys.Right : gUI.NameOffset = New Point(gUI.NameOffset.X + k, gUI.NameOffset.Y)
    '                                                  End Select
    '                                                  txtNameEditor.Location = New Point(gUI.X + gUI.NameOffset.X - 1, gUI.Y + gUI.NameOffset.Y - 1)
    '                                              End If
    '                                          End Sub
    '    End If
    'End Sub

    'Private Sub RemoveTextbox(updateGateText As Boolean)
    '    If updateGateText Then CType(txtNameEditor.Tag, BaseGate).Name = txtNameEditor.Text
    '    Me.Controls.Remove(txtNameEditor)
    '    txtNameEditor = Nothing
    'End Sub
End Class
