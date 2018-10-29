<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.CircuitSurfaceContainer = New LogicSIM.CircuitSurface()
        Me.CircuitSurfaceGatePicker = New LogicSIM.CircuitSurface()
        Me.SuspendLayout()
        '
        'CircuitSurfaceContainer
        '
        Me.CircuitSurfaceContainer.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CircuitSurfaceContainer.BackColor = System.Drawing.Color.FromArgb(CType(CType(32, Byte), Integer), CType(CType(32, Byte), Integer), CType(CType(32, Byte), Integer))
        Me.CircuitSurfaceContainer.Circuit = Nothing
        Me.CircuitSurfaceContainer.Font = New System.Drawing.Font("Consolas", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CircuitSurfaceContainer.ForeColor = System.Drawing.Color.WhiteSmoke
        Me.CircuitSurfaceContainer.Location = New System.Drawing.Point(214, 16)
        Me.CircuitSurfaceContainer.Margin = New System.Windows.Forms.Padding(3, 5, 3, 5)
        Me.CircuitSurfaceContainer.Name = "CircuitSurfaceContainer"
        Me.CircuitSurfaceContainer.Size = New System.Drawing.Size(795, 473)
        Me.CircuitSurfaceContainer.TabIndex = 0
        '
        'CircuitSurfaceGatePicker
        '
        Me.CircuitSurfaceGatePicker.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.CircuitSurfaceGatePicker.BackColor = System.Drawing.Color.FromArgb(CType(CType(32, Byte), Integer), CType(CType(32, Byte), Integer), CType(CType(32, Byte), Integer))
        Me.CircuitSurfaceGatePicker.Circuit = Nothing
        Me.CircuitSurfaceGatePicker.Font = New System.Drawing.Font("Consolas", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CircuitSurfaceGatePicker.Location = New System.Drawing.Point(12, 16)
        Me.CircuitSurfaceGatePicker.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.CircuitSurfaceGatePicker.Name = "CircuitSurfaceGatePicker"
        Me.CircuitSurfaceGatePicker.Size = New System.Drawing.Size(196, 473)
        Me.CircuitSurfaceGatePicker.TabIndex = 1
        '
        'FormMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 17.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1023, 505)
        Me.Controls.Add(Me.CircuitSurfaceGatePicker)
        Me.Controls.Add(Me.CircuitSurfaceContainer)
        Me.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.Name = "FormMain"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "LogicSIM"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents CircuitSurfaceContainer As LogicSIM.CircuitSurface
    Friend WithEvents CircuitSurfaceGatePicker As CircuitSurface
End Class
