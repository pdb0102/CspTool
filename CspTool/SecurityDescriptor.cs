using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace amaic.de.csptool
{
    [StructLayout(LayoutKind.Explicit)]
    struct ACL
    {
        [FieldOffset(0)]
        byte AclRevision;
        [FieldOffset(1)]
        byte Sbz1;
        [FieldOffset(2)]
        short AclSize;
        [FieldOffset(4)]
        short AceCount;
        [FieldOffset(6)]
        short Sbz2;
    }

    [StructLayout(LayoutKind.Explicit)]
    struct SECURITY_DESCRIPTOR
    {
        [FieldOffset(0)]
        public byte Revision;
        [FieldOffset(1)]
        public byte Sbz1;
        [FieldOffset(2)]
        public short Control;
        [FieldOffset(4)]
        public IntPtr Owner;
        [FieldOffset(8)]
        public IntPtr Group;
        [FieldOffset(12)]
        public IntPtr Sacl;
        [FieldOffset(16)]
        public IntPtr Dacl;
    }
}
