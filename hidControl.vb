Option Explicit On

Imports Microsoft.Win32.SafeHandles
Imports SkylanderEditor.Hid
Imports SkylanderEditor.FileIO
Imports SkylanderEditor.DeviceManagement
Imports System.Runtime.InteropServices
Imports System.IO

Module hidControl

    Dim deviceStream As FileStream
    Dim MyHid As New Hid()
    Dim MyDeviceManagement As New DeviceManagement()
    Dim deviceNotificationHandle As IntPtr
    Dim myDevicePathName As String

    Const reportSize = 33

    'gets the handle of Portal of Power connected to the computer
    Public Function FindThePortal() As SafeFileHandle

        Dim deviceFound As Boolean
        Dim devicePathName(127) As String
        Dim hidGuid As System.Guid
        Dim memberIndex As Int32
        Dim myProductID As Int32
        Dim myVendorID As Int32
        Dim success As Boolean
        Dim preparsedData As IntPtr
        Dim myDeviceDetected As Boolean
        Dim hidHandle As SafeFileHandle

        myDeviceDetected = False

        myVendorID = 5168
        myProductID = 336

        Hid.HidD_GetHidGuid(hidGuid)

        deviceFound = MyDeviceManagement.FindDeviceFromGuid _
         (hidGuid, _
         devicePathName)

        If deviceFound Then

            memberIndex = 0

            Do
                hidHandle = FileIO.CreateFile _
                 (devicePathName(memberIndex),
                 0,
                 FileIO.FILE_SHARE_READ Or FileIO.FILE_SHARE_WRITE,
                 IntPtr.Zero,
                 FileIO.OPEN_EXISTING,
                 0,
                 0)

                If Not (hidHandle.IsInvalid) Then

                    MyHid.DeviceAttributes.Size = Marshal.SizeOf(MyHid.DeviceAttributes)

                    success = Hid.HidD_GetAttributes(hidHandle, MyHid.DeviceAttributes)

                    If success Then
                        If (MyHid.DeviceAttributes.VendorID = myVendorID) And
                         (MyHid.DeviceAttributes.ProductID = myProductID) Then

                            myDeviceDetected = True
                            myDevicePathName = devicePathName(memberIndex)
                            Console.WriteLine("Portal found at: " & myDevicePathName)
                        Else
                            myDeviceDetected = False
                            hidHandle.Close()
                        End If

                    Else
                        myDeviceDetected = False
                        hidHandle.Close()
                    End If

                End If

                memberIndex = memberIndex + 1

            Loop Until (myDeviceDetected Or (memberIndex = devicePathName.Length))

        End If

        If myDeviceDetected Then

            MyDeviceManagement.RegisterForDeviceNotifications _
             (myDevicePathName,
             IntPtr.Zero, ' No Form1.Handle, so use IntPtr.Zero
             hidGuid,
             deviceNotificationHandle)

            Hid.HidD_GetPreparsedData(hidHandle, preparsedData)
            Hid.HidP_GetCaps(preparsedData, MyHid.Capabilities)
            If Not (preparsedData = IntPtr.Zero) Then
                Hid.HidD_FreePreparsedData(preparsedData)
            End If

            hidHandle.Close()

            hidHandle = FileIO.CreateFile _
             (myDevicePathName,
             FileIO.GENERIC_READ Or FileIO.GENERIC_WRITE,
             FileIO.FILE_SHARE_READ Or FileIO.FILE_SHARE_WRITE,
             IntPtr.Zero,
             FileIO.OPEN_EXISTING,
             0,
             0)

            deviceStream = New FileStream(hidHandle, FileAccess.Read Or FileAccess.Write, reportSize, False)

            Hid.HidD_FlushQueue(hidHandle)
            Console.WriteLine("Portal Connected!")

        Else
            Console.WriteLine("Portal Not Found!")
        End If

        Return hidHandle

    End Function

    'send a report to the portal, they are 33 bytes long
    Public Sub outputReport(ByVal hidHandle As SafeFileHandle, ByRef outReport As Byte())
        Hid.HidD_SetOutputReport(hidHandle, outReport(0), reportSize)
    End Sub

    'receive a report from the portal into the specified array, reports are 33 bytes long
    Public Sub inputReport(ByVal hidHandle As SafeFileHandle, ByRef inReport As Byte())
        If (deviceStream.CanRead) Then
            deviceStream.Read(inReport, 0, reportSize)
        End If
    End Sub

    'this flushes the input reports from the portal, we need to clear it before reading
    Public Sub flushHid(ByVal hidHandle As SafeFileHandle)
        Hid.HidD_FlushQueue(hidHandle)
    End Sub

    'cleanup function to close the handles to the Portal
    Public Sub CloseCommunications(ByRef hidHandle As SafeFileHandle)

        If (Not (deviceStream Is Nothing)) Then
            deviceStream.Close()
        End If

        If (Not (hidHandle Is Nothing)) Then
            If (Not hidHandle.IsInvalid) Then
                hidHandle.Close()
            End If
        End If

        MyDeviceManagement.StopReceivingDeviceNotifications(deviceNotificationHandle)
        Console.WriteLine("Portal communications closed.")
    End Sub

    'function to check if the device removed is the previously openned portal
    Public Function checkDevice(ByRef m As Object) As Boolean
        ' No device name to check in console mode, always return False
        Return False
    End Function
End Module
