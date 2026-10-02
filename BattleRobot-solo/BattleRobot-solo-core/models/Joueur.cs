using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace BattleRobot.Core;

public abstract class Joueur
{
    public string Nom { get; }
    public int Port { get; set; }
    public IPEndPoint? EndPoint { get; set; }
    public Socket? Socket { get; protected set; }
    public Partie Partie { get; set; } = new();

    private static readonly JsonSerializerOptions Options = new()
    {
        IncludeFields = true,
        PropertyNameCaseInsensitive = true
    };

    protected Joueur(string nom = "", int port = 0)
    {
        Nom = nom;
        Port = port;
    }

    public abstract (int pv, int armure, int degats) ConfigRobot();
    
    public void EnvoyerObjet<T>(T objet)
    {
        string json = JsonSerializer.Serialize(objet, Options);
        byte[] donnees = Encoding.UTF8.GetBytes(json);
        Socket!.Send(donnees);
    }
    
    public T? RecevoirObjet<T>()
    {
        byte[] buffer = new byte[4096];
        int nbOctets = Socket!.Receive(buffer);
        if (nbOctets == 0)
            return default;

        string json = Encoding.UTF8.GetString(buffer, 0, nbOctets);
        return JsonSerializer.Deserialize<T>(json, Options);
    }

    
    public void EnvoyerByte(int valeur)
    {
        Socket!.Send(new[] { (byte)valeur });
    }

    public int? RecevoirByte()
    {
        byte[] buffer = new byte[1];
        int nbOctets = Socket!.Receive(buffer);
        return nbOctets == 0 ? null : buffer[0];
    }
}
