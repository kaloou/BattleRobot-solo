using System.Net;
using BattleRobot.Core;
using System.Net.Sockets;

namespace BattleRobot_solo_server;

public class Server : Joueur
{
    public override (int pv, int armure, int degats) ConfigRobot() => Systeme.SaisirConfigRobot();

    public void LancerHebergement()
    {
        Console.WriteLine("Lancement de l'hébergement ...");
        EndPoint = new IPEndPoint(IPAddress.Any, this.Port);

        using var sondeIp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        sondeIp.Connect("8.8.8.8", 65530);
        var ipLocale = ((IPEndPoint)sondeIp.LocalEndPoint!).Address;
        Console.WriteLine($"IP à donner au client : {ipLocale}, port {Port}");

        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        socket.Bind(EndPoint);

        socket.Listen(1);

        Systeme.AfficherEnAttente();

        Socket = socket.Accept();
    }

    public Robot CreerRobot(int ptVie, int ptArmor, int ptForce)
    {
        Robot Robot = new Robot(ptVie, ptArmor, ptForce);
        
        return Robot;
        
    }

    public bool RecevoirConfigClient()
    {
        (int pv, int armure, int degats) conf = RecevoirObjet<(int, int, int)>();
        Robot robot_client = CreerRobot(conf.pv, conf.armure, conf.degats);

        if (!robot_client.VerifierConfiguration())
        {
            Console.WriteLine("config invalide");
            Partie.Status = Partie.ConfigInvalide;
            EnvoyerObjet(Partie);
            return false;
        }
        Partie.RobotClient = robot_client;
        return true;
    }
        
    public void TransmettrePartie()
    {
        //
    }

    public bool VerifierAction(int action, Robot attaquant, Robot defenseur)
    {
        switch (action)
        {
            case 1 : 
                attaquant.Attaquer(defenseur, false);
                break;
            case 2 :
                return attaquant.Attaquer(defenseur, true);
            case 3 :
                attaquant.Defendre();
                break;
            case 4:
                attaquant.Recharger();
                break;
            default:
                return false;
        }
        return true;
    }


    public bool RecevoirAction()
    {
        int? action = RecevoirByte();
        if (action == null)
        {
            Partie.Status = Partie.ConnexionPerdue;
            return true;
        }

        bool valide = VerifierAction(action.Value, Partie.RobotClient!, Partie.RobotServeur!);

        if (!valide)
        {
            Partie.Status = Partie.ActionInvalide;
            EnvoyerObjet(Partie);
            return false;
        }

        Partie.Status = Partie.EnCours;
        if (Partie.RobotServeur!.EstDetruit)
            Partie.Status = Partie.ClientAGagne;
        return true;
    }

    // Redemande localement au joueur serveur tant que son action est invalide.
    public void JouerTourServeur()
    {
        bool valide;
        do
        {
            int action = Systeme.JouerAction();
            valide = VerifierAction(action, Partie.RobotServeur!, Partie.RobotClient!);
            if (!valide)
                Console.WriteLine("Action invalide (pas assez d'énergie ?), réessayez.");
        } while (!valide);

        Partie.Status = Partie.EnCours;
        if (Partie.RobotClient!.EstDetruit)
            Partie.Status = Partie.ServeurAGagne;
    }
    public bool DemanderRejouer() => RecevoirByte() == 1;

    public Server (){}
    public Server(string nom, int port) : base(nom, port)
    {
        
    }
}