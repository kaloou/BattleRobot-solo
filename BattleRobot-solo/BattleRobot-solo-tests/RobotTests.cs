using BattleRobot.Core;

namespace BattleRobot_solo_tests;

public class RobotTests
{
    [Fact]
    public void Robot_par_defaut_a_les_valeurs_de_base()
    {
        var r = new Robot();
        Assert.Equal((100, 0, 10, 0), (r.Pv, r.Armure, r.Degats, r.Energie));
    }

    [Theory]
    [InlineData(10, 0, 0)]  // tout en Pv
    [InlineData(0, 10, 0)]  // tout en armure
    [InlineData(0, 0, 10)]  // tout en dégâts
    [InlineData(3, 3, 4)]   // réparti
    public void Configuration_valide_acceptee(int pv, int armure, int force)
    {
        Assert.True(new Robot(pv, armure, force).VerifierConfiguration());
    }

    [Theory]
    [InlineData(9, 0, 0)]     // somme 9
    [InlineData(5, 5, 1)]     // somme 11
    [InlineData(-5, 15, 0)]   // somme 10 mais points négatifs
    public void Configuration_invalide_refusee(int pv, int armure, int force)
    {
        Assert.False(new Robot(pv, armure, force).VerifierConfiguration());
    }

    [Fact]
    public void Attaque_normale_retire_les_degats_reduits_par_larmure()
    {
        var cible = new Robot(0, 10, 0); // armure 20
        new Robot().Attaquer(cible, false); // 10 dégâts

        Assert.Equal(99, cible.Pv); // minimum 1, jamais 0 dégât
    }

    [Fact]
    public void Attaque_puissante_refusee_sans_energie_et_rien_ne_change()
    {
        var attaquant = new Robot { Energie = 49 };
        var cible = new Robot();

        Assert.False(attaquant.Attaquer(cible, true));
        Assert.Equal(100, cible.Pv);
        Assert.Equal(49, attaquant.Energie);
    }

    [Fact]
    public void Attaque_puissante_coute_50_et_double_les_degats()
    {
        var attaquant = new Robot { Energie = 50 };
        var cible = new Robot { Armure = 5 };

        Assert.True(attaquant.Attaquer(cible, true));
        Assert.Equal(0, attaquant.Energie);
        Assert.Equal(85, cible.Pv); // 10*2 - 5
    }

    [Fact]
    public void Recharge_plafonne_a_100()
    {
        var r = new Robot { Energie = 80 };
        r.Recharger();
        Assert.Equal(100, r.Energie);
    }

    [Fact]
    public void Defense_protege_seulement_la_prochaine_attaque()
    {
        var cible = new Robot();
        var attaquant = new Robot { Degats = 20 };
        cible.Defendre();

        attaquant.Attaquer(cible, false);
        Assert.Equal(90, cible.Pv); // 20 - 10 d'armure temporaire

        attaquant.Attaquer(cible, false);
        Assert.Equal(70, cible.Pv); // défense déjà consommée
    }

    [Fact]
    public void Robot_detruit_a_0_pv_ou_moins()
    {
        var cible = new Robot { Pv = 5 };
        new Robot().Attaquer(cible, false);
        Assert.True(cible.EstDetruit);
    }
}
