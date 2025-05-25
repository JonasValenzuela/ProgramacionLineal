
Imports iTextSharp.text
Imports System.Drawing.Font
Imports iTextSharp.text.pdf
Imports System.IO
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging

Public Class Form1
    ' Variables globales
    Private AllIteraciones As DataSet = New DataSet()
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        numVariables.Minimum = 2
        numVariables.Maximum = 10
        numRestricciones.Minimum = 1
        numRestricciones.Maximum = 10
        cmbTipoObjetivo.SelectedIndex = 0
        btnGraficar.Enabled = False
        btnResolver.Enabled = False
    End Sub

#Region "Botones"
    Private Sub btnGenerarModelo_Click(sender As Object, e As EventArgs) Handles btnGenerarModelo.Click
        Dim numVar As Integer = numVariables.Value
        Dim numRes As Integer = numRestricciones.Value
        Dim tabla As DataTable = CrearTabla(numVar, numRes)

        dgvRestricciones.DataSource = tabla

        ConfigurarComboBoxOperador(dgvRestricciones, "Operador")

        dgvRestricciones.AllowUserToAddRows = False

        GenerarFuncionObjetivo(numVar)

    End Sub
    Private Sub btnResolver_Click(sender As Object, e As EventArgs) Handles btnResolver.Click
        Dim numVar As Integer = numVariables.Value
        Dim numRes As Integer = numRestricciones.Value
        Dim feZ As List(Of Double) = FormaEstandarZ(numVar, numRes)
        Dim iteracion1 As DataTable = PrimerIteracion(feZ, numVar, numRes)

        Dim resultado As DataSet = ResolverSimplex(iteracion1, feZ)

        Dim tablaFinal As DataTable = CombinarIteraciones(resultado)

        'Dim solucion = ObtenerSolucionOptima(tablaFinal)

        dgvTablaSimplex.DataSource = tablaFinal


        With dgvTablaSimplex
            .AllowUserToAddRows = False
            .AllowUserToDeleteRows = False
            .AllowUserToResizeColumns = False
            .AllowUserToOrderColumns = False
            .ReadOnly = True
        End With

        If numVar = 2 Then
            btnGraficar.Enabled = True
        Else
            btnGraficar.Enabled = False
        End If
    End Sub
    Private Sub btnExportarPDF_Click(sender As Object, e As EventArgs) Handles btnExportarPDF.Click
        Try
            Dim saveDialog As New SaveFileDialog()
            saveDialog.Filter = "PDF files (*.pdf)|*.pdf"
            saveDialog.Title = "Guardar reporte como PDF"
            saveDialog.FileName = "Reporte_Programacion_Lineal.pdf"

            If saveDialog.ShowDialog() = DialogResult.OK Then
                ExportarAPDF(saveDialog.FileName)
                MessageBox.Show("PDF generado exitosamente", "Exportar PDF", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If

        Catch ex As Exception
            MessageBox.Show($"Error al generar PDF: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub btnGraficar_Click(sender As Object, e As EventArgs) Handles btnGraficar.Click
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

        ' Fuente para números y etiquetas
        Dim fuente As New Drawing.Font("Arial", 9)
        Dim fuenteEtiquetas As New Drawing.Font("Arial", 12, FontStyle.Bold)

        ' Dibujar números en eje X
        For i As Integer = 0 To 25 ' 0 a 25 (ajusta según necesidad)
            If i Mod 5 = 0 OrElse i <= 10 Then ' Mostrar cada 5 números o los primeros 10
                Dim x As Integer = origenX + i * escalaX
                If x < ancho - 30 Then
                    ' Línea de marca
                    g.DrawLine(Pens.Gray, x, origenY - 3, x, origenY + 3)
                    ' Número
                    g.DrawString(i.ToString(), fuente, Brushes.Black, x - 8, origenY + 8)
                End If
            End If
        Next

        ' Dibujar números en eje Y
        For i As Integer = 0 To 25 ' 0 a 25 (ajusta según necesidad)
            If i Mod 5 = 0 OrElse i <= 10 Then ' Mostrar cada 5 números o los primeros 10
                Dim y As Integer = origenY - i * escalaY
                If y > 30 Then
                    ' Línea de marca
                    g.DrawLine(Pens.Gray, origenX - 3, y, origenX + 3, y)
                    ' Número
                    g.DrawString(i.ToString(), fuente, Brushes.Black, origenX - 25, y - 8)
                End If
            End If
        Next

        ' Dibujar líneas de cuadrícula (opcional)
        Dim lapizCuadricula As New Pen(Drawing.Color.LightGray, 1)

        ' Cuadrícula vertical
        For i As Integer = 5 To 25 Step 5
            Dim x As Integer = origenX + i * escalaX
            If x < ancho - 30 Then
                g.DrawLine(lapizCuadricula, x, 0, x, origenY)
            End If
        Next

        ' Cuadrícula horizontal
        For i As Integer = 5 To 25 Step 5
            Dim y As Integer = origenY - i * escalaY
            If y > 30 Then
                g.DrawLine(lapizCuadricula, origenX, y, ancho, y)
            End If
        Next

        ' Colores para restricciones
        Dim colores() As Drawing.Color = {Drawing.Color.Red, Drawing.Color.Blue, Drawing.Color.Green, Drawing.Color.Cyan, Drawing.Color.Orange}

        ' Graficar restricciones desde la fila 0
        For i As Integer = 0 To dgvRestricciones.Rows.Count - 1
            Try
                Dim fila As DataGridViewRow = dgvRestricciones.Rows(i)

                ' Saltar filas vacías o nuevas
                If fila.IsNewRow Then Continue For

                ' Obtener coeficientes usando los nombres de columnas correctos
                Dim a As Double = 0
                Dim b As Double = 0
                Dim c As Double = 0
                Dim operador As String = ""

                ' Verificar que las celdas no estén vacías
                If fila.Cells("x1").Value IsNot Nothing Then
                    Double.TryParse(fila.Cells("x1").Value.ToString(), a)
                End If

                If fila.Cells("x2").Value IsNot Nothing Then
                    Double.TryParse(fila.Cells("x2").Value.ToString(), b)
                End If

                If fila.Cells("Solucion").Value IsNot Nothing Then
                    Double.TryParse(fila.Cells("Solucion").Value.ToString(), c)
                End If

                If fila.Cells("Operador").Value IsNot Nothing Then
                    operador = fila.Cells("Operador").Value.ToString()
                End If

                ' Evitar restricciones inválidas
                If a = 0 AndAlso b = 0 Then Continue For

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

                ' Dibujar línea de restricción
                Dim lapiz As New Pen(colores(i Mod colores.Length), 2)
                g.DrawLine(lapiz, CSng(px1), CSng(py1), CSng(px2), CSng(py2))

                ' Agregar etiqueta de restricción
                Dim textoRestriccion As String = $"R{i + 1}: {a}X1 + {b}X2 {operador} {c}"
                g.DrawString(textoRestriccion, fuente, New SolidBrush(colores(i Mod colores.Length)), 10, 20 + (i * 15))

                ' Sombrear región factible (opcional)
                If operador = "<=" Then
                    SombrearRegion(g, a, b, c, origenX, origenY, escalaX, escalaY, ancho, alto, colores(i Mod colores.Length))
                End If

            Catch ex As Exception
                MessageBox.Show($"Error al graficar la restricción {i + 1}: {ex.Message}")
            End Try
        Next

        ' Graficar función objetivo si está disponible
        Try
            Dim valoresZ As List(Of Double) = ObtenerValoresFuncionObjetivo(PanelZ)
            If valoresZ.Count >= 2 Then
                GraficarFuncionObjetivo(g, valoresZ(0), valoresZ(1), origenX, origenY, escalaX, escalaY, ancho, alto)
            End If
        Catch ex As Exception
            ' No hacer nada si hay error con la función objetivo
        End Try
    End Sub
#End Region

#Region "Generar y obtener de datos Z"
    ' Función para obtener los valores de los NumericUpDown
    Public Function ObtenerValoresFuncionObjetivo(panel As Panel) As List(Of Double)
        Dim valores As New List(Of Double)()

        ' Buscar todos los NumericUpDown que empiecen con "nudX"
        For Each ctrl As Control In panel.Controls
            If TypeOf ctrl Is NumericUpDown AndAlso ctrl.Name.StartsWith("nudX") Then
                Dim nud As NumericUpDown = DirectCast(ctrl, NumericUpDown)
                valores.Add(Convert.ToDouble(nud.Value))
            End If
        Next

        Return valores
    End Function
    'Funcion para generar los textbox de Z
    Private Function GenerarFuncionObjetivo(numVar As Integer)
        ' Limpiar controles existentes
        PanelZ.Controls.Clear()

        ' Configuración de posicionamiento
        Dim xInicial As Integer = 10
        Dim yPosicion As Integer = 15
        Dim espacioEntre As Integer = 110

        'Crear Label inicial "Z = "
        Dim lblZ As New Label()
        With lblZ
            .Text = "Z = "
            .Font = New Drawing.Font("Arial", 10, FontStyle.Bold)
            .Location = New Point(xInicial, yPosicion + 2)
            .Size = New Size(30, 20)
            .TextAlign = ContentAlignment.MiddleLeft
        End With
        PanelZ.Controls.Add(lblZ)

        ' Posición inicial para los controles
        Dim xActual As Integer = xInicial + 35

        ' Crear Labels y TextBoxes para cada variable
        For i As Integer = 1 To numVar
            ' Crear Label para Xi
            Dim lblVariable As New Label()
            With lblVariable
                .Text = $"X{i} = "
                .Font = New Drawing.Font("Arial", 9)
                .Location = New Point(xActual, yPosicion)
                .Size = New Size(35, 20)
                .TextAlign = ContentAlignment.MiddleLeft
            End With
            PanelZ.Controls.Add(lblVariable)

            ' Crear TextBox
            Dim nudVariable As New NumericUpDown()
            With nudVariable
                .Name = $"nudX{i}"
                .Location = New Point(xActual + 35, yPosicion - 2)
                .Size = New Size(60, 20)
                .Minimum = 0
                .Value = 0
            End With
            PanelZ.Controls.Add(nudVariable)

            ' Actualizar posición para el siguiente grupo
            xActual += espacioEntre

            ' Si se sale del panel, crear nueva fila
            If xActual + espacioEntre > PanelZ.Width Then
                xActual = xInicial + 35
                yPosicion += 35
            End If
        Next

        ' Ajustar altura del panel si es necesario
        If yPosicion + 30 > PanelZ.Height Then
            PanelZ.Height = yPosicion + 30
        End If
    End Function

    'Funcion para obtener la funcion objetivo Z
    Private Function ObtenerFuncionObjetivo(numVars As Integer) As List(Of Double)
        Dim obj As New List(Of Double)
        For i As Integer = 1 To numVars
            Dim nudControl As NumericUpDown = CType(PanelZ.Controls("nudX" & i), NumericUpDown)
            Dim val As Double = Convert.ToDouble(nudControl.Value)
            obj.Add(val)
        Next
        Return obj
    End Function
#End Region

#Region "Funciones para exportar"
    Private Sub ExportarAPDF(rutaArchivo As String)
        Dim documento As New Document(PageSize.A4, 20, 20, 20, 20)
        Dim writer As PdfWriter = PdfWriter.GetInstance(documento, New FileStream(rutaArchivo, FileMode.Create))

        documento.Open()

        ' Fuentes usando iTextSharp
        Dim fuenteTitulo As iTextSharp.text.Font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16)
        Dim fuenteSubtitulo As iTextSharp.text.Font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)
        Dim fuenteNormal As iTextSharp.text.Font = FontFactory.GetFont(FontFactory.HELVETICA, 10)

        ' 1. TÍTULO Y INFORMACIÓN GENERAL
        documento.Add(New Paragraph("REPORTE DE PROGRAMACIÓN LINEAL", fuenteTitulo))
        documento.Add(New Paragraph(" "))

        ' Información del problema
        Dim infoProblema As String = $"Número de Variables: {numVariables.Value}" & vbCrLf &
                               $"Número de Restricciones: {numRestricciones.Value}" & vbCrLf &
                               "Tipo: Maximizar"
        documento.Add(New Paragraph(infoProblema, fuenteNormal))
        documento.Add(New Paragraph(" "))

        ' 2. FUNCIÓN OBJETIVO
        documento.Add(New Paragraph("FUNCIÓN OBJETIVO", fuenteSubtitulo))

        Try
            Dim valoresZ As List(Of Double) = ObtenerValoresFuncionObjetivo(PanelZ)
            Dim funcionObjetivo As String = "Z = "
            For i As Integer = 0 To valoresZ.Count - 1
                If i > 0 AndAlso valoresZ(i) >= 0 Then funcionObjetivo += " + "
                funcionObjetivo += $"{valoresZ(i)}X{i + 1}"
            Next
            documento.Add(New Paragraph(funcionObjetivo, fuenteNormal))
        Catch
            documento.Add(New Paragraph("Z = (No definida)", fuenteNormal))
        End Try

        documento.Add(New Paragraph(" "))

        ' 3. RESTRICCIONES
        documento.Add(New Paragraph("RESTRICCIONES", fuenteSubtitulo))
        AgregarTablaDataGridView(documento, dgvRestricciones, "Tabla de Restricciones")
        documento.Add(New Paragraph(" "))

        ' 4. TABLA SIMPLEX
        documento.Add(New Paragraph("ITERACIONES DEL MÉTODO SIMPLEX", fuenteSubtitulo))
        AgregarTablaDataGridView(documento, dgvTablaSimplex, "Tabla Simplex")
        documento.Add(New Paragraph(" "))

        ' 5. GRÁFICA (solo si hay 2 variables)
        If numVariables.Value = 2 Then
            documento.Add(New Paragraph("REPRESENTACIÓN GRÁFICA", fuenteSubtitulo))
            AgregarGraficaPDF(documento)
        End If

        documento.Close()
    End Sub
    Private Sub AgregarTablaDataGridView(documento As Document, dgv As DataGridView, titulo As String)
        Dim tabla As New PdfPTable(dgv.ColumnCount)
        tabla.WidthPercentage = 100

        ' Encabezados
        For Each columna As DataGridViewColumn In dgv.Columns
            Dim celda As New PdfPCell(New Phrase(columna.HeaderText, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9)))
            celda.HorizontalAlignment = Element.ALIGN_CENTER
            celda.BackgroundColor = New CMYKColor(0, 0, 0, 0.2F)
            tabla.AddCell(celda)
        Next

        ' Datos
        For Each fila As DataGridViewRow In dgv.Rows
            If Not fila.IsNewRow Then
                For Each celda As DataGridViewCell In fila.Cells
                    Dim valor As String = If(celda.Value?.ToString(), "")
                    Dim celdaPDF As New PdfPCell(New Phrase(valor, FontFactory.GetFont(FontFactory.HELVETICA, 8)))
                    celdaPDF.HorizontalAlignment = Element.ALIGN_CENTER
                    tabla.AddCell(celdaPDF)
                Next
            End If
        Next

        documento.Add(tabla)
    End Sub
    Private Sub AgregarGraficaPDF(documento As Document)
        Try
            ' Crear una imagen temporal de la gráfica
            Dim bitmap As New Bitmap(600, 400)
            Dim g As Graphics = Graphics.FromImage(bitmap)

            ' Redibujar la gráfica en el bitmap
            DibujarGraficaEnGraphics(g, bitmap.Width, bitmap.Height)

            ' Guardar temporalmente
            Dim rutaTemp As String = Path.GetTempFileName() & ".png"
            bitmap.Save(rutaTemp, ImageFormat.Png)

            ' Agregar imagen al PDF
            Dim imagen As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(rutaTemp)
            imagen.ScaleToFit(400, 300)
            imagen.Alignment = Element.ALIGN_CENTER
            documento.Add(imagen)

            ' Limpiar
            g.Dispose()
            bitmap.Dispose()
            File.Delete(rutaTemp)

        Catch ex As Exception
            documento.Add(New Paragraph("Error al generar gráfica", FontFactory.GetFont(FontFactory.HELVETICA, 10)))
        End Try
    End Sub
#End Region

#Region "Generacion de tablas"
    'Funcion para crear la tabla de operaciones
    Private Function CrearTabla(numVar As Integer, numRes As Integer) As DataTable
        Dim tabla As New DataTable()

        ' Crear columnas para las variables (x1, x2, x3, etc.)
        For i As Integer = 1 To numVar
            tabla.Columns.Add($"X{i}", GetType(Double))
        Next

        ' Crear columna para el operador (ComboBox)
        Dim columnaOperador As New DataColumn("Operador", GetType(String))
        tabla.Columns.Add(columnaOperador)

        ' Crear columna para la solución
        tabla.Columns.Add("Solucion", GetType(Double))

        AgregarFilasVacias(tabla, numRes)

        Return tabla
    End Function
    'Funcion para agregar una fila
    Public Sub AgregarFilaVacia(tabla As DataTable)
        tabla.Rows.Add(tabla.NewRow())
    End Sub
    'Funcion para agregar filas segun el numero de variables
    Public Sub AgregarFilasVacias(tabla As DataTable, numeroFilas As Integer)
        For i As Integer = 1 To numeroFilas
            tabla.Rows.Add(tabla.NewRow())
        Next
    End Sub
    Public Function CombinarIteraciones(ds As DataSet) As DataTable
        If ds.Tables.Count = 0 Then
            Return New DataTable()
        End If

        ' Crear nueva tabla con columnas de tipo String para permitir texto y números
        Dim tablaCombinada As New DataTable("TodasIteraciones")

        ' Copiar estructura de columnas pero como String
        For Each columna As DataColumn In ds.Tables(0).Columns
            tablaCombinada.Columns.Add(columna.ColumnName, GetType(String))
        Next

        ' Recorrer todas las tablas del DataSet
        For i As Integer = 0 To ds.Tables.Count - 1
            Dim tablaActual As DataTable = ds.Tables(i)

            ' Agregar encabezado de iteración
            Dim filaEncabezado As DataRow = tablaCombinada.NewRow()
            filaEncabezado(0) = $"--- {tablaActual.TableName} ---"
            ' Dejar las demás columnas vacías
            For j As Integer = 1 To tablaCombinada.Columns.Count - 1
                filaEncabezado(j) = ""
            Next
            tablaCombinada.Rows.Add(filaEncabezado)

            ' Copiar todas las filas de la tabla actual (convertir a String)
            For Each fila As DataRow In tablaActual.Rows
                Dim nuevaFila As DataRow = tablaCombinada.NewRow()
                For j As Integer = 0 To tablaActual.Columns.Count - 1
                    If fila(j) IsNot DBNull.Value Then
                        nuevaFila(j) = fila(j).ToString()
                    Else
                        nuevaFila(j) = ""
                    End If
                Next
                tablaCombinada.Rows.Add(nuevaFila)
            Next

            ' Agregar 2 filas vacías entre tablas (excepto después de la última)
            If i < ds.Tables.Count - 1 Then
                For k As Integer = 1 To 2
                    Dim filaVacia As DataRow = tablaCombinada.NewRow()
                    For j As Integer = 0 To tablaCombinada.Columns.Count - 1
                        filaVacia(j) = ""
                    Next
                    tablaCombinada.Rows.Add(filaVacia)
                Next
            End If
        Next

        Return tablaCombinada
    End Function
#End Region

#Region "Configuracion DataGridView"
    ' Función auxiliar para configurar el ComboBox "Operador" en el DataGridView
    Private Sub ConfigurarComboBoxOperador(dgv As DataGridView, ColumnaOperador As String)
        ' Buscar el índice de la columna operador
        Dim indiceColumna As Integer = -1
        For i As Integer = 0 To dgv.Columns.Count - 1
            If dgv.Columns(i).Name = ColumnaOperador Then
                indiceColumna = i
                Exit For
            End If
        Next

        If indiceColumna >= 0 Then
            ' Convertir la columna a ComboBox
            Dim comboColumn As New DataGridViewComboBoxColumn()
            comboColumn.Name = ColumnaOperador
            comboColumn.HeaderText = "Operador"
            comboColumn.DataPropertyName = ColumnaOperador

            ' Agregar las opciones
            comboColumn.Items.Add("=")
            comboColumn.Items.Add("<=")
            comboColumn.Items.Add(">=")

            ' Valor por defecto
            comboColumn.DefaultCellStyle.NullValue = "="

            ' Reemplazar la columna existente
            dgv.Columns.RemoveAt(indiceColumna)
            dgv.Columns.Insert(indiceColumna, comboColumn)
        End If
    End Sub

#End Region

#Region "Metodo Simplex"
    Private Function ResolverSimplex(iteracion1 As DataTable, coeficientesZ As List(Of Double)) As DataSet
        AllIteraciones.Clear()

        Dim tablaActual As DataTable = iteracion1.Copy()
        tablaActual.TableName = "Iteracion_1"
        AllIteraciones.Tables.Add(tablaActual.Copy())

        Dim iteracion As Integer = 2
        Dim maxIteraciones As Integer = 100

        Do While iteracion <= maxIteraciones
            Dim nuevaTabla As DataTable = SiguienteIteracion(tablaActual, iteracion, coeficientesZ)

            If nuevaTabla Is tablaActual Then
                Exit Do
            End If

            tablaActual = nuevaTabla
            iteracion += 1
        Loop

        Return AllIteraciones
    End Function
    Public Function SiguienteIteracion(tablaAnterior As DataTable, iteracionNumero As Integer, coeficientesOriginalesZ As List(Of Double)) As DataTable
        ' Buscar columna entrante (más negativo en fila Z)
        Dim primeraFila As List(Of Double) = New List(Of Double)()
        For j As Integer = 0 To tablaAnterior.Columns.Count - 3 ' Excluir operador y solución
            primeraFila.Add(Convert.ToDouble(tablaAnterior.Rows(0)(j)))
        Next

        Dim columnaEntrante As Integer = ObtenerMasNegativo(primeraFila)

        ' Si no hay valores negativos, ya llegamos al óptimo
        If primeraFila.Min() >= 0 Then
            Return tablaAnterior ' Retornar la misma tabla (solución óptima)
        End If

        ' Encontrar pivote
        Dim pivote = BuscarPivote(tablaAnterior, columnaEntrante)

        If pivote.fila = -1 Then
            Return tablaAnterior ' No hay pivote válido (solución no acotada)
        End If

        ' Crear nueva tabla con la misma estructura
        Dim nuevaTabla As DataTable = tablaAnterior.Clone()
        nuevaTabla.TableName = $"Iteracion_{iteracionNumero}"

        ' Copiar todos los datos de la tabla anterior
        For i As Integer = 0 To tablaAnterior.Rows.Count - 1
            Dim nuevaFila As DataRow = nuevaTabla.NewRow()
            For j As Integer = 0 To tablaAnterior.Columns.Count - 1
                nuevaFila(j) = tablaAnterior.Rows(i)(j)
            Next
            nuevaTabla.Rows.Add(nuevaFila)
        Next

        ' Realizar operaciones de Gauss-Jordan
        '' 1. Normalizar fila pivote (dividir toda la fila por el valor del pivote)
        For j As Integer = 0 To nuevaTabla.Columns.Count - 3 ' Excluir operador y solución
            If nuevaTabla.Rows(pivote.fila)(j) IsNot DBNull.Value Then
                Dim valorActual As Double = Convert.ToDouble(nuevaTabla.Rows(pivote.fila)(j))
                nuevaTabla.Rows(pivote.fila)(j) = valorActual / pivote.valor
            End If
        Next

        ' La columna de solución también se divide por el pivote
        If nuevaTabla.Rows(pivote.fila)(nuevaTabla.Columns.Count - 1) IsNot DBNull.Value Then
            Dim solucionActual As Double = Convert.ToDouble(nuevaTabla.Rows(pivote.fila)(nuevaTabla.Columns.Count - 1))
            nuevaTabla.Rows(pivote.fila)(nuevaTabla.Columns.Count - 1) = solucionActual / pivote.valor
        End If

        ' 2. Hacer ceros en toda la columna entrante (excepto en la fila pivote)
        For i As Integer = 0 To nuevaTabla.Rows.Count - 1
            If i <> pivote.fila Then
                If nuevaTabla.Rows(i)(columnaEntrante) IsNot DBNull.Value Then
                    Dim factor As Double = Convert.ToDouble(nuevaTabla.Rows(i)(columnaEntrante))

                    ' Solo operar en columnas numéricas (excluir operador)
                    For j As Integer = 0 To nuevaTabla.Columns.Count - 3
                        If nuevaTabla.Rows(i)(j) IsNot DBNull.Value AndAlso nuevaTabla.Rows(pivote.fila)(j) IsNot DBNull.Value Then
                            Dim valorActual As Double = Convert.ToDouble(nuevaTabla.Rows(i)(j))
                            Dim valorPivoteFila As Double = Convert.ToDouble(nuevaTabla.Rows(pivote.fila)(j))
                            nuevaTabla.Rows(i)(j) = valorActual - (factor * valorPivoteFila)
                        End If
                    Next

                    ' También operar en la columna de solución
                    If nuevaTabla.Rows(i)(nuevaTabla.Columns.Count - 1) IsNot DBNull.Value AndAlso
                        nuevaTabla.Rows(pivote.fila)(nuevaTabla.Columns.Count - 1) IsNot DBNull.Value Then
                        Dim valorSolucionActual As Double = Convert.ToDouble(nuevaTabla.Rows(i)(nuevaTabla.Columns.Count - 1))
                        Dim valorSolucionPivote As Double = Convert.ToDouble(nuevaTabla.Rows(pivote.fila)(nuevaTabla.Columns.Count - 1))
                        nuevaTabla.Rows(i)(nuevaTabla.Columns.Count - 1) = valorSolucionActual - (factor * valorSolucionPivote)
                    End If
                End If
            End If
        Next

        ' *** AGREGAR: Calcular y poner el valor de Z en la columna solución ***
        Dim valorZ As Double = CalcularValorZ(nuevaTabla, coeficientesOriginalesZ)
        nuevaTabla.Rows(0)(nuevaTabla.Columns.Count - 1) = valorZ

        ' Agregar la nueva tabla al DataSet
        If AllIteraciones.Tables.Contains(nuevaTabla.TableName) Then
            AllIteraciones.Tables.Remove(nuevaTabla.TableName)
        End If
        AllIteraciones.Tables.Add(nuevaTabla.Copy())

        Return nuevaTabla
    End Function
    ' Función corregida para calcular el valor actual de Z
    Private Function CalcularValorZ(tabla As DataTable, coeficientesOriginalesZ As List(Of Double)) As Double
        Dim valorZ As Double = 0

        ' Para cada variable original (X1, X2, X3, etc.)
        For j As Integer = 0 To Math.Min(coeficientesOriginalesZ.Count - 1, tabla.Columns.Count - 3)
            Dim valorVariable As Double = 0 ' Por defecto las variables no básicas = 0

            ' Verificar si es variable básica
            Dim esBasica As Boolean = True
            Dim filaConUno As Integer = -1

            For i As Integer = 1 To tabla.Rows.Count - 1 ' Empezar desde fila 1 (saltar fila Z)
                If tabla.Rows(i)(j) IsNot DBNull.Value Then
                    Dim valor As Double = Convert.ToDouble(tabla.Rows(i)(j))

                    If Math.Abs(valor - 1.0) < 0.0001 AndAlso filaConUno = -1 Then
                        filaConUno = i
                    ElseIf Math.Abs(valor) > 0.0001 Then
                        esBasica = False
                        Exit For
                    End If
                End If
            Next

            ' Si es básica, tomar su valor de la columna solución
            If esBasica AndAlso filaConUno > 0 Then
                valorVariable = Convert.ToDouble(tabla.Rows(filaConUno)(tabla.Columns.Count - 1))
            End If
            ' Si no es básica, valorVariable = 0 (ya está inicializado)

            ' Z = suma de (coeficiente original * valor variable)
            ' Como feZ son negativos, usar el valor absoluto
            valorZ += Math.Abs(coeficientesOriginalesZ(j)) * valorVariable
        Next

        Return valorZ
    End Function
    Private Function BuscarPivote(tabla As DataTable, colEntrante As Integer) As (fila As Integer, columna As Integer, valor As Double)
        Dim filaPivote As Integer = -1
        Dim menorRazon As Double = Double.MaxValue

        For i As Integer = 1 To tabla.Rows.Count - 1
            Dim elemento As Double = Convert.ToDouble(tabla.Rows(i)(colEntrante))
            Dim solucion As Double = Convert.ToDouble(tabla.Rows(i)(tabla.Columns.Count - 1))

            ' Solo considerar elementos positivos para evitar división por cero o negativos
            If elemento > 0 Then
                Dim razon As Double = solucion / elemento

                ' Encontrar la menor razón positiva
                If razon >= 0 AndAlso razon < menorRazon Then
                    menorRazon = razon
                    filaPivote = i
                End If
            End If
        Next

        If filaPivote <> -1 Then
            Dim valorPivote As Double = Convert.ToDouble(tabla.Rows(filaPivote)(colEntrante))
            Return (filaPivote, colEntrante, valorPivote)
        Else
            Return (-1, -1, 0) ' No hay pivote válido
        End If
    End Function
    Public Function ObtenerMasNegativo(lista As List(Of Double)) As Double
        Dim valorMasNegativo As Double = lista.Min()
        Return lista.IndexOf(valorMasNegativo)
    End Function
    Private Function PrimerIteracion(feZ As List(Of Double), numVar As Integer, numRes As Integer) As DataTable
        Dim tabla As DataTable = CrearTabla((numVar * 2), numRes + 1) ' +1 para incluir la fila Z

        ' PRIMERA FILA: Coeficientes de Z (negativos)
        For j As Integer = 0 To feZ.Count - 1
            tabla.Rows(0)(j) = feZ(j)
        Next

        ' Completar primera fila con ceros para variables de holgura
        For j As Integer = numVar To (numVar * 2) - 1
            tabla.Rows(0)(j) = 0
        Next

        ' FILAS DE RESTRICCIONES: Copiar del dgvRestricciones
        Dim filaTabla As Integer = 1 ' Empezar desde la fila 1 (fila 0 es para Z)

        For i As Integer = 0 To dgvRestricciones.Rows.Count - 1
            If Not dgvRestricciones.Rows(i).IsNewRow AndAlso filaTabla < tabla.Rows.Count Then

                ' Copiar coeficientes de variables originales
                For j As Integer = 0 To numVar - 1
                    tabla.Rows(filaTabla)(j) = dgvRestricciones.Rows(i).Cells(j).Value
                Next

                ' Agregar variables de holgura (identidad)
                For j As Integer = numVar To (numVar * 2) - 1
                    If j - numVar = (filaTabla - 1) Then
                        tabla.Rows(filaTabla)(j) = 1 ' Diagonal de la matriz identidad
                    Else
                        tabla.Rows(filaTabla)(j) = 0
                    End If
                Next

                ' Copiar operador y solución
                tabla.Rows(filaTabla)(tabla.Columns.Count - 2) = dgvRestricciones.Rows(i).Cells(numVar).Value ' Operador
                tabla.Rows(filaTabla)(tabla.Columns.Count - 1) = dgvRestricciones.Rows(i).Cells(numVar + 1).Value ' Solución

                filaTabla += 1
            End If
        Next

        ' Al final, antes del Return:
        tabla.Rows(0)(tabla.Columns.Count - 1) = 0 ' Z inicial = 0

        Return tabla
    End Function
    Private Function FormaEstandarZ(numVars, numRes) As List(Of Double)
        Dim funcionObjetivo As List(Of Double) = ObtenerFuncionObjetivo(numVars)
        If funcionObjetivo.Count = 0 OrElse funcionObjetivo.All(Function(c) c = 0) Then
            MessageBox.Show("Debes llenar los coeficientes de la función objetivo.", "Falta F.O.", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return Nothing
        End If

        funcionObjetivo = funcionObjetivo.Select(Function(x) -x).ToList()

        Return funcionObjetivo
    End Function
#End Region

#Region "Graficacion"
    Private Sub DibujarGraficaEnGraphics(g As Graphics, ancho As Integer, alto As Integer)
        g.Clear(System.Drawing.Color.White)
        Dim escalaX As Double = 15
        Dim escalaY As Double = 15
        Dim origenX As Integer = 60
        Dim origenY As Integer = alto - 60

        ' Dibujar ejes
        Dim lapizEjes As New System.Drawing.Pen(System.Drawing.Color.Black, 2)
        g.DrawLine(lapizEjes, origenX, 0, origenX, alto)
        g.DrawLine(lapizEjes, 0, origenY, ancho, origenY)

        ' Números en ejes
        Dim fuente As New System.Drawing.Font("Arial", 10)
        For i As Integer = 0 To 20 Step 5
            Dim x As Integer = origenX + i * escalaX
            Dim y As Integer = origenY - i * escalaY
            If x < ancho - 30 Then
                g.DrawString(i.ToString(), fuente, System.Drawing.Brushes.Black, CSng(x - 8), CSng(origenY + 5))
            End If
            If y > 30 Then
                g.DrawString(i.ToString(), fuente, System.Drawing.Brushes.Black, CSng(origenX - 25), CSng(y - 8))
            End If
        Next

        ' Etiquetas
        Dim fuenteBold As New System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold)
        g.DrawString("X1", fuenteBold, System.Drawing.Brushes.Black, CSng(ancho - 30), CSng(origenY + 5))
        g.DrawString("X2", fuenteBold, System.Drawing.Brushes.Black, CSng(origenX - 30), 15)

        ' Restricciones
        Dim colores() As System.Drawing.Color = {
        System.Drawing.Color.Red,
        System.Drawing.Color.Blue,
        System.Drawing.Color.Green,
        System.Drawing.Color.Orange,
        System.Drawing.Color.Magenta
    }

        For i As Integer = 0 To dgvRestricciones.Rows.Count - 1
            Try
                Dim fila As DataGridViewRow = dgvRestricciones.Rows(i)
                If fila.IsNewRow Then Continue For

                Dim a, b, c As Double
                If fila.Cells("x1").Value IsNot Nothing Then Double.TryParse(fila.Cells("x1").Value.ToString(), a)
                If fila.Cells("x2").Value IsNot Nothing Then Double.TryParse(fila.Cells("x2").Value.ToString(), b)
                If fila.Cells("Solucion").Value IsNot Nothing Then Double.TryParse(fila.Cells("Solucion").Value.ToString(), c)

                If a = 0 AndAlso b = 0 Then Continue For

                Dim x1 As Double = 0
                Dim y1 As Double = If(b <> 0, c / b, 0)
                Dim x2 As Double = If(a <> 0, c / a, 0)
                Dim y2 As Double = 0

                Dim px1 = origenX + x1 * escalaX
                Dim py1 = origenY - y1 * escalaY
                Dim px2 = origenX + x2 * escalaX
                Dim py2 = origenY - y2 * escalaY

                Dim lapiz As New System.Drawing.Pen(colores(i Mod colores.Length), 3)
                g.DrawLine(lapiz, CSng(px1), CSng(py1), CSng(px2), CSng(py2))

                lapiz.Dispose()

            Catch ex As Exception
                ' Ignorar errores en restricciones individuales
            End Try
        Next

        lapizEjes.Dispose()
        fuente.Dispose()
        fuenteBold.Dispose()
    End Sub
    ' Función auxiliar para sombrear región factible
    Private Sub SombrearRegion(g As Graphics, a As Double, b As Double, c As Double,
                          origenX As Integer, origenY As Integer, escalaX As Double, escalaY As Double,
                          ancho As Integer, alto As Integer, color As Drawing.Color)
        ' Implementar sombreado de región factible (opcional)
        ' Esto es más complejo y depende de si quieres mostrar la región factible
    End Sub
    ' Función auxiliar para graficar función objetivo
    Private Sub GraficarFuncionObjetivo(g As Graphics, c1 As Double, c2 As Double,
                                   origenX As Integer, origenY As Integer, escalaX As Double, escalaY As Double,
                                   ancho As Integer, alto As Integer)
        ' Graficar líneas de nivel de la función objetivo
        Dim lapizObj As New Pen(Drawing.Color.Purple, 2)
        lapizObj.DashStyle = DashStyle.Dash  ' Establecer el estilo después de crear el Pen

        ' Para diferentes valores de Z, graficar c1*x1 + c2*x2 = Z
        For z As Double = 100 To 1000 Step 200
            If c1 <> 0 AndAlso c2 <> 0 Then
                Dim x1 As Double = 0
                Dim y1 As Double = z / c2
                Dim x2 As Double = z / c1
                Dim y2 As Double = 0

                Dim px1 = origenX + x1 * escalaX
                Dim py1 = origenY - y1 * escalaY
                Dim px2 = origenX + x2 * escalaX
                Dim py2 = origenY - y2 * escalaY

                g.DrawLine(lapizObj, CSng(px1), CSng(py1), CSng(px2), CSng(py2))
            End If
        Next
    End Sub
#End Region
End Class