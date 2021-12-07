
using UdpKit;


public class AuthResultToken : Photon.Bolt.IProtocolToken
{
    public static AuthResultToken Invalid = new AuthResultToken();
    

    public int Ticket { get; private set; }
    public string Id { get; private set; }

    
    public bool create { get;  set; }
    private static int CurrentTicket = 0;

    public AuthResultToken()
    {
        Ticket = CurrentTicket++;
        Id = "";


    }
    public AuthResultToken(string id) : this()
    {
        Id = id;
    }
   

    public void Read(UdpPacket packet)
    {
        Ticket = packet.ReadInt();
        Id = packet.ReadString();
        create = packet.ReadBool();
     
    }

    public void Write(UdpPacket packet)
    {
        packet.WriteInt(Ticket);
        packet.WriteString(Id);
        packet.WriteBool(create);
       

    }
}
