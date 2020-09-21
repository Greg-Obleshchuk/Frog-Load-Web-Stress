Public Class LoadProcessing
    Private _WebRequests As List(Of har.Request)
    Private _Threads As List(Of Threading.Thread)
    Sub New(WebRequests As List(Of har.Request))
        _WebRequests = WebRequests
        For Each _webrequest In _WebRequests
            Dim _newThread As New Threading.Thread(AddressOf ProcessWebReuest)
            _newThread.IsBackground = True
            _Threads.Add(_newThread)
        Next
    End Sub
    Private Sub ProcessWebReuest(webrequest As har.Request)

    End Sub

End Class
