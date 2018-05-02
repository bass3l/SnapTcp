using System;
using System.Collections.Generic;
using PcapDotNet.Core;
using PcapDotNet.Packets;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.Transport;


namespace SnapTCP
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!. It's SnapTCP!");

            IList<LivePacketDevice> allDevices = LivePacketDevice.AllLocalMachine;            

            AdaptersService adaptersService = new AdaptersService();
            adaptersService.PrintDevices(allDevices);

            if(allDevices.Count == 0) 
                return;

            int deviceIndex = 0;

            do {
                Console.WriteLine($"Enter the interface number to capture packets on: (1 - {allDevices.Count})");
                string deviceIndexString = Console.ReadLine();
                if(!int.TryParse(deviceIndexString, out deviceIndex) || deviceIndex < 1 || deviceIndex > allDevices.Count)
                    deviceIndex = 0;
            }while(deviceIndex == 0);


            PacketDevice selectedDevice = allDevices[deviceIndex - 1];

            using(PacketCommunicator communicator = 
                    selectedDevice.Open(65536, PacketDeviceOpenAttributes.Promiscuous, 1000))
            {

                using(BerkeleyPacketFilter filter = communicator.CreateFilter("tcp and ip"))
                {
                    communicator.SetFilter(filter);
                }

                Console.WriteLine($"Listening On: {selectedDevice.Description} ...");
                communicator.ReceivePackets(0, PacketHandler);
            }

        }

        private static void PacketHandler(Packet packet){

            IpV4Datagram ip = packet.Ethernet.IpV4;
            TcpDatagram tcp = ip.Tcp;

            if(ip.Destination.ToString() == "192.168.43.1" || ip.Source.ToString() == "192.168.43.1"){
                Console.WriteLine("#PACKET:");
                Console.WriteLine(packet.Timestamp.ToString("yyyy-MM-dd hh:mm:ss.fff") + $" Length: {packet.Length}");
                Console.WriteLine($"{ip.Source}:{tcp.SourcePort} -> {ip.Destination}:{tcp.DestinationPort}");
                Console.WriteLine($"Sequence Number: {tcp.SequenceNumber}");
                Console.WriteLine($"Payload Length: {tcp.PayloadLength}");
                Console.WriteLine($"Payload: {tcp.Decode(System.Text.Encoding.ASCII)}");
            }
        }
    }
}
