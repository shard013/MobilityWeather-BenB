using System;
using System.Runtime.InteropServices;
using System.Text;

// Unicode input support
// From Stack overflow user Jcl - https://stackoverflow.com/a/9627255

namespace MobilityWeather_BenB.Weather
{
    public static class Unicode
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        static extern bool ReadConsoleW(IntPtr hConsoleInput, [Out] byte[]
           lpBuffer, uint nNumberOfCharsToRead, out uint lpNumberOfCharsRead,
           IntPtr lpReserved);


        public static IntPtr GetWin32InputHandle()
        {
            const int STD_INPUT_HANDLE = -10;
            IntPtr inHandle = GetStdHandle(STD_INPUT_HANDLE);
            return inHandle;
        }

        public static string Readline()
        {
            const int bufferSize = 1024;
            var buffer = new byte[bufferSize];

            ReadConsoleW(GetWin32InputHandle(), buffer, bufferSize, out uint charsRead, (IntPtr)0);
            // -2 to remove ending \n\r
            int nc = ((int)charsRead - 2) * 2;
            var b = new byte[nc];
            for (var i = 0; i < nc; i++)
                b[i] = buffer[i];

            var utf8enc = Encoding.UTF8;
            var unicodeenc = Encoding.Unicode;
            return utf8enc.GetString(Encoding.Convert(unicodeenc, utf8enc, b));
        }
    }
}
