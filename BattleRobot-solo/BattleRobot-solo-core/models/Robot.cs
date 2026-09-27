namespace BattleRobot.Core;

public class Robot
{
    public const int PointsAAllouer = 10;
    public const int EnergieMax = 100;
    public const int CoutAttaquePuissante = 50;
    public const int GainRecharge = 50;
    public const int BonusDefenseTour = 10;

    public int Pv { get; set; } = 100;
    public int Armure { get; set; } = 0;
    public int Degats { get; set; } = 10;
    public int Energie { get; set; } = 0;
    public int ArmureTemporaire { get; set; } = 0;

    public bool EstDetruit => Pv <= 0;
    
    public Robot() { }
    
    public Robot(int ptPv, int ptArmure, int ptForce)
    {
        Pv = 100 + 10 * ptPv;
        Armure = 2 * ptArmure;
        Degats = 10 + 2 * ptForce;
    }
    
    public Robot(int pv, int armure, int degats, int energie)
    {
        Pv = pv;
        Armure = armure;
        Degats = degats;
        Energie = energie;
    }
    
    public bool VerifierConfiguration()
    {
        int dPv = Pv - 100;
        int dArmure = Armure;
        int dDegats = Degats - 10;

        return dPv >= 0 && dArmure >= 0 && dDegats >= 0
            && dPv % 10 == 0 && dArmure % 2 == 0 && dDegats % 2 == 0
            && dPv / 10 + dArmure / 2 + dDegats / 2 == PointsAAllouer;
    }
    
    public bool Attaquer(Robot cible, bool puissante)
    {
        bool possible = !puissante || Energie >= CoutAttaquePuissante;
        if (possible)
        {
            if (puissante)
            {
                Energie -= CoutAttaquePuissante;
            }
            cible.SubirDegat(Degats, puissante);
        }
        return possible; // false si pas assez de mana
    }

    public void SubirDegat(int degat, bool puissante = false)
    {
        int brut = puissante ? degat * 2 : degat;
        Pv -= Math.Max(1, brut - (Armure + ArmureTemporaire)); // minimum 1
        ArmureTemporaire = 0;
    }

    public void Defendre() => ArmureTemporaire = BonusDefenseTour;

    public void Recharger()
    {
        Energie = Math.Min(EnergieMax, Energie + GainRecharge);
    } 
    
    public void DebutDeTour()
    {
        ArmureTemporaire = 0; 
    } 
}
