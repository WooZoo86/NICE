﻿using NICE.Hardware;
using NICE.Layer2;
using NICE.Layer3;
using NICE.Layer4;

namespace NetEmu
{
    static class Program
    {
        static readonly string ETH01 = "eth0/1";
        static readonly string FA01 = "fa0/1";
        static readonly string FA02 = "fa0/2";
        
        static void Main()
        {
            Vlan.Register(1, "DEFAULT");

            /*
             * PC1 ---  eth0/1
             *       |
             *       | fa0/1
             *      SW1
             *       |  fa0/2
             *       |
             *       | fa0/1
             *      SW2
             *       |  fa0/1
             *       |
             * PC2 --- eth0/1
             */
            
            var pc1 = new EthernetDevice();
            var pc2 = new EthernetDevice();

            var sw1 = new EthernetSwitch();
            var sw2 = new EthernetSwitch();
            
            //pc ports
            pc1[ETH01].Init();
            pc2[ETH01].Init();
            
            //Connection from sw1 to pc1
            sw1[FA01].Init();
            
            //Connection from sw2 to pc2
            sw2[FA01].Init();

            //Connection from sw1 to sw2
            sw1[FA02].Init();
            sw2[FA02].Init();
            
            //Connect the pcs to the switches
            pc1[ETH01].ConnectTo(sw1[FA01]);
            pc2[ETH01].ConnectTo(sw2[FA01]);
            
            //Connect the switches to each other
            sw1[FA02].ConnectTo(sw2[FA02]);
            
            //Set the ports from pc to switch to access vlan 1
            sw1.SetPort(FA01, EthernetSwitch.AccessMode.ACCESS, Vlan.Get(1));
            sw2.SetPort(FA01, EthernetSwitch.AccessMode.ACCESS, Vlan.Get(1));
            
            //Set the ports from switch to switch to trunk
            sw1.SetPort(FA02, EthernetSwitch.AccessMode.TRUNK, null);
            sw2.SetPort(FA02, EthernetSwitch.AccessMode.TRUNK, null);
            
            pc1[ETH01].Send(new EthernetFrame(pc2[ETH01], pc1[ETH01], null, EtherType.IPv4, new RawLayer3Packet(new byte[100]), false));
            
            pc2[ETH01].Disconnect();
        }
    }
}