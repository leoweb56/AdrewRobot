using System;
using System.Collections.Generic;
using System.Drawing;
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
        private double dist = 60; 
        private double margin = 20;

        private double shots = 1;
        private double hits = 1;

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

            if (EvitarParedes())
            {
                return;
            }

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
            Random randonGen = new Random();
            MyRobot.BodyColor = Color.FromArgb(randonGen.Next(255), randonGen.Next(255),
            randonGen.Next(255));
            MyRobot.BulletColor = Color.FromArgb(randonGen.Next(255), randonGen.Next(255),
            randonGen.Next(255));
            MyRobot.GunColor = Color.FromArgb(randonGen.Next(255), randonGen.Next(255),
            randonGen.Next(255));
            MyRobot.RadarColor = Color.FromArgb(randonGen.Next(255), randonGen.Next(255),
            randonGen.Next(255));
            MyRobot.ScanColor = Color.FromArgb(randonGen.Next(255), randonGen.Next(255),
            randonGen.Next(255));

            MyRobot.SetTurnGunRight(Utils.NormalRelativeAngleDegrees(MyRobot.Heading + e.bearing - MyRobot.GunHeading));
            DoFire(e);

        }

        private void DoFire(Enemy e)
        {
            if (e.distance < 100 && MyRobot.Energy > 50)
            {
                MyRobot.SetFire(Rules.MAX_BULLET_POWER);
                shots++;
            }
            else if (e.distance < 200 && MyRobot.Energy > 50)
            {
                MyRobot.SetFire(2);
                shots++;
            }
            else if(shots%hits < 2 || e.distance < 200 || e.speed == 0)
            {
                MyRobot.SetFire(1);
                shots++;
            }
        }

        public override void ActionBulletHit(BulletHitEvent ev)
        {
            hits++;
        }

    }
}
