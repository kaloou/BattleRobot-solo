using System.Net;

namespace BattleRobot.Core;

public static class Systeme
{
    // Saisies
    public static int SaisirInt(string invite, int min, int max)
    {
        int valeur;
        bool valide;
        do
        {
            Console.Write($"{invite} [{min}-{max}] : ");
            valide = int.TryParse(Console.ReadLine(), out valeur) && valeur >= min && valeur <= max;
            if (!valide)
            {
                Console.WriteLine("Saisie invalide.");
            }
        } while (!valide);
        return valeur;
    }

    public static IPAddress SaisirIP(string invite = "Adresse IP du serveur")
    {
        IPAddress? ip;
        bool valide;
        do
        {
            Console.Write($"{invite} : ");
            valide = IPAddress.TryParse(Console.ReadLine(), out ip);
            if (!valide)
            {
                Console.WriteLine("Adresse IP invalide.");
            }
        } while (!valide);
        return ip!;
    }

    public static (int pv, int armure, int degats, int dexterite) SaisirConfigRobot()
    {
        int reste = Robot.PointsAAllouer;
        Console.WriteLine($"Répartissez {reste} points (Pv +10, Armure +2, Dégâts +2, Dextérité +1 par point).");

        int pv = SaisirInt("Points en Pv", 0, reste);
        reste -= pv;

        int armure = reste > 0 ? SaisirInt("Points en Armure", 0, reste) : 0;
        reste -= armure;

        int degats = reste > 0 ? SaisirInt("Points en Dégâts", 0, reste) : 0;
        reste -= degats;

        Console.WriteLine($"Points en Dextérité : {reste} (le reste)");
        return (pv, armure, degats, reste);
    }

    public static int JouerAction()
    {
        Console.WriteLine("=======================");
        Console.WriteLine("1 : Attaquer\n2 : Attaque puissante\n3 : Défendre\n4 : Recharger");
        return SaisirInt("Votre action", 1, 4);
    }
    
    public static bool DemanderRejouer()
    {
        Console.WriteLine("1 : Rejouer\n2 : Quitter");
        return SaisirInt("Votre choix", 1, 2) == 1;
    }

    // Affichage
    public static void AfficherPartie(Partie partie)
    {
        Console.WriteLine("=======================");
        Console.WriteLine("voici la partie : ");
        AfficherRobot("Robot serveur", partie.RobotServeur);
        AfficherRobot("Robot client ", partie.RobotClient);
        if (!string.IsNullOrEmpty(partie.MessageAction))
            Console.Write(partie.MessageAction);
        Console.WriteLine($"Statut de la partie : {partie.Status}");
        Console.WriteLine("=======================");

        Console.WriteLine("""
                           _______             _______        _______             _______
                          |@|@|@|@|           |@|@|@|@|      |@|@|@|@|           |@|@|@|@|
                          |@|@|@|@|   _____   |@|@|@|@|      |@|@|@|@|   _____   |@|@|@|@|
                          |@|@|@|@| /\_T_T_/\ |@|@|@|@|      |@|@|@|@| /\_T_T_/\ |@|@|@|@|
                          |@|@|@|@||/\ T T /\||@|@|@|@|      |@|@|@|@||/\ T T /\||@|@|@|@|
                           ~/T~~T~||~\/~T~\/~||~T~~T\~        ~/T~~T~||~\/~T~\/~||~T~~T\~
                            \|__|_| \(-(O)-)/ |_|__|/          \|__|_| \(-(O)-)/ |_|__|/
                            _| _|    \\8_8//    |_ |_          _| _|    \\8_8//    |_ |_
                          |(@)]   /~~[_____]~~\   [(@)|      |(@)]   /~~[_____]~~\   [(@)|
                            ~    (  |       |  )    ~          ~    (  |       |  )    ~
                                [~` ]       [ '~]                  [~` ]       [ '~]
                                |~~|         |~~|                  |~~|         |~~|
                                |  |         |  |                  |  |         |  |
                               _<\/>_       _<\/>_                _<\/>_       _<\/>_
                              /_====_\     /_====_\              /_====_\     /_====_\
                          """
            );

        if (partie.RobotServeur == null || partie.RobotClient == null)
            return;
        Console.WriteLine();
        Console.Write(new string(' ', 9));
        EcrireBarre(partie.RobotServeur.Pv, partie.RobotServeur.PvMax);
        Console.Write(new string(' ', 25));
        EcrireBarre(partie.RobotClient.Pv, partie.RobotClient.PvMax);
        Console.WriteLine();
        Console.WriteLine($"Serveur {Math.Max(partie.RobotServeur.Pv, 0)}/{partie.RobotServeur.PvMax}".PadRight(35)
                          + $"Client {Math.Max(partie.RobotClient.Pv, 0)}/{partie.RobotClient.PvMax}");
    }

    public static void AfficherEnAttente()
    {
        Console.WriteLine("En attente d'un joueur...");
    }

    private static void AfficherRobot(string titre, Robot? robot)
    {
        if (robot != null)
        {
            Console.WriteLine($"{titre} : {robot.Pv} PV | armure {robot.Armure} | dégâts {robot.Degats} | dextérité {robot.Dexterite} | énergie {robot.Energie}");
        }
    }

    private static void EcrireBarre(int pv, int pvMax)
    {
        int pleines = pvMax <= 0 ? 0 : Math.Clamp((int)Math.Ceiling(pv * 10.0 / pvMax), 0, 10);

        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write(new string('█', pleines));
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write(new string('░', 10 - pleines));
        Console.ResetColor();
    }
}
