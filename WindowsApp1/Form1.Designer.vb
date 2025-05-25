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
        Me.nudVariables = New System.Windows.Forms.NumericUpDown()
        Me.nudRestricciones = New System.Windows.Forms.NumericUpDown()
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
        Me.btnPreprocesar = New System.Windows.Forms.Button()
        CType(Me.nudVariables, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudRestricciones, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.panelObjetivo.SuspendLayout()
        CType(Me.dgvRestricciones, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dgvTablaSimplex, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picGrafica, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'nudVariables
        '
        Me.nudVariables.Location = New System.Drawing.Point(117, 23)
        Me.nudVariables.Name = "nudVariables"
        Me.nudVariables.Size = New System.Drawing.Size(120, 20)
        Me.nudVariables.TabIndex = 0
        '
        'nudRestricciones
        '
        Me.nudRestricciones.Location = New System.Drawing.Point(117, 53)
        Me.nudRestricciones.Name = "nudRestricciones"
        Me.nudRestricciones.Size = New System.Drawing.Size(120, 20)
        Me.nudRestricciones.TabIndex = 1
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
        Me.btnResolver.Location = New System.Drawing.Point(144, 447)
        Me.btnResolver.Name = "btnResolver"
        Me.btnResolver.Size = New System.Drawing.Size(75, 23)
        Me.btnResolver.TabIndex = 3
        Me.btnResolver.Text = "Resolver"
        Me.btnResolver.UseVisualStyleBackColor = True
        '
        'btnExportarPDF
        '
        Me.btnExportarPDF.Location = New System.Drawing.Point(12, 418)
        Me.btnExportarPDF.Name = "btnExportarPDF"
        Me.btnExportarPDF.Size = New System.Drawing.Size(75, 23)
        Me.btnExportarPDF.TabIndex = 4
        Me.btnExportarPDF.Text = "PDF"
        Me.btnExportarPDF.UseVisualStyleBackColor = True
        '
        'cmbTipoObjetivo
        '
        Me.cmbTipoObjetivo.FormattingEnabled = True
        Me.cmbTipoObjetivo.Items.AddRange(New Object() {"Maximizar", "Minimizar"})
        Me.cmbTipoObjetivo.Location = New System.Drawing.Point(255, 22)
        Me.cmbTipoObjetivo.Name = "cmbTipoObjetivo"
        Me.cmbTipoObjetivo.Size = New System.Drawing.Size(121, 21)
        Me.cmbTipoObjetivo.TabIndex = 5
        '
        'panelObjetivo
        '
        Me.panelObjetivo.Controls.Add(Me.Label1)
        Me.panelObjetivo.Controls.Add(Me.Label2)
        Me.panelObjetivo.Controls.Add(Me.nudVariables)
        Me.panelObjetivo.Controls.Add(Me.nudRestricciones)
        Me.panelObjetivo.Controls.Add(Me.btnGenerarModelo)
        Me.panelObjetivo.Controls.Add(Me.cmbTipoObjetivo)
        Me.panelObjetivo.Location = New System.Drawing.Point(0, 2)
        Me.panelObjetivo.Name = "panelObjetivo"
        Me.panelObjetivo.Size = New System.Drawing.Size(828, 87)
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
        Me.dgvRestricciones.Location = New System.Drawing.Point(21, 110)
        Me.dgvRestricciones.Name = "dgvRestricciones"
        Me.dgvRestricciones.Size = New System.Drawing.Size(343, 128)
        Me.dgvRestricciones.TabIndex = 0
        '
        'dgvTablaSimplex
        '
        Me.dgvTablaSimplex.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvTablaSimplex.Location = New System.Drawing.Point(21, 266)
        Me.dgvTablaSimplex.Name = "dgvTablaSimplex"
        Me.dgvTablaSimplex.Size = New System.Drawing.Size(343, 125)
        Me.dgvTablaSimplex.TabIndex = 7
        '
        'lblResultado
        '
        Me.lblResultado.AutoSize = True
        Me.lblResultado.Location = New System.Drawing.Point(443, 110)
        Me.lblResultado.Name = "lblResultado"
        Me.lblResultado.Size = New System.Drawing.Size(55, 13)
        Me.lblResultado.TabIndex = 8
        Me.lblResultado.Text = "Resultado"
        '
        'btnGraficar
        '
        Me.btnGraficar.Location = New System.Drawing.Point(586, 418)
        Me.btnGraficar.Name = "btnGraficar"
        Me.btnGraficar.Size = New System.Drawing.Size(75, 23)
        Me.btnGraficar.TabIndex = 9
        Me.btnGraficar.Text = "Graficar"
        Me.btnGraficar.UseVisualStyleBackColor = True
        '
        'picGrafica
        '
        Me.picGrafica.Location = New System.Drawing.Point(492, 201)
        Me.picGrafica.Name = "picGrafica"
        Me.picGrafica.Size = New System.Drawing.Size(262, 190)
        Me.picGrafica.TabIndex = 10
        Me.picGrafica.TabStop = False
        '
        'btnPreprocesar
        '
        Me.btnPreprocesar.Location = New System.Drawing.Point(301, 418)
        Me.btnPreprocesar.Name = "btnPreprocesar"
        Me.btnPreprocesar.Size = New System.Drawing.Size(75, 23)
        Me.btnPreprocesar.TabIndex = 11
        Me.btnPreprocesar.Text = "Preprocesar"
        Me.btnPreprocesar.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(823, 482)
        Me.Controls.Add(Me.btnPreprocesar)
        Me.Controls.Add(Me.picGrafica)
        Me.Controls.Add(Me.btnGraficar)
        Me.Controls.Add(Me.lblResultado)
        Me.Controls.Add(Me.dgvTablaSimplex)
        Me.Controls.Add(Me.btnResolver)
        Me.Controls.Add(Me.dgvRestricciones)
        Me.Controls.Add(Me.panelObjetivo)
        Me.Controls.Add(Me.btnExportarPDF)
        Me.Name = "Form1"
        Me.Text = "Form1"
        CType(Me.nudVariables, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudRestricciones, System.ComponentModel.ISupportInitialize).EndInit()
        Me.panelObjetivo.ResumeLayout(False)
        Me.panelObjetivo.PerformLayout()
        CType(Me.dgvRestricciones, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dgvTablaSimplex, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picGrafica, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents nudVariables As NumericUpDown
    Friend WithEvents nudRestricciones As NumericUpDown
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
    Friend WithEvents btnPreprocesar As Button
End Class
