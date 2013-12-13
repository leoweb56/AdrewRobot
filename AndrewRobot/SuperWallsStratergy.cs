using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using Robocode;
using Robocode.Util;

namespace ART
{
    class SuperWallsStratergy : Strategy
    {
        private int dir = 1;
        private int enemyFireCount = 0;
        private Enemy NearTarget = null;
        private double Shots = 0;
        private double Hits = 0;
        private int margin = 60;

        public override void InitializeRobot()
        {
        }

        public override void ActionMove()
        {
            MyRobot.IsAdjustGunForRobotTurn = true;
            MyRobot.IsAdjustRadarForGunTurn = true;
            MyRobot.IsAdjustRadarForRobotTurn = true;

            MyRobot.SetTurnRadarRight(double.PositiveInfinity);

            if (MyRobot.Heading % 90 > 0)
            {
                MyRobot.TurnRight(-MyRobot.Heading);
            }

            if (Utils.IsNear(MyRobot.HeadingRadians, 5D) || Utils.IsNear(MyRobot.HeadingRadians, Math.PI))
            {
                MyRobot.Ahead((Math.Max(MyRobot.BattleFieldHeight - MyRobot.Y, MyRobot.Y) - margin) * dir);
            }
            else
            {
                MyRobot.Ahead((Math.Max(MyRobot.BattleFieldWidth - MyRobot.X, MyRobot.X) - margin) * dir);
            }
            MyRobot.TurnRight(90 * dir);

            //if (!EvitarParedes())
            //{
            //    dir *= -1;
            //    MyRobot.SetTurnRight(ang * dir);
            //    MyRobot.SetAhead(dist);
            //}
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

            MyRobot.IsAdjustGunForRobotTurn = true;

            if (NearTarget == null || (NearTarget.name != e.name && NearTarget.distance > e.distance) ||
                (NearTarget.name == e.name))
            {
                this.NearTarget = e;
            }
            else
            {
                return;
            }

            double absBearing = Utils.ToRadians(NearTarget.bearing) + MyRobot.HeadingRadians;

            double radarTurn = absBearing - MyRobot.RadarHeadingRadians;

            MyRobot.MaxVelocity = Rules.MAX_VELOCITY;

            MyRobot.SetTurnGunRightRadians(Utils.NormalRelativeAngle(absBearing - MyRobot.GunHeadingRadians));

            Shots++;
            DoFire(NearTarget);

            //MyRobot.SetTurnRadarRightRadians(Utils.NormalRelativeAngle(radarTurn)*2);
            if (MyRobot.Others == 1)
            {
                MyRobot.SetTurnRadarRightRadians(Utils.NormalRelativeAngle(radarTurn) * 2); // make the radar lock on
            }
            else
            {
                MyRobot.SetTurnRadarRight(double.PositiveInfinity);
            }

        }

        private void DoFire(Enemy e)
        {
            if (e.distance < 100 && MyRobot.Energy > 50)
            {
                MyRobot.SetFire(Rules.MAX_BULLET_POWER);
            }
            else if (e.distance < 200 && MyRobot.Energy > 50)
            {
                MyRobot.SetFire(2);
            }
            else
            {
                MyRobot.SetFire(1);
            }
        }

        public override void ActionMoveRadar()
        {
            MyRobot.SetTurnRadarRight(360);
        }

        public override void ActionHitByBullet(HitByBulletEvent ev)
        {
            MyRobot.TurnRadarRight(360);
        }

        public override void ActionBulletHit(BulletHitEvent ev)
        {
            Hits++;
        }
    }
}
