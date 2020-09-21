Imports System.Net
Imports System.Text.RegularExpressions
Imports System.Windows.Forms
Imports System.Text
Imports System.IO
Imports System.Web
Imports System.Security.Cryptography.X509Certificates
Imports System.Net.Security
Imports System.Net.Http
Imports System.Threading
Imports System.Collections.Concurrent


Public Class WebProcessingClass
    Private theRunSheet As New WebRun
    Private theWebTimeOutMS As Int32
    Private theThreadMessagesQueue As ConcurrentQueue(Of String)
    Private _StopProcessing As Boolean = False
    Private _RunWebRequest As Boolean = False
    Public MainProcessingThread As Thread
    Sub New(RunSheet As WebRun, WebTimeOutSeconds As Int32, ThreadMessagesQueue As ConcurrentQueue(Of String))
        theRunSheet = RunSheet
        If WebTimeOutSeconds = 0 Then
            WebTimeOutSeconds = 300
        End If
        theWebTimeOutMS = WebTimeOutSeconds * 1000
        theThreadMessagesQueue = ThreadMessagesQueue
        For Each akey In theRunSheet.WebRequests.Keys
            For x = 0 To 10
                For Each bkey In theRunSheet.WebRequests(akey).UserReplacementData.Keys
                    If theRunSheet.WebRequests(akey).UserReplacementData(bkey).Value(x) <> "" Then
                        theRunSheet.WebRequests(akey).UserReplacementDataID.Value(x) = bkey + 1
                        theRunSheet.WebRequests(akey).UserReplacementDataMax.Value(x) = bkey
                    End If
                Next
            Next
        Next


    End Sub
    Public Sub StopProcessing()
        _StopProcessing = True
    End Sub
    Public Sub RunWebRequest()
        _RunWebRequest = True
    End Sub
    Private Sub GetNewReplacementValues(ByRef newvalue As ReplacementData, ByRef UserReplacementDataMax As ReplacementDataID,
                                        ByRef UserReplacementDataID As ReplacementDataID, UserReplacementData As SortedList(Of Int32, ReplacementData))

        For x = 1 To 10
            If UserReplacementDataMax.Value(x) > 0 Then
                If UserReplacementDataID.Value(x) >= UserReplacementDataMax.Value(x) Then
                    UserReplacementDataID.Value(x) = -1
                End If

                UserReplacementDataID.Value(x) += 1
                newvalue.Value(x) = UserReplacementData(UserReplacementDataID.Value(x)).Value(x)

            End If
        Next





    End Sub
    Public Sub MainProcessingLoop()
        MainProcessingThread = Thread.CurrentThread
        Dim _DS1ID As Int32 = 0
        '        Dim _DS1Max As Int32 = theRunSheet.

        While _StopProcessing = False
            While _StopProcessing = False AndAlso _RunWebRequest = True
                Dim _WebRequestTiming As New WebRequestStep
                For Each key In theRunSheet.WebRequests.Keys
                    Dim _thread As New List(Of Thread)

                    Dim _wr = theRunSheet.WebRequests(key)
                    Dim _newvalue As New ReplacementData
                    GetNewReplacementValues(_newvalue, _wr.UserReplacementDataMax, _wr.UserReplacementDataID, _wr.UserReplacementData)
                    Dim thisrequestString = Newtonsoft.Json.JsonConvert.SerializeObject(_wr.WebRequestItem)
                    For x = 1 To 10
                        thisrequestString = Replace(thisrequestString, "$DS" & x & "$", _newvalue.Value(x),,, CompareMethod.Text)
                    Next
                    Dim ThisRequest = Newtonsoft.Json.JsonConvert.DeserializeObject(Of List(Of har.Request))(thisrequestString)
                    'we have replacement text in the elements 
                    Dim _RunWebRequest As New List(Of RunWebRequest)
                    For Each _wri In ThisRequest

                        RunWebRequest()
                        Dim _newRunWebReques As New RunWebRequest(_wri, theWebTimeOutMS)
                        _RunWebRequest.Add(_newRunWebReques)
                        Dim _RunWebRequestthread As New Thread(AddressOf _newRunWebReques.Run)
                        _RunWebRequestthread.IsBackground = True
                        _RunWebRequestthread.Start()
                    Next
                    Dim _isProcessing As Boolean = True
                    While _StopProcessing = False AndAlso _isProcessing = True
                        _isProcessing = False
                        For Each _RWR In _RunWebRequest
                            If Not _RWR.ProcessingThread Is Nothing AndAlso _RWR.ProcessingThread.IsAlive = True Then
                                _isProcessing = True
                                Exit For
                            End If
                        Next
                        Thread.Sleep(1)
                    End While


                    If _wr.MinTime > 0 Then
                        Dim _delayTime = _wr.MinTime * 1000
                        If _wr.RndTime = True Then
                            Randomize(Now.Ticks)
                            _delayTime = CInt(Math.Floor(((_wr.MaxTime * 1000) - (_wr.MinTime * 1000) + 1) * Rnd())) + (_wr.MinTime * 1000)
                        End If
                        Thread.Sleep(_delayTime)
                    End If

                    Dim _WebRequestStep As New WebRequestStep
                    For Each _wri In _RunWebRequest
                        If _WebRequestStep.maxtime < _wri.gettimeinticks() Then
                            _WebRequestStep.maxtime = _wri.gettimeinticks()
                        End If
                        If _wri.IsSuccess = True Then
                            _WebRequestStep.numberofsuccess += 1
                        Else
                            _WebRequestStep.numberoferrors += 1
                        End If

                    Next
                    _WebRequestTiming.maxtime += _WebRequestStep.maxtime
                    _WebRequestTiming.numberofsuccess += _WebRequestStep.numberofsuccess
                    _WebRequestTiming.numberoferrors += _WebRequestStep.numberoferrors
                    '_WebRequestTiming.totalitems = _RunWebRequest.Count
                Next

                theThreadMessagesQueue.Enqueue(Newtonsoft.Json.JsonConvert.SerializeObject(_WebRequestTiming))
                Thread.Sleep(1)
            End While
            Thread.Sleep(1)
        End While
        MainProcessingThread = Nothing
    End Sub
End Class
Public Class RunWebRequest
    Private _webRequest As har.Request
    Public ProcessingThread As Thread
    Private _startticks As Int64
    Private _endticks As Int64
    Private _timeout As Int32
    Public IsSuccess As Boolean = False

    Sub New(webRequest As har.Request, timeout As Int32)
        _webRequest = webRequest
        _timeout = timeout
    End Sub
    Public Function gettimeinticks() As Int64
        Return _endticks - _startticks
    End Function

    Public Sub Run()
        ProcessingThread = Thread.CurrentThread
        _startticks = Now.Ticks

#If DEBUG Then

        '  _webRequest.url = Replace(_webRequest.url, "https://premium.go-touring.com.au", "http://localhost:59428/",,, CompareMethod.Text)
#End If
        Using webclient As New LocalWebClient(_timeout)

            webclient.Encoding = System.Text.Encoding.UTF8
            For Each _h In _webRequest.headers
                webclient.Headers.Add(_h.name, _h.value)
            Next
            Try
                _startticks = Now.Ticks
                Dim returnValue As String = ""
                Select Case _webRequest.method.ToUpper
                    Case "GET"
                        returnValue = webclient.DownloadString(_webRequest.url)
                    Case "POST"
                        returnValue = webclient.UploadString(_webRequest.url, "")
                        Debug.WriteLine("")
                End Select
                _endticks = Now.Ticks
                IsSuccess = True
            Catch ex As Exception
                _endticks = Now.Ticks
                End Try
        End Using
    End Sub



    Private Class LocalWebClient
        Inherits Net.WebClient
        Private _TimeOutPeriodMS As Int32 = 300000 ' Default to five minutes
        Sub New(Optional TimeOutPeriodMS As Int32 = 300000) ' Default to five minutes
            _TimeOutPeriodMS = TimeOutPeriodMS
        End Sub
        Private Function ServerCertificateValidationCallbackdelegate(sender As Object, certificate As X509Certificate, chain As X509Chain, sslPolicyErrors As SslPolicyErrors) As Boolean
            ' This is to manage SSL certificates which are incorrect or expired.
            Return True
        End Function
        Protected Overrides Function GetWebRequest(address As Uri) As WebRequest
            ' This is to manage SSL certificates which are incorrect or expired.
            System.Net.ServicePointManager.ServerCertificateValidationCallback = AddressOf ServerCertificateValidationCallbackdelegate
            Dim returnWebRequest = MyBase.GetWebRequest(address)
            returnWebRequest.Timeout = _TimeOutPeriodMS
            Return returnWebRequest
        End Function
    End Class
End Class
