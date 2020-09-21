Public Class ManageWebRequest
    Public _WebRequests As New List(Of har.Request)
    Private _SelectWebRequest As har.Request
    Public _ReplacementData As New SortedList(Of Int32, ReplacementData)

    Private ReplacementDataGD As New DataTable
    Sub LoadtheData()
        ReloadWebReuestGrid()
        ReplacementDataGD = New DataTable

        ReplacementDataGD.Columns.Add("DS1")
        ReplacementDataGD.Columns.Add("DS2")
        ReplacementDataGD.Columns.Add("DS3")
        ReplacementDataGD.Columns.Add("DS4")
        ReplacementDataGD.Columns.Add("DS5")
        ReplacementDataGD.Columns.Add("DS6")
        ReplacementDataGD.Columns.Add("DS7")
        ReplacementDataGD.Columns.Add("DS8")
        ReplacementDataGD.Columns.Add("DS9")
        ReplacementDataGD.Columns.Add("DS10")
        For Each _key In _ReplacementData.Keys
            ReplacementDataGD.Rows.Add({_ReplacementData(_key).DS1, _ReplacementData(_key).DS2, _ReplacementData(_key).DS3, _ReplacementData(_key).DS4,
                                       _ReplacementData(_key).DS5, _ReplacementData(_key).DS6, _ReplacementData(_key).DS7, _ReplacementData(_key).DS8,
                                       _ReplacementData(_key).DS9, _ReplacementData(_key).DS10})

        Next
        ReloadReplacementdataGrid()
    End Sub

    Private Sub ReloadReplacementdataGrid()
        DataGridView1.Columns.Clear()
        DataGridView1.DataSource = Nothing
        DataGridView1.AllowUserToOrderColumns = False
        DataGridView1.AutoGenerateColumns = True
        DataGridView1.DataSource = ReplacementDataGD

    End Sub


    Private Sub ReloadWebReuestGrid()
        lb_WebCalls.DataSource = Nothing
        lb_WebCalls.DataSource = _WebRequests
        lb_WebCalls.DisplayMember = "URL"
    End Sub
    Private Sub ReloadWebRequestHeaders()
        lb_Headers.DataSource = Nothing
        lb_Headers.Refresh()
        lb_Headers.DataSource = _SelectWebRequest.headers
        lb_Headers.DisplayMember = "theHeader"
    End Sub
    Private Sub ReloadWebRequestCookies()
        lb_cookies.DataSource = Nothing
        lb_cookies.Refresh()
        lb_cookies.DataSource = _SelectWebRequest.cookies
        lb_cookies.DisplayMember = "theCooky"
    End Sub

    Private Sub btnUpdateHeader_Click(sender As Object, e As EventArgs) Handles btnUpdateHeader.Click
        Dim _selectValue As har.Header = lb_Headers.SelectedValue
        If Not _selectValue Is Nothing Then
            _selectValue.name = txtHeadername.Text
            _selectValue.value = txtHeaderValue.Text

            ReloadWebRequestHeaders()
        End If
    End Sub

    Private Sub btnDeleteHeader_Click(sender As Object, e As EventArgs) Handles btnDeleteHeader.Click
        Dim _selectValue As har.Header = lb_Headers.SelectedValue
        If Not _selectValue Is Nothing Then
            _SelectWebRequest.headers.Remove(_selectValue)
            ReloadWebRequestHeaders()
        End If
    End Sub

    Private Sub lb_WebCalls_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lb_WebCalls.SelectedIndexChanged
        Debug.WriteLine("")
        _SelectWebRequest = lb_WebCalls.SelectedValue
        If Not _SelectWebRequest Is Nothing Then
            cb_Method.Text = _SelectWebRequest.method
            txtURL.Text = _SelectWebRequest.url
            ReloadWebRequestHeaders()
            ReloadWebRequestCookies()
        End If
    End Sub

    Private Sub lb_Headers_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lb_Headers.SelectedIndexChanged
        Dim _selectValue As har.Header = lb_Headers.SelectedValue
        If Not _selectValue Is Nothing Then
            txtHeadername.Text = _selectValue.name
            txtHeaderValue.Text = _selectValue.value

        End If
    End Sub

    Private Sub ManageWebRequest_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        _ReplacementData = New SortedList(Of Int32, ReplacementData)

        For x = 0 To ReplacementDataGD.Rows.Count - 1
            Dim _r = ReplacementDataGD.Rows(x)
            _ReplacementData.Add(x, New ReplacementData() With {.DS1 = _r(0).ToString, .DS2 = _r(1).ToString, .DS3 = _r(2).ToString, .DS4 = _r(3).ToString, .DS5 = _r(4).ToString, .DS6 = _r(5).ToString,
                                 .DS7 = _r(6).ToString, .DS8 = _r(7).ToString, .DS9 = _r(8).ToString, .DS10 = _r(9).ToString})
        Next


        Me.DialogResult = DialogResult.OK
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        _SelectWebRequest = lb_WebCalls.SelectedValue
        _WebRequests.Remove(_SelectWebRequest)
        ReloadWebReuestGrid()
    End Sub

    Private Sub lb_cookies_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lb_cookies.SelectedIndexChanged
        Dim _selectValue As har.Cooky = lb_cookies.SelectedValue
        If Not _selectValue Is Nothing Then
            txtCookiename.Text = _selectValue.name
            txtCookieValue.Text = _selectValue.value

        End If
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim _selectValue As har.Header = lb_cookies.SelectedValue
        If Not _selectValue Is Nothing Then
            _selectValue.name = txtCookiename.Text
            _selectValue.value = txtCookieValue.Text

            ReloadWebRequestCookies()
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim _selectValue As har.Cooky = lb_cookies.SelectedValue
        If Not _selectValue Is Nothing Then
            _SelectWebRequest.cookies.Remove(_selectValue)
            ReloadWebRequestCookies()
        End If
    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick

    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Dim _data_raw = Clipboard.GetText()
        If _data_raw.Length > 0 Then
            Dim _data = Split(_data_raw, vbCrLf)
            If Not DataGridView1.SelectedCells Is Nothing AndAlso DataGridView1.SelectedCells.Count = 1 Then
                Dim _rowid = DataGridView1.SelectedCells(0).RowIndex
                Dim _cellid = DataGridView1.SelectedCells(0).ColumnIndex

                For Each _s In _data
                    If ReplacementDataGD.Rows.Count <= _rowid Then
                        ReplacementDataGD.Rows.Add(ReplacementDataGD.NewRow())
                    End If
                    ReplacementDataGD.Rows(_rowid)(_cellid) = _s
                    _rowid += 1
                Next

                ReloadReplacementdataGrid()
            Else
                MessageBox.Show("Select cell first")
            End If
            Debug.WriteLine("")
        End If
    End Sub

    Private Sub txtURL_TextChanged(sender As Object, e As EventArgs) Handles txtURL.TextChanged
        _SelectWebRequest.url = txtURL.Text
        DirectCast(lb_WebCalls.SelectedValue, har.Request).url = txtURL.Text
        lb_WebCalls.Refresh()
    End Sub

    Private Sub txtURL_Leave(sender As Object, e As EventArgs) Handles txtURL.Leave
        ReloadWebReuestGrid()
    End Sub
End Class