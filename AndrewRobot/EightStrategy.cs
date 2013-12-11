using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Robocode;
using Robocode.Util;

namespace ART
{
    public class EightStrategy : Strategy
    {
        private double turn = 0;
        private int dir = 1;
        private int ang = 20;
        private int desplazar = 20;
        private double currentHeading = 1;

        public override void InitializeRobot()
        {
            currentHeading = MyRobot.Heading;
            MyRobot.MaxVelocity = Rules.MAX_VELOCITY;

        }

        public override void ActionMoveRadar()
        {
            MyRobot.SetTurnRadarRight(360);
        }

        public override void ActionMove()
        {
            MyRobot.IsAdjustGunForRobotTurn = true;
            MyRobot.IsAdjustRadarForGunTurn = true;
            MyRobot.IsAdjustRadarForRobotTurn = true;

            if (Math.Abs(currentHeading - MyRobot.Heading) < 10 && Math.Abs(currentHeading - MyRobot.Heading) > 1)
            {
                dir = -dir;
                currentHeading = MyRobot.Heading;
                MyRobot.TurnRight(ang * dir);
              
            }
            MyRobot.SetTurnRight(ang * dir);
            MyRobot.SetAhead(desplazar);


            MyRobot.Execute();
        }

        public override void ActionFire(Enemy e)
        {
            MyRobot.SetTurnGunRight(Utils.NormalRelativeAngleDegrees(MyRobot.Heading + e.bearing - MyRobot.GunHeading));
            MyRobot.Fire(1);
        }
    }
}
