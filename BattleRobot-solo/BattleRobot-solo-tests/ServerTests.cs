using BattleRobot.Core;
using BattleRobot_solo_server;

namespace BattleRobot_solo_tests;

// VerifierAction est la seule logique de Server qui ne dépend pas du réseau/clavier :
// c'est elle qui est testable directement, sans simuler une connexion.
// Le tirage est remplacé pour rendre les tests prévisibles :
//   0.0  -> le tirage tombe toujours sous la chance : l'action réussit
//   0.99 -> le tirage dépasse toute chance sauf 100 % : l'action rate
public class ServerTests
{
    private static Server ServeurQuiReussit() => new() { Tirage = () => 0.0 };
    private static Server ServeurQuiRate() => new() { Tirage = () => 0.99 };

    [Fact]
    public void Action1_attaque_normale_retire_les_degats_et_reussit()
    {
        var server = ServeurQuiReussit();
        var attaquant = new Robot(); // Degats = 10
        var defenseur = new Robot(); // Pv = 100

        var resultat = server.VerifierAction(1, attaquant, defenseur);

        Assert.Equal(ResultatAction.Reussie, resultat);
        Assert.Equal(90, defenseur.Pv);
    }

    [Fact]
    public void Action2_puissante_refusee_sans_energie_et_rien_ne_change()
    {
        var server = ServeurQuiReussit(); // même avec un tirage gagnant : refus avant le tirage
        var attaquant = new Robot { Energie = 0 };
        var defenseur = new Robot();

        var resultat = server.VerifierAction(2, attaquant, defenseur);

        Assert.Equal(ResultatAction.Invalide, resultat);
        Assert.Equal(100, defenseur.Pv);
        Assert.Equal(0, attaquant.Energie);
    }

    [Fact]
    public void Action2_puissante_acceptee_coute_50_et_triple_les_degats()
    {
        var server = ServeurQuiReussit();
        var attaquant = new Robot { Energie = 50 };
        var defenseur = new Robot();

        var resultat = server.VerifierAction(2, attaquant, defenseur);

        Assert.Equal(ResultatAction.Reussie, resultat);
        Assert.Equal(0, attaquant.Energie);
        Assert.Equal(70, defenseur.Pv); // 10*3 - 0 armure
    }

    [Fact]
    public void Action3_defense_pose_larmure_temporaire()
    {
        var server = ServeurQuiReussit();
        var robot = new Robot();

        var resultat = server.VerifierAction(3, robot, new Robot());

        Assert.Equal(ResultatAction.Reussie, resultat);
        Assert.Equal(Robot.BonusDefenseTour, robot.ArmureTemporaire);
    }

    [Fact]
    public void Action4_recharge_ajoute_50_energie_meme_avec_un_mauvais_tirage()
    {
        var server = ServeurQuiRate(); // 0.99 * 100 = 99 < 100 : la recharge ne rate jamais
        var robot = new Robot();

        var resultat = server.VerifierAction(4, robot, new Robot());

        Assert.Equal(ResultatAction.Reussie, resultat);
        Assert.Equal(50, robot.Energie);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(-1)]
    public void Action_hors_de_1_a_4_est_refusee(int action)
    {
        var server = ServeurQuiReussit();

        Assert.Equal(ResultatAction.Invalide, server.VerifierAction(action, new Robot(), new Robot()));
    }

    [Fact]
    public void Attaque_ratee_ne_fait_aucun_degat_et_laisse_un_message()
    {
        var server = ServeurQuiRate();
        var defenseur = new Robot();

        var resultat = server.VerifierAction(1, new Robot(), defenseur, "Client");

        Assert.Equal(ResultatAction.Ratee, resultat);
        Assert.Equal(100, defenseur.Pv);
        Assert.Contains("Client a raté son attaque", server.Partie.MessageAction);
        Assert.Contains("75%", server.Partie.MessageAction);
    }

    [Fact]
    public void Defense_ratee_ne_donne_pas_darmure()
    {
        var server = ServeurQuiRate();
        var robot = new Robot();

        var resultat = server.VerifierAction(3, robot, new Robot(), "Serveur");

        Assert.Equal(ResultatAction.Ratee, resultat);
        Assert.Equal(0, robot.ArmureTemporaire);
    }

    [Fact]
    public void Puissante_ratee_coute_quand_meme_50_energie_sans_degats()
    {
        var server = ServeurQuiRate();
        var attaquant = new Robot { Energie = 50 };
        var defenseur = new Robot();

        var resultat = server.VerifierAction(2, attaquant, defenseur);

        Assert.Equal(ResultatAction.Ratee, resultat);
        Assert.Equal(0, attaquant.Energie);
        Assert.Equal(100, defenseur.Pv);
    }

    [Fact]
    public void Un_attaquant_plus_agile_reussit_la_ou_un_autre_echoue()
    {
        // Tirage de 0.80 -> 80. À dextérité égale (chance 75) : raté. Avec +2 de dextérité (chance 85) : réussi.
        var server = new Server { Tirage = () => 0.80 };

        Assert.Equal(ResultatAction.Ratee, server.VerifierAction(1, new Robot(), new Robot()));
        Assert.Equal(ResultatAction.Reussie, server.VerifierAction(1, new Robot { Dexterite = 2 }, new Robot()));
    }

    [Fact]
    public void CreerRobot_applique_les_memes_regles_que_le_constructeur_de_Robot()
    {
        var server = new Server();

        Assert.True(server.CreerRobot(3, 3, 4).VerifierConfiguration());
        Assert.True(server.CreerRobot(2, 2, 3, 3).VerifierConfiguration()); // avec dextérité
    }
}
