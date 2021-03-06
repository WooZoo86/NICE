// ReSharper disable InconsistentNaming

using System;
using System.Collections.Generic;
using System.Linq;
using NICE.Abstraction;
using NICE.Foundation;

namespace NICE.Protocols.Ethernet.MAC
{
    public class MACHeader : IByteable<MACHeader>
    {
        public MACAddress Dst;
        public ushort EtherType;
        public MACAddress Src;

        MACHeader IByteable<MACHeader>.FromBytes(byte[] bytes)
        {
            return FromBytes(bytes);
        }

        public byte[] ToBytes()
        {
            var list = new List<byte>();
            list.AddRange(Dst.ToBytes());
            list.AddRange(Src.ToBytes());
            list.AddRange(BitConverter.GetBytes(EtherType).Reverse());
            return list.ToArray();
        }

        public static MACHeader FromBytes(byte[] bytes)
        {
            var header = new MACHeader();

            if (bytes.Length != 14) throw new ArgumentException("MAC Header must be 14 bytes long!", nameof(bytes));

            header.Dst = MACAddress.FromBytes(bytes.Take(6).ToArray());
            header.Src = MACAddress.FromBytes(bytes.Skip(6).Take(6).ToArray());
            header.EtherType = bytes.Skip(12).Take(2).ToArray().ToUshort();

            return header;
        }
    }
}