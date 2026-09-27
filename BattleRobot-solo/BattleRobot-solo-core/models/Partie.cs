namespace BattleRobot.Core;

public class Partie
{
    public const int EnCours = 1;
    public const int Configuration = 2;
    public const int ClientAGagne = -1;
    public const int ServeurAGagne = -2;
    public const int ConnexionPerdue = -3;
    public const int ConfigInvalide = -10;
    public const int ActionInvalide = -11;

    public int Status { get; set; } = Configuration;
    
    public Robot? RobotServeur { get; set; }
    public Robot? RobotClient { get; set; }

    public bool TourServeur { get; set; } = true;
    
    public Partie(){}
}
