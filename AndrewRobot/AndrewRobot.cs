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
        public double ang = 30;
        public double dist = 70;
        public int sentido = 1;
        public string tar_name;

        /****** parametros de Intercept */
        private Coordinate impactPoint = new Coordinate(0, 0);
        public double bulletHeading_deg;

        protected Coordinate bulletStartingPoint = new Coordinate();
        protected Coordinate targetStartingPoint = new Coordinate();
        public double targetHeading;
        public double targetVelocity;
        public double bulletPower;
        public double angleThreshold;
        public double distance;

        /******************************************/

        //TODO hacer una clase que guarde cuantas veces le di, cuantos tiros le dispare, vida, distancia, etc. 
        //TODO Tomar de todos los enemigos, tomar el q le di mas con menos disparos. Si hay mas de uno, tomar el mas cercano.


        public override void Run()
        {
            IsAdjustGunForRobotTurn = true;
            IsAdjustRadarForGunTurn = true;
            IsAdjustRadarForRobotTurn = true;
            //TurnRadarLeft(360);
            //TurnGunRight(45);
            while (true)
            {





                //SetTurnRight(ang * sentido);
                //SetAhead(dist);
                //sentido *= -1;
                SetTurnRadarLeft(360);

                Execute();
            }


        }

        public override void OnScannedRobot(ScannedRobotEvent eu)
        {
            /*
            UpdateTarget(eu);

            var e = GetTarget();

            TurnGunRight(Utils.NormalRelativeAngleDegrees(e.Bearing + Heading - GunHeading));

            if (e.Distance < 100)
            {
                Fire(3);
            }
            else if (e.Distance < 120)
            {
                Fire(2);
            }
            else
            {
                Fire(1);
            }
             */
            Console.WriteLine("grados Heading + eu.Bearing: " + (Heading + eu.Bearing).ToString());
            Console.WriteLine("eu.Heading: " + eu.Heading);
            var Xi = Math.Sin(Utils.ToRadians(eu.Heading));
            var Yi = Math.Cos(Utils.ToRadians(eu.Bearing));

            this.calculate(X, Y, Xi, Yi, eu.Heading, eu.Velocity, 1, 0);

            TurnGunRight(Utils.NormalRelativeAngleDegrees(bulletHeading_deg ));
            if (
                (impactPoint.x > 0) &&
                (impactPoint.x < BattleFieldWidth) &&
                (impactPoint.y > 0) &&
                (impactPoint.y < BattleFieldHeight)
                )
            {
                // Ensure that the predicted impact point is within 
                // the battlefield
                Fire(1);
            }

            //var Xf = Xi + 20*eu.Velocity*

           

        }

        public void calculate(

            // Initial bullet position x coordinate 
            double xb,
            // Initial bullet position y coordinate
            double yb,
            // Initial target position x coordinate
            double xt,
            // Initial target position y coordinate
            double yt,
            // Target heading
            double tHeading,
            // Target velocity
            double vt,
            // Power of the bullet that we will be firing
            double bPower,
            // Angular velocity of the target
            double angularVelocity_deg_per_sec
            )
        {
            //angularVelocity_rad_per_sec =
            //    Math.toRadians(angularVelocity_deg_per_sec);

            bulletStartingPoint.set(xb, yb);
            targetStartingPoint.set(xt, yt);

            targetHeading = tHeading;
            targetVelocity = vt;
            bulletPower = bPower;
            double vb = 20 - 3*bulletPower;

            double dX, dY;

            // Start with initial guesses at 10 and 20 ticks
            var impactTime = getImpactTime(10, 20, 0.01);
            impactPoint = getEstimatedPosition(impactTime);

            dX = (impactPoint.x - bulletStartingPoint.x);
            dY = (impactPoint.y - bulletStartingPoint.y);

            distance = Math.Sqrt(dX*dX + dY*dY);

            bulletHeading_deg = Utils.ToDegrees(Math.Atan2(dX, dY));
            //angleThreshold = Math.toDegrees
            //    (Math.atan(ROBOT_RADIUS/distance));
        }


        protected Coordinate getEstimatedPosition(double time)
        {

            double x = targetStartingPoint.x +
                       targetVelocity*time*Math.Sin(Utils.ToRadians(targetHeading));
            double y = targetStartingPoint.y +
                       targetVelocity * time * Math.Cos(Utils.ToRadians(targetHeading));
            return new Coordinate(x, y);
        }

        private double f(double time)
        {

            double vb = 20 - 3*bulletPower;

            Coordinate targetPosition = getEstimatedPosition(time);
            double dX = (targetPosition.x - bulletStartingPoint.x);
            double dY = (targetPosition.y - bulletStartingPoint.y);

            return Math.Sqrt(dX*dX + dY*dY) - vb*time;
        }

        private double getImpactTime(double t0,
            double t1, double accuracy)
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

        protected class Coordinate
        {
            public double x;
            public double y;

            public Coordinate(double X, double Y)
            {
                x = X;
                y = Y;
            }

            public Coordinate()
            {
                x = 0;
                y = 0;
            }


            public void set(double X, double Y)
            {
                x = X;
                y = Y;
            }
        }

        /*
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
            TurnRight(Utils.NormalRelativeAngleDegrees(90 + e.Heading));

            dist *= -1;
            Ahead(dist);
            Scan();
        }

        public override void OnHitWall(HitWallEvent evnt)
        {
            TurnLeft(90);
        }
       */
    }
}

