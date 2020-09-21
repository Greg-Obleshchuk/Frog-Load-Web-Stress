Public Class LoadHarDialog
    Public Requests As List(Of har.Request)
    Public Name As String
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If txtBaseURL.Text.Trim.Length > 0 Then
            If OpenFileDialog1.ShowDialog = DialogResult.OK Then
                Dim _fileName As String = OpenFileDialog1.FileName
                Requests = LoadHar(_fileName, txtBaseURL.Text.Trim)
                Name = txtname.Text
                Me.DialogResult = DialogResult.OK
                Me.Close()
            End If
        Else
            MessageBox.Show("Need base URL to file file by")
        End If
    End Sub

    Private Sub LoadHarDialog_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        txtname.Text = Guid.NewGuid.ToString
    End Sub
End Class