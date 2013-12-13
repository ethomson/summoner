using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Summoner.Util
{
    [ComImport, Guid("000214f9-0000-0000-c000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IShellLink
    {
        uint GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cch, out IntPtr pfd, IntPtr fFlags);
        uint GetIDList([Out] out IntPtr ppidl);
        uint SetIDList(IntPtr pidl);
        uint GetDescription([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cch);
        uint SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
        uint GetWorkingDirectory([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cch);
        uint SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
        uint GetArguments([MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cch);
        uint SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
        uint GetHotkey(out short pwHotkey);
        uint SetHotkey(short wHotkey);
        uint GetShowCmd(out int piShowCmd);
        uint SetShowCmd(int iShowCmd);
        uint GetIconLocation([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cch, out int piIcon);
        uint SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
        uint SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, int dwReserved);
        uint Resolve(IntPtr hwnd, uint fFlags);
        uint SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
    }

    [ComImport, Guid("886d8eeb-8cf2-4446-8d02-cdba1dbdcf99"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPropertyStore
    {
        uint GetCount(out uint cProps);
        uint GetAt(uint iProp, out PROPERTYKEY pkey);
        uint GetValue(ref PROPERTYKEY key, out PROPVARIANT pv);
        uint SetValue(ref PROPERTYKEY key, ref PROPVARIANT pv);
        uint Commit();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct PROPERTYKEY
    {
        private Guid fmtid;
        private uint pid;

        public PROPERTYKEY(Guid fmtid, uint pid)
        {
            this.fmtid = fmtid;
            this.pid = pid;
        }
    }

    public enum VARTYPE : ushort
    {
        VT_EMPTY = 0x0000,
        VT_NULL = 0x0001,
        VT_I2 = 0x0002,
        VT_I4 = 0x0003,
        VT_R4 = 0x0004,
        VT_R8 = 0x0005,
        VT_CY = 0x0006,
        VT_DATE = 0x0007,
        VT_BSTR = 0x0008,
        VT_DISPATCH = 0x0009,
        VT_ERROR = 0x000A,
        VT_BOOL = 0x000B,
        VT_VARIANT = 0x000C,
        VT_UNKNOWN = 0x000D,
        VT_DECIMAL = 0x000E,
        VT_I1 = 0x0010,
        VT_UI1 = 0x0011,
        VT_UI2 = 0x0012,
        VT_UI4 = 0x0013,
        VT_I8 = 0x0014,
        VT_UI8 = 0x0015,
        VT_INT = 0x0016,
        VT_UINT = 0x0017,
        VT_VOID = 0x0018,
        VT_HRESULT = 0x0019,
        VT_PTR = 0x001A,
        VT_SAFEARRAY = 0x001B,
        VT_CARRAY = 0x001C,
        VT_USERDEFINED = 0x001D,
        VT_LPSTR = 0x001E,
        VT_LPWSTR = 0x001F,
        VT_RECORD = 0x0024,
        VT_INT_PTR = 0x0025,
        VT_UINT_PTR = 0x0026,
        VT_ARRAY = 0x2000,
        VT_BYREF = 0x4000
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct PROPVARIANT
    {
        [FieldOffset(0)]
        VARTYPE vt;
        [FieldOffset(8)]
        IntPtr p;

        public PROPVARIANT(string value)
        {
            vt = VARTYPE.VT_LPWSTR;
            p = Marshal.StringToCoTaskMemUni(value);
        }
    }

    [ComImport, Guid("00021401-0000-0000-c000-000000000046"), ClassInterface(ClassInterfaceType.None)]
    public class CShellLink
    {
    }

    public class ShellLink : IDisposable
    {
        private static readonly PROPERTYKEY PKEY_AppUserModelId =
            new PROPERTYKEY(new Guid("{9f4c2855-9f79-4b39-a8d0-e1d42De1D5f3}"), 5);

        private CShellLink shellLink;

        public ShellLink()
        {
            shellLink = new CShellLink();
        }

        private void EnsureSuccess(uint hr)
        {
            if (hr == 0)
            {
                return;
            }

            throw new COMException("COM error occurred", (int)hr);
        }

        public string Path
        {
            set
            {
                EnsureSuccess(((IShellLink)shellLink).SetPath(value));
            }
        }

        public string Arguments
        {
            set
            {
                EnsureSuccess(((IShellLink)shellLink).SetArguments(value));
            }
        }

        public void SetValue(PROPERTYKEY key, PROPVARIANT var)
        {
            EnsureSuccess(((IPropertyStore)shellLink).SetValue(key, var));
        }

        public string AppUserModelId
        {
            set
            {
                PROPVARIANT appIdProp = new PROPVARIANT(value);
                SetValue(PKEY_AppUserModelId, appIdProp);
            }
        }

        public void Commit()
        {
            ((IPropertyStore)shellLink).Commit();
        }

        public void SaveTo(string path)
        {
            ((IPersistFile)shellLink).Save(path, true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (shellLink != null)
            {
                while (Marshal.ReleaseComObject(shellLink) > 0)
                    ;

                shellLink = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        ~ShellLink()
        {
            Dispose(false);
        }
    }
}
