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
        if (!string.IsNullOrEmpty(partie.Message))
            Console.Write(partie.Message);
        Console.WriteLine($"Statut de la partie : {partie.Status}");
        Console.WriteLine("=======================");
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

   //
}
