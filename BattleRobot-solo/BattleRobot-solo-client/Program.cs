using System.Net;
using BattleRobot_solo_client;
using BattleRobot.Core;

Console.WriteLine("Hello, World! je suis le client");

Client client = new Client();

IPAddress ip = Systeme.SaisirIP("Veuillez l'adresse IP du serveur");
int port = Systeme.SaisirInt("Veuillez donner le port du serveur", 1, 65535);

client.SeConnecter(ip, port);

bool rejouer;
do
{
    Partie? partie;
    do
    {
        Console.Clear();
        (int pv, int armure, int degats, int dexterite) conf = client.ConfigRobot();
        client.EnvoyerObjet(conf);
        partie = client.RecevoirObjet<Partie>();

        if (partie == null)
        {
            Console.WriteLine("Serveur déconnecté.");
            return;
        }
        if (partie.Status == Partie.ConfigInvalide)
        {
            Console.WriteLine("Configuration invalide (la somme des points doit faire 10). Recommencez.");
        }
    } while (partie.Status == Partie.ConfigInvalide);

    client.Partie = partie;

    Partie? reponse = null;
    while (client.Partie.Status == Partie.EnCours)
    {
        // Boucle tant que le serveur répond "action invalide".
        do
        {
            Console.Clear();
            Systeme.AfficherPartie(client.Partie);
            if (reponse?.Status == Partie.ActionInvalide)
                Console.WriteLine("Action invalide (pas assez d'énergie ?), réessayez.");

            int action = Systeme.JouerAction();
            client.EnvoyerAction(action);

            Console.Clear();
            Systeme.AfficherPartie(client.Partie);
            reponse = client.RecevoirObjet<Partie>();

            if (reponse == null)
            {
                Console.WriteLine("Serveur déconnecté.");
                return;
            }
        } while (reponse.Status == Partie.ActionInvalide);

        client.Partie = reponse;
    }

    Console.Clear();
    Systeme.AfficherPartie(client.Partie);
    Console.WriteLine(client.Partie.Status switch
    {
        Partie.ClientAGagne => "Vous avez gagné !",
        Partie.ServeurAGagne => "Vous avez perdu.",
        _ => $"Partie terminée (status {client.Partie.Status})."
    });

    rejouer = Systeme.DemanderRejouer();
    if (rejouer)
        client.RejouerPartie();
    else
        client.QuitterPartie();
} while (rejouer);
