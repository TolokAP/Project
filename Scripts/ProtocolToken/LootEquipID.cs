using UdpKit;


public class LootEquipID : Photon.Bolt.IProtocolToken
{
    public string EquipID { get; set; }
    public ulong NetworkID { get; set; }


    public void Read(UdpPacket packet)
    {
  
        EquipID = packet.ReadString();

        NetworkID = packet.ReadULong();
    }

    public void Write(UdpPacket packet)
    {
        packet.WriteString(EquipID);

        packet.WriteULong(NetworkID);

    }
}
