using System;
using System.Runtime.InteropServices;
namespace GameRes
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DEVMODE
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string dmDeviceName;
        public short dmSpecVersion;
        public short dmDriverVersion;
        public short dmSize;
        public short dmDriverExtra;
        public int dmFields;
        public int dmPositionX;
        public int dmPositionY;
        public int dmDisplayOrientation;
        public int dmDisplayFixedOutput;
        public short dmColor;
        public short dmDuplex;
        public short dmYResolution;
        public short dmTTOption;
        public short dmCollate;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string dmFormName;
        public short dmLogPixels;
        public short dmBitsPerPel;
        public int dmPelsWidth;
        public int dmPelsHeight;
        public int dmDisplayFlags;
        public int dmDisplayFrequency;
        public int dmICMMethod;
        public int dmICMIntent;
        public int dmMediaType;
        public int dmDitherType;
        public int dmReserved1;
        public int dmReserved2;
        public int dmPanningWidth;
        public int dmPanningHeight;
    };
    class NativeMethods
    {
        [DllImport("user32.dll")]
        public static extern int EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);
        [DllImport("user32.dll")]
        public static extern int ChangeDisplaySettings(ref DEVMODE devMode, int flags);
        public const int ENUM_CURRENT_SETTINGS = -1;
        public const int CDS_UPDATEREGISTRY = 0x01;
        public const int CDS_TEST = 0x02;
        public const int DISP_CHANGE_SUCCESSFUL = 0;
        public const int DISP_CHANGE_RESTART = 1;
        public const int DISP_CHANGE_FAILED = -1;
        public const int DMDO_DEFAULT = 0;
        public const int DMDO_90 = 1;
        public const int DMDO_180 = 2;
        public const int DMDO_270 = 3;
    }
    public class PrmaryScreenResolution
    {
        static public Tuple<int,int,bool> GetResAndOr()
        {
            DEVMODE dm = GetDevMode();
            if (0 != NativeMethods.EnumDisplaySettings(null, NativeMethods.ENUM_CURRENT_SETTINGS, ref dm))
            {
                if(dm.dmDisplayOrientation == NativeMethods.DMDO_DEFAULT)
                {
                    return new Tuple<int, int, bool>(dm.dmPelsWidth, dm.dmPelsHeight, false);
                }
                else if(dm.dmDisplayOrientation == NativeMethods.DMDO_90)
                {
                    return new Tuple<int, int, bool>(dm.dmPelsHeight, dm.dmPelsWidth, true);

                }
                else
                {
                    return new Tuple<int, int, bool>(dm.dmPelsWidth, dm.dmPelsHeight, true);
                }
            }
            return null;
        }

                static public string ChangeResolution(int width, int height, int rotation)
        {
            DEVMODE dm = GetDevMode();
            if (0 != NativeMethods.EnumDisplaySettings(null, NativeMethods.ENUM_CURRENT_SETTINGS, ref dm))
            {
                if (rotation == 90)
                {
                    dm.dmDisplayOrientation = NativeMethods.DMDO_90;
                    dm.dmPelsWidth = height;
                    dm.dmPelsHeight = width;
                }
                else if (rotation == 180)
                {
                    dm.dmDisplayOrientation = NativeMethods.DMDO_180;
                    dm.dmPelsWidth = width;
                    dm.dmPelsHeight = height;
                }
                else if (rotation == 270)
                {
                    dm.dmDisplayOrientation = NativeMethods.DMDO_270;
                    dm.dmPelsWidth = height;
                    dm.dmPelsHeight = width;
                }
                else
                {
                    dm.dmDisplayOrientation = NativeMethods.DMDO_DEFAULT;
                    dm.dmPelsWidth = width;
                    dm.dmPelsHeight = height;
                }

                int iRet = NativeMethods.ChangeDisplaySettings(ref dm, NativeMethods.CDS_TEST);
                if (iRet == NativeMethods.DISP_CHANGE_FAILED)
                {
                    return "DISP_CHANGE_FAILED";
                }
                else
                {
                    iRet = NativeMethods.ChangeDisplaySettings(ref dm, NativeMethods.CDS_UPDATEREGISTRY);
                    switch (iRet)
                    {
                        case NativeMethods.DISP_CHANGE_SUCCESSFUL:
                            {
                                return "DISP_CHANGE_SUCCESSFUL";
                            }
                        case NativeMethods.DISP_CHANGE_RESTART:
                            {
                                return "DISP_CHANGE_RESTART";
                            }
                        default:
                            {
                                return "Failed " + iRet + " https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-changedisplaysettingsw#return-value";
                            }
                    }
                }
            }
            else
            {
                return "Failed EnumDisplaySettings";
            }
        }
        private static DEVMODE GetDevMode()
        {
            DEVMODE dm = new DEVMODE();
            dm.dmDeviceName = new String(new char[32]);
            dm.dmFormName = new String(new char[32]);
            dm.dmSize = (short)Marshal.SizeOf(dm);
            return dm;
        }
    }
}

