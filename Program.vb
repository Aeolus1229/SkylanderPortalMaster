Imports System.Xml.Linq
Imports System.Threading
Imports System.IO

Module Program
    Public Class SkylanderInfo
        Public Property ID As String
        Public Property Name As String
        Public Property Element As String
    End Class

    Function LoadSkylanderDB(path As String) As Dictionary(Of Integer, SkylanderInfo)
        Dim db As New Dictionary(Of Integer, SkylanderInfo)
        Dim doc = XDocument.Load(path)
        ' Explicitly select Skylander elements under SkylanderList
        Dim skylanders = doc.Root.Element("SkylanderList")?.Elements("Skylander")
        If skylanders IsNot Nothing Then
            For Each sk In skylanders
                Dim idStr = sk.Element("index")?.Value
                Dim name = sk.Element("name")?.Value
                Dim element = sk.Element("element")?.Value
                Dim id As Integer
                If Integer.TryParse(idStr, id) Then
                    db(id) = New SkylanderInfo With {.ID = idStr, .Name = name, .Element = element}
                End If
            Next
        End If
        Console.WriteLine("Loaded " & db.Count & " Skylanders from DB.")
        Return db
    End Function

    ' Returns the value of the requested property for the Skylander at the given index.
    Function GetSkylanderFieldByIndex(db As Dictionary(Of Integer, SkylanderInfo), index As Integer, fieldName As String) As String
        If db.ContainsKey(index) Then
            Dim info = db(index)
            Select Case fieldName.ToLower()
                Case "id"
                    Return info.ID
                Case "name"
                    Return info.Name
                Case "element"
                    Return info.Element
                Case Else
                    Return Nothing
            End Select
        End If
        Console.WriteLine($"DEBUG: Index {index} not found in DB or field '{fieldName}' not present.")
        Return Nothing
    End Function

    Sub ResetPortal(hidHandle As Microsoft.Win32.SafeHandles.SafeFileHandle)
        Dim outReport(32) As Byte
        outReport(1) = &H52
        hidControl.outputReport(hidHandle, outReport)
        Thread.Sleep(100)
        outReport(1) = &H41
        outReport(2) = 1
        hidControl.outputReport(hidHandle, outReport)
        Thread.Sleep(500)
    End Sub

    Function ReadSkylanderFromPortal(hidHandle As Microsoft.Win32.SafeHandles.SafeFileHandle) As Byte()
        Dim outReport(32) As Byte
        Dim inReport(32) As Byte
        Dim skylanderBytes(1023) As Byte
        Dim readBlock As Integer
        Dim timeout As Integer

        outReport(1) = &H51
        outReport(2) = &H20
        readBlock = 0
        Do
            outReport(3) = readBlock
            hidControl.outputReport(hidHandle, outReport)
            hidControl.flushHid(hidHandle)
            timeout = 0
            Do
                hidControl.inputReport(hidHandle, inReport)
                timeout += 1
            Loop Until inReport(1) <> &H53 Or timeout = 4

            If timeout <> 4 Then
                Array.Copy(inReport, 4, skylanderBytes, readBlock * 16, 16)
                readBlock += 1
            End If
        Loop While readBlock <= &H3F

        Return skylanderBytes
    End Function

    Sub Main()
        Dim db = LoadSkylanderDB("SkylanderDB.xml")
        Dim outputPath = "active_skylanders.json"
        Dim lastIndex As String = ""
        Dim lastPresence As Boolean = False
        Dim hidHandle As Microsoft.Win32.SafeHandles.SafeFileHandle = Nothing

        ' Ensure JSON is empty at startup
        File.WriteAllText(outputPath, "{}")

        Console.WriteLine("Listening for Skylanders... (Press Ctrl+C to exit)")

        ' Open the portal handle once
        hidHandle = hidControl.FindThePortal()
        If hidHandle Is Nothing OrElse hidHandle.IsInvalid Then
            Console.WriteLine("Portal not found.")
            Return
        End If

        ' Optionally, reset once at start
        ResetPortal(hidHandle)

        While True
            ' Poll quick report
            Dim quickReport(32) As Byte
            hidControl.flushHid(hidHandle)
            hidControl.inputReport(hidHandle, quickReport)

            ' Check for presence: any of bytes 2, 3, 4 nonzero (indices 2, 3, 4)
            Dim present As Boolean = (quickReport(2) <> 0 Or quickReport(3) <> 0 Or quickReport(4) <> 0)

            If present AndAlso Not lastPresence Then
                ' Figure just placed
                Console.WriteLine("Skylander placed, reading data...")
                Dim skylanderBytes() As Byte = ReadSkylanderFromPortal(hidHandle)

                Dim index As Integer = skylanderBytes(16) + (skylanderBytes(17) << 8)

                Dim id As String = GetSkylanderFieldByIndex(db, index, "id")
                Dim name As String = GetSkylanderFieldByIndex(db, index, "name")
                Dim element As String = GetSkylanderFieldByIndex(db, index, "element")

                If Not String.IsNullOrEmpty(name) Then
                    Console.WriteLine("Skylander detected: " & name)
                Else
                    id = index.ToString()
                    name = "Unknown"
                    element = "Unknown"
                    Console.WriteLine("Skylander detected: Unknown (" & index & ")")
                End If

                If index > 0 Then
                    lastIndex = index.ToString()
                    ' Write a single JSON object
                    Dim jsonObj As String = $"{{""ID"":""{id}"",""Name"":""{name}"",""Element"":""{element}""}}"
                    File.WriteAllText(outputPath, jsonObj)
                End If
            ElseIf Not present AndAlso lastPresence Then
                ' Figure just removed
                Console.WriteLine("Skylander removed.")
                lastIndex = ""
                File.WriteAllText(outputPath, "{}")
                ResetPortal(hidHandle) ' <-- Reset after removal to prepare for next figure
            End If

            lastPresence = present
            Thread.Sleep(500)
        End While

        ' On exit, close the handle (not reached in this loop)
        ' hidControl.CloseCommunications(hidHandle)
    End Sub
End Module