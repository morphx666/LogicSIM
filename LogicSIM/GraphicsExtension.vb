Imports System.Drawing.Drawing2D
Imports System.Runtime.InteropServices
Imports System.Runtime.CompilerServices

Public Module GraphicsExtension
    Private Const Rad2Deg As Double = 180 / Math.PI
    Private Const Deg2Rad As Double = 1 / Rad2Deg

    <Extension()>
    Public Function GenerateRoundedRectangle(graphics As Graphics, rectangle As RectangleF, radius As Single, filter As RectangleEdgeFilter) As GraphicsPath
        Dim diameter As Single
        Dim path As New GraphicsPath()
        If radius <= 0.0F OrElse filter = RectangleEdgeFilter.None Then
            path.AddRectangle(rectangle)
            path.CloseFigure()
            Return path
        Else
            If radius >= (Math.Min(rectangle.Width, rectangle.Height)) / 2.0 Then
                Return graphics.GenerateCapsule(rectangle)
            End If
            diameter = radius * 2.0F
            Dim sizeF As New SizeF(diameter, diameter)
            Dim arc As New RectangleF(rectangle.Location, sizeF)
            If (RectangleEdgeFilter.TopLeft And filter) = RectangleEdgeFilter.TopLeft Then
                path.AddArc(arc, 180, 90)
            Else
                path.AddLine(arc.X, arc.Y + arc.Height, arc.X, arc.Y)
                path.AddLine(arc.X, arc.Y, arc.X + arc.Width, arc.Y)
            End If
            arc.X = rectangle.Right - diameter
            If (RectangleEdgeFilter.TopRight And filter) = RectangleEdgeFilter.TopRight Then
                path.AddArc(arc, 270, 90)
            Else
                path.AddLine(arc.X, arc.Y, arc.X + arc.Width, arc.Y)
                path.AddLine(arc.X + arc.Width, arc.Y + arc.Height, arc.X + arc.Width, arc.Y)
            End If
            arc.Y = rectangle.Bottom - diameter
            If (RectangleEdgeFilter.BottomRight And filter) = RectangleEdgeFilter.BottomRight Then
                path.AddArc(arc, 0, 90)
            Else
                path.AddLine(arc.X + arc.Width, arc.Y, arc.X + arc.Width, arc.Y + arc.Height)
                path.AddLine(arc.X, arc.Y + arc.Height, arc.X + arc.Width, arc.Y + arc.Height)
            End If
            arc.X = rectangle.Left
            If (RectangleEdgeFilter.BottomLeft And filter) = RectangleEdgeFilter.BottomLeft Then
                path.AddArc(arc, 90, 90)
            Else
                path.AddLine(arc.X + arc.Width, arc.Y + arc.Height, arc.X, arc.Y + arc.Height)
                path.AddLine(arc.X, arc.Y + arc.Height, arc.X, arc.Y)
            End If
            path.CloseFigure()
        End If
        Return path
    End Function

    <Extension()>
    Public Function GenerateCapsule(graphics As Graphics, rectangle As RectangleF) As GraphicsPath
        Dim diameter As Single
        Dim arc As RectangleF
        Dim path As New GraphicsPath()

        If rectangle.Width < 0 OrElse rectangle.Height < 0 Then
            path.AddEllipse(rectangle)
            path.CloseFigure()
            Return path
        End If

        Try
            If rectangle.Width > rectangle.Height Then
                diameter = rectangle.Height
                Dim sizeF As New SizeF(diameter, diameter)
                arc = New RectangleF(rectangle.Location, sizeF)
                path.AddArc(arc, 90, 180)
                arc.X = rectangle.Right - diameter
                path.AddArc(arc, 270, 180)
            ElseIf rectangle.Width < rectangle.Height Then
                diameter = rectangle.Width
                Dim sizeF As New SizeF(diameter, diameter)
                arc = New RectangleF(rectangle.Location, sizeF)
                path.AddArc(arc, 180, 180)
                arc.Y = rectangle.Bottom - diameter
                path.AddArc(arc, 0, 180)
            Else
                path.AddEllipse(rectangle)
            End If
        Catch
            path.AddEllipse(rectangle)
        Finally
            path.CloseFigure()
        End Try

        Return path
    End Function

    <Extension()>
    Public Sub DrawRoundedRectangle(graphics As Graphics, pen As Pen, x As Single, y As Single, width As Single, height As Single, radius As Single, filter As RectangleEdgeFilter)
        Dim rectangle As New RectangleF(x, y, width, height)
        Using path As GraphicsPath = graphics.GenerateRoundedRectangle(rectangle, radius, filter)
            Dim old As SmoothingMode = graphics.SmoothingMode
            graphics.SmoothingMode = SmoothingMode.AntiAlias
            graphics.DrawPath(pen, path)
            graphics.SmoothingMode = old
        End Using
    End Sub

    <Extension()>
    Public Sub DrawRoundedRectangle(graphics As Graphics, pen As Pen, x As Single, y As Single, width As Single, height As Single, radius As Single)
        graphics.DrawRoundedRectangle(pen, x, y, width, height, radius, RectangleEdgeFilter.All)
    End Sub

    <Extension()>
    Public Sub DrawRoundedRectangle(graphics As Graphics, pen As Pen, x As Integer, y As Integer, width As Integer, height As Integer, radius As Integer)
        graphics.DrawRoundedRectangle(pen, Convert.ToSingle(x), Convert.ToSingle(y), Convert.ToSingle(width), Convert.ToSingle(height), Convert.ToSingle(radius))
    End Sub

    <Extension()>
    Public Sub DrawRoundedRectangle(graphics As Graphics, pen As Pen, rectangle As Rectangle, radius As Integer, filter As RectangleEdgeFilter)
        graphics.DrawRoundedRectangle(pen, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, radius, filter)
    End Sub

    <Extension()>
    Public Sub DrawRoundedRectangle(graphics As Graphics, pen As Pen, rectangle As Rectangle, radius As Integer)
        graphics.DrawRoundedRectangle(pen, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, radius, RectangleEdgeFilter.All)
    End Sub

    <Extension()>
    Public Sub DrawRoundedRectangle(graphics As Graphics, pen As Pen, rectangle As RectangleF, radius As Integer, filter As RectangleEdgeFilter)
        graphics.DrawRoundedRectangle(pen, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, radius, filter)
    End Sub

    <Extension()>
    Public Sub DrawRoundedRectangle(graphics As Graphics, pen As Pen, rectangle As RectangleF, radius As Integer)
        graphics.DrawRoundedRectangle(pen, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, radius, RectangleEdgeFilter.All)
    End Sub

    <Extension()>
    Public Sub FillRoundedRectangle(graphics As Graphics, brush As Brush, x As Single, y As Single, width As Single, height As Single, radius As Single, filter As RectangleEdgeFilter)
        Dim rectangle As New RectangleF(x, y, width, height)
        Using path As GraphicsPath = graphics.GenerateRoundedRectangle(rectangle, radius, filter)
            Dim old As SmoothingMode = graphics.SmoothingMode
            graphics.SmoothingMode = SmoothingMode.AntiAlias
            graphics.FillPath(brush, path)
            graphics.SmoothingMode = old
        End Using
    End Sub

    <Extension()>
    Public Sub FillRoundedRectangle(graphics As Graphics, brush As Brush, x As Single, y As Single, width As Single, height As Single, radius As Single)
        graphics.FillRoundedRectangle(brush, x, y, width, height, radius, RectangleEdgeFilter.All)
    End Sub

    <Extension()>
    Public Sub FillRoundedRectangle(graphics As Graphics, brush As Brush, x As Integer, y As Integer, width As Integer, height As Integer, radius As Integer)
        graphics.FillRoundedRectangle(brush, Convert.ToSingle(x), Convert.ToSingle(y), Convert.ToSingle(width), Convert.ToSingle(height), Convert.ToSingle(radius))
    End Sub

    <Extension()>
    Public Sub FillRoundedRectangle(graphics As Graphics, brush As Brush, rectangle As Rectangle, radius As Integer, filter As RectangleEdgeFilter)
        graphics.FillRoundedRectangle(brush, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, radius, filter)
    End Sub

    <Extension()>
    Public Sub FillRoundedRectangle(graphics As Graphics, brush As Brush, rectangle As Rectangle, radius As Integer)
        graphics.FillRoundedRectangle(brush, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, radius, RectangleEdgeFilter.All)
    End Sub

    <Extension()>
    Public Sub FillRoundedRectangle(graphics As Graphics, brush As Brush, rectangle As RectangleF, radius As Integer, filter As RectangleEdgeFilter)
        graphics.FillRoundedRectangle(brush, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, radius, filter)
    End Sub

    <Extension()>
    Public Sub FillRoundedRectangle(graphics As Graphics, brush As Brush, rectangle As RectangleF, radius As Integer)
        graphics.FillRoundedRectangle(brush, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, radius, RectangleEdgeFilter.All)
    End Sub

    <Extension()>
    Public Sub DrawEllipseFromCenter(g As Graphics, pen As Pen, x As Single, y As Single, width As Single, height As Single)
        g.DrawEllipse(pen, x - width / 2, y - height / 2, width, height)
    End Sub

    <Extension()>
    Public Sub DrawEllipseFromCenter(g As Graphics, pen As Pen, x As Integer, y As Integer, width As Integer, height As Integer)
        g.DrawEllipse(pen, CInt(x - width / 2), CInt(y - height / 2), width, height)
    End Sub

    <Extension()>
    Public Sub DrawEllipseFromCenter(g As Graphics, pen As Pen, rect As Rectangle)
        DrawEllipseFromCenter(g, pen, rect.X, rect.Y, rect.Width, rect.Height)
    End Sub

    <Extension()>
    Public Sub DrawEllipseFromCenter(g As Graphics, pen As Pen, rect As RectangleF)
        DrawEllipseFromCenter(g, pen, rect.X, rect.Y, rect.Width, rect.Height)
    End Sub

    <Extension()>
    Public Sub FillEllipseFromCenter(g As Graphics, brush As Brush, x As Single, y As Single, width As Single, height As Single)
        g.FillEllipse(brush, CInt(x - width / 2), CInt(y - height / 2), width, height)
    End Sub

    <Extension()>
    Public Sub FillEllipseFromCenter(g As Graphics, brush As Brush, x As Integer, y As Integer, width As Integer, height As Integer)
        g.FillEllipse(brush, x - width \ 2, y - height \ 2, width, height)
    End Sub

    <Extension()>
    Public Sub FillEllipseFromCenter(g As Graphics, brush As Brush, rect As Rectangle)
        FillEllipseFromCenter(g, brush, rect.X, rect.Y, rect.Width, rect.Height)
    End Sub

    <Extension()>
    Public Sub FillEllipseFromCenter(g As Graphics, brush As Brush, rect As RectangleF)
        FillEllipseFromCenter(g, brush, rect.X, rect.Y, rect.Width, rect.Height)
    End Sub

    <Extension()>
    Public Sub DrawEllipseFromCenter(g As Graphics, brush As Brush, rect As RectangleF)
        FillEllipseFromCenter(g, brush, rect.X, rect.Y, rect.Width, rect.Height)
    End Sub

    <Extension()>
    Public Sub FillDonut(g As Graphics, backColor As Brush, donutColor As Brush, highlightColor As Brush, ByVal rect As Rectangle, donutSize As Integer, startAngle As Single, sweepAngle As Single, Optional drawEndCaps As Boolean = False)
        g.FillEllipse(donutColor, rect)
        If sweepAngle > 0 Then g.FillPie(highlightColor, rect, startAngle, sweepAngle)

        If drawEndCaps AndAlso sweepAngle > 0 Then
            Dim r As RectangleF
            r.X = CSng((rect.X + rect.Width / 2) + (rect.Width - donutSize) / 2 * Math.Cos(-startAngle * Deg2Rad))
            r.Y = CSng((rect.Y + rect.Height / 2) + (rect.Height - donutSize) / 2 * Math.Sin(startAngle * Deg2Rad))
            r.Width = donutSize
            r.Height = donutSize
            g.FillEllipseFromCenter(highlightColor, r)

            r.X = CSng((rect.X + rect.Width / 2) + (rect.Width - donutSize) / 2 * Math.Cos(-(startAngle + sweepAngle) * Deg2Rad))
            r.Y = CSng((rect.Y + rect.Height / 2) + (rect.Height - donutSize) / 2 * Math.Sin((startAngle + sweepAngle) * Deg2Rad))
            g.FillEllipseFromCenter(highlightColor, r)
        End If

        rect.Inflate(-donutSize, -donutSize)
        g.FillEllipse(backColor, rect)
    End Sub

    <Extension()>
    Public Sub DrawCurvedText(g As Graphics, text As String, centre As Point, distanceFromCentreToBaseOfText As Single, radiansToTextCentre As Single, font As Font, brush As Brush, Optional clockwise As Boolean = True)
        ' http://stackoverflow.com/a/11151457/518872

        ' Circumference for use later
        Dim circleCircumference As Double = (Math.PI * 2 * distanceFromCentreToBaseOfText)

        ' Get the width of each character
        Dim characterWidths = GetCharacterWidths(g, text, font).ToArray()

        ' The overall height of the string
        Dim characterHeight = g.MeasureString(text, font).Height

        Dim textLength = characterWidths.Sum()

        ' The string length above Is the arc length we'll use for rendering the string. Work out the starting angle required to 
        ' center the text across the radiansToTextCentre.
        Dim fractionOfCircumference As Double = textLength / circleCircumference

        Dim currentCharacterRadians As Double = radiansToTextCentre + If(clockwise, -1, 1) * (Math.PI * fractionOfCircumference)

        For characterIndex = 0 To text.Length - 1
            Dim c As Char = text(characterIndex)

            ' Polar to Cartesian
            Dim x As Double = (distanceFromCentreToBaseOfText * Math.Sin(currentCharacterRadians))
            Dim y As Double = -(distanceFromCentreToBaseOfText * Math.Cos(currentCharacterRadians))

            Using characterPath As GraphicsPath = New GraphicsPath()
                characterPath.AddString(c.ToString(), font.FontFamily, font.Style, font.Size, Point.Empty,
                                    StringFormat.GenericTypographic)

                Dim pathBounds = characterPath.GetBounds()

                ' Transformation matrix to move the character to the correct location. 
                ' Note that all actions on the Matrix class are prepended, so we apply them in reverse.
                Dim transform = New Matrix()

                ' Translate to the final position
                transform.Translate(CSng(centre.X + x), CSng(centre.Y + y))

                ' Rotate the character
                Dim rotationAngleDegrees As Single = CSng(currentCharacterRadians * 180.0F / Math.PI - If(clockwise, 0, 180.0F))
                transform.Rotate(rotationAngleDegrees)

                ' Translate the character so the center of its base Is over the origin
                transform.Translate(-pathBounds.Width / 2.0F, -characterHeight)

                characterPath.Transform(transform)

                ' Draw the character
                g.FillPath(brush, characterPath)
            End Using

            If characterIndex <> text.Length - 1 Then
                ' Move "currentCharacterRadians" on to the next character
                Dim distanceToNextChar = (characterWidths(characterIndex) + characterWidths(characterIndex + 1)) / 2.0F
                Dim charFractionOfCircumference As Double = distanceToNextChar / circleCircumference
                If clockwise Then
                    currentCharacterRadians += charFractionOfCircumference * (2.0F * Math.PI)
                Else
                    currentCharacterRadians -= charFractionOfCircumference * (2.0F * Math.PI)
                End If
            End If
        Next
    End Sub

    <Extension()>
    Public Sub Resize(ByRef bmp As Bitmap, newSize As Size, Optional mantainAspectRatio As Boolean = False)
        Dim srcRect As New Rectangle(Point.Empty, New Size(bmp.Width, bmp.Height))
        Dim trgRect As Rectangle

        If mantainAspectRatio Then
            Dim ar As Double = srcRect.Width / srcRect.Height
            trgRect = New Rectangle(Point.Empty, New Size(newSize.Width, CInt(newSize.Height / ar)))
        Else
            trgRect = New Rectangle(Point.Empty, newSize)
        End If

        Dim newBmp As New Bitmap(trgRect.Width, trgRect.Height)

        Using g As Graphics = Graphics.FromImage(newBmp)
            g.InterpolationMode = InterpolationMode.High
            g.CompositingQuality = CompositingQuality.HighQuality
            g.SmoothingMode = SmoothingMode.AntiAlias

            g.DrawImage(bmp, trgRect, srcRect, GraphicsUnit.Pixel)
        End Using

        bmp.Dispose()
        bmp = newBmp
    End Sub

    <Extension()>
    Public Sub Resize(ByRef bmp As Bitmap, width As Integer, height As Integer, Optional mantainAspectRatio As Boolean = False)
        bmp.Resize(New Size(width, height), mantainAspectRatio)
    End Sub

    Private Function GetCharacterWidths(g As Graphics, text As String, font As Font) As IEnumerable(Of Single)
        ' The length of a space. Necessary because a space measured using StringFormat.GenericTypographic has no width.
        ' We can't use StringFormat.GenericDefault for the characters themselves, as it adds unwanted spacing.
        Dim spaceLength = g.MeasureString(" ", font, Point.Empty, StringFormat.GenericDefault).Width

        Return text.Select(Function(c) If(c = " ", spaceLength, g.MeasureString(c.ToString(), font, Point.Empty, StringFormat.GenericTypographic).Width))
    End Function

    Private Enum TernaryRasterOperations As UInteger
        ''' <summary>dest = source</summary>
        SRCCOPY = &HCC0020
        ''' <summary>dest = source OR dest</summary>
        SRCPAINT = &HEE0086
        ''' <summary>dest = source AND dest</summary>
        SRCAND = &H8800C6
        ''' <summary>dest = source XOR dest</summary>
        SRCINVERT = &H660046
        ''' <summary>dest = source AND (NOT dest)</summary>
        SRCERASE = &H440328
        ''' <summary>dest = (NOT source)</summary>
        NOTSRCCOPY = &H330008
        ''' <summary>dest = (NOT src) AND (NOT dest)</summary>
        NOTSRCERASE = &H1100A6
        ''' <summary>dest = (source AND pattern)</summary>
        MERGECOPY = &HC000CA
        ''' <summary>dest = (NOT source) OR dest</summary>
        MERGEPAINT = &HBB0226
        ''' <summary>dest = pattern</summary>
        PATCOPY = &HF00021
        ''' <summary>dest = DPSnoo</summary>
        PATPAINT = &HFB0A09
        ''' <summary>dest = pattern XOR dest</summary>
        PATINVERT = &H5A0049
        ''' <summary>dest = (NOT dest)</summary>
        DSTINVERT = &H550009
        ''' <summary>dest = BLACK</summary>
        BLACKNESS = &H42
        ''' <summary>dest = WHITE</summary>
        WHITENESS = &HFF0062
        ''' <summary>
        ''' Capture window as seen on screen.  This includes layered windows 
        ''' such as WPF windows with AllowsTransparency="true"
        ''' </summary>
        CAPTUREBLT = &H40000000
    End Enum

    <DllImport("gdi32.dll")>
    Private Function BitBlt(hdc As IntPtr, nXDest As Integer, nYDest As Integer, nWidth As Integer, nHeight As Integer, hdcSrc As IntPtr, nXSrc As Integer, nYSrc As Integer, dwRop As TernaryRasterOperations) As Boolean
    End Function

    <DllImport("Gdi32.dll")>
    Private Function SelectObject(hdc As IntPtr, hObject As IntPtr) As IntPtr
    End Function

    <DllImport("gdi32.dll", SetLastError:=True)>
    Private Function CreateCompatibleDC(hRefDC As IntPtr) As IntPtr
    End Function

    Private Declare Function DeleteDC Lib "gdi32.dll" (hdc As IntPtr) As Boolean
    Private Declare Function StretchBlt Lib "gdi32.dll" (hdcDest As IntPtr, nXOriginDest As Integer, nYOriginDest As Integer, nWidthDest As Integer, nHeightDest As Integer, hdcSrc As IntPtr, nXOriginSrc As Integer, nYOriginSrc As Integer, nWidthSrc As Integer, nHeightSrc As Integer, dwRop As TernaryRasterOperations) As Boolean
    Private Declare Function DeleteObject Lib "gdi32.dll" (hObject As IntPtr) As Boolean

    <Extension()>
    Public Sub DrawImageFast(g As Graphics, image As Bitmap, dstRect As Rectangle, srcRect As Rectangle)
        Dim srcGraphics As Graphics = Graphics.FromImage(image)
        Dim srcHDC As IntPtr = srcGraphics.GetHdc
        Dim pSource As IntPtr = CreateCompatibleDC(srcHDC)
        Dim dstHDC As IntPtr = g.GetHdc
        Dim srcHbitmap As IntPtr = image.GetHbitmap()

        SelectObject(pSource, srcHbitmap)

        If dstRect.Size = srcRect.Size Then
            BitBlt(dstHDC, dstRect.X, dstRect.Y, dstRect.Width, dstRect.Height, pSource, srcRect.X, srcRect.Y, TernaryRasterOperations.SRCCOPY)
        Else
            StretchBlt(dstHDC, dstRect.X, dstRect.Y, dstRect.Width, dstRect.Height, pSource, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, TernaryRasterOperations.SRCCOPY)
        End If

        DeleteObject(srcHbitmap)
        DeleteDC(pSource)
        g.ReleaseHdc(dstHDC)
        srcGraphics.ReleaseHdc(srcHDC)
        srcGraphics.Dispose()
    End Sub

    <Extension()>
    Public Sub DrawImageFast(g As Graphics, image As Bitmap, destRect As Rectangle, x As Integer, y As Integer, width As Integer, height As Integer)
        g.DrawImageFast(image, destRect, New Rectangle(Point.Empty, image.Size))
    End Sub

    <Extension()>
    Public Sub DrawImageFast(g As Graphics, image As Bitmap, destRect As Rectangle)
        g.DrawImageFast(image, destRect, New Rectangle(Point.Empty, image.Size))
    End Sub

    <Extension()>
    Public Sub DrawImageFast(g As Graphics, image As Bitmap, destPoint As Point)
        g.DrawImageFast(image, New Rectangle(destPoint, image.Size), New Rectangle(Point.Empty, image.Size))
    End Sub

    <Extension()>
    Public Function GetFontMetrics(graphics As Graphics, font As Font) As FontMetrics
        Return FontMetricsImpl.GetFontMetrics(graphics, font)
    End Function

    <Extension()>
    Public Function GetCenter(r As Rectangle) As Point
        Return New Point(r.Left + r.Width / 2, r.Top + r.Height / 2)
    End Function

    Private Class FontMetricsImpl
        Inherits FontMetrics
        <StructLayout(LayoutKind.Sequential)>
        Public Structure TEXTMETRIC
            Public tmHeight As Integer
            Public tmAscent As Integer
            Public tmDescent As Integer
            Public tmInternalLeading As Integer
            Public tmExternalLeading As Integer
            Public tmAveCharWidth As Integer
            Public tmMaxCharWidth As Integer
            Public tmWeight As Integer
            Public tmOverhang As Integer
            Public tmDigitizedAspectX As Integer
            Public tmDigitizedAspectY As Integer
            Public tmFirstChar As Char
            Public tmLastChar As Char
            Public tmDefaultChar As Char
            Public tmBreakChar As Char
            Public tmItalic As Byte
            Public tmUnderlined As Byte
            Public tmStruckOut As Byte
            Public tmPitchAndFamily As Byte
            Public tmCharSet As Byte
        End Structure
        <DllImport("gdi32.dll", CharSet:=CharSet.Unicode)>
        Public Shared Function SelectObject(hdc As IntPtr, hgdiobj As IntPtr) As IntPtr
        End Function
        <DllImport("gdi32.dll", CharSet:=CharSet.Unicode)>
        Public Shared Function GetTextMetrics(hdc As IntPtr, lptm As TEXTMETRIC) As Boolean
        End Function
        <DllImport("gdi32.dll", CharSet:=CharSet.Unicode)>
        Public Shared Function DeleteObject(hdc As IntPtr) As Boolean
        End Function
        Private Function GenerateTextMetrics(graphics As Graphics, font As Font) As TEXTMETRIC
            Dim hDC As IntPtr = IntPtr.Zero
            Dim textMetric As TEXTMETRIC
            Dim hFont As IntPtr = IntPtr.Zero
            Try
                hDC = graphics.GetHdc()
                hFont = font.ToHfont()
                Dim hFontDefault As IntPtr = SelectObject(hDC, hFont)
                Dim result As Boolean = GetTextMetrics(hDC, textMetric)
                SelectObject(hDC, hFontDefault)
            Finally
                If hFont <> IntPtr.Zero Then
                    DeleteObject(hFont)
                End If
                If hDC <> IntPtr.Zero Then
                    graphics.ReleaseHdc(hDC)
                End If
            End Try
            Return textMetric
        End Function
        Private metrics As TEXTMETRIC
        Public Overrides ReadOnly Property Height() As Integer
            Get
                Return Me.metrics.tmHeight
            End Get
        End Property
        Public Overrides ReadOnly Property Ascent() As Integer
            Get
                Return Me.metrics.tmAscent
            End Get
        End Property
        Public Overrides ReadOnly Property Descent() As Integer
            Get
                Return Me.metrics.tmDescent
            End Get
        End Property
        Public Overrides ReadOnly Property InternalLeading() As Integer
            Get
                Return Me.metrics.tmInternalLeading
            End Get
        End Property
        Public Overrides ReadOnly Property ExternalLeading() As Integer
            Get
                Return Me.metrics.tmExternalLeading
            End Get
        End Property
        Public Overrides ReadOnly Property AverageCharacterWidth() As Integer
            Get
                Return Me.metrics.tmAveCharWidth
            End Get
        End Property
        Public Overrides ReadOnly Property MaximumCharacterWidth() As Integer
            Get
                Return Me.metrics.tmMaxCharWidth
            End Get
        End Property
        Public Overrides ReadOnly Property Weight() As Integer
            Get
                Return Me.metrics.tmWeight
            End Get
        End Property
        Public Overrides ReadOnly Property Overhang() As Integer
            Get
                Return Me.metrics.tmOverhang
            End Get
        End Property
        Public Overrides ReadOnly Property DigitizedAspectX() As Integer
            Get
                Return Me.metrics.tmDigitizedAspectX
            End Get
        End Property
        Public Overrides ReadOnly Property DigitizedAspectY() As Integer
            Get
                Return Me.metrics.tmDigitizedAspectY
            End Get
        End Property
        Private Sub New(graphics As Graphics, font As Font)
            Me.metrics = Me.GenerateTextMetrics(graphics, font)
        End Sub
        Public Shared Function GetFontMetrics(graphics As Graphics, font As Font) As FontMetrics
            Return New FontMetricsImpl(graphics, font)
        End Function
    End Class
End Module

Public Enum RectangleEdgeFilter
    None = 0
    TopLeft = 1
    TopRight = 2
    BottomLeft = 4
    BottomRight = 8
    All = TopLeft Or TopRight Or BottomLeft Or BottomRight
End Enum

Public MustInherit Class FontMetrics
    Public Overridable ReadOnly Property Height() As Integer
        Get
            Return 0
        End Get
    End Property
    Public Overridable ReadOnly Property Ascent() As Integer
        Get
            Return 0
        End Get
    End Property
    Public Overridable ReadOnly Property Descent() As Integer
        Get
            Return 0
        End Get
    End Property
    Public Overridable ReadOnly Property InternalLeading() As Integer
        Get
            Return 0
        End Get
    End Property
    Public Overridable ReadOnly Property ExternalLeading() As Integer
        Get
            Return 0
        End Get
    End Property
    Public Overridable ReadOnly Property AverageCharacterWidth() As Integer
        Get
            Return 0
        End Get
    End Property
    Public Overridable ReadOnly Property MaximumCharacterWidth() As Integer
        Get
            Return 0
        End Get
    End Property
    Public Overridable ReadOnly Property Weight() As Integer
        Get
            Return 0
        End Get
    End Property
    Public Overridable ReadOnly Property Overhang() As Integer
        Get
            Return 0
        End Get
    End Property
    Public Overridable ReadOnly Property DigitizedAspectX() As Integer
        Get
            Return 0
        End Get
    End Property
    Public Overridable ReadOnly Property DigitizedAspectY() As Integer
        Get
            Return 0
        End Get
    End Property
End Class