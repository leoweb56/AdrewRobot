using System;
using System.Collections.Generic;
using System.Data;
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

            if (Utils.IsNear(MyRobot.HeadingRadians, 0D) || Utils.IsNear(MyRobot.HeadingRadians, Math.PI))
            {
                MyRobot.Ahead((Math.Max(MyRobot.BattleFieldHeight - MyRobot.Y, MyRobot.Y) - 28) * dir);
            }
            else
            {
                MyRobot.Ahead((Math.Max(MyRobot.BattleFieldWidth - MyRobot.X, MyRobot.X) - 28) * dir);
            }
            MyRobot.TurnRight(90 * dir);

            //if (!EvitarParedes())
            //{
            //    dir *= -1;
            //    MyRobot.SetTurnRight(ang * dir);
            //    MyRobot.SetAhead(dist);
            //}
        }

        //private bool EvitarParedes()
        //{
        //    if ((MyRobot.X >= MyRobot.BattleFieldWidth - (margin + MyRobot.Height)))
        //    {
        //        MyRobot.TurnRight(Utils.NormalRelativeAngleDegrees(-MyRobot.Heading - 90));
        //        MyRobot.Ahead(dist - 20);
        //        return true;
        //    }
        //    else if (MyRobot.Y >= MyRobot.BattleFieldHeight - (margin + MyRobot.Height))
        //    {
        //        MyRobot.TurnRight(Utils.NormalRelativeAngleDegrees(-MyRobot.Heading - 180));
        //        MyRobot.Ahead(dist - 20);
        //        return true;
        //    }
        //    else if (MyRobot.Y <= (margin + MyRobot.Height))
        //    {
        //        MyRobot.TurnRight(Utils.NormalRelativeAngleDegrees(-MyRobot.Heading));
        //        MyRobot.Ahead(dist - 20);
        //        return true;
        //    }
        //    else if (MyRobot.X <= (margin + MyRobot.Height))
        //    {
        //        MyRobot.TurnRight(Utils.NormalRelativeAngleDegrees(-MyRobot.Heading + 90));
        //        MyRobot.Ahead(dist - 20);
        //        return true;
        //    }
        //    return false;
        //}

        public override void ActionFire(Enemy e)
        {
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

            if (Utils.IsNear(MyRobot.HeadingRadians, 0D) || Utils.IsNear(MyRobot.HeadingRadians, Math.PI))
            {
                MyRobot.SetAhead((Math.Max(MyRobot.BattleFieldHeight - MyRobot.Y, MyRobot.Y) - 28)*dir);
            }
            else
            {
                MyRobot.SetAhead((Math.Max(MyRobot.BattleFieldWidth - MyRobot.X, MyRobot.X) - 28)*dir);
            }


            MyRobot.MaxVelocity = Rules.MAX_VELOCITY;

            MyRobot.SetTurnGunRightRadians(Utils.NormalRelativeAngle(absBearing - MyRobot.GunHeadingRadians));

            Shots++;
            DoFire(NearTarget);

            MyRobot.SetTurnRadarRightRadians(Utils.NormalRelativeAngle(radarTurn)*2);
            //if (MyRobot.Others == 1)
            //{
            //    MyRobot.SetTurnRadarRightRadians(Utils.NormalRelativeAngle(radarTurn)*2); // Make the radar lock on
            //}
            //else
            //{
            //    MyRobot.SetTurnRadarRight(double.PositiveInfinity);
            //}

            //MyRobot.TurnGunRight(Utils.NormalRelativeAngleDegrees(MyRobot.Heading + e.bearing - MyRobot.GunHeading));

            //DoFire(e);
            MyRobot.Execute();

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
            //MyRobot.TurnRight(90);
            MyRobot.TurnRadarRight(360);
            dir = -dir;
            if (Utils.IsNear(MyRobot.HeadingRadians, 0D) || Utils.IsNear(MyRobot.HeadingRadians, Math.PI))
            {
                MyRobot.Ahead((Math.Min(MyRobot.BattleFieldHeight - MyRobot.Y, 60) - 28) * dir);
            }
            else
            {
                MyRobot.Ahead((Math.Min(MyRobot.BattleFieldWidth - MyRobot.X, 60) - 28) * dir);
            }
        }

        public override void ActionBulletHit(BulletHitEvent ev)
        {
            Hits++;
        }
    }
}
