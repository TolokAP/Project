using UdpKit;


public class LootEquipID : Photon.Bolt.IProtocolToken
{
    public string EquipID { get; set; }
   


    public void Read(UdpPacket packet)
    {
  
        EquipID = packet.ReadString();

      
    }

    public void Write(UdpPacket packet)
    {
        packet.WriteString(EquipID);

 
    }
}
