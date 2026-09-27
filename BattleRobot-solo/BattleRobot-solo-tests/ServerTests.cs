using BattleRobot.Core;
using BattleRobot_solo_server;

namespace BattleRobot_solo_tests;

// VerifierAction est la seule logique de Server qui ne dépend pas du réseau/clavier :
// c'est elle qui est testable directement, sans simuler une connexion.
public class ServerTests
{
    [Fact]
    public void Action1_attaque_normale_retire_les_degats_et_reussit()
    {
        var server = new Server();
        var attaquant = new Robot(); // Degats = 10
        var defenseur = new Robot(); // Pv = 100

        bool valide = server.VerifierAction(1, attaquant, defenseur);

        Assert.True(valide);
        Assert.Equal(90, defenseur.Pv);
    }

    [Fact]
    public void Action2_puissante_refusee_sans_energie_et_rien_ne_change()
    {
        var server = new Server();
        var attaquant = new Robot { Energie = 0 };
        var defenseur = new Robot();

        bool valide = server.VerifierAction(2, attaquant, defenseur);

        Assert.False(valide);
        Assert.Equal(100, defenseur.Pv);
        Assert.Equal(0, attaquant.Energie);
    }

    [Fact]
    public void Action2_puissante_acceptee_coute_50_et_double_les_degats()
    {
        var server = new Server();
        var attaquant = new Robot { Energie = 50 };
        var defenseur = new Robot();

        bool valide = server.VerifierAction(2, attaquant, defenseur);

        Assert.True(valide);
        Assert.Equal(0, attaquant.Energie);
        Assert.Equal(80, defenseur.Pv); // 10*2 - 0 armure
    }

    [Fact]
    public void Action3_defense_pose_larmure_temporaire()
    {
        var server = new Server();
        var robot = new Robot();

        bool valide = server.VerifierAction(3, robot, new Robot());

        Assert.True(valide);
        Assert.Equal(Robot.BonusDefenseTour, robot.ArmureTemporaire);
    }

    [Fact]
    public void Action4_recharge_ajoute_50_energie()
    {
        var server = new Server();
        var robot = new Robot();

        bool valide = server.VerifierAction(4, robot, new Robot());

        Assert.True(valide);
        Assert.Equal(50, robot.Energie);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(-1)]
    public void Action_hors_de_1_a_4_est_refusee(int action)
    {
        var server = new Server();
        bool valide = server.VerifierAction(action, new Robot(), new Robot());

        Assert.False(valide);
    }

    [Fact]
    public void CreerRobot_applique_les_memes_regles_que_le_constructeur_de_Robot()
    {
        var server = new Server();
        Robot robot = server.CreerRobot(3, 3, 4);

        Assert.True(robot.VerifierConfiguration());
    }
}
