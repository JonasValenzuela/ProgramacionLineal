<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form reemplaza a Dispose para limpiar la lista de componentes.
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

    'Requerido por el Diseñador de Windows Forms
    Private components As System.ComponentModel.IContainer

    'NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
    'Se puede modificar usando el Diseñador de Windows Forms.  
    'No lo modifique con el editor de código.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.numVariables = New System.Windows.Forms.NumericUpDown()
        Me.numRestricciones = New System.Windows.Forms.NumericUpDown()
        Me.btnGenerarModelo = New System.Windows.Forms.Button()
        Me.btnResolver = New System.Windows.Forms.Button()
        Me.btnExportarPDF = New System.Windows.Forms.Button()
        Me.cmbTipoObjetivo = New System.Windows.Forms.ComboBox()
        Me.panelObjetivo = New System.Windows.Forms.Panel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.dgvRestricciones = New System.Windows.Forms.DataGridView()
        Me.dgvTablaSimplex = New System.Windows.Forms.DataGridView()
        Me.lblResultado = New System.Windows.Forms.Label()
        Me.btnGraficar = New System.Windows.Forms.Button()
        Me.picGrafica = New System.Windows.Forms.PictureBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.PanelZ = New System.Windows.Forms.Panel()
        CType(Me.numVariables, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.numRestricciones, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.panelObjetivo.SuspendLayout()
        CType(Me.dgvRestricciones, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dgvTablaSimplex, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picGrafica, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'numVariables
        '
        Me.numVariables.Location = New System.Drawing.Point(117, 23)
        Me.numVariables.Name = "numVariables"
        Me.numVariables.Size = New System.Drawing.Size(120, 20)
        Me.numVariables.TabIndex = 0
        '
        'numRestricciones
        '
        Me.numRestricciones.Location = New System.Drawing.Point(117, 53)
        Me.numRestricciones.Name = "numRestricciones"
        Me.numRestricciones.Size = New System.Drawing.Size(120, 20)
        Me.numRestricciones.TabIndex = 1
        '
        'btnGenerarModelo
        '
        Me.btnGenerarModelo.Location = New System.Drawing.Point(255, 53)
        Me.btnGenerarModelo.Name = "btnGenerarModelo"
        Me.btnGenerarModelo.Size = New System.Drawing.Size(121, 23)
        Me.btnGenerarModelo.TabIndex = 2
        Me.btnGenerarModelo.Text = "Generar Modelo"
        Me.btnGenerarModelo.UseVisualStyleBackColor = True
        '
        'btnResolver
        '
        Me.btnResolver.Location = New System.Drawing.Point(12, 478)
        Me.btnResolver.Name = "btnResolver"
        Me.btnResolver.Size = New System.Drawing.Size(385, 23)
        Me.btnResolver.TabIndex = 3
        Me.btnResolver.Text = "Resolver"
        Me.btnResolver.UseVisualStyleBackColor = True
        '
        'btnExportarPDF
        '
        Me.btnExportarPDF.Location = New System.Drawing.Point(403, 478)
        Me.btnExportarPDF.Name = "btnExportarPDF"
        Me.btnExportarPDF.Size = New System.Drawing.Size(75, 23)
        Me.btnExportarPDF.TabIndex = 4
        Me.btnExportarPDF.Text = "PDF"
        Me.btnExportarPDF.UseVisualStyleBackColor = True
        '
        'cmbTipoObjetivo
        '
        Me.cmbTipoObjetivo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbTipoObjetivo.FormattingEnabled = True
        Me.cmbTipoObjetivo.Items.AddRange(New Object() {"Maximizar", "Minimizar"})
        Me.cmbTipoObjetivo.Location = New System.Drawing.Point(255, 22)
        Me.cmbTipoObjetivo.Name = "cmbTipoObjetivo"
        Me.cmbTipoObjetivo.Size = New System.Drawing.Size(121, 21)
        Me.cmbTipoObjetivo.Sorted = True
        Me.cmbTipoObjetivo.TabIndex = 5
        '
        'panelObjetivo
        '
        Me.panelObjetivo.BackColor = System.Drawing.SystemColors.ControlLight
        Me.panelObjetivo.Controls.Add(Me.Label1)
        Me.panelObjetivo.Controls.Add(Me.Label2)
        Me.panelObjetivo.Controls.Add(Me.numVariables)
        Me.panelObjetivo.Controls.Add(Me.numRestricciones)
        Me.panelObjetivo.Controls.Add(Me.btnGenerarModelo)
        Me.panelObjetivo.Controls.Add(Me.cmbTipoObjetivo)
        Me.panelObjetivo.Dock = System.Windows.Forms.DockStyle.Top
        Me.panelObjetivo.Location = New System.Drawing.Point(0, 0)
        Me.panelObjetivo.Name = "panelObjetivo"
        Me.panelObjetivo.Size = New System.Drawing.Size(1171, 87)
        Me.panelObjetivo.TabIndex = 6
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(3, 26)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(78, 13)
        Me.Label1.TabIndex = 9
        Me.Label1.Text = "Num. Variables"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(3, 55)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(99, 13)
        Me.Label2.TabIndex = 10
        Me.Label2.Text = "Num. Restricciones"
        '
        'dgvRestricciones
        '
        Me.dgvRestricciones.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvRestricciones.Location = New System.Drawing.Point(12, 139)
        Me.dgvRestricciones.Name = "dgvRestricciones"
        Me.dgvRestricciones.Size = New System.Drawing.Size(547, 160)
        Me.dgvRestricciones.TabIndex = 0
        '
        'dgvTablaSimplex
        '
        Me.dgvTablaSimplex.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvTablaSimplex.Location = New System.Drawing.Point(12, 305)
        Me.dgvTablaSimplex.Name = "dgvTablaSimplex"
        Me.dgvTablaSimplex.Size = New System.Drawing.Size(547, 167)
        Me.dgvTablaSimplex.TabIndex = 7
        '
        'lblResultado
        '
        Me.lblResultado.AutoSize = True
        Me.lblResultado.Location = New System.Drawing.Point(565, 139)
        Me.lblResultado.Name = "lblResultado"
        Me.lblResultado.Size = New System.Drawing.Size(55, 13)
        Me.lblResultado.TabIndex = 8
        Me.lblResultado.Text = "Resultado"
        '
        'btnGraficar
        '
        Me.btnGraficar.Location = New System.Drawing.Point(484, 478)
        Me.btnGraficar.Name = "btnGraficar"
        Me.btnGraficar.Size = New System.Drawing.Size(75, 23)
        Me.btnGraficar.TabIndex = 9
        Me.btnGraficar.Text = "Graficar"
        Me.btnGraficar.UseVisualStyleBackColor = True
        '
        'picGrafica
        '
        Me.picGrafica.Location = New System.Drawing.Point(568, 282)
        Me.picGrafica.Name = "picGrafica"
        Me.picGrafica.Size = New System.Drawing.Size(262, 190)
        Me.picGrafica.TabIndex = 10
        Me.picGrafica.TabStop = False
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.PanelZ)
        Me.Panel1.Controls.Add(Me.panelObjetivo)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1171, 133)
        Me.Panel1.TabIndex = 11
        '
        'PanelZ
        '
        Me.PanelZ.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
        Me.PanelZ.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelZ.Location = New System.Drawing.Point(0, 87)
        Me.PanelZ.Name = "PanelZ"
        Me.PanelZ.Size = New System.Drawing.Size(1171, 46)
        Me.PanelZ.TabIndex = 12
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1171, 511)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.picGrafica)
        Me.Controls.Add(Me.btnGraficar)
        Me.Controls.Add(Me.lblResultado)
        Me.Controls.Add(Me.dgvTablaSimplex)
        Me.Controls.Add(Me.btnResolver)
        Me.Controls.Add(Me.dgvRestricciones)
        Me.Controls.Add(Me.btnExportarPDF)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "Form1"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Form1"
        CType(Me.numVariables, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.numRestricciones, System.ComponentModel.ISupportInitialize).EndInit()
        Me.panelObjetivo.ResumeLayout(False)
        Me.panelObjetivo.PerformLayout()
        CType(Me.dgvRestricciones, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dgvTablaSimplex, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picGrafica, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents numVariables As NumericUpDown
    Friend WithEvents numRestricciones As NumericUpDown
    Friend WithEvents btnGenerarModelo As Button
    Friend WithEvents btnResolver As Button
    Friend WithEvents btnExportarPDF As Button
    Friend WithEvents cmbTipoObjetivo As ComboBox
    Friend WithEvents panelObjetivo As Panel
    Friend WithEvents dgvRestricciones As DataGridView
    Friend WithEvents dgvTablaSimplex As DataGridView
    Friend WithEvents lblResultado As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents btnGraficar As Button
    Friend WithEvents picGrafica As PictureBox
    Friend WithEvents Panel1 As Panel
    Friend WithEvents PanelZ As Panel
End Class
