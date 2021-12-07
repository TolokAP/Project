using UdpKit;


public class TargetPosition : Photon.Bolt.IProtocolToken
{

   
    public ulong NetworkID { get; set; }
    public ulong NetworkIDOwner { get; set; }


    public int Damage { get; set; }

    public void Read(UdpPacket packet)
    {

        NetworkID = packet.ReadULong();
        NetworkIDOwner = packet.ReadULong();

        Damage = packet.ReadInt();


    }

    public void Write(UdpPacket packet)
    {

        packet.WriteULong(NetworkID);
        packet.WriteULong(NetworkIDOwner);

        packet.WriteInt(Damage);
    }

   
}
