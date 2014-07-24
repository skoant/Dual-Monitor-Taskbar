// This is the main DLL file.
//#pragma comment(linker,"\"/manifestdependency:type='win32' name='Microsoft.Windows.Common-Controls' version='6.0.0.0' processorArchitecture='*' publicKeyToken='6595b64144ccf1df' language='*'\"")

#include "Hook.h"
#include "windows.h"
#include "atlbase.h"
#include "shlobj.h"
#include "winuser.h"
#include "Shobjidl.h"
#include "commctrl.h"

#pragma warning(push)
#pragma warning(disable: 4996) // std::_Copy_impl - Function call with parameters that may be unsafe #include <algorithm> #pragma warning(pop)

#include <sstream>
#include <iostream>
#include <fstream>
using namespace std;

HWND toolbarListenerWindow;
HWND toolbarWindow;

HWND runningAppsListenerWindow;

// forward declarations
BOOL SaveIconToFile(HICON hico, LPCTSTR szFileName, BOOL bAutoDelete);
void GetTempIconFolder(TCHAR* str);

LPWSTR ConvertToLPWSTR( const std::string& s )
{
  LPWSTR ws = new wchar_t[s.size()+1]; // +1 for zero at the end
  copy( s.begin(), s.end(), ws );
  ws[s.size()] = 0; // zero at the end
  return ws;
}

// hook for SendMessage
extern "C" __declspec(dllexport) LRESULT CallWndProc(int nCode, WPARAM wParam, LPARAM lParam)
{
	HHOOK hhook = (HHOOK)0;
	CWPSTRUCT *cp = (CWPSTRUCT*)lParam;

	// init message
	if (cp->message == WM_APP + 1514) {		
		toolbarListenerWindow = (HWND)cp->wParam;
		
		toolbarWindow = FindWindow(TEXT("Shell_TrayWnd"), NULL);
		toolbarWindow = FindWindowEx(toolbarWindow, NULL, TEXT("TrayNotifyWnd"), NULL);
		toolbarWindow = FindWindowEx(toolbarWindow, NULL, TEXT("SysPager"), NULL);
		toolbarWindow = FindWindowEx(toolbarWindow, NULL, TEXT("ToolbarWindow32"), NULL);	
	}
	// custom paint message sent from DualMonitor app
	else if (cp->message == WM_APP + 1515) {
		HWND hwnd = cp->hwnd;
		HIMAGELIST himglist = (HIMAGELIST)SendMessage(hwnd, TB_GETIMAGELIST, NULL, NULL);

		// paint icon by index at specified relative coordinates
		int param = (int)cp->wParam;
		int index = param & 0xFF;
		param >>= 8;
		int x = param & 0xFF;
		int y = param >> 8;

		HDC hdc = GetDC((HWND)cp->lParam);

		BOOL result = ImageList_Draw(himglist, index, hdc, x, y, ILD_NORMAL);

		ReleaseDC((HWND)cp->lParam, hdc);
	}
	// need to update notification area
	else if (cp->message == TB_INSERTBUTTON || cp->message == TB_DELETEBUTTON)
	{
		if (cp->hwnd == toolbarWindow) {
			PostMessage(toolbarListenerWindow, WM_APP + 1516, -1, 0);
		}
	}	
	// need to update notification area
	else if (cp->message == TB_SETBUTTONINFOW)
	{
		if (cp->hwnd == toolbarWindow) {
			PostMessage(toolbarListenerWindow, WM_APP + 1516, cp->wParam, 0);
		}
	}
	// taskbar overlay icon
	else if (cp->message == WM_USER + 79) 
	{		
		if (cp->lParam != 0) 
		{
			TCHAR iconPath[MAX_PATH];
			GetTempIconFolder(iconPath);

			stringstream out;
			int i = 0;
			while (iconPath[i] != 0)
				out << (char)iconPath[i++];
			out << "overlayicon_" << cp->lParam << ".ico";
			LPWSTR szFileName = ConvertToLPWSTR(out.str());
			if (SaveIconToFile((HICON)cp->lParam, szFileName, FALSE))
			{								
				PostMessage(runningAppsListenerWindow, WM_APP + 1603, cp->wParam, cp->lParam);
			}
			delete[] szFileName;
		}

		PostMessage(runningAppsListenerWindow, WM_APP + 1603, cp->wParam, cp->lParam);
	}	

	return CallNextHookEx(hhook, nCode, wParam, lParam);
}

// Hook for PostMessage
extern "C" __declspec(dllexport) LRESULT GetMessageProc(int nCode, WPARAM wParam, LPARAM lParam)
{
	HHOOK hhook = (HHOOK)0;
	MSG *cp = (MSG*)lParam;
	
	if (cp->message == WM_APP + 1600) 
	{		
		runningAppsListenerWindow = (HWND)cp->wParam;
	}
	// taskbar button progress value
	else if (cp->message == WM_USER + 64) 
	{
		PostMessage(runningAppsListenerWindow, WM_APP + 1601, cp->wParam, cp->lParam);
	}
	// taskbar button progress state
	else if (cp->message == WM_USER + 65) 
	{		
		PostMessage(runningAppsListenerWindow, WM_APP + 1602, cp->wParam, cp->lParam);
	}	

	return CallNextHookEx(hhook, nCode, wParam, lParam);
}

BOOL WINAPI DllMain(
    HINSTANCE hinstDLL,  // handle to DLL module
    DWORD fdwReason,     // reason for calling function
    LPVOID lpReserved)  // reserved
{		
    return TRUE;  // Successful DLL_PROCESS_ATTACH.
}

BOOL SaveIconToFile(HICON hico, LPCTSTR szFileName, BOOL bAutoDelete)
{	
	PICTDESC pd = {sizeof(pd), PICTYPE_ICON};
	pd.icon.hicon = hico;

	CComPtr<IPicture> pPict = NULL;
	CComPtr<IStream>  pStrm = NULL;
	LONG cbSize = 0;

	if (!SUCCEEDED( ::CreateStreamOnHGlobal(NULL, TRUE, &pStrm) )) return FALSE;
	if (!SUCCEEDED( ::OleCreatePictureIndirect(&pd, IID_IPicture, bAutoDelete, (void**)&pPict) )) return FALSE;
	if (!SUCCEEDED( pPict->SaveAsFile( pStrm, TRUE, &cbSize ) )) return FALSE;
	
	// rewind stream to the beginning
	LARGE_INTEGER li = {0};
	pStrm->Seek(li, STREAM_SEEK_SET, NULL);

	// write to file
	HANDLE hFile = ::CreateFile(szFileName, GENERIC_WRITE, 0, NULL, CREATE_ALWAYS, 0, NULL);
	if( INVALID_HANDLE_VALUE != hFile )
	{		
		DWORD dwWritten = 0, dwRead = 0, dwDone = 0;
		BYTE  buf[4096];
		while( dwDone < static_cast<DWORD>( cbSize ) )
		{
			if( SUCCEEDED(pStrm->Read(buf, sizeof(buf), &dwRead)) )
			{
				::WriteFile(hFile, buf, dwRead, &dwWritten, NULL);
				if( dwWritten != dwRead )
					break;
				dwDone += dwRead;
			}
			else
				break;
		}

		_ASSERTE(dwDone == cbSize);
		::CloseHandle(hFile);

		return TRUE;
	}

	return FALSE;
}

void GetTempIconFolder(TCHAR* iconPath)
{
	// create app data folder
	::SHGetFolderPath(NULL, CSIDL_COMMON_APPDATA, NULL, SHGFP_TYPE_CURRENT, iconPath);
	PathAppend(iconPath, TEXT("DualMonitor\\TempIcons\\"));
	::SHCreateDirectoryEx(NULL, iconPath, NULL); // will fail if exists, but it's fine :)
}