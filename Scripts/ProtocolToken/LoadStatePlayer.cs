using UdpKit;
using Photon.Bolt;


    public class LoadStatePlayer : IProtocolToken
    {
        public string data { get; set; }

        public void Read(UdpPacket packet)
        {
            data = packet.ReadString();
        }

        public void Write(UdpPacket packet)
        {
            packet.WriteString(data);
        }


    }
