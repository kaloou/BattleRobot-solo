using System.Net;
using BattleRobot.Core;
using System.Net.Sockets;

namespace BattleRobot_solo_client;

public class Client : Joueur
{
    public override (int pv, int armure, int degats) ConfigRobot()
    {
        return Systeme.SaisirConfigRobot();
    }

    public void SeConnecter(IPAddress ip, int port)
    {
        this.EndPoint = new IPEndPoint(ip, port);

         this.Socket = new(
            this.EndPoint.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp);


             Socket.Connect(EndPoint);
    }

    public void EnvoyerAction(int action) => EnvoyerByte(action);

    public void RejouerPartie() => EnvoyerByte(1);

    public void QuitterPartie() => EnvoyerByte(2);
}
    
