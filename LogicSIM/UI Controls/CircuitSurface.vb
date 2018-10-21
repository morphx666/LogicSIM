Public Class CircuitSurface

    Private mCircuit As LogicGates.Component

    Private gr As GateRenderer

    Private overGate As LogicGates.BaseGate
    Private selGates As New List(Of LogicGates.BaseGate)
    Private selPin As LogicGates.Pin
    Private selPinUI As GateUI

    Private overPin As LogicGates.Pin
    Private overPinBounds As Rectangle

    Private isMouseDown As Boolean
    Private isCtrlDown As Boolean

    Private mouseOrigin As Point
    Private mousePos As Point

    Private selRect As Rectangle = Rectangle.Empty

    Private mSnapToGrid As Boolean = True
    Private mSnap As New Size(10, 10)

    Private Sub CircuitSurface_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        Me.SetStyle(ControlStyles.ResizeRedraw, True)
        Me.SetStyle(ControlStyles.UserPaint, True)
        Me.SetStyle(ControlStyles.Selectable, True)

        AddHandler KeyUp, Sub(s1 As Object, e1 As KeyEventArgs) isCtrlDown = ((e1.Modifiers And Keys.Control) = Keys.Control)
        AddHandler KeyDown, Sub(s1 As Object, e1 As KeyEventArgs) isCtrlDown = ((e1.Modifiers And Keys.Control) = Keys.Control)

        AddHandler Me.SizeChanged, Sub()
                                       gr.SetGridResolution()
                                       Me.Invalidate()
                                   End Sub
    End Sub

    Public Property Circuit As LogicGates.Component
        Get
            Return mCircuit
        End Get
        Set(value As LogicGates.Component)
            mCircuit = value
            gr = New GateRenderer(Me, mCircuit)
            Me.Invalidate()
        End Set
    End Property

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        Dim g As Graphics = e.Graphics
        Dim r As Rectangle = Me.DisplayRectangle
        r.Width -= 1
        r.Height -= 1

        If mCircuit Is Nothing Then Exit Sub

        g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias

        gr.UpgardeGrid()

        If selRect.Width > 0 AndAlso selRect.Height > 0 Then
            Using b As New SolidBrush(Color.FromArgb(64, Color.DimGray))
                g.FillRoundedRectangle(b, selRect, 10, RectangleEdgeFilter.All)
            End Using
            Using p As New Pen(Brushes.DarkGray, 2)
                g.DrawRoundedRectangle(p, selRect, 10, RectangleEdgeFilter.All)
            End Using
        End If

        For Each gt In mCircuit.Gates
            If gt.UI.Path Is Nothing Then
                Select Case gt.GateType
                    Case IBaseGate.GateTypes.AND : gt.UI.Path = gr.DrawANDGate(gt)
                    Case IBaseGate.GateTypes.NAND : gt.UI.Path = gr.DrawNANDGate(gt)
                    Case IBaseGate.GateTypes.OR : gt.UI.Path = gr.DrawORGate(gt)
                    Case IBaseGate.GateTypes.NOR : gt.UI.Path = gr.DrawNORGate(gt)
                    Case IBaseGate.GateTypes.XOR : gt.UI.Path = gr.DrawXORGate(gt)
                    Case IBaseGate.GateTypes.NOT : gt.UI.Path = gr.DrawNOTGate(gt)
                    Case IBaseGate.GateTypes.Node : gt.UI.Path = gr.DrawNode(gt)
                    Case IBaseGate.GateTypes.Component : Continue For
                End Select
            End If

            gr.ApplyRotation(g, gt.UI)

            If gt.UI.Path Is Nothing Then
                Select Case gt.GateType
                    Case IBaseGate.GateTypes.Led : gr.DrawLed(g, gt)
                    Case IBaseGate.GateTypes.Switch : gr.DrawSwitch(g, gt)
                End Select
            Else
                If gt.GateType = IBaseGate.GateTypes.Node Then
                    Using p As New SolidBrush(gr.GetWireColor(gt.Output))
                        g.FillPath(p, gt.UI.Path)
                    End Using
                Else
                    g.FillPath(gt.UI.FillColor, gt.UI.Path)
                    g.DrawPath(gt.UI.BorderColor, gt.UI.Path)
                End If
            End If

            Dim pinUI As GateUI
            If gt.GateType = IBaseGate.GateTypes.Node Then
                Dim n = CType(gt, LogicGates.Node)
                If selGates.Contains(n) OrElse overGate = n Then
                    pinUI = n.Input.UI
                    g.FillRectangle(Brushes.Blue, New Rectangle(gt.UI.Location + pinUI.Location, pinUI.Size))

                    For Each pinUI In n.OutputsUIs
                        g.FillRectangle(Brushes.MediumVioletRed, New Rectangle(gt.UI.Location + pinUI.Location, pinUI.Size))
                    Next
                End If
            Else
                If gt.Flow <> IBaseGate.DataFlow.Out Then
                    For Each ip In gt.Inputs
                        pinUI = If(selPin = ip, selPinUI, ip.UI)
                        g.FillRectangle(Brushes.MediumVioletRed, New Rectangle(gt.UI.Location + pinUI.Location, pinUI.Size))
                    Next
                End If
                If gt.Flow <> IBaseGate.DataFlow.In Then
                    pinUI = If(selPin = gt.Output, selPinUI, gt.Output.UI)
                    g.FillRectangle(Brushes.Orange, New Rectangle(gt.UI.Location + pinUI.Location, pinUI.Size))
                End If
            End If

            g.ResetTransform()
            g.DrawString(gt.Name, gt.UI.Font, gt.UI.ForeColor, gt.UI.Location + gt.UI.NameOffset)
        Next

        gr.DrawWiresAStar(g, r, selPin, selPinUI)
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

        If selPin IsNot Nothing Then
            g.FillRectangle(Brushes.Blue, New Rectangle(selPin.ParentGate.UI.Location + selPin.UI.Location, selPin.UI.Size))

            If overPin IsNot Nothing Then
                Dim opb = overPinBounds
                opb.Inflate(10, 10)
                DrawSelection(g, opb)
            End If
        ElseIf overPin IsNot Nothing Then
            gr.ApplyRotation(g, overPin.ParentGate.UI)
            g.FillRectangle(Brushes.Red, overPinBounds)
            g.ResetTransform()
        Else
            If overGate IsNot Nothing AndAlso overGate.GateType <> IBaseGate.GateTypes.Node Then
                Dim overBounds = overGate.UI.Bounds
                overBounds.Inflate(10, 10)

                gr.ApplyRotation(g, overGate.UI)
                DrawSelection(g, overBounds)
                g.ResetTransform()
            End If

            For Each gt In selGates
                Dim selBounds = gt.UI.Bounds
                selBounds.Inflate(10, 10)

                gr.ApplyRotation(g, gt.UI)
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
            For x As Integer = 0 To gr.GridSize.Width - 1
                For y As Integer = 0 To gr.GridSize.Height - 1
                    r = New Rectangle(x * gr.GridResolution.Width, y * gr.GridResolution.Height, gr.GridResolution.Width + 1, gr.GridResolution.Height + 1)
                    If gr.Grid(x, y) <> 1 Then g.FillRectangle(p, r)
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

        Select Case keyData
            Case Keys.Tab
                For Each gt In selGates
                    gt.UI.Angle += 15 * If(isShiftDown, -1, 1)
                Next

                gr.UpgardeGrid()
                Me.Invalidate()
                Return False
            Case Else
                Return MyBase.ProcessCmdKey(msg, keyData)
        End Select
    End Function

    Private Sub CircuitSurface_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown
        selRect = New Rectangle(mouseOrigin, New Size())
        isMouseDown = (e.Button = Windows.Forms.MouseButtons.Left)

        mouseOrigin = gr.TransformPoint(e.Location)

        If mSnapToGrid Then
            mouseOrigin.X -= mouseOrigin.X Mod mSnap.Width
            mouseOrigin.Y -= mouseOrigin.Y Mod mSnap.Height
        End If

        If overPin IsNot Nothing Then
            selPin = overPin
            selPinUI = selPin.UI.Clone()
            selPin.UI.Location = gr.TransformPoint(selPin.ParentGate.UI.Location + selPin.UI.Location, selPin.ParentGate) - selPin.ParentGate.UI.Location
            overPin = Nothing
            selGates.Clear()
        Else
            selPin = Nothing
            If overGate Is Nothing Then
                selGates.Clear()
                'Me.Invalidate()
            Else
                If Not isCtrlDown AndAlso Not selGates.Contains(overGate) Then selGates.Clear()
                If isCtrlDown AndAlso selGates.Contains(overGate) Then
                    selGates.Remove(overGate)
                    Exit Sub
                End If
                If Not selGates.Contains(overGate) Then selGates.Add(overGate)
            End If
        End If
    End Sub

    Private Sub CircuitSurface_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
        mousePos = gr.TransformPoint(e.Location)

        If mSnapToGrid Then
            mousePos.X -= mousePos.X Mod mSnap.Width
            mousePos.Y -= mousePos.Y Mod mSnap.Height
        End If

        Dim deltaX As Integer = (mousePos.X - mouseOrigin.X)
        Dim deltaY As Integer = (mousePos.Y - mouseOrigin.Y)

        If isMouseDown Then
            If selGates.Count > 0 Then
                For Each gt In selGates
                    gt.UI.Path = Nothing
                    gt.UI.Bounds = New Rectangle(gt.UI.Bounds.X + deltaX,
                                                 gt.UI.Bounds.Y + deltaY,
                                                 gt.UI.Bounds.Width,
                                                 gt.UI.Bounds.Height)
                Next
                mouseOrigin += (mousePos - mouseOrigin)
            ElseIf overGate Is Nothing Then
                If selPin Is Nothing Then
                    Dim p1 As Point = New Point(Math.Min(mouseOrigin.X, mousePos.X), Math.Min(mouseOrigin.Y, mousePos.Y))
                    Dim p2 As Point = New Point(Math.Max(mouseOrigin.X, mousePos.X), Math.Max(mouseOrigin.Y, mousePos.Y))
                    selRect = New Rectangle(p1, New Size(p2.X - p1.X, p2.Y - p1.Y))
                Else
                    selPin.UI.Bounds = New Rectangle(selPin.UI.Bounds.X + deltaX,
                                                 selPin.UI.Bounds.Y + deltaY,
                                                 selPin.UI.Bounds.Width,
                                                 selPin.UI.Bounds.Height)
                    mouseOrigin += (mousePos - mouseOrigin)
                End If
            End If
        End If

        Dim lastMousePos As Point = mousePos
        For Each gt In mCircuit.Gates
            mousePos = If(gt.UI.Angle <> 0, gr.TransformPoint(mousePos, gt), lastMousePos)

            If Not isMouseDown AndAlso gt.UI.Bounds.Contains(mousePos) Then
                overGate = gt
                overPin = Nothing
                Me.Invalidate()
                Exit Sub
            Else
                If gt.GateType <> IBaseGate.GateTypes.Node OrElse True Then
                    Dim pb As Rectangle

                    If gt.Flow <> IBaseGate.DataFlow.In Then
                        pb = If(selPin = gt.Output,
                            New Rectangle(gt.UI.Location + selPinUI.Location, selPinUI.Size),
                            New Rectangle(gt.UI.Location + gt.Output.UI.Location, gt.Output.UI.Size))
                        If gt.UI.Angle <> 0 Then pb.Location = gr.TransformPoint(pb.Location, gt)

                        If pb.Contains(e.Location) Then
                            overPin = gt.Output
                            overPinBounds = New Rectangle(overPin.ParentGate.UI.Location + overPin.UI.Location, overPin.UI.Size)
                            overGate = Nothing
                            Me.Invalidate()
                            Exit Sub
                        End If
                    End If

                    If gt.Flow <> IBaseGate.DataFlow.Out Then
                        For Each ip In gt.Inputs
                            pb = If(selPin = ip,
                                New Rectangle(gt.UI.Location + selPinUI.Location, selPinUI.Size),
                                New Rectangle(gt.UI.Location + ip.UI.Location, ip.UI.Size))
                            If gt.UI.Angle <> 0 Then pb.Location = gr.TransformPoint(pb.Location, gt)

                            If pb.Contains(e.Location) Then
                                overPin = ip
                                overPinBounds = New Rectangle(overPin.ParentGate.UI.Location + overPin.UI.Location, overPin.UI.Size)
                                overGate = Nothing
                                Me.Invalidate()
                                Exit Sub
                            End If
                        Next
                    End If
                End If
            End If
        Next

        If overGate IsNot Nothing Then
            overGate = Nothing
            Me.Invalidate()
            Exit Sub
        ElseIf overPin IsNot Nothing Then
            overPin = Nothing
            Me.Invalidate()
            Exit Sub
        End If

        If isMouseDown Then Me.Invalidate()
    End Sub

    Private Sub CircuitSurface_MouseUp(sender As Object, e As MouseEventArgs) Handles Me.MouseUp
        isMouseDown = False

        If selRect.Width > 0 Then
            selGates.Clear()

            For Each gt In mCircuit.Gates
                Dim p1 As Point = selRect.Location
                Dim p2 As Point = New Point(selRect.Right, selRect.Bottom)
                p2.X -= p1.X
                p2.Y -= p1.Y
                Dim testRect As Rectangle = New Rectangle(p1, p2)
                If testRect.IntersectsWith(gt.UI.Bounds) Then selGates.Add(gt)
            Next

            If selGates.Count > 0 Then Me.Invalidate()
            selRect = Rectangle.Empty
        ElseIf selPin IsNot Nothing Then
            If overPin Is Nothing Then
                If selPin.ConnectedToPinNumber = -1 Then
                    ' Disconnect Input Pin
                    Dim pc As LogicGates.Component.PinConnection = Nothing

                    For Each gt In mCircuit.Gates
                        If gt.GateType = IBaseGate.GateTypes.Node Then
                            Dim node = CType(gt, LogicGates.Node)
                            For Each o In node.Outputs
                                If o Is Nothing Then Continue For
                                If o.Pin = selPin Then
                                    pc = o
                                    Exit For
                                End If
                            Next
                            If pc IsNot Nothing Then
                                node.Disconnect(pc)
                                Exit For
                            End If
                        ElseIf gt.Output.ConnectedToPin = selPin Then
                            If gt.Output IsNot Nothing Then gt.Output.Disconnect()
                            Exit For
                        End If
                    Next
                Else
                    ' Disconnect Output Pin
                    selPin.ParentGate.Output.Disconnect()
                End If
            ElseIf selPin <> overPin Then
                If selPin = selPin.ParentGate.Output Then
                    ' Connect Output Pin
                    If LogicGates.BaseGate.GetGateConnectedToInput(mCircuit, overPin) Is Nothing Then
                        If overPin.ParentGate.GateType = IBaseGate.GateTypes.Node Then
                            Dim node = CType(overPin.ParentGate, LogicGates.Node)
                            If LogicGates.BaseGate.GetGateConnectedToInput(mCircuit, node.Input) Is Nothing Then
                                node.ConnectTo(selPin.ParentGate, selPin)
                            Else
                                ' TODO: Notify user that nodes do not support multiple connections from outputs
                                ' as they only have one input.
                            End If
                        Else
                            selPin.Disconnect()
                            selPin.ConnectTo(overPin.ParentGate, overPin)
                        End If
                    Else
                        ' TODO: Notify user that inputs only support one connection.
                        ' Or, alternatively, add a node.
                    End If
                Else
                    If overPin.ConnectedToPinNumber = -1 Then
                        ' Connect Output Pin
                        Dim gt = LogicGates.BaseGate.GetGateConnectedToInput(mCircuit, selPin)
                        If gt Is Nothing Then
                            If overPin.ParentGate.GateType = IBaseGate.GateTypes.Node Then
                                CType(overPin.ParentGate, LogicGates.Node).ConnectTo(selPin.ParentGate, selPin)
                            Else
                                If overPin.ParentGate.Output = overPin Then
                                    ' Connect from Input pin to Output pin
                                    overPin.Disconnect()
                                    overPin.ConnectTo(selPin.ParentGate, selPin)
                                Else
                                    ' TODO: Notify user that inputs cannot be connected to inputs.
                                    ' Or, alternatively, add a node.
                                End If
                            End If
                        Else
                            If gt.GateType = IBaseGate.GateTypes.Node Then
                                Dim node = CType(gt, LogicGates.Node)
                                For Each o In node.Outputs
                                    If o.Pin = selPin Then
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
                        overPin.ConnectTo(selPin.ParentGate, selPin)
                    End If
                End If
            End If

            Dim isDone As Boolean
            Do
                isDone = True
                For Each gt In mCircuit.Gates.Where(Function(k) k.GateType = IBaseGate.GateTypes.Node)
                    Dim node = CType(gt, LogicGates.Node)
                    Dim hasInput As Boolean = False

                    For Each gt2 In mCircuit.Gates
                        If gt2.Output.ConnectedToPin = node.Input Then
                            hasInput = True
                            Exit For
                        End If
                    Next

                    'If hasInput Then
                    '    If node.Outputs.Count = 1 Then
                    '        Dim g = GetGateConnectedToInput(node.Input)
                    '        g.Output.ConnectTo(node.Outputs(0).Pin.ParentGate, node.Outputs(0).PinNumber)
                    '        mCircuit.Gates.Remove(node)
                    '        isDone = False
                    '        Exit For
                    '    End If
                    'Else
                    '    mCircuit.Gates.Remove(node)
                    '    isDone = False
                    '    Exit For
                    'End If

                    If node.Outputs.Count = 1 AndAlso hasInput Then
                        Dim g = LogicGates.BaseGate.GetGateConnectedToInput(mCircuit, node.Input)
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

            selPin.UI.Location = selPinUI.Location
            overPin = Nothing
            selPin = Nothing
        End If

        Me.Invalidate()
    End Sub

    Private Sub CircuitSurface_Click(sender As Object, e As EventArgs) Handles Me.Click
        For Each gt In mCircuit.Gates.Where(Function(k) k.GateType = IBaseGate.GateTypes.Switch)
            If gt.UI.Bounds.Contains(mousePos) Then
                gt.Inputs(0).Value = Not gt.Inputs(0).Value
                'Me.Invalidate()
                Exit Sub
            End If
        Next
    End Sub
End Class
