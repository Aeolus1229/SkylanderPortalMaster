Option Explicit On
Imports System.Xml

Module HatControl
    ' Simulate hat loading: just print hat names to the console
    Public Sub hatsimulator(ByVal exepath As String)
        Dim count As Integer
        Dim xmlFile As XmlReader
        xmlFile = XmlReader.Create(exepath & "SkylanderDB.xml")
        Dim ds As New DataSet
        Dim dv As DataView
        ds.ReadXml(xmlFile)

        dv = New DataView(ds.Tables(3))
        dv.Sort = "index"

        count = 0
        Console.WriteLine("Available Hats:")
        Do
            Dim index As Integer = dv.Find(count)
            If index = -1 Then
                Exit Do
            Else
                Console.WriteLine($"{count}: {dv(index)("name")}")
            End If
            count = count + 1
        Loop
        Console.WriteLine("Hat list loaded.")
    End Sub

    ' Get which hat is written in the figure
    Public Function gethat(ByVal hatid1 As Byte, ByVal hatid2 As Byte, ByVal hatid3 As Byte, ByVal hatid4 As Byte) As Integer
        If hatid1 <> 0 Then
            Return Int(hatid1)
        ElseIf hatid2 <> 0 Then
            Return Int(hatid2)
        ElseIf hatid3 <> 0 Then
            Return Int(hatid3)
        ElseIf hatid4 <> 0 Then
            Return Int(hatid4) + 255
        Else
            Return 0
        End If
    End Function

    Public Function searchXML(ByVal exepath As String, ByVal SkyNum As Integer) As String()
        Dim SkyXML(2) As String
        Dim xmlFile As XmlReader
        xmlFile = XmlReader.Create(exepath & "SkylanderDB.xml")
        Dim ds As New DataSet
        Dim dv As DataView
        ds.ReadXml(xmlFile)

        dv = New DataView(ds.Tables(1))
        dv.Sort = "index"
        Dim index As Integer = dv.Find(SkyNum)

        If index = -1 Then
            SkyXML(0) = ""
        Else
            SkyXML(0) = dv(index)("name").ToString()
            SkyXML(1) = dv(index)("element").ToString()
        End If

        Return SkyXML
    End Function
End Module
