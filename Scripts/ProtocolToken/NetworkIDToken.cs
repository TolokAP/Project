using UdpKit;


public class NetworkIDToken : Photon.Bolt.IProtocolToken
{
    public ulong NetworkID { get; set; }
    public int Hand { get; set; }

    public int ID { get; set; }
    public void Read(UdpPacket packet)
    {
        NetworkID = packet.ReadULong();
        Hand = packet.ReadInt();
        ID = packet.ReadInt();
    }

    public void Write(UdpPacket packet)
    {
        packet.WriteULong(NetworkID);
        packet.WriteInt(Hand);
        packet.WriteInt(ID);
    }
}
