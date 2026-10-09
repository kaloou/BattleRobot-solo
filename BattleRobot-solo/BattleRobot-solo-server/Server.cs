using System.Net;
using BattleRobot.Core;
using System.Net.Sockets;

namespace BattleRobot_solo_server;

public class Server : Joueur
{
    public Func<double> Tirage { get; set; } = () => Random.Shared.NextDouble();

    public override (int pv, int armure, int degats, int dexterite) ConfigRobot() => Systeme.SaisirConfigRobot();

    public void LancerHebergement()
    {
        Console.WriteLine("Lancement de l'hébergement ...");
        EndPoint = new IPEndPoint(IPAddress.Any, this.Port);

        using var sondeIp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        sondeIp.Connect("8.8.8.8", 65530);

        var ipLocale = ((IPEndPoint)sondeIp.LocalEndPoint!).Address;
        Console.WriteLine($"IP à donner au client : {ipLocale} & port {Port}");

        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        socket.Bind(EndPoint);

        socket.Listen(1);

        Systeme.AfficherEnAttente();

        Socket = socket.Accept();
    }

    public Robot CreerRobot(int ptVie, int ptArmor, int ptForce, int ptDexterite = 0)
    {
        return new Robot(ptVie, ptArmor, ptForce, ptDexterite);
    }

    public bool RecevoirConfigClient()
    {
        (int pv, int armure, int degats, int dexterite) conf = RecevoirObjet<(int, int, int, int)>();
        Robot robot_client = CreerRobot(conf.pv, conf.armure, conf.degats, conf.dexterite);

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

    public ResultatAction VerifierAction(int action, Robot attaquant, Robot defenseur, string nom = "Joueur")
    {
        if (action < 1 || action > 4)
            return ResultatAction.Invalide;

        if (action == 2 && attaquant.Energie < Robot.CoutAttaquePuissante)
            return ResultatAction.Invalide;
        
        int chance = attaquant.ChanceReussite(action, defenseur);
        bool reussi = Tirage() * 100 < chance;
        
        if (reussi)
        {
            switch (action)
            {
                case 1:
                    attaquant.Attaquer(defenseur, false);
                    break;
                case 2:
                    attaquant.Attaquer(defenseur, true);
                    break;
                case 3:
                    attaquant.Defendre();
                    break;
                case 4:
                    attaquant.Recharger();
                    break;
            }
        }
        else if (action == 2)
        {
            // depense quand même l'énergie.
            attaquant.Energie -= Robot.CoutAttaquePuissante;
        }
        
        if (!reussi)
            Partie.MessageAction += $"{nom} a raté {ActionName(action)} ! Chance de réussite : {chance}%\n";

        return reussi ? ResultatAction.Reussie : ResultatAction.Ratee;
    }

    private static string ActionName(int action)
    {
        switch (action)
        {
        case 1 :
            return "son attaque";
        case 2 :
            return "son attaque puissante";
        case 3 :
            return "sa défense";
        default :
            return "son action";
        }
    }

    // Tour du client. Retourne false si l'action est invalide : le client doit rejouer.
    public bool RecevoirAction()
    {
        Partie.MessageAction = "";

        int? action = RecevoirByte();
        if (action == null)
        {
            Partie.Status = Partie.ConnexionPerdue;
            return true;
        }

        ResultatAction resultat = VerifierAction(action.Value, Partie.RobotClient!, Partie.RobotServeur!, "Client");

        if (resultat == ResultatAction.Invalide)
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

    // Tour du serveur. Redemande localement tant que l'action est invalide.
    public void JouerTourServeur()
    {
        ResultatAction resultat;
        do
        {
            int action = Systeme.JouerAction();
            resultat = VerifierAction(action, Partie.RobotServeur!, Partie.RobotClient!, "Serveur");
            if (resultat == ResultatAction.Invalide)
                Console.WriteLine("Action invalide, réessayez.");
        } while (resultat == ResultatAction.Invalide);

        Partie.Status = Partie.EnCours;
        if (Partie.RobotClient!.EstDetruit)
            Partie.Status = Partie.ServeurAGagne;
    }

    public bool DemanderRejouer() => RecevoirByte() == 1;

    public Server() { }

    public Server(string nom, int port) : base(nom, port)
    {
    }
}
