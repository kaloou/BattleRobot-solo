using System.IO;
using System.Net.Sockets;
using BattleRobot_solo_server;
using BattleRobot.Core;

Console.WriteLine("Hello, World! je suis le server");

int port = Systeme.SaisirInt("saisir-port", 1, 65535);

while (true)
{
    Server serv = new Server("server", port);

    try
    {
        serv.LancerHebergement();

        bool rejouer;
        do
        {
            serv.Partie = new Partie(); // nouvelle partie, même connexion

            (int pv, int armure, int degats) = serv.ConfigRobot();
            Robot robotServeur = serv.CreerRobot(pv, armure, degats);

            while (serv.RecevoirConfigClient() == false)
            {
                continue;
            }
            
            serv.Partie.RobotServeur = robotServeur;
            serv.Partie.Status = Partie.EnCours;
            serv.EnvoyerObjet(serv.Partie);

            while (serv.Partie.Status == Partie.EnCours)
            {
                while (serv.RecevoirAction() == false)
                {
                }
                if (serv.Partie.Status == Partie.EnCours)
                    serv.JouerTourServeur();
                serv.EnvoyerObjet(serv.Partie);
            }

            Console.WriteLine(serv.Partie.Status switch
            {
                Partie.ClientAGagne => "Le client a gagné.",
                Partie.ServeurAGagne => "Le serveur a gagné.",
                Partie.ConnexionPerdue => "Connexion perdue avec le client.",
                _ => $"Partie terminée (status {serv.Partie.Status})."
            });
            
            rejouer = serv.Partie.Status != Partie.ConnexionPerdue && serv.DemanderRejouer();
        } while (rejouer);
    }
    catch (Exception ex) when (ex is IOException or SocketException)
    {
        Console.WriteLine($"Connexion perdue avec le client ({ex.Message}).");
    }

    Console.WriteLine("Retour en attente d'une nouvelle connexion...\n");
}
