Option Strict On
Option Explicit On

Imports Microsoft.Win32.SafeHandles
Imports System.Runtime.InteropServices


Public NotInheritable Class Hid

	' API declarations for HID communications.

	' from hidpi.h
	' Typedef enum defines a set of integer constants for HidP_Report_Type

	Public Const HidP_Input As Int16 = 0
	Public Const HidP_Output As Int16 = 1
	Public Const HidP_Feature As Int16 = 2

	Public Structure HIDD_ATTRIBUTES
		Public Size As Int32
		Public VendorID As UInt16
		Public ProductID As UInt16
		Public VersionNumber As UInt16
	End Structure

	Public Structure HIDP_CAPS
		Public Usage As Int16
		Public UsagePage As Int16
		Public InputReportByteLength As Int16
		Public OutputReportByteLength As Int16
		Public FeatureReportByteLength As Int16
		<MarshalAs(UnmanagedType.ByValArray, SizeConst:=17)> Dim Reserved() As Int16
		Public NumberLinkCollectionNodes As Int16
		Public NumberInputButtonCaps As Int16
		Public NumberInputValueCaps As Int16
		Public NumberInputDataIndices As Int16
		Public NumberOutputButtonCaps As Int16
		Public NumberOutputValueCaps As Int16
		Public NumberOutputDataIndices As Int16
		Public NumberFeatureButtonCaps As Int16
		Public NumberFeatureValueCaps As Int16
		Public NumberFeatureDataIndices As Int16

	End Structure

	Public Capabilities As HIDP_CAPS
	Public DeviceAttributes As HIDD_ATTRIBUTES

	' If IsRange is false, UsageMin is the Usage and UsageMax is unused.
	' If IsStringRange is false, StringMin is the string index and StringMax is unused.
	' If IsDesignatorRange is false, DesignatorMin is the designator index and DesignatorMax is unused.

	Public Structure HidP_Value_Caps
		Public UsagePage As Int16
		Public ReportID As Byte
		Public IsAlias As Int32
		Public BitField As Int16
		Public LinkCollection As Int16
		Public LinkUsage As Int16
		Public LinkUsagePage As Int16
		Public IsRange As Int32
		Public IsStringRange As Int32
		Public IsDesignatorRange As Int32
		Public IsAbsolute As Int32
		Public HasNull As Int32
		Public Reserved As Byte
		Public BitSize As Int16
		Public ReportCount As Int16
		Public Reserved2 As Int16
		Public Reserved3 As Int16
		Public Reserved4 As Int16
		Public Reserved5 As Int16
		Public Reserved6 As Int16
		Public LogicalMin As Int32
		Public LogicalMax As Int32
		Public PhysicalMin As Int32
		Public PhysicalMax As Int32
		Public UsageMin As Int16
		Public UsageMax As Int16
		Public StringMin As Int16
		Public StringMax As Int16
		Public DesignatorMin As Int16
		Public DesignatorMax As Int16
		Public DataIndexMin As Int16
		Public DataIndexMax As Int16
	End Structure

	<DllImport("hid.dll", SetLastError:=True)>
	Shared Function HidD_FlushQueue _
		(ByVal HidDeviceObject As SafeFileHandle) _
		As Boolean
	End Function

	<DllImport("hid.dll", SetLastError:=True)>
	Shared Function HidD_FreePreparsedData _
		(ByVal PreparsedData As IntPtr) _
		As Boolean
	End Function

	<DllImport("hid.dll", SetLastError:=True)>
	Shared Function HidD_GetAttributes _
	 (ByVal HidDeviceObject As SafeFileHandle,
	 ByRef Attributes As HIDD_ATTRIBUTES) _
	 As Boolean
	End Function

	<DllImport("hid.dll", SetLastError:=True)>
	Shared Function HidD_GetFeature _
		(ByVal HidDeviceObject As SafeFileHandle,
		ByVal lpReportBuffer() As Byte,
		ByVal ReportBufferLength As Int32) _
		As Boolean
	End Function

	<DllImport("hid.dll", SetLastError:=True)>
	Shared Function HidD_GetInputReport _
		(ByVal HidDeviceObject As SafeFileHandle,
		ByVal lpReportBuffer() As Byte,
		ByVal ReportBufferLength As Int32) _
		As Boolean
	End Function

	<DllImport("hid.dll", SetLastError:=True)>
	Shared Sub HidD_GetHidGuid _
		(ByRef HidGuid As System.Guid)
	End Sub

	<DllImport("hid.dll", SetLastError:=True)>
	Shared Function HidD_GetNumInputBuffers _
		(ByVal HidDeviceObject As SafeFileHandle,
		ByRef NumberBuffers As Int32) _
		As Boolean
	End Function

	<DllImport("hid.dll", SetLastError:=True)>
	Shared Function HidD_GetPreparsedData _
	 (ByVal HidDeviceObject As SafeFileHandle,
	 ByRef PreparsedData As IntPtr) _
	 As Boolean
	End Function

	<DllImport("hid.dll", SetLastError:=True)>
	Shared Function HidD_SetFeature _
		(ByVal HidDeviceObject As SafeFileHandle,
		ByVal lpReportBuffer() As Byte,
		ByVal ReportBufferLength As Int32) _
		As Boolean
	End Function

	<DllImport("hid.dll", SetLastError:=True)>
	Shared Function HidD_SetNumInputBuffers _
		(ByVal HidDeviceObject As SafeFileHandle,
		ByVal NumberBuffers As Int32) _
		As Boolean
	End Function

	<DllImport("hid.dll", SetLastError:=True)>
	Shared Function HidD_SetOutputReport _
		(ByVal HidDeviceObject As SafeFileHandle,
		ByRef lpReportBuffer As Byte,
		ByVal ReportBufferLength As Int32) _
		As Boolean
	End Function

	<DllImport("hid.dll", SetLastError:=True)>
	Shared Function HidP_GetCaps _
		(ByVal PreparsedData As IntPtr,
		ByRef Capabilities As HIDP_CAPS) _
		As Int32
	End Function

	<DllImport("hid.dll", SetLastError:=True)>
	Shared Function HidP_GetValueCaps _
	 (ByVal ReportType As Int32,
	 ByVal ValueCaps As Byte(),
	 ByRef ValueCapsLength As Int32,
	 ByVal PreparsedData As IntPtr) _
	 As Int32
	End Function

End Class
