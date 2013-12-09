using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Robocode;
using Robocode.Util;

namespace ART
{
    class SuperWallsStratergy : Strategy
    {
        private double ang = 45;
        private double dist = 100;
        private double margin = 50;
        private int sentido = 1;
        private string tar_name;

        public override void InitializeRobot()
        {
        }

        public override void ActionMove()
        {
            MyRobot.IsAdjustGunForRobotTurn = true;
            MyRobot.IsAdjustRadarForGunTurn = true;
            MyRobot.IsAdjustRadarForRobotTurn = true;

            if (!EvitarParedes())
            {
                sentido *= -1;
                MyRobot.SetTurnRight(ang * sentido);
                MyRobot.SetAhead(dist);
            }
        }

        private bool EvitarParedes()
        {
            if ((MyRobot.X >= MyRobot.BattleFieldWidth - (margin + MyRobot.Height)))
            {
                MyRobot.TurnRight(Utils.NormalRelativeAngleDegrees(-MyRobot.Heading - 90));
                MyRobot.Ahead(dist - 20);
                return true;
            }
            else if (MyRobot.Y >= MyRobot.BattleFieldHeight - (margin + MyRobot.Height))
            {
                MyRobot.TurnRight(Utils.NormalRelativeAngleDegrees(-MyRobot.Heading - 180));
                MyRobot.Ahead(dist - 20);
                return true;
            }
            else if (MyRobot.Y <= (margin + MyRobot.Height))
            {
                MyRobot.TurnRight(Utils.NormalRelativeAngleDegrees(-MyRobot.Heading));
                MyRobot.Ahead(dist - 20);
                return true;
            }
            else if (MyRobot.X <= (margin + MyRobot.Height))
            {
                MyRobot.TurnRight(Utils.NormalRelativeAngleDegrees(-MyRobot.Heading + 90));
                MyRobot.Ahead(dist - 20);
                return true;
            }
            return false;
        }

        public override void ActionFire(Enemy e)
        {
            MyRobot.TurnGunRight(Utils.NormalRelativeAngleDegrees(MyRobot.Heading + e.bearing - MyRobot.GunHeading));

            if (e.distance < 100 && MyRobot.Energy > 50)
            {
                MyRobot.Fire(e.changehead/4);
            }
            else if (e.distance < 200 && MyRobot.Energy > 50)
            {
                MyRobot.Fire(e.changehead/10);
            }
            else
            {
                MyRobot.Fire(1);
            }
            
        }

        public override void ActionMoveRadar()
        {
            MyRobot.SetTurnRadarRight(360);
        }
    }
}
