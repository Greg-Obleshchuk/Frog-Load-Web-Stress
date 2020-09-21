Module Utilites
    Public theData As New WebRun

    Public Function ConvertFiletoHar(filename As String) As har.root
        Dim _returnValue As New har.root
        Using _inputfile As New IO.StreamReader(filename)
            Dim _filedata = _inputfile.ReadToEnd()
            _returnValue = Newtonsoft.Json.JsonConvert.DeserializeObject(Of har.root)(_filedata)
        End Using
        Return _returnValue
    End Function
    Public Sub LoadData(FileName As String)
        Using _inputfile As New IO.StreamReader(FileName)
            Dim _filedata = _inputfile.ReadToEnd()
            theData = Newtonsoft.Json.JsonConvert.DeserializeObject(Of WebRun)(_filedata)
        End Using
    End Sub
    Public Sub SaveData(FileName As String)
        Using _Outputfile As New IO.StreamWriter(FileName)
            Dim _filedata = Newtonsoft.Json.JsonConvert.SerializeObject(theData)
            _Outputfile.Write(_filedata)
        End Using
    End Sub
    Public Function LoadHar(filename As String, BaseURL As String) As List(Of har.Request)
        Dim _WebRequests As New List(Of har.Request)
        Dim _newhar = ConvertFiletoHar(filename)
        For Each _entry In _newhar.log.entries
            If _entry.request.url.ToLower.Contains(BaseURL.ToLower.Trim) Then
                _WebRequests.Add(_entry.request)
            End If
        Next

        For Each _r In _WebRequests
            Dim _headertoremove As New List(Of har.Header)
            For Each _h In _r.headers
                If Microsoft.VisualBasic.Left(_h.name, 1) = ":" Then
                    _headertoremove.Add(_h)
                End If
                If _h.name.ToLower = "content-length" Then
                    _headertoremove.Add(_h)
                End If
                If _h.name.ToLower = "cookie" Then
                    _headertoremove.Add(_h)
                End If
            Next
            For Each _h In _headertoremove
                _r.headers.Remove(_h)
            Next
        Next
        Return _WebRequests

    End Function
End Module
Public Class WebRun

    Public WebRequests As New SortedList(Of Int32, WebRunItem)

End Class

Public Class WebRunItem
    Public Name As String
    Public WebRequestItem As New List(Of har.Request)
    Public MinTime As Int32 = 0
    Public MaxTime As Int32 = 0
    Public RndTime As Boolean = False
    Public UserReplacementData As New SortedList(Of Int32, ReplacementData)
    Public UserReplacementDataID As New ReplacementDataID
    Public UserReplacementDataMax As New ReplacementDataID

End Class
Public Class ReplacementDataID
    Public DS1 As Int32
    Public DS2 As Int32
    Public DS3 As Int32
    Public DS4 As Int32
    Public DS5 As Int32
    Public DS6 As Int32
    Public DS7 As Int32
    Public DS8 As Int32
    Public DS9 As Int32
    Public DS10 As Int32
    Public Property Value(id As Int32) As Int32
        Get
            Dim _returnValue As Int32 = 0
            Select Case id
                Case 1 : _returnValue = DS1
                Case 2 : _returnValue = DS2
                Case 3 : _returnValue = DS3
                Case 4 : _returnValue = DS4
                Case 5 : _returnValue = DS5
                Case 6 : _returnValue = DS6
                Case 7 : _returnValue = DS7
                Case 8 : _returnValue = DS8
                Case 9 : _returnValue = DS9
                Case 10 : _returnValue = DS10
            End Select
            Return _returnValue
        End Get
        Set(value As Int32)
            Select Case id
                Case 1 : DS1 = value
                Case 2 : DS2 = value
                Case 3 : DS3 = value
                Case 4 : DS4 = value
                Case 5 : DS5 = value
                Case 6 : DS6 = value
                Case 7 : DS7 = value
                Case 8 : DS8 = value
                Case 9 : DS9 = value
                Case 10 : DS10 = value
            End Select
        End Set
    End Property
End Class
Public Class WebRequestStep
    Public maxtime As Int64 = 0
    Public numberoferrors As Int32 = 0
    Public numberofsuccess As Int32 = 0
    Public averagetime As Int64 = 0
    Public totalitems As Int64 = 0

End Class
Public Class ReplacementData
    Public DS1 As String
    Public DS2 As String
    Public DS3 As String
    Public DS4 As String
    Public DS5 As String
    Public DS6 As String
    Public DS7 As String
    Public DS8 As String
    Public DS9 As String
    Public DS10 As String
    Public Property Value(id As Int32) As String
        Get
            Dim _returnValue As String = ""
            Select Case id
                Case 1 : _returnValue = DS1
                Case 2 : _returnValue = DS2
                Case 3 : _returnValue = DS3
                Case 4 : _returnValue = DS4
                Case 5 : _returnValue = DS5
                Case 6 : _returnValue = DS6
                Case 7 : _returnValue = DS7
                Case 8 : _returnValue = DS8
                Case 9 : _returnValue = DS9
                Case 10 : _returnValue = DS10
            End Select
            Return _returnValue
        End Get
        Set(value As String)
            Select Case id
                Case 1 : DS1 = value
                Case 2 : DS2 = value
                Case 3 : DS3 = value
                Case 4 : DS4 = value
                Case 5 : DS5 = value
                Case 6 : DS6 = value
                Case 7 : DS7 = value
                Case 8 : DS8 = value
                Case 9 : DS9 = value
                Case 10 : DS10 = value
            End Select
        End Set
    End Property
End Class

