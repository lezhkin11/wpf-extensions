namespace WpfExtensions.Comparers
{
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class StringLogicalComparer : Comparer<string>
    {
        [DllImport("Shlwapi.dll", CharSet = CharSet.Unicode)]
        private static extern int StrCmpLogicalW(string x, string y);

        /// <inheritdoc />
        public override int Compare(string x, string y)
        {
            return StrCmpLogicalW(x, y);
        }
    }
}
