
Imports iTextSharp.text
Imports System.Drawing
Imports iTextSharp.text.pdf
Imports System.IO
Public Class Form1
    ' Variables globales
    Private AllIteraciones As DataSet = New DataSet()
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        numVariables.Minimum = 2
        numVariables.Maximum = 10
        numRestricciones.Minimum = 1
        numRestricciones.Maximum = 10
        cmbTipoObjetivo.SelectedIndex = 0
    End Sub

    Private Sub btnGenerarModelo_Click(sender As Object, e As EventArgs) Handles btnGenerarModelo.Click
        Dim numVar As Integer = numVariables.Value
        Dim numRes As Integer = numRestricciones.Value
        Dim tabla As DataTable = CrearTabla(numVar, numRes)

        dgvRestricciones.DataSource = tabla

        ConfigurarComboBoxOperador(dgvRestricciones, "Operador")

        dgvRestricciones.AllowUserToAddRows = False

        GenerarFuncionObjetivo(numVar)

    End Sub
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
                .Maximum = 10
                .DecimalPlaces = 2
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

    Private Sub btnResolver_Click(sender As Object, e As EventArgs) Handles btnResolver.Click
        Dim numVar As Integer = numVariables.Value
        Dim numRes As Integer = numRestricciones.Value
        Dim feZ As List(Of Double) = FormaEstandarZ(numVar, numRes)
        Dim iteracion1 As DataTable = PrimerIteracion(feZ, numVar, numRes)

        Dim resultado As DataSet = ResolverSimplex(iteracion1)

        Dim tablaCombinada As DataTable = CombinarIteraciones(resultado)

        dgvTablaSimplex.DataSource = resultado.Tables(resultado.Tables.Count - 1)


        With dgvTablaSimplex
            .AllowUserToAddRows = False
            .AllowUserToDeleteRows = False
            .AllowUserToResizeColumns = False
            .AllowUserToOrderColumns = False
            .ReadOnly = True
        End With
    End Sub
    Private Function ResolverSimplex(iteracion1 As DataTable) As DataSet
        AllIteraciones.Clear()

        Dim tablaActual As DataTable = iteracion1.Copy()
        tablaActual.TableName = "Iteracion_0"
        AllIteraciones.Tables.Add(tablaActual.Copy())

        Dim iteracion As Integer = 1
        Dim maxIteraciones As Integer = 20

        Do While iteracion <= maxIteraciones
            Dim nuevaTabla As DataTable = SiguienteIteracion(tablaActual, iteracion)

            If nuevaTabla Is tablaActual Then
                Exit Do
            End If

            iteracion += 1
        Loop

        Return AllIteraciones
    End Function
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
    Public Function SiguienteIteracion(tablaAnterior As DataTable, iteracionNumero As Integer) As DataTable
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

        ' Agregar la nueva tabla al DataSet
        If AllIteraciones.Tables.Contains(nuevaTabla.TableName) Then
            AllIteraciones.Tables.Remove(nuevaTabla.TableName)
        End If
        AllIteraciones.Tables.Add(nuevaTabla.Copy())

        Return nuevaTabla
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

        Return tabla
    End Function

    Private Function CrearTablasHolgura(numVar As Integer, numRes As Integer) As DataTable

    End Function
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

    Private Sub btnGraficar_Click(sender As Object, e As EventArgs) Handles btnGraficar.Click
        If numVariables.Value <> 2 Then
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
        For i As Integer = 0 To numRestricciones.Value - 1
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

    Private Sub btnPreprocesar()
        Dim numVars As Integer = numVariables.Value
        Dim numRes As Integer = numRestricciones.Value
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

    Private Function FormaEstandarZ(numVars, numRes) As List(Of Double)
        Dim funcionObjetivo As List(Of Double) = ObtenerFuncionObjetivo(numVars)
        If funcionObjetivo.Count = 0 OrElse funcionObjetivo.All(Function(c) c = 0) Then
            MessageBox.Show("Debes llenar los coeficientes de la función objetivo.", "Falta F.O.", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return Nothing
        End If

        funcionObjetivo = funcionObjetivo.Select(Function(x) -x).ToList()

        Return funcionObjetivo
    End Function

End Class