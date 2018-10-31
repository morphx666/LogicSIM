Public Class GateRenderer
    Public Structure WireSegment
        Public Property P1 As Point
        Public Property P2 As Point

        Public Sub New(p1 As Point, p2 As Point)
            Me.P1 = p1
            Me.P2 = p2
        End Sub

        'Public Function Intersetcs(p3 As Point, p4 As Point) As Boolean
        '    Return Intersetcs(p3.X, p3.Y, p4.X, p4.Y)
        'End Function

        'Public Function Intersetcs(x3 As Integer, y3 As Integer, x4 As Integer, y4 As Integer) As Boolean
        '    Dim q As Double = (P1.Y - y3) * (x4 - x3) - (P1.X - x3) * (y4 - y3)
        '    Dim d As Double = (P2.X - P1.X) * (y4 - y3) - (P2.Y - P1.Y) * (x4 - x3)

        '    If d = 0 Then Return False

        '    Dim r As Double = q / d

        '    q = (P1.Y - y3) * (P2.X - P1.X) - (P1.X - x3) * (P2.Y - P1.Y)
        '    Dim s As Double = q / d

        '    If r < 0 OrElse r > 1 OrElse s < 0 OrElse s > 1 Then Return False

        '    Return True
        'End Function

        'Public Function Intersetcs(ws As WireSegment) As Boolean
        '    Return Intersetcs(ws.P1, ws.P2)
        'End Function

        'Public Function Intersetcs(r As Rectangle) As Boolean
        '    Return Intersetcs(r.X, r.Y, r.Right, r.Y) OrElse
        '           Intersetcs(r.Right, r.Y, r.Right, r.Bottom) OrElse
        '           Intersetcs(r.Right, r.Bottom, r.X, r.Bottom) OrElse
        '           Intersetcs(r.X, r.Bottom, r.X, r.Y)
        'End Function
    End Structure

    Private mCircuit As LogicGates.Component
    Private mGrid(,) As Byte

    Private mGridSize As New Size(2 ^ 9, 2 ^ 9)
    Private mGridResolution As Size

    'Private Structure Line
    '    Public p1 As Point
    '    Public p2 As Point

    '    Public Sub New(p1 As Point, p2 As Point)
    '        Me.p1 = p1
    '        Me.p2 = p2
    '    End Sub
    'End Structure
    'Private wiresCache As New Dictionary(Of LogicGates.Pin, Line)

    Private pf As PathFinder.PathFinderFast

    Public ReadOnly Property WiresSegments As New Dictionary(Of Integer, Point())

    Public Sub New(parent As Control, circuit As LogicGates.Component)
        AddHandler parent.SizeChanged, Sub() SetGridResolution()

        mCircuit = circuit
        DefineGrid()
    End Sub

    Private Sub DefineGrid()
        ReDim mGrid(mGridSize.Width - 1, mGridSize.Height - 1)

        pf = New PathFinder.PathFinderFast(mGrid)
        mGridResolution = New Size(mGrid.GetUpperBound(0) - 1, mGrid.GetUpperBound(1) - 1)

        pf.Diagonals = False
        pf.HeavyDiagonals = True
        pf.PunishChangeDirection = True
        pf.TieBreaker = True
        pf.HeuristicEstimate = 20
        pf.Formula = PathFinder.HeuristicFormula.Manhattan
        pf.SearchLimit = 512 * 512

        SetGridResolution()
    End Sub

    Public Sub SetGridResolution()
        If mCircuit Is Nothing Then Exit Sub
        mGridResolution = New Size(2000 / mGridSize.Width, 2000 / mGridSize.Height)
        UpgardeGrid()
    End Sub

    Public Sub UpgardeGrid()
        If mCircuit Is Nothing OrElse mGridResolution.Width = 0 OrElse mGridResolution.Height = 0 Then Exit Sub

        WiresSegments.Clear()

        For x As Integer = 0 To mGridSize.Width - 1
            For y = 0 To mGridSize.Height - 1
                mGrid(x, y) = 1
            Next
        Next

        Dim b As Rectangle
        For Each gt As LogicGates.BaseGate In mCircuit.Gates
            If gt.GateType = IBaseGate.GateTypes.Component Then Continue For

            b = gt.UI.Bounds
            Select Case gt.GateType
                Case IBaseGate.GateTypes.Node
                    b.Inflate(10, 10)
                Case Else
                    b.Inflate(20, 20)
            End Select
            For x As Integer = b.X / mGridResolution.Width To b.Right / mGridResolution.Width - 1
                If x < 0 OrElse x > mGridSize.Width Then Continue For
                For y As Integer = b.Y / mGridResolution.Height To b.Bottom / mGridResolution.Height - 1
                    If y < 0 OrElse y > mGridSize.Height Then Continue For
                    mGrid(x, y) = 255
                Next
            Next
        Next
    End Sub

    Public Property GridSize As Size
        Get
            Return mGridSize
        End Get
        Set(value As Size)
            mGridSize = value
            DefineGrid()
        End Set
    End Property

    Public ReadOnly Property GridResolution As Size
        Get
            Return mGridResolution
        End Get
    End Property

    Public ReadOnly Property Grid As Byte(,)
        Get
            Return mGrid
        End Get
    End Property

    Public Function DrawANDGate(gt As LogicGates.BaseGate, Optional leftMargin As Integer = 0, Optional rightMargin As Integer = 0) As Drawing2D.GraphicsPath
        Dim r = gt.UI.Bounds

        r.X += leftMargin
        r.Width -= (leftMargin + rightMargin)

        Dim p As New Drawing2D.GraphicsPath()

        Dim kx As Integer = r.X + r.Width * 0.1

        Dim p1 = New Point(kx, r.Y)
        Dim p2 = New Point(r.Width / 2, r.Y)
        Dim p3 = New Point(r.Right - r.Width / 5, r.Y)
        Dim p4 = New Point(r.Right, r.Y + r.Height / 2)

        p.AddLine(r.X, r.Y, kx, r.Y)

        p.AddArc(New Rectangle(kx, r.Y, r.Right - kx, r.Height), -90, 180)

        p.AddLine(kx, r.Bottom, r.X, r.Bottom)

        p.CloseFigure()

        Return p
    End Function

    Public Function DrawORGate(gt As LogicGates.BaseGate, Optional leftMargin As Integer = 0, Optional rightMargin As Integer = 0) As Drawing2D.GraphicsPath
        Dim r = gt.UI.Bounds

        r.X += leftMargin
        r.Width -= (leftMargin + rightMargin)

        Dim p As New Drawing2D.GraphicsPath()

        Dim kx As Integer = r.Width * 0.5

        Dim p1 = New Point(r.X + kx, r.Y)
        Dim p2 = New Point(r.X + r.Width / 2, r.Y)
        Dim p3 = New Point(r.Right - r.Width / 5, r.Y)
        Dim p4 = New Point(r.Right, r.Y + r.Height / 2)

        p.AddLine(r.X, r.Y, r.X + kx, r.Y)

        p.AddBezier(p1, p2, p3, p4)
        p1.Y = r.Bottom
        p2.Y = r.Bottom
        p3.Y = r.Bottom
        p.AddBezier(p4, p3, p2, p1)

        p.AddLine(r.X + kx, r.Bottom, r.X, r.Bottom)

        p.AddBezier(New Point(r.X, r.Bottom),
                    New Point(r.X + r.Width / 10, r.Y + r.Height / 2 + r.Height / 5),
                    New Point(r.X + r.Width / 10, r.Y + r.Height / 2 - r.Height / 5),
                    New Point(r.X, r.Y))

        p.CloseFigure()

        Return p
    End Function

    Public Function DrawXORGate(gt As LogicGates.BaseGate) As Drawing2D.GraphicsPath
        Dim p As Drawing2D.GraphicsPath = DrawORGate(gt, 10)

        Dim r = gt.UI.Bounds

        p.AddBezier(New Point(r.X, r.Bottom),
                    New Point(r.X + r.Width / 10, r.Y + r.Height / 2 + r.Height / 5),
                    New Point(r.X + r.Width / 10, r.Y + r.Height / 2 - r.Height / 5),
                    New Point(r.X, r.Y))

        p.AddBezier(New Point(r.X, r.Y),
                    New Point(r.X + r.Width / 10, r.Y + r.Height / 2 - r.Height / 5),
                    New Point(r.X + r.Width / 10, r.Y + r.Height / 2 + r.Height / 5),
                    New Point(r.X, r.Bottom))

        Return p
    End Function

    Public Function DrawNANDGate(gt As LogicGates.BaseGate) As Drawing2D.GraphicsPath
        Dim r = gt.UI.Bounds

        Dim k As Integer = GetNOTSymbolSize(r)
        Dim p As Drawing2D.GraphicsPath = DrawANDGate(gt, 0, k)

        DrawNOT(p, k, r)

        Return p
    End Function

    Public Function DrawNORGate(gt As LogicGates.BaseGate) As Drawing2D.GraphicsPath
        Dim r = gt.UI.Bounds

        Dim k As Integer = GetNOTSymbolSize(r)

        Dim p As Drawing2D.GraphicsPath = DrawORGate(gt, 0, k)

        DrawNOT(p, k, r)

        Return p
    End Function

    Public Function DrawNOTGate(gt As LogicGates.BaseGate) As Drawing2D.GraphicsPath
        Dim r = gt.UI.Bounds

        Dim p As New Drawing2D.GraphicsPath()

        Dim k As Integer = GetNOTSymbolSize(r)

        p.AddLine(r.X, r.Y, r.Right - k, r.Y + r.Height \ 2)
        p.AddLine(r.Right - k, r.Y + r.Height \ 2, r.X, r.Bottom)
        p.CloseFigure()

        DrawNOT(p, k, r)

        Return p
    End Function

    Public Function DrawNode(gt As LogicGates.BaseGate) As Drawing2D.GraphicsPath
        Dim r = gt.UI.Bounds

        Dim p As New Drawing2D.GraphicsPath()

        p.AddEllipse(r.X, r.Y - 1, r.Width, r.Height)

        Return p
    End Function

    Public Function GetNOTSymbolSize(r As Rectangle) As Integer
        Return Math.Max(r.Width, r.Height) / 8
    End Function

    Public Sub DrawNOT(p As Drawing2D.GraphicsPath, k As Integer, r As Rectangle)
        p.AddEllipse(r.Right - k, r.Y + r.Height \ 2 - k \ 2, k, k)
    End Sub

    Private Function ScreenPointToGridPoint(p As Point) As Point
        Return New Point(p.X / mGridResolution.Width, p.Y / mGridResolution.Height)
    End Function

    Private Function GridNodeToScreenPoint(n As PathFinder.PathFinderNode) As Point
        Return New Point(n.X * mGridResolution.Width, n.Y * mGridResolution.Height)
    End Function

    Private Function GridPointToScreenPoint(p As Point) As Point
        Return New Point(p.X * mGridResolution.Width, p.Y * mGridResolution.Height)
    End Function

    ' http://www.policyalmanac.org/games/aStarTutorial.htm
    ' http://www.codeguru.com/csharp/csharp/cs_misc/designtechniques/article.php/c12527/AStar-A-Implementation-in-C-Path-Finding-PathFinder.htm#page-2
    Public Sub DrawWiresAStar(g As Graphics, r As Rectangle, selPin As LogicGates.Pin, selPinUI As GateUI)
        Dim p1 As Point
        Dim p2 As Point
        Dim n As LogicGates.Node
        Dim recalculate As Boolean = WiresSegments.Count = 0

        If selPin IsNot Nothing AndAlso selPin.ConnectedToPinNumber = -1 Then
            If selPin.ParentGate.GateType = IBaseGate.GateTypes.Node Then
                n = CType(selPin.ParentGate, LogicGates.Node)
                For i As Integer = 0 To n.Outputs.Count - 1
                    If n.OutputsUIs(i) = selPin Then
                        p1 = selPin.ParentGate.UI.Location + selPinUI.Location
                        p1.X += selPinUI.Width
                        p1.Y += selPinUI.Height / 2

                        p2 = selPin.ParentGate.UI.Location + selPin.UI.Location
                        p2.Y += selPin.UI.Height / 2

                        DrawWire(g, p1, p2, n.OutputsUIs(i), recalculate)
                        Exit For
                    End If
                Next
            End If

            'If LogicGates.BaseGate.GetGateConnectedToInput(mCircuit, selPin) Is Nothing Then
            If selPin.ConnectedFromGate Is Nothing Then
                For Each ip In selPin.ParentGate.Inputs
                    If ip = selPin Then
                        p1 = selPin.ParentGate.UI.Location + selPinUI.Location
                        p1.X += selPinUI.Width
                        p1.Y += selPinUI.Height / 2

                        p2 = selPin.ParentGate.UI.Location + selPin.UI.Location
                        p2.Y += selPin.UI.Height / 2

                        DrawWire(g, p1, p2, ip, recalculate)
                        Exit For
                    End If
                Next
            End If
        End If

        For Each gt In mCircuit.Gates
            If gt.GateType = IBaseGate.GateTypes.Node Then
                n = CType(gt, LogicGates.Node)
                For o As Integer = 0 To n.Outputs.Count - 1
                    Dim op = n.Outputs(o)
                    If op Is Nothing Then Continue For
                    Dim opUI = n.OutputsUIs(o)

                    p1 = gt.UI.Location + opUI.UI.Location + New Point(opUI.UI.Width / 2, opUI.UI.Height / 2 - 1)
                    p1 = TransformPoint(p1, gt)

                    p2 = op.Gate.UI.Location + op.Pin.UI.Location
                    p2.Y += op.Pin.UI.Height / 2

                    If selPin Is Nothing OrElse (op.Pin <> selPin) Then p2 = TransformPoint(p2, op.Gate)

                    DrawWire(g, p1, p2, op.Pin, recalculate)
                    DrawWire(g, p1, gt.UI.Location + New Point(gt.UI.Width / 2, gt.UI.Height / 2), op.Pin, recalculate)
                Next
            ElseIf gt.Output.ConnectedToPinNumber <> -1 Then
                Dim p As LogicGates.Pin = If(gt.Output = selPin, selPin, gt.Output)

                p1 = gt.UI.Location + p.UI.Location
                p1.X += p.UI.Width
                p1.Y += p.UI.Height / 2

                If gt.Output <> selPin Then p1 = TransformPoint(p1, gt)

                If p.ConnectedToGate.GateType = IBaseGate.GateTypes.Node Then
                    n = CType(p.ConnectedToGate, LogicGates.Node)
                    p2 = n.UI.Location + New Point(-n.UI.Width / 2, n.UI.Height / 2)
                    DrawWire(g, p2, n.UI.Location + New Point(n.UI.Width / 2, n.UI.Height / 2), p, recalculate)
                Else
                    n = Nothing
                    p2 = p.ConnectedToGate.UI.Location + p.ConnectedToPin.UI.Location
                    p2.Y += p.ConnectedToPin.UI.Height / 2
                End If
                p2 = TransformPoint(p2, p.ConnectedToGate)

                DrawWire(g, p1, p2, p, recalculate)
            ElseIf gt.Flow <> IBaseGate.DataFlow.In AndAlso gt.Output.ConnectedToPinNumber = -1 AndAlso gt.Output = selPin Then
                p1 = gt.UI.Location + selPinUI.Location
                p1.X += selPinUI.Width
                p1.Y += selPinUI.Height / 2
                p1 = TransformPoint(p1, gt)

                p2 = gt.UI.Location + selPin.UI.Location
                p2.Y += selPin.UI.Height / 2

                DrawWire(g, p1, p2, gt.Output, recalculate)
            End If
        Next
    End Sub

    Private Sub DrawWire(g As Graphics, p1 As Point, p2 As Point, pin As LogicGates.Pin, recalculate As Boolean)
        Dim hash As Integer = GetHash(p1, p2)
        If WiresSegments.ContainsKey(hash) Then
            If recalculate Then Exit Sub
        ElseIf Not recalculate Then
            Exit Sub
        End If

        Dim pts() As Point = If(recalculate, FindAndSimplifyPath(ScreenPointToGridPoint(p1), ScreenPointToGridPoint(p2)), WiresSegments(hash))
        Using c As New Pen(GetWireColor(pin), 2)
            If pts Is Nothing Then
                g.DrawLine(c, p1, p2)
                If recalculate Then WiresSegments.Add(hash, {p1, p2})
            Else
                g.DrawLines(c, pts)
                If recalculate Then WiresSegments.Add(hash, pts)
            End If
        End Using
    End Sub

    Private Function GetHash(p1 As Point, p2 As Point) As Integer
        Return p1.X * mGridSize.Width + p1.Y + (p1.X * mGridSize.Width + p1.Y) * mGridSize.Height
    End Function

    Public Function GetWireColor(pin As LogicGates.Pin) As Color
        Return If(pin.Value, Color.PaleVioletRed, Color.LightBlue)
    End Function

    Private Function FindAndSimplifyPath(p1 As Point, p2 As Point) As Point()
        Dim nodes As List(Of PathFinder.PathFinderNode) = pf.FindPath(p1, p2)

        If nodes Is Nothing Then
            Return Nothing
        Else
            p1 = nodes(0).ToPoint()
            Dim pts As New List(Of Point) From {GridPointToScreenPoint(p1)}

            For i As Integer = 1 To nodes.Count - 1
                p2 = nodes(i).ToPoint()
                '' Fast Mode
                If p1.X <> p2.X AndAlso p1.Y <> p2.Y Then
                    pts.Add(GridNodeToScreenPoint(nodes(i - 1)))
                    AddWireToGrid(p1, p2)
                    p1 = p2
                End If

                'pts.Add(GridNodeToScreenPoint(nodes(i - 1)))
                'AddWireToGrid(p1, p2)
                'p1 = p2
            Next
            pts.Add(GridNodeToScreenPoint(nodes.Last()))
            AddWireToGrid(p1, nodes.Last().ToPoint())

            Return pts.ToArray()
        End If
    End Function

    Private ReadOnly wm As Integer = 0
    Private Sub AddWireToGrid(p1 As Point, p2 As Point)
        For x = Math.Min(p1.X, p2.X) - 1 To Math.Max(p1.X, p2.X)
            For y = Math.Min(p1.Y, p2.Y) - 1 To Math.Max(p1.Y, p2.Y)
                For x1 = x - wm To x + wm
                    If x1 < 0 OrElse x1 >= mGridSize.Width Then Continue For
                    For y1 = y - wm To y + wm
                        If y1 < 0 OrElse y1 >= mGridSize.Height Then Continue For
                        mGrid(x1, y1) = 128
                    Next
                Next
            Next
        Next
    End Sub

    Private Function SurroundRectangle(ws As WireSegment, r As Rectangle) As List(Of WireSegment)
        Return New List(Of WireSegment) From {ws}
    End Function

    Public Sub DrawLed(g As Graphics, gt As LogicGates.BaseGate)
        g.FillEllipse(If(gt.Inputs(0).Value, gt.UI.ActiveColorBrush, gt.UI.FillColorBrush), gt.UI.Bounds)
        g.DrawEllipse(gt.UI.BorderColorPen, gt.UI.Bounds)
    End Sub

    Public Sub DrawSwitch(g As Graphics, gt As LogicGates.BaseGate)
        g.FillRectangle(If(gt.Inputs(0).Value, gt.UI.ActiveColorBrush, gt.UI.FillColorBrush), gt.UI.Bounds)
        g.DrawRectangle(gt.UI.BorderColorPen, gt.UI.Bounds)
    End Sub

    Public Sub DrawClock(g As Graphics, gt As LogicGates.BaseGate)
        g.FillRectangle(gt.UI.FillColorBrush, gt.UI.Bounds)
        g.DrawRectangle(gt.UI.BorderColorPen, gt.UI.Bounds)

        g.FillEllipse(If(gt.Output.Value, gt.UI.ActiveColorBrush, gt.UI.ForeColorBrush), gt.UI.Bounds.Right - 20, gt.UI.Y + 6, 12, 12)

        ' FIXME: This needs to be cached inside a DirectBitmap
        Dim gc As LogicGates.Clock = CType(gt, LogicGates.Clock)
        Dim l As Integer = gt.UI.Bounds.Width / 6
        Dim lx As Integer = gt.UI.X + l
        Dim h As Integer = gt.UI.Bounds.Height / 2
        Dim x As Integer
        Dim y As Integer = gt.UI.Bounds.Bottom - h
        Dim o As Integer = 0
        h -= 12
        Using p As New Pen(Color.Black, 2)
            For x = lx To gt.UI.Bounds.Right - l - 1 Step 2
                g.DrawLine(p, x, y + o, x + 1, y + o)

                If x - lx >= If(o = 0, 1 - gc.DutyCycle, gc.DutyCycle) * h Then
                    If o = 0 Then
                        g.DrawLine(p, lx, y, lx, y + h)
                    Else
                        g.DrawLine(p, lx, y + h, lx, y)
                    End If
                    lx = x
                    o = If(o = 0, h, 0)
                End If
            Next
        End Using
    End Sub

    Public Sub ApplyRotation(g As Graphics, ui As GateUI)
        If ui.Angle <> 0 Then
            Using m As Drawing2D.Matrix = New Drawing2D.Matrix()
                m.RotateAt(ui.Angle, New PointF(ui.X + ui.Width / 2, ui.Y + ui.Height / 2))
                g.Transform = m
            End Using
        End If
    End Sub

    Public Function TransformPoint(p As Point, Optional gt As LogicGates.BaseGate = Nothing) As Point
        If gt IsNot Nothing AndAlso gt.UI.Angle <> 0 Then
            Dim pts() As Point = New Point() {p}
            Using m As Drawing2D.Matrix = New Drawing2D.Matrix()
                m.RotateAt(gt.UI.Angle, New PointF(gt.UI.X + gt.UI.Width / 2, gt.UI.Y + gt.UI.Height / 2))
                'm.Invert()
                m.TransformPoints(pts)
            End Using
            Return pts(0)
        Else
            Return p
        End If
    End Function
End Class
