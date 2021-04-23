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
        Me.PropertyGridGateEditor = New System.Windows.Forms.PropertyGrid()
        Me.SplitContainerCircuit = New System.Windows.Forms.SplitContainer()
        Me.CircuitSurfaceContainer = New LogicSIM.CircuitSurface()
        Me.LabelSelectedItem = New System.Windows.Forms.Label()
        Me.CircuitSurfaceGatePicker = New LogicSIM.CircuitSurface()
        CType(Me.SplitContainerCircuit, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainerCircuit.Panel1.SuspendLayout()
        Me.SplitContainerCircuit.Panel2.SuspendLayout()
        Me.SplitContainerCircuit.SuspendLayout()
        Me.SuspendLayout()
        '
        'PropertyGridGateEditor
        '
        Me.PropertyGridGateEditor.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PropertyGridGateEditor.BackColor = System.Drawing.Color.FromArgb(CType(CType(32, Byte), Integer), CType(CType(32, Byte), Integer), CType(CType(32, Byte), Integer))
        Me.PropertyGridGateEditor.CategoryForeColor = System.Drawing.Color.WhiteSmoke
        Me.PropertyGridGateEditor.CategorySplitterColor = System.Drawing.Color.FromArgb(CType(CType(32, Byte), Integer), CType(CType(32, Byte), Integer), CType(CType(32, Byte), Integer))
        Me.PropertyGridGateEditor.CommandsLinkColor = System.Drawing.SystemColors.HotTrack
        Me.PropertyGridGateEditor.DisabledItemForeColor = System.Drawing.Color.FromArgb(CType(CType(127, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(245, Byte), Integer))
        Me.PropertyGridGateEditor.HelpBackColor = System.Drawing.Color.FromArgb(CType(CType(32, Byte), Integer), CType(CType(32, Byte), Integer), CType(CType(32, Byte), Integer))
        Me.PropertyGridGateEditor.HelpForeColor = System.Drawing.Color.WhiteSmoke
        Me.PropertyGridGateEditor.HelpVisible = False
        Me.PropertyGridGateEditor.LineColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.PropertyGridGateEditor.Location = New System.Drawing.Point(0, 20)
        Me.PropertyGridGateEditor.Name = "PropertyGridGateEditor"
        Me.PropertyGridGateEditor.Size = New System.Drawing.Size(186, 457)
        Me.PropertyGridGateEditor.TabIndex = 2
        Me.PropertyGridGateEditor.ViewBackColor = System.Drawing.Color.FromArgb(CType(CType(32, Byte), Integer), CType(CType(32, Byte), Integer), CType(CType(32, Byte), Integer))
        Me.PropertyGridGateEditor.ViewBorderColor = System.Drawing.Color.FromArgb(CType(CType(32, Byte), Integer), CType(CType(32, Byte), Integer), CType(CType(32, Byte), Integer))
        Me.PropertyGridGateEditor.ViewForeColor = System.Drawing.Color.WhiteSmoke
        '
        'SplitContainerCircuit
        '
        Me.SplitContainerCircuit.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SplitContainerCircuit.Location = New System.Drawing.Point(214, 16)
        Me.SplitContainerCircuit.Name = "SplitContainerCircuit"
        '
        'SplitContainerCircuit.Panel1
        '
        Me.SplitContainerCircuit.Panel1.Controls.Add(Me.CircuitSurfaceContainer)
        '
        'SplitContainerCircuit.Panel2
        '
        Me.SplitContainerCircuit.Panel2.BackColor = System.Drawing.Color.FromArgb(CType(CType(32, Byte), Integer), CType(CType(32, Byte), Integer), CType(CType(32, Byte), Integer))
        Me.SplitContainerCircuit.Panel2.Controls.Add(Me.LabelSelectedItem)
        Me.SplitContainerCircuit.Panel2.Controls.Add(Me.PropertyGridGateEditor)
        Me.SplitContainerCircuit.Panel2.ForeColor = System.Drawing.Color.WhiteSmoke
        Me.SplitContainerCircuit.Size = New System.Drawing.Size(797, 477)
        Me.SplitContainerCircuit.SplitterDistance = 607
        Me.SplitContainerCircuit.TabIndex = 3
        '
        'CircuitSurfaceContainer
        '
        Me.CircuitSurfaceContainer.BackColor = System.Drawing.Color.FromArgb(CType(CType(32, Byte), Integer), CType(CType(32, Byte), Integer), CType(CType(32, Byte), Integer))
        Me.CircuitSurfaceContainer.Circuit = Nothing
        Me.CircuitSurfaceContainer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CircuitSurfaceContainer.Font = New System.Drawing.Font("Consolas", 27.75!, System.Drawing.FontStyle.Bold)
        Me.CircuitSurfaceContainer.ForeColor = System.Drawing.Color.DimGray
        Me.CircuitSurfaceContainer.Location = New System.Drawing.Point(0, 0)
        Me.CircuitSurfaceContainer.Margin = New System.Windows.Forms.Padding(3, 5, 3, 5)
        Me.CircuitSurfaceContainer.MultiSelect = True
        Me.CircuitSurfaceContainer.Name = "CircuitSurfaceContainer"
        Me.CircuitSurfaceContainer.Readonly = False
        Me.CircuitSurfaceContainer.Size = New System.Drawing.Size(607, 477)
        Me.CircuitSurfaceContainer.Snap = New System.Drawing.Size(10, 10)
        Me.CircuitSurfaceContainer.SnapToGrid = True
        Me.CircuitSurfaceContainer.TabIndex = 0
        Me.CircuitSurfaceContainer.Zoom = 1.0R
        '
        'LabelSelectedItem
        '
        Me.LabelSelectedItem.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LabelSelectedItem.Location = New System.Drawing.Point(-3, 0)
        Me.LabelSelectedItem.Name = "LabelSelectedItem"
        Me.LabelSelectedItem.Size = New System.Drawing.Size(189, 17)
        Me.LabelSelectedItem.TabIndex = 3
        Me.LabelSelectedItem.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'CircuitSurfaceGatePicker
        '
        Me.CircuitSurfaceGatePicker.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.CircuitSurfaceGatePicker.BackColor = System.Drawing.Color.FromArgb(CType(CType(32, Byte), Integer), CType(CType(32, Byte), Integer), CType(CType(32, Byte), Integer))
        Me.CircuitSurfaceGatePicker.Circuit = Nothing
        Me.CircuitSurfaceGatePicker.Font = New System.Drawing.Font("Consolas", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CircuitSurfaceGatePicker.ForeColor = System.Drawing.Color.DimGray
        Me.CircuitSurfaceGatePicker.Location = New System.Drawing.Point(12, 16)
        Me.CircuitSurfaceGatePicker.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.CircuitSurfaceGatePicker.MultiSelect = True
        Me.CircuitSurfaceGatePicker.Name = "CircuitSurfaceGatePicker"
        Me.CircuitSurfaceGatePicker.Readonly = False
        Me.CircuitSurfaceGatePicker.Size = New System.Drawing.Size(196, 476)
        Me.CircuitSurfaceGatePicker.Snap = New System.Drawing.Size(10, 10)
        Me.CircuitSurfaceGatePicker.SnapToGrid = True
        Me.CircuitSurfaceGatePicker.TabIndex = 1
        Me.CircuitSurfaceGatePicker.Zoom = 1.0R
        '
        'FormMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 21.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(22, Byte), Integer), CType(CType(22, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(1023, 505)
        Me.Controls.Add(Me.SplitContainerCircuit)
        Me.Controls.Add(Me.CircuitSurfaceGatePicker)
        Me.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.KeyPreview = True
        Me.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.Name = "FormMain"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "LogicSIM"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.SplitContainerCircuit.Panel1.ResumeLayout(False)
        Me.SplitContainerCircuit.Panel2.ResumeLayout(False)
        CType(Me.SplitContainerCircuit, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainerCircuit.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents CircuitSurfaceContainer As LogicSIM.CircuitSurface
    Friend WithEvents CircuitSurfaceGatePicker As CircuitSurface
    Friend WithEvents PropertyGridGateEditor As PropertyGrid
    Friend WithEvents SplitContainerCircuit As SplitContainer
    Friend WithEvents LabelSelectedItem As Label
End Class
