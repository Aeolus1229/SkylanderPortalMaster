Option Strict On
Option Explicit On 
Imports System.Runtime.InteropServices

Partial Public NotInheritable Class DeviceManagement

	'''<remarks>
	''' API declarations relating to device management (SetupDixxx and 
	''' RegisterDeviceNotification functions).
	''' Constants are from dbt.h and setupapi.h.
	'''</remarks>

	Public Const DBT_DEVICEARRIVAL As Int32 = &H8000
	Public Const DBT_DEVICEREMOVECOMPLETE As Int32 = &H8004
	Public Const DBT_DEVTYP_DEVICEINTERFACE As Int32 = 5
	Public Const DBT_DEVTYP_HANDLE As Int32 = 6
	Public Const DEVICE_NOTIFY_ALL_INTERFACE_CLASSES As Int32 = 4
	Public Const DEVICE_NOTIFY_SERVICE_HANDLE As Int32 = 1
	Public Const DEVICE_NOTIFY_WINDOW_HANDLE As Int32 = 0
	Public Const WM_DEVICECHANGE As Int32 = &H219

	Public Const DIGCF_PRESENT As Int32 = 2
	Public Const DIGCF_DEVICEINTERFACE As Int32 = &H10

	'Two declarations for the DEV_BROADCAST_DEVICEINTERFACE structure.

	'Use this one in the call to RegisterDeviceNotification() and
	'in checking dbch_devicetype in a DEV_BROADCAST_HDR structure:

	<StructLayout(LayoutKind.Sequential)>
	Public Class DEV_BROADCAST_DEVICEINTERFACE
		Public dbcc_size As Int32
		Public dbcc_devicetype As Int32
		Public dbcc_reserved As Int32
		Public dbcc_classguid As Guid
		Public dbcc_name As Int16
	End Class

	'Use this to read the dbcc_name string and classguid:

	<StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)>
	Public Class DEV_BROADCAST_DEVICEINTERFACE_1
		Public dbcc_size As Int32
		Public dbcc_devicetype As Int32
		Public dbcc_reserved As Int32
		<MarshalAs(UnmanagedType.ByValArray, ArraySubType:=UnmanagedType.U1, SizeConst:=16)>
		Public dbcc_classguid() As Byte
		<MarshalAs(UnmanagedType.ByValArray, SizeConst:=255)>
		Public dbcc_name() As Char
	End Class

	<StructLayout(LayoutKind.Sequential)>
	Public Class DEV_BROADCAST_HDR
		Public dbch_size As Int32
		Public dbch_devicetype As Int32
		Public dbch_reserved As Int32
	End Class

	Public Structure SP_DEVICE_INTERFACE_DATA
		Public cbSize As Int32
		Public InterfaceClassGuid As System.Guid
		Public Flags As Int32
		Public Reserved As IntPtr
	End Structure

	Public Structure SP_DEVICE_INTERFACE_DETAIL_DATA
		Public cbSize As Int32
		Public DevicePath As String
	End Structure

	Public Structure SP_DEVINFO_DATA
		Public cbSize As Int32
		Public ClassGuid As System.Guid
		Public DevInst As Int32
		Public Reserved As Int32
	End Structure

	<DllImport("user32.dll", CharSet:=CharSet.Auto, SetLastError:=True)> Shared Function RegisterDeviceNotification _
	 (ByVal hRecipient As IntPtr,
	 ByVal NotificationFilter As IntPtr,
	 ByVal Flags As Int32) _
	 As IntPtr
	End Function

	<DllImport("setupapi.dll", SetLastError:=True)> Shared Function SetupDiCreateDeviceInfoList _
	 (ByRef ClassGuid As System.Guid,
	 ByVal hwndParent As Int32) _
	 As Int32
	End Function

	<DllImport("setupapi.dll", SetLastError:=True)> Shared Function SetupDiDestroyDeviceInfoList _
	 (ByVal DeviceInfoSet As IntPtr) _
	 As Int32
	End Function

	<DllImport("setupapi.dll", SetLastError:=True)> Shared Function SetupDiEnumDeviceInterfaces _
	 (ByVal DeviceInfoSet As IntPtr,
	 ByVal DeviceInfoData As IntPtr,
	 ByRef InterfaceClassGuid As System.Guid,
	 ByVal MemberIndex As Int32,
	 ByRef DeviceInterfaceData As SP_DEVICE_INTERFACE_DATA) _
	 As Boolean
	End Function

	<DllImport("setupapi.dll", CharSet:=CharSet.Auto, SetLastError:=True)> Shared Function SetupDiGetClassDevs _
	  (ByRef ClassGuid As System.Guid,
	  ByVal Enumerator As IntPtr,
	  ByVal hwndParent As IntPtr,
	  ByVal Flags As Int32) _
	 As IntPtr
	End Function

	<DllImport("setupapi.dll", CharSet:=CharSet.Auto, SetLastError:=True)> Shared Function SetupDiGetDeviceInterfaceDetail _
	 (ByVal DeviceInfoSet As IntPtr,
	 ByRef DeviceInterfaceData As SP_DEVICE_INTERFACE_DATA,
	 ByVal DeviceInterfaceDetailData As IntPtr,
	 ByVal DeviceInterfaceDetailDataSize As Int32,
	 ByRef RequiredSize As Int32,
	 ByVal DeviceInfoData As IntPtr) _
	 As Boolean
	End Function

	<DllImport("user32.dll", SetLastError:=True)> Shared Function UnregisterDeviceNotification _
	 (ByVal Handle As IntPtr) _
	As Boolean
	End Function

End Class

