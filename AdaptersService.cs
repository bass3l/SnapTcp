using System;
using System.Collections.Generic;
using PcapDotNet.Core;

namespace SnapTCP
{
    public class AdaptersService
    {
        public void PrintDevices(IList<LivePacketDevice> allDevices)
        {
            Console.WriteLine("=================== Available Devices =========================");

            if(allDevices.Count == 0)
                Console.WriteLine("No interfaces found! Make sure WinPcap is installed.");


            for(int i = 0; i < allDevices.Count; i++)
            {
                LivePacketDevice device = allDevices[i];

                Console.WriteLine((i+1) + ". " + device.Name);
                
                PrintFullInformationAboutDevice(device);
            }

            Console.WriteLine("==============================================================");
        }

        private void PrintFullInformationAboutDevice(IPacketDevice device){

            if(device.Description != null)
                Console.WriteLine($"\tDescription: {device.Description}");

            Console.WriteLine($"\tLoopback: " + (((device.Attributes & DeviceAttributes.Loopback) == DeviceAttributes.Loopback)
                                                ? "yes" : "no"));

            Console.WriteLine();

            foreach(DeviceAddress address in device.Addresses){

                if(address.Address != null){
                    Console.WriteLine($"\tAddress Family: {address.Address.Family}");
                    Console.WriteLine($"\tAddress: {address.Address}");                    
                }

                if(address.Netmask != null)
                    Console.WriteLine($"\tNetmask: {address.Netmask}");

                if(address.Broadcast != null)
                    Console.WriteLine($"\tBroadcast: {address.Broadcast}");
                
                if(address.Destination != null)
                    Console.WriteLine($"\tDestination: {address.Destination}");

                Console.WriteLine();
            }
        }
    }
}
