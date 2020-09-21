Imports System.Collections.Concurrent
Imports System.Threading
Imports System.Threading.Tasks
Public Class Form1
    Private __webRunDS As New DataTable

    Private MonitorTask As Task
    Public threadMessages As New ConcurrentQueue(Of String)
    Public _systemTImer As System.Timers.Timer



    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _systemTImer = New System.Timers.Timer(10)

        TextBox.CheckForIllegalCrossThreadCalls = False
        Label.CheckForIllegalCrossThreadCalls = False
        AddHandler _systemTImer.Elapsed, AddressOf MonitorforMessages
        _systemTImer.AutoReset = True

        theData = New WebRun
        If My.Settings.LastFileSave <> "" Then
            LoadData(My.Settings.LastFileSave)

        End If
    End Sub
    Private Sub Create__webRunDS()
        __webRunDS = New DataTable
        __webRunDS.Columns.Add("Order", GetType(Int32))
        __webRunDS.Columns.Add("Name", GetType(String))
    End Sub
    Private Sub ToolStripMenuItem3_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem3.Click
        theData = New WebRun
        BindWebRun()
    End Sub

    Private Sub BindWebRun()
        lb_WebRun.DataSource = Nothing
        Create__webRunDS()

        For Each _wr In theData.WebRequests
            __webRunDS.Rows.Add({_wr.Key, _wr.Value.Name})
        Next
        lb_WebRun.DataSource = __webRunDS
        lb_WebRun.DisplayMember = "Name"

    End Sub
    Private Sub ImportHARFileToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ImportHARFileToolStripMenuItem.Click
        Dim _LoadHarDialog As New LoadHarDialog
        If LoadHarDialog.ShowDialog() = DialogResult.OK Then
            Dim _r = LoadHarDialog.Requests
            Dim _wri As New WebRunItem
            _wri.Name = LoadHarDialog.Name
            _wri.WebRequestItem = _r
            theData.WebRequests.Add(theData.WebRequests.Count, _wri)
            BindWebRun()
        End If

    End Sub

    Private Sub ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem1.Click
        If SaveFileDialog1.ShowDialog() = DialogResult.OK Then
            SaveData(SaveFileDialog1.FileName)
            My.Settings.LastFileSave = SaveFileDialog1.FileName
        End If
    End Sub

    Private Sub ToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem2.Click
        If ofd_selectfile.ShowDialog() = DialogResult.OK Then
            LoadData(ofd_selectfile.FileName)
            BindWebRun()
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim _se = lb_WebRun.SelectedItem
        Dim _index = _se("Order")
        If _index > 0 Then
            Dim _newWebRequest = New SortedList(Of Int32, WebRunItem)
            Dim _id As Int32 = 0
            For Each _key In theData.WebRequests.Keys
                If _key <> _index Then
                    If _key = _index - 1 Then
                        _newWebRequest.Add(_id, theData.WebRequests(_key + 1))
                        _id += 1
                        _newWebRequest.Add(_id, theData.WebRequests(_key))
                    Else
                        _newWebRequest.Add(_id, theData.WebRequests(_key))
                    End If

                End If

            Next
            theData.WebRequests = _newWebRequest
            BindWebRun()
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim _se = lb_WebRun.SelectedItem
        Dim _index = _se("Order")
        If _index < theData.WebRequests.Count - 1 Then
            Dim _newWebRequest = New SortedList(Of Int32, WebRunItem)
            Dim _id As Int32 = 0
            For Each _key In theData.WebRequests.Keys
                If _key <> _index Then
                    If _key = _index + 1 Then
                        _newWebRequest.Add(_id, theData.WebRequests(_key))
                        _id += 1
                        _newWebRequest.Add(_id, theData.WebRequests(_key - 1))
                    Else
                        _newWebRequest.Add(_id, theData.WebRequests(_key))
                    End If
                End If
            Next
            theData.WebRequests = _newWebRequest
            BindWebRun()
        End If
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim _se = lb_WebRun.SelectedItem
        Dim _index = _se("Order")


        Dim _ManageWebRequest As New ManageWebRequest
        _ManageWebRequest._WebRequests = theData.WebRequests(_index).WebRequestItem
        _ManageWebRequest.txtName.Text = theData.WebRequests(_index).Name
        _ManageWebRequest._ReplacementData = theData.WebRequests(_index).UserReplacementData
        _ManageWebRequest.minTime.Value = theData.WebRequests(_index).MinTime
        _ManageWebRequest.MaxTime.Value = theData.WebRequests(_index).MaxTime
        _ManageWebRequest.rndtime.Checked = theData.WebRequests(_index).RndTime

        _ManageWebRequest.LoadtheData()

        If _ManageWebRequest.ShowDialog() = DialogResult.OK Then
            theData.WebRequests(_index).Name = _ManageWebRequest.txtName.Text
            theData.WebRequests(_index).WebRequestItem = _ManageWebRequest._WebRequests
            theData.WebRequests(_index).UserReplacementData = _ManageWebRequest._ReplacementData
            theData.WebRequests(_index).MinTime = _ManageWebRequest.minTime.Value
            theData.WebRequests(_index).MaxTime = _ManageWebRequest.MaxTime.Value
            theData.WebRequests(_index).RndTime = _ManageWebRequest.rndtime.Checked
            BindWebRun()
        End If

    End Sub

    Private Sub Form1_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        BindWebRun()
    End Sub


    Private Sub btnGo_Click(sender As Object, e As EventArgs) Handles btnGo.Click
        GroupBox2.Enabled = False
        If btnGo.Text = "Stop" Then
            _systemTImer.Enabled = False
            Dim _oldValue = nb_NumberofClients.Value
            nb_NumberofClients.Minimum = 0
            nb_NumberofClients.Value = 0
            StopProcessing()
            btnGo.Text = "Go"
            Button4.Visible = False
            nb_NumberofClients.Value = _oldValue
            nb_NumberofClients.Minimum = 1
        Else
            threadMessages = New ConcurrentQueue(Of String)()
            _systemTImer.Enabled = True

            StartProcessing()
            btnGo.Text = "Stop"
            Button4.Visible = True
        End If
        GroupBox2.Enabled = True
    End Sub
    Private Sub StopProcessing()
        Dim _newLevel = nb_NumberofClients.Value
        If _newLevel = 0 Then
            For Each _wc In _webClients
                _wc.StopProcessing()
            Next
            Dim t = Task.Run(Async Function()
                                 Await Task.Delay(100)
                                 Return 42
                             End Function)
            t.Wait()
        End If
        While _webClients.Count > _newLevel
            Dim _wc = _webClients(0)

            _wc.StopProcessing()
            While Not _wc.MainProcessingThread Is Nothing AndAlso _wc.MainProcessingThread.IsAlive
                Dim t = Task.Run(Async Function()
                                     Await Task.Delay(100)
                                     Return 42
                                 End Function)
                t.Wait()

            End While
            _webClients.Remove(_wc)
            UpdateClients()
            _wc = Nothing
        End While
    End Sub
    Private Sub MonitorforMessages()


        While threadMessages.Count > 0
            Dim _message As String = ""
            If threadMessages.TryDequeue(_message) = True Then
                Dim _wrm = Newtonsoft.Json.JsonConvert.DeserializeObject(Of WebRequestStep)(_message)

                _OverallWebRequestTiming.numberoferrors += _wrm.numberoferrors
                _OverallWebRequestTiming.numberofsuccess += _wrm.numberofsuccess
                Dim _at As Int64 = (_OverallWebRequestTiming.averagetime * _OverallWebRequestTiming.totalitems) + _wrm.maxtime
                _OverallWebRequestTiming.totalitems += 1
                _OverallWebRequestTiming.averagetime = CLng(_at / _OverallWebRequestTiming.totalitems)
            End If
        End While

        lbAvgTime.Text = New TimeSpan(_OverallWebRequestTiming.averagetime).ToString
        lbrequestcount.Text = _OverallWebRequestTiming.numberoferrors + _OverallWebRequestTiming.numberofsuccess
        lbrequesterrors.Text = _OverallWebRequestTiming.numberoferrors
        lbrequestsuccess.Text = _OverallWebRequestTiming.numberofsuccess
    End Sub
    Dim _OverallWebRequestTiming As New WebRequestStep

    Dim _webClients As New List(Of WebProcessingClass)

    Private Sub StartProcessing()
        Dim _newLevel = nb_NumberofClients.Value
        While _webClients.Count < _newLevel
            Dim _newWebProcessingClass As New WebProcessingClass(theData, My.Settings.WebTimeOutSeconds, threadMessages)
            _webClients.Add(_newWebProcessingClass)
            Dim _thread As New Thread(AddressOf _newWebProcessingClass.MainProcessingLoop)
            _thread.IsBackground = True
            _thread.Start()
            UpdateClients()
        End While
        For Each _w In _webClients
            _w.RunWebRequest()
        Next
    End Sub


    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        nb_NumberofClients.Enabled = False
        If _webClients.Count < nb_NumberofClients.Value Then
            StartProcessing()
        End If
        If _webClients.Count > nb_NumberofClients.Value Then
            StopProcessing()
        End If
        nb_NumberofClients.Enabled = True

    End Sub


    Private Sub UpdateClients()
        lbClients.Text = FormatNumber(_webClients.Count, 0, TriState.True, TriState.False, TriState.True)
    End Sub

End Class
