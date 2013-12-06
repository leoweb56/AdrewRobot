using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Robocode;
//Andrew Robot Team
using Robocode.Util;

namespace ART
{
    public class AndrewRobot : AdvancedRobot
    {
        public Dictionary<string, ScannedRobotEvent> tagerts = new Dictionary<string, ScannedRobotEvent>(); 
        public bool isFire = false;
        public double ang = 45;
        public double dist = 100;
        public int sentido = 1;
        public string tar_name;

        public double margin = 100;


        #region Variables Intercept

        private Coordinate bulletStartingPoint = null;
        private Coordinate targetStartingPoint = null;
        private double targetVelocity;
        private double targetHeading;

        #endregion
        //TODO hacer una clase que guarde cuantas veces le di, cuantos tiros le dispare, vida, distancia, etc. 
        //TODO Tomar de todos los enemigos, tomar el q le di mas con menos disparos. Si hay mas de uno, tomar el mas cercano.


        public override void Run()
        {
            IsAdjustGunForRobotTurn = true;
            IsAdjustRadarForGunTurn = true;
            IsAdjustRadarForRobotTurn = true;
            while (true)
            {
                Console.WriteLine("x: " + X);
                Console.WriteLine("y: " + Y);
                if ((X >= BattleFieldWidth - (margin + Height)))
                {
                    //TurnRight(-Heading);
                    TurnRight(Utils.NormalRelativeAngleDegrees(-Heading - 90));
                    Ahead(dist - 20);
                    //WaitFor(new TurnCompleteCondition(this));
                }
                else if (Y >= BattleFieldHeight - (margin + Height))
                {
                    TurnRight(Utils.NormalRelativeAngleDegrees(-Heading - 180));
                    Ahead(dist - 20);
                    //WaitFor(new TurnCompleteCondition(this));
                }
                else if (Y <= (margin + Height))
                {
                    //TurnRight(-Heading);
                    TurnRight(Utils.NormalRelativeAngleDegrees(-Heading));
                    Ahead(dist - 20);
                    //WaitFor(new TurnCompleteCondition(this));
                }
                else if (X <= (margin + Height))
                {
                    TurnRight(Utils.NormalRelativeAngleDegrees(-Heading + 90));
                    Ahead(dist - 20);
                    //WaitFor(new TurnCompleteCondition(this));
                }
                else
                {
                    sentido *= -1;
                    SetTurnRight(ang*sentido);
                    SetAhead(dist);

                }

                SetTurnRadarLeft(360);

                Execute();
            }


        }

        public override void OnScannedRobot(ScannedRobotEvent eu)
        {
            double absBearingDeg = Utils.NormalRelativeAngleDegrees( (Heading + eu.Bearing - GunHeading));
            TurnGunRight(absBearingDeg);
            GoFire(eu.Distance);

            //if (absBearingDeg < 0) absBearingDeg += 360;


//            // yes, you use the _sine_ to get the X value because 0 deg is North
//            var x = X + Math.Sin(Utils.ToRadians(absBearingDeg)) * eu.Distance;

//            // yes, you use the _cosine_ to get the Y value because 0 deg is North
//            var y = Y + Math.Cos(Utils.ToRadians(absBearingDeg)) * eu.Distance;

//            bulletStartingPoint = new Coordinate(X, Y);
//            targetStartingPoint = new Coordinate(x, y);
//            targetVelocity = eu.Velocity;
//            targetHeading = eu.Heading;

//            var impactPoint = getEstimatedPosition(getImpactTime(0, 10, 0.1));

//            var dX = (impactPoint.x - bulletStartingPoint.x);
//            var dY = (impactPoint.y - bulletStartingPoint.y);

//            var distance = Math.Sqrt(dX * dX + dY * dY);

//            var bulletHeading_deg = Utils.ToDegrees(Math.Atan2(dX, dY));


//            double turnAngle = Utils.NormalRelativeAngle(bulletHeading_deg + GunHeading);

//// Move gun to target angle
//            TurnGunRight(bulletHeading_deg);

//            var angleThreshold = Utils.ToDegrees(Math.Atan(20/distance));

            //if (Math.Abs(turnAngle) <= angleThreshold)
            //{
            //GoFire(eu.Distance);
                
            //}


            Execute();

            //UpdateTarget(eu);

            //var e = GetTarget();

            //TurnGunRight(Utils.NormalRelativeAngleDegrees(e.Bearing + Heading - GunHeading));

            //if (e.Distance < 100)
            //{
            //    Fire(3);
            //}
            //else if (e.Distance < 120)
            //{
            //    Fire(2);
            //}
            //else
            //{
            //    Fire(1);
            //}



        }

        private void GoFire(double distance)
        {
            if (distance < 100)
            {
                Fire(3);
            }
            else if (distance < 120)
            {
                Fire(2);
            }
            else
            {
                Fire(1);
            }
        }

        private double f(double time)
        {
            var bulletPower = 1;
            double vb = 20 - 3 * bulletPower;

            Coordinate targetPosition = getEstimatedPosition(time);
            double dX = (targetPosition.x - bulletStartingPoint.x);
            double dY = (targetPosition.y - bulletStartingPoint.y);

            return Math.Sqrt(dX * dX + dY * dY) - vb * time;
        }

        private double getImpactTime(double t0, double t1, double accuracy)
        {

            double X = t1;
            double lastX = t0;
            int iterationCount = 0;
            double lastfX = f(lastX);

            while ((Math.Abs(X - lastX) >= accuracy) &&
                   (iterationCount < 15))
            {

                iterationCount++;
                double fX = f(X);

                if ((fX - lastfX) == 0.0) break;

                double nextX = X - fX*(X - lastX)/(fX - lastfX);
                lastX = X;
                X = nextX;
                lastfX = fX;
            }

            return X;
        }

        protected Coordinate getEstimatedPosition(double time)
        {
            double x = targetStartingPoint.x +
                       targetVelocity*time*Math.Sin(Utils.ToRadians(targetHeading));
            double y = targetStartingPoint.y +
                       targetVelocity*time*Math.Cos(Utils.ToRadians(targetHeading));
            return new Coordinate(x, y);
        }

        private ScannedRobotEvent GetTarget()
        {
            ScannedRobotEvent aux = null;

            foreach (var target in tagerts.Values)
            {
                if (aux == null)
                {
                    aux = target;
                }
                else
                {
                    if (aux.Distance > target.Distance)
                    {
                        aux = target;
                    }
                }
            }

            return aux;
        }

        private void UpdateTarget(ScannedRobotEvent e)
        {
            if (tagerts.Keys.Contains(e.Name))
            {
                tagerts[e.Name] = e;
            }
            else
            {
                tagerts.Add(e.Name,e);
            }
        }

        public override void OnRobotDeath(RobotDeathEvent evnt)
        {
            tagerts.Remove(evnt.Name);
        }

        public override void OnHitByBullet(HitByBulletEvent e)
        {
            TurnRight(Utils.NormalRelativeAngleDegrees(45 - e.Heading));

            //Ahead(dist);
            //dist *= -1;
            //TurnLeft(45);
            //Ahead(dist);
            //Scan();
        }

        public override void OnHitWall(HitWallEvent evnt)
        {
            //TurnLeft(90);
        }
    }
}

