
Imports iTextSharp.text
Imports System.Drawing
Imports iTextSharp.text.pdf
Imports System.IO
Public Class Form1
    ' Variables globales
    Private numVars As Integer = 0
    Private numRes As Integer = 0
    Private totalVars As Integer = 0
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        cmbTipoObjetivo.Items.AddRange({"Maximizar", "Minimizar"})
        cmbTipoObjetivo.SelectedIndex = 0
        nudVariables.Minimum = 2
        nudVariables.Maximum = 10
        nudRestricciones.Minimum = 1
        nudRestricciones.Maximum = 10
    End Sub

    Private Sub btnGenerarModelo_Click(sender As Object, e As EventArgs) Handles btnGenerarModelo.Click
        Dim numVars As Integer = nudVariables.Value
        Dim numRes As Integer = nudRestricciones.Value

        ' Cargar función objetivo con etiquetas y Z =
        panelObjetivo.Controls.Clear()

        ' Etiqueta Z =
        Dim lblZ As New Label()
        lblZ.Text = "Z ="
        lblZ.AutoSize = True
        lblZ.Location = New Point(10, 15)
        panelObjetivo.Controls.Add(lblZ)

        ' TextBoxes con etiquetas X1, X2, ...
        For i As Integer = 1 To numVars
            ' Etiqueta Xi
            Dim lbl As New Label()
            lbl.Text = "X" & i
            lbl.AutoSize = True
            lbl.Location = New Point(40 + (i - 1) * 70, 0)
            panelObjetivo.Controls.Add(lbl)

            ' TextBox
            Dim txt As New TextBox()
            txt.Name = "txtObj" & i
            txt.Width = 50
            txt.Location = New Point(40 + (i - 1) * 70, 20)
            panelObjetivo.Controls.Add(txt)
        Next


        ' Cargar restricciones
        dgvRestricciones.Columns.Clear()
        For i As Integer = 1 To numVars
            dgvRestricciones.Columns.Add("X" & i, "X" & i)
        Next

        ' Agregar columna de operador como ComboBox
        Dim colOperador As New DataGridViewComboBoxColumn()
        colOperador.Name = "Operador"
        colOperador.HeaderText = "Operador"
        colOperador.Items.AddRange({"<=", ">=", "="})
        dgvRestricciones.Columns.Add(colOperador)

        ' Agregar columna de resultado RHS
        dgvRestricciones.Columns.Add("RHS", "Resultado")

        ' Establecer número de filas
        dgvRestricciones.RowCount = numRes
    End Sub


    Private Function ObtenerFuncionObjetivo(numVars As Integer) As List(Of Double)
        Dim obj As New List(Of Double)
        For i As Integer = 1 To numVars
            Dim txtBox As TextBox = CType(panelObjetivo.Controls("txtObj" & i), TextBox)
            Dim val As Double = If(Double.TryParse(txtBox.Text, val), val, 0)
            obj.Add(val)
        Next
        Return obj
    End Function

    Private Function ObtenerRestricciones(numVars As Integer, numRes As Integer) As List(Of Double())
        Dim restricciones As New List(Of Double())
        For i As Integer = 0 To numRes - 1
            Dim fila As DataGridViewRow = dgvRestricciones.Rows(i)
            Dim r(numVars + 1) As Double ' Coeficientes + RHS

            For j As Integer = 0 To numVars - 1
                r(j) = Val(fila.Cells(j).Value)
            Next
            r(numVars) = Val(fila.Cells(numVars + 1).Value) ' RHS
            restricciones.Add(r)
        Next
        Return restricciones
    End Function
    Private Sub MostrarTabla(tabla(,) As Double)
        dgvTablaSimplex.Rows.Clear()
        dgvTablaSimplex.Columns.Clear()

        Try
            ' Crear columnas
            For j As Integer = 0 To tabla.GetLength(1) - 1
                dgvTablaSimplex.Columns.Add("C" & j, "C" & j)
            Next

            ' Crear filas
            For i As Integer = 0 To tabla.GetLength(0) - 1
                Dim row As New DataGridViewRow()
                row.CreateCells(dgvTablaSimplex)
                For j As Integer = 0 To tabla.GetLength(1) - 1
                    row.Cells(j).Value = Math.Round(tabla(i, j), 2)
                Next
                dgvTablaSimplex.Rows.Add(row)
            Next
        Catch ex As Exception
            MessageBox.Show("ERROR al mostrar la tabla: " & ex.Message)
        End Try
    End Sub

    Private Sub btnResolver_Click(sender As Object, e As EventArgs) Handles btnResolver.Click
        Const M As Double = 1_000_000

        numVars = nudVariables.Value
        numRes = nudRestricciones.Value

        Dim obj = ObtenerFuncionObjetivo(numVars)
        Dim tipoMax As Boolean = (cmbTipoObjetivo.SelectedItem.ToString() = "Maximizar")

        Dim nombresColumnas As New List(Of String)
        For Each col As DataGridViewColumn In dgvTablaSimplex.Columns
            nombresColumnas.Add(col.HeaderText)
        Next

        Dim totalColumnasGrid As Integer = dgvTablaSimplex.ColumnCount
        totalVars = totalColumnasGrid - 1 ' Incluye todas las columnas excepto RHS

        Dim tableau(numRes, totalColumnasGrid - 1) As Double

        For i As Integer = 0 To numRes - 1
            For j As Integer = 0 To totalColumnasGrid - 1
                Dim valor As Object = dgvTablaSimplex.Rows(i).Cells(j).Value
                Dim num As Double
                If Double.TryParse(valor?.ToString(), num) Then
                    tableau(i, j) = num
                Else
                    tableau(i, j) = 0
                End If
            Next
        Next

        ' Fila Z
        For j As Integer = 0 To totalColumnasGrid - 2
            Dim nombre = nombresColumnas(j)
            Dim valorZ As Double = 0

            If nombre.StartsWith("X") Then
                Dim index = Integer.Parse(nombre.Substring(1)) - 1
                If index >= 0 AndAlso index < obj.Count Then
                    valorZ = obj(index)
                End If
            ElseIf nombre.StartsWith("A") Then
                valorZ = If(tipoMax, M, -M)
            End If

            tableau(numRes, j) = If(tipoMax, -valorZ, valorZ)
        Next

        tableau(numRes, totalColumnasGrid - 1) = 0

        MostrarTabla(tableau)

        ' SIMPLEX
        While True
            Dim colPiv As Integer = -1
            Dim min As Double = 0
            For j As Integer = 0 To totalVars - 1
                If tableau(numRes, j) < min Then
                    min = tableau(numRes, j)
                    colPiv = j
                End If
            Next
            If colPiv = -1 Then Exit While

            Dim filaPiv As Integer = -1
            Dim minRatio As Double = Double.MaxValue
            For i As Integer = 0 To numRes - 1
                If tableau(i, colPiv) > 0 Then
                    Dim ratio = tableau(i, totalColumnasGrid - 1) / tableau(i, colPiv)
                    If ratio < minRatio Then
                        minRatio = ratio
                        filaPiv = i
                    End If
                End If
            Next

            If filaPiv = -1 Then
                MessageBox.Show("Solución ilimitada.")
                lblResultado.Text = "Sin solución."
                Return
            End If

            Dim pivote = tableau(filaPiv, colPiv)
            For j As Integer = 0 To totalColumnasGrid - 1
                tableau(filaPiv, j) /= pivote
            Next
            For i As Integer = 0 To numRes
                If i <> filaPiv Then
                    Dim factor = tableau(i, colPiv)
                    For j As Integer = 0 To totalColumnasGrid - 1
                        tableau(i, j) -= factor * tableau(filaPiv, j)
                    Next
                End If
            Next

            MostrarTabla(tableau)
        End While

        ' Validación artificial
        For j As Integer = 0 To totalVars - 1
            If nombresColumnas(j).StartsWith("A") Then
                For i As Integer = 0 To numRes - 1
                    If Math.Abs(tableau(i, j) - 1) < 0.0001 Then
                        Dim unica = True
                        For k As Integer = 0 To numRes - 1
                            If k <> i AndAlso Math.Abs(tableau(k, j)) > 0.0001 Then
                                unica = False
                                Exit For
                            End If
                        Next
                        If unica Then
                            MessageBox.Show("Solución no factible: variable artificial activa.")
                            lblResultado.Text = "Solución no factible."
                            Return
                        End If
                    End If
                Next
            End If
        Next

        ' RESULTADOS
        Dim resultado As String = "Z óptimo: " & Math.Round(tableau(numRes, totalColumnasGrid - 1), 2) & vbCrLf
        For j As Integer = 0 To totalVars - 1
            If nombresColumnas(j).StartsWith("X") Then
                Dim valor As Double = 0
                For i As Integer = 0 To numRes - 1
                    If Math.Abs(tableau(i, j) - 1) < 0.0001 Then
                        Dim unica = True
                        For k As Integer = 0 To numRes - 1
                            If k <> i AndAlso Math.Abs(tableau(k, j)) > 0.0001 Then
                                unica = False
                                Exit For
                            End If
                        Next
                        If unica Then
                            valor = tableau(i, totalColumnasGrid - 1)
                            Exit For
                        End If
                    End If
                Next
                resultado &= nombresColumnas(j) & " = " & Math.Round(valor, 2) & vbCrLf
            End If
        Next

        '  Validación final y despliegue
        If String.IsNullOrEmpty(resultado.Trim()) Then
            MsgBox("No se generó resultado. Verifica si el proceso terminó antes.")
        Else
            MsgBox(resultado, MsgBoxStyle.Information, "Resultado Simplex")
            lblResultado.Text = resultado
        End If
    End Sub


    Private Sub btnExportarPDF_Click(sender As Object, e As EventArgs) Handles btnExportarPDF.Click
        Dim saveDlg As New SaveFileDialog()
        saveDlg.Filter = "Archivo PDF|*.pdf"
        saveDlg.Title = "Guardar resultados en PDF"
        saveDlg.FileName = "ResultadoSimplex.pdf"

        If saveDlg.ShowDialog() = DialogResult.OK Then
            Try
                Dim doc As New Document(PageSize.A4)
                PdfWriter.GetInstance(doc, New FileStream(saveDlg.FileName, FileMode.Create))
                doc.Open()

                Dim titulo As New Paragraph("Resultado del Método Simplex", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16))
                titulo.Alignment = iTextSharp.text.Element.ALIGN_CENTER
                doc.Add(titulo)
                doc.Add(New Paragraph(" "))
                doc.Add(New Paragraph(lblResultado.Text))
                doc.Add(New Paragraph(" "))

                Dim pdfTable As New PdfPTable(dgvTablaSimplex.Columns.Count)
                For Each col As DataGridViewColumn In dgvTablaSimplex.Columns
                    pdfTable.AddCell(New Phrase(col.HeaderText))
                Next
                For Each row As DataGridViewRow In dgvTablaSimplex.Rows
                    If Not row.IsNewRow Then
                        For Each cell As DataGridViewCell In row.Cells
                            pdfTable.AddCell(If(cell.Value IsNot Nothing, cell.Value.ToString(), ""))
                        Next
                    End If
                Next

                doc.Add(pdfTable)
                doc.Close()
                MessageBox.Show("PDF exportado correctamente.")
            Catch ex As Exception
                MessageBox.Show("Error al exportar PDF: " & ex.Message)
            End Try
        End If
    End Sub
    Private Function ObtenerFuncionObjetivo() As List(Of Double)
        Dim coeficientes As New List(Of Double)

        ' Verifica que haya al menos una fila y columna
        If dgvTablaSimplex.RowCount = 0 Or dgvTablaSimplex.ColumnCount = 0 Then
            Return coeficientes
        End If

        ' Recorre las columnas de la fila 0
        For Each columna As DataGridViewColumn In dgvTablaSimplex.Columns
            Dim valorCelda As Object = dgvTablaSimplex.Rows(0).Cells(columna.Index).Value

            ' Intenta convertir el valor a número
            Dim numero As Double
            If Double.TryParse(valorCelda?.ToString(), numero) Then
                coeficientes.Add(numero)
            Else
                coeficientes.Add(0) ' Si la celda está vacía o es inválida, agrega 0
            End If
        Next
        Return coeficientes
    End Function

    Private Sub btnGraficar_Click(sender As Object, e As EventArgs) Handles btnGraficar.Click
        If nudVariables.Value <> 2 Then
            MessageBox.Show("La gráfica solo está disponible para 2 variables.")
            Exit Sub
        End If

        Dim g As Graphics = picGrafica.CreateGraphics()
        g.Clear(Drawing.Color.White)

        Dim ancho As Integer = picGrafica.Width
        Dim alto As Integer = picGrafica.Height

        Dim escalaX As Double = 20 ' Ajusta escala según necesidad
        Dim escalaY As Double = 20
        Dim origenX As Integer = 40
        Dim origenY As Integer = alto - 40

        ' Dibujar ejes
        Dim lapizEjes As New Pen(Drawing.Color.Black, 2)
        g.DrawLine(lapizEjes, origenX, 0, origenX, alto) ' eje Y
        g.DrawLine(lapizEjes, 0, origenY, ancho, origenY) ' eje X

        ' Colores para restricciones
        Dim colores() As Drawing.Color = {Drawing.Color.Red, Drawing.Color.Blue, Drawing.Color.Green, Drawing.Color.Cyan, Drawing.Color.Orange}

        ' Graficar restricciones desde la fila 0
        For i As Integer = 0 To nudRestricciones.Value - 1
            Try
                Dim fila As DataGridViewRow = dgvRestricciones.Rows(i)
                Dim a As Double = Val(fila.Cells(0).Value)
                Dim b As Double = Val(fila.Cells(1).Value)
                Dim c As Double = Val(fila.Cells("RHS").Value)
                Dim operador As String = Convert.ToString(fila.Cells("Operador").Value)

                If a = 0 AndAlso b = 0 Then Continue For ' Evitar división por cero

                ' Calcular intersecciones con los ejes
                Dim x1 As Double = 0
                Dim y1 As Double = If(b <> 0, c / b, 0)
                Dim x2 As Double = If(a <> 0, c / a, 0)
                Dim y2 As Double = 0

                ' Convertir a coordenadas de pantalla
                Dim px1 = origenX + x1 * escalaX
                Dim py1 = origenY - y1 * escalaY
                Dim px2 = origenX + x2 * escalaX
                Dim py2 = origenY - y2 * escalaY

                Dim lapiz As New Pen(colores(i Mod colores.Length), 2)
                g.DrawLine(lapiz, CSng(px1), CSng(py1), CSng(px2), CSng(py2))
            Catch ex As Exception
                MessageBox.Show("Error al graficar la restricción " & (i + 1) & ": " & ex.Message)
            End Try
        Next
    End Sub

    Private Sub btnPreprocesar_Click(sender As Object, e As EventArgs) Handles btnPreprocesar.Click
        Dim numVars As Integer = nudVariables.Value
        Dim numRes As Integer = nudRestricciones.Value
        Dim nombresColumnas As New List(Of String)()

        Dim holgura As Integer = 0
        Dim exceso As Integer = 0
        Dim artificial As Integer = 0

        dgvTablaSimplex.Columns.Clear()
        dgvTablaSimplex.Rows.Clear()

        ' Leer función objetivo desde los TextBox (no desde la tabla)
        Dim funcionObjetivo As List(Of Double) = ObtenerFuncionObjetivo(numVars)
        If funcionObjetivo.Count = 0 OrElse funcionObjetivo.All(Function(c) c = 0) Then
            MessageBox.Show("Debes llenar los coeficientes de la función objetivo.", "Falta F.O.", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Construir encabezados
        For i = 1 To numVars
            nombresColumnas.Add("X" & i)
        Next

        Dim filasTabla As New List(Of List(Of Double))()

        ' Procesar cada restricción
        For i As Integer = 0 To numRes - 1
            Dim fila As DataGridViewRow = dgvRestricciones.Rows(i)
            Dim operador As String = Convert.ToString(fila.Cells("Operador").Value)
            Dim rhs As Double = Val(fila.Cells("RHS").Value)
            Dim filaNumerica As New List(Of Double)

            ' Coeficientes de X1 a Xn
            For j As Integer = 0 To numVars - 1
                Dim valor As Object = fila.Cells(j).Value
                Dim coef As Double
                If Not Double.TryParse(valor?.ToString(), coef) Then
                    coef = 0 ' por defecto si no es válido
                End If
                filaNumerica.Add(coef)
            Next

            ' Variables de holgura, exceso y artificial
            Select Case operador
                Case "<="
                    holgura += 1
                    For k = 0 To holgura + exceso + artificial - 2
                        filaNumerica.Add(0)
                    Next
                    filaNumerica.Add(1)
                    nombresColumnas.Add("S" & holgura)

                Case ">="
                    exceso += 1
                    artificial += 1
                    For k = 0 To holgura + exceso + artificial - 3
                        filaNumerica.Add(0)
                    Next
                    filaNumerica.Add(-1)
                    filaNumerica.Add(1)
                    nombresColumnas.Add("E" & exceso)
                    nombresColumnas.Add("A" & artificial)

                Case "="
                    artificial += 1
                    For k = 0 To holgura + exceso + artificial - 2
                        filaNumerica.Add(0)
                    Next
                    filaNumerica.Add(1)
                    nombresColumnas.Add("A" & artificial)
            End Select

            filaNumerica.Add(rhs)
            filasTabla.Add(filaNumerica)
        Next

        ' Construir columnas del DataGridView
        For Each nombre In nombresColumnas
            dgvTablaSimplex.Columns.Add(nombre, nombre)
        Next
        dgvTablaSimplex.Columns.Add("RHS", "Resultado")

        ' Agregar filas numéricas una por una
        For Each filaDatos In filasTabla
            Dim index = dgvTablaSimplex.Rows.Add()
            For j = 0 To filaDatos.Count - 1
                dgvTablaSimplex.Rows(index).Cells(j).Value = filaDatos(j)
            Next
        Next

        MessageBox.Show("Preprocesamiento completo.")
    End Sub
End Class