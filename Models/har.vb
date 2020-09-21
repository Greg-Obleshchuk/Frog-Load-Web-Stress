Public Class har
    Public Class Creator
        Public Property name As String
        Public Property version As String
    End Class

    Public Class PageTimings
        Public Property onContentLoad As Double
        Public Property onLoad As Double
    End Class

    Public Class Page
        Public Property startedDateTime As DateTime
        Public Property id As String
        Public Property title As String
        Public Property pageTimings As PageTimings
    End Class

    Public Class Initiator
        Public Property type As String
    End Class

    Public Class Cache
    End Class

    Public Class Header
        Public Property name As String
        Public Property value As String
        Public ReadOnly Property theHeader As String
            Get
                Return name & ":" & value
            End Get
        End Property

    End Class



    Public Class Request

        Public Property method As String
        Public Property url As String
        'Public Property httpVersion As String
        Public Property headers As New List(Of Header)
        'Public Property queryString As List(Of Header)
        Public Property cookies As New List(Of Cooky)


    End Class



    Public Class Cooky
        Public Property name As String
        Public Property value As String
        Public Property path As String
        Public Property domain As String
        Public Property expires As Object
        Public Property httpOnly As Boolean
        Public Property secure As Boolean
        Public Property sameSite As String
        Public ReadOnly Property theCooky As String
            Get
                Return name & ":" & value
            End Get
        End Property
    End Class

    Public Class Content
        Public Property size As Integer
        Public Property mimeType As String
        Public Property text As String
    End Class

    Public Class Response
        Public Property status As Integer
        Public Property statusText As String
        Public Property httpVersion As String
        Public Property headers As Header()
        Public Property cookies As Cooky()
        Public Property content As Content
        Public Property redirectURL As String
        Public Property headersSize As Integer
        Public Property bodySize As Integer
        Public Property _transferSize As Integer
        Public Property _error As Object
    End Class

    Public Class Timings
        Public Property blocked As Double
        Public Property dns As Double
        Public Property ssl As Double
        Public Property connect As Double
        Public Property send As Double
        Public Property wait As Double
        Public Property receive As Double
        Public Property _blocked_queueing As Double
    End Class

    Public Class Entry
        Public Property _initiator As Initiator
        Public Property _priority As String
        Public Property _resourceType As String
        Public Property cache As Cache
        Public Property connection As String
        Public Property pageref As String
        Public Property request As Request
        Public Property response As Response
        Public Property serverIPAddress As String
        Public Property startedDateTime As DateTime
        Public Property time As Double
        Public Property timings As Timings

    End Class

    Public Class Log
        Public Property version As String
        Public Property creator As Creator
        Public Property pages As Page()
        Public Property entries As Entry()
    End Class

    Public Class root
        Public Property log As Log
    End Class
End Class
