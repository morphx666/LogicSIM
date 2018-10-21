Public Class GateUI
    Implements ICloneable

    Private mLocation As Point
    Private mSize As Size
    Private mPath As Drawing2D.GraphicsPath
    Private mBorderColor As Pen
    Private mFillColor As SolidBrush
    Private mActiveColor As SolidBrush
    Private mForeColor As SolidBrush
    Private mFont As Font
    Private mNameOffset As Point
    Private mAngle As Double
    'Private mWireLineSegments As Point()

    Public Sub New()
        mLocation = New Point(0, 0)
        mSize = New Size(100, 80)

        mBorderColor = New Pen(Brushes.White, 2)
        mFillColor = Brushes.DeepSkyBlue
        mActiveColor = Brushes.MediumPurple
        mForeColor = Brushes.Yellow
        mFont = New Font("Consolas", 12, FontStyle.Regular)
    End Sub

    Public Property Path As Drawing2D.GraphicsPath
        Get
            Return mPath
        End Get
        Set(value As Drawing2D.GraphicsPath)
            mPath = value
        End Set
    End Property

    Public Property Location As Point
        Get
            Return mLocation
        End Get
        Set(value As Point)
            mLocation = value
        End Set
    End Property

    Public Property X As Integer
        Get
            Return mLocation.X
        End Get
        Set(value As Integer)
            mLocation.X = value
        End Set
    End Property

    Public Property Y As Integer
        Get
            Return mLocation.Y
        End Get
        Set(value As Integer)
            mLocation.Y = value
        End Set
    End Property

    Public Property Size As Size
        Get
            Return mSize
        End Get
        Set(value As Size)
            mSize = value
        End Set
    End Property

    Public Property Width As Integer
        Get
            Return mSize.Width
        End Get
        Set(value As Integer)
            mSize.Width = value
        End Set
    End Property

    Public Property Height As Integer
        Get
            Return mSize.Height
        End Get
        Set(value As Integer)
            mSize.Height = value
        End Set
    End Property

    Public Property Bounds As Rectangle
        Get
            Return New Rectangle(mLocation, mSize)
        End Get
        Set(value As Rectangle)
            mLocation = value.Location
            mSize = value.Size
        End Set
    End Property

    Public Property BorderColor As Pen
        Get
            Return mBorderColor
        End Get
        Set(value As Pen)
            mBorderColor = value
        End Set
    End Property

    Public Property FillColor As SolidBrush
        Get
            Return mFillColor
        End Get
        Set(value As SolidBrush)
            mFillColor = value
        End Set
    End Property

    Public Property ActiveColor As SolidBrush
        Get
            Return mActiveColor
        End Get
        Set(value As SolidBrush)
            mActiveColor = value
        End Set
    End Property

    Public Property ForeColor As Brush
        Get
            Return mForeColor
        End Get
        Set(value As Brush)
            mForeColor = value
        End Set
    End Property

    Public Property Font As Font
        Get
            Return mFont
        End Get
        Set(value As Font)
            mFont = value
        End Set
    End Property

    Public Property NameOffset As Point
        Get
            Return mNameOffset
        End Get
        Set(value As Point)
            mNameOffset = value
        End Set
    End Property

    Public Property Angle As Double
        Get
            Return mAngle
        End Get
        Set(value As Double)
            mAngle = value Mod 360
        End Set
    End Property

    'Public Property WireLineSegments As Point()
    '    Get
    '        Return mWireLineSegments
    '    End Get
    '    Set(value As Point())
    '        mWireLineSegments = value
    '    End Set
    'End Property

    Public Shared Operator =(pUI1 As GateUI, pUI2 As GateUI) As Boolean
        Return pUI1.Bounds = pUI2.Bounds
    End Operator

    Public Shared Operator <>(pUI1 As GateUI, pUI2 As GateUI) As Boolean
        Return Not (pUI1.Bounds = pUI2.Bounds)
    End Operator

    Public Shared Function FromXML(xml As XElement) As GateUI
        Dim ui As New GateUI()

        ui.Location = LogicGates.BaseGate.ParseString(Of Point)(xml.<location>.Value)
        ui.Size = LogicGates.BaseGate.ParseString(Of Size)(xml.<size>.Value)
        ui.BorderColor = New Pen(Color.FromArgb(xml.<borderColor>.Value), xml.<borderWidth>.Value)
        ui.FillColor = New SolidBrush(Color.FromArgb(xml.<fillColor>.Value))
        ui.ActiveColor = New SolidBrush(Color.FromArgb(xml.<activeColor>.Value))
        ui.ForeColor = New SolidBrush(Color.FromArgb(xml.<foreColor>.Value))
        ui.Font = LogicGates.BaseGate.XMLToFont(xml.<font>(0))
        ui.NameOffset = LogicGates.BaseGate.ParseString(Of Point)(xml.<nameOffset>.Value)
        Double.TryParse(xml.<angle>.Value, ui.Angle)

        Return ui
    End Function

    Public Function ToXML() As XElement
        Return <ui>
                   <location><%= mLocation %></location>
                   <size><%= mSize %></size>
                   <borderColor><%= mBorderColor.Color.ToArgb() %></borderColor>
                   <borderWidth><%= mBorderColor.Width %></borderWidth>
                   <fillColor><%= mFillColor.Color.ToArgb() %></fillColor>
                   <activeColor><%= mActiveColor.Color.ToArgb() %></activeColor>
                   <foreColor><%= mForeColor.Color.ToArgb() %></foreColor>
                   <font>
                       <family><%= mFont.FontFamily.Name %></family>
                       <size><%= mFont.SizeInPoints %></size>
                       <style><%= mFont.Style %></style>
                   </font>
                   <nameOffset><%= mNameOffset %></nameOffset>
                   <angle><%= mAngle %></angle>
               </ui>
    End Function

    Public Function Clone() As Object Implements ICloneable.Clone
        Return GateUI.FromXML(Me.ToXML())
    End Function
End Class
