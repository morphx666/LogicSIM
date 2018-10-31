Imports System.ComponentModel

<TypeConverter(GetType(GateUIConverter))>
Public Class GateUI
    Implements ICloneable

    Private mLocation As Point
    Private mSize As Size
    Private mPath As Drawing2D.GraphicsPath
    Private mBorderColor As Color
    Private mBorderWidth As Integer
    Private mFillColor As Color
    Private mActiveColor As Color
    Private mForeColor As Color
    Private mFont As Font
    Private mNameOffset As Point
    Private mAngle As Double

    Private mBorderColorPen As Pen
    Private mFillColorBrush As SolidBrush
    Private mActiveColorBrush As SolidBrush
    Private mForeColorBrush As SolidBrush

    Public Sub New()
        mLocation = New Point(0, 0)
        mSize = New Size(100, 80)

        BorderWidth = 2
        BorderColor = Color.White
        FillColor = Color.DeepSkyBlue
        ActiveColor = Color.MediumPurple
        ForeColor = Color.Yellow
        mFont = New Font("Consolas", 12, FontStyle.Regular)
    End Sub

    <BrowsableAttribute(False)> Public Property Path As Drawing2D.GraphicsPath
        Get
            Return mPath
        End Get
        Set(value As Drawing2D.GraphicsPath)
            mPath = value
        End Set
    End Property

    <BrowsableAttribute(False)> Public Property Location As Point
        Get
            Return mLocation
        End Get
        Set(value As Point)
            mLocation = value
        End Set
    End Property

    <BrowsableAttribute(False)> Public Property X As Integer
        Get
            Return mLocation.X
        End Get
        Set(value As Integer)
            mLocation.X = value
        End Set
    End Property

    <BrowsableAttribute(False)> Public Property Y As Integer
        Get
            Return mLocation.Y
        End Get
        Set(value As Integer)
            mLocation.Y = value
        End Set
    End Property

    <BrowsableAttribute(False)> Public Property Size As Size
        Get
            Return mSize
        End Get
        Set(value As Size)
            mSize = value
        End Set
    End Property

    <BrowsableAttribute(False)> Public Property Width As Integer
        Get
            Return mSize.Width
        End Get
        Set(value As Integer)
            mSize.Width = value
        End Set
    End Property

    <BrowsableAttribute(False)> Public Property Height As Integer
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

    Public Property BorderWidth As Integer
        Get
            Return mBorderWidth
        End Get
        Set(value As Integer)
            mBorderWidth = value
            BorderColor = BorderColor
        End Set
    End Property

    Public Property BorderColor As Color
        Get
            Return mBorderColor
        End Get
        Set(value As Color)
            mBorderColor = value
            mBorderColorPen = New Pen(mBorderColor, mBorderWidth)
        End Set
    End Property

    Public Property FillColor As Color
        Get
            Return mFillColor
        End Get
        Set(value As Color)
            mFillColor = value
            mFillColorBrush = New SolidBrush(mFillColor)
        End Set
    End Property

    Public Property ActiveColor As Color
        Get
            Return mActiveColor
        End Get
        Set(value As Color)
            mActiveColor = value
            mActiveColorBrush = New SolidBrush(mActiveColor)
        End Set
    End Property

    Public Property ForeColor As Color
        Get
            Return mForeColor
        End Get
        Set(value As Color)
            mForeColor = value
            mForeColorBrush = New SolidBrush(mForeColor)
        End Set
    End Property

    <BrowsableAttribute(False)> Public ReadOnly Property BorderColorPen As Pen
        Get
            Return mBorderColorPen
        End Get
    End Property

    <BrowsableAttribute(False)> Public ReadOnly Property FillColorBrush As SolidBrush
        Get
            Return mFillColorBrush
        End Get
    End Property

    <BrowsableAttribute(False)> Public ReadOnly Property ActiveColorBrush As SolidBrush
        Get
            Return mActiveColorBrush
        End Get
    End Property

    <BrowsableAttribute(False)> Public ReadOnly Property ForeColorBrush As SolidBrush
        Get
            Return mForeColorBrush
        End Get
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
        Dim ui As New GateUI With {
            .Location = LogicGates.BaseGate.ParseString(Of Point)(xml.<location>.Value),
            .Size = LogicGates.BaseGate.ParseString(Of Size)(xml.<size>.Value),
            .BorderWidth = Integer.Parse(xml.<borderWidth>.Value),
            .BorderColor = Color.FromArgb(xml.<borderColor>.Value),
            .FillColor = Color.FromArgb(xml.<fillColor>.Value),
            .ActiveColor = Color.FromArgb(xml.<activeColor>.Value),
            .ForeColor = Color.FromArgb(xml.<foreColor>.Value),
            .Font = LogicGates.BaseGate.XMLToFont(xml.<font>(0)),
            .NameOffset = LogicGates.BaseGate.ParseString(Of Point)(xml.<nameOffset>.Value),
            .Angle = Double.Parse(xml.<angle>.Value)
        }

        Return ui
    End Function

    Public Function ToXML() As XElement
        Return <ui>
                   <location><%= mLocation %></location>
                   <size><%= mSize %></size>
                   <borderColor><%= mBorderColor.ToArgb() %></borderColor>
                   <borderWidth><%= mBorderWidth %></borderWidth>
                   <fillColor><%= mFillColor.ToArgb() %></fillColor>
                   <activeColor><%= mActiveColor.ToArgb() %></activeColor>
                   <foreColor><%= mForeColor.ToArgb() %></foreColor>
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

Public Class GateUIConverter
    Inherits ExpandableObjectConverter

    Private selectedGateUI As GateUI

    Public Overrides Function CanConvertTo(ByVal context As ITypeDescriptorContext, ByVal destinationType As Type) As Boolean
        If destinationType Is GetType(GateUI) Then
            Return True
        Else
            Return MyBase.CanConvertTo(context, destinationType)
        End If
    End Function

    Public Overloads Overrides Function ConvertTo(ByVal context As ITypeDescriptorContext, ByVal culture As Globalization.CultureInfo, ByVal value As Object, ByVal destinationType As System.Type) As Object
        If destinationType Is GetType(String) AndAlso TypeOf value Is GateUI Then
            selectedGateUI = CType(value, GateUI)
            Return selectedGateUI.ToString()
        End If

        Return MyBase.ConvertTo(context, culture, value, destinationType)
    End Function

    Public Overloads Overrides Function ConvertFrom(ByVal context As ITypeDescriptorContext, ByVal culture As Globalization.CultureInfo, ByVal value As Object) As Object
        If TypeOf value Is String Then
            Dim description As String = CType(value, String)
            'selectedGateUI.Description = description.Replace(selectedGateUI.Time.ToString() + " ", "")
            Return selectedGateUI
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
