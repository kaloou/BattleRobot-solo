using System.Net;
using System.Net.Sockets;
using System.Text.Json;

namespace BattleRobot.Core;

public abstract class Joueur
{
    public string Nom { get; }
    public int Port { get; set; }
    public IPEndPoint? EndPoint { get; set; }
    public Socket? Socket { get; protected set; }
    public Partie Partie { get; set; } = new();

    private StreamWriter? _writer;
    private StreamReader? _reader;

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

    protected void OuvrirFlux()
    {
        var flux = new NetworkStream(Socket!);
        _writer = new StreamWriter(flux) { AutoFlush = true };
        _reader = new StreamReader(flux);
    }

    public void EnvoyerLigne(string ligne) => _writer!.WriteLine(ligne);
    
    public string? RecevoirLigne() => _reader!.ReadLine();

    public void EnvoyerObjet<T>(T objet)
    {
        string obj = JsonSerializer.Serialize(objet, Options);
        EnvoyerLigne(obj);
    }

    public T? RecevoirObjet<T>()
    {
        string? ligne = RecevoirLigne();
        return ligne == null ? default : JsonSerializer.Deserialize<T>(ligne, Options);
    }
    
    public void EnvoyerByte(int valeur) => _writer!.Write((char)valeur);
    
    public int? RecevoirByte()
    {
        int valeur = _reader!.Read();
        return valeur == -1 ? null : valeur;
    }
}
