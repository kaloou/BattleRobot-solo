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

    public static (int pv, int armure, int degats) SaisirConfigRobot()
    {
        int total = Robot.PointsAAllouer;
        Console.WriteLine($"Répartissez {total} points (Pv +10, Armure +2, Dégâts +2 par point).");

        int pv = SaisirInt("Points en Pv", 0, total);
        int armure = total - pv > 0 ? SaisirInt("Points en Armure", 0, total - pv) : 0;
        int degats = total - pv - armure;

        Console.WriteLine($"Points en Dégâts : {degats} (le reste)");
        return (pv, armure, degats);
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
        Console.WriteLine("voici la partie : ");
        AfficherRobot("Robot serveur", partie.RobotServeur);
        AfficherRobot("Robot client ", partie.RobotClient);
        Console.WriteLine("Statut de la partie", partie.Status);
        
    }

    public static void AfficherEnAttente()
    {
        Console.WriteLine("En attente d'une connexion...");
    }

    private static void AfficherRobot(string titre, Robot? robot)
    {
        if (robot != null)
        {
            Console.WriteLine($"{titre} : {robot.Pv} PV | armure {robot.Armure} | dégâts {robot.Degats} | énergie {robot.Energie}");
        }
    }

   //
}
