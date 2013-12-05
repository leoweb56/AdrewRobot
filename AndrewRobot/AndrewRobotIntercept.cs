using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Robocode;

namespace ART
{
    public class AndrewRobotIntercept : AdvancedRobot
    {
    }

//    public override void OnScannedRobot(ScannedRobotEvent e)
//        {
//            Intercept intercept = new Intercept();
//            intercept.calculate
//                (
//                    X,
//                    Y,
//                    e.,
//                    currentTargetPositionY,
//                    curentTargetHeading_deg,
//                    currentTargetVelocity,
//                    bulletPower,
//                    0 // Angular velocity
//                );

//// Helper function that converts any angle into  
//// an angle between +180 and -180 degrees.
//            double turnAngle = normalRelativeAngle
//                (intercept.bulletHeading_deg - robot.getGunHeading());

//// Move gun to target angle
//            robot.setTurnGunRight(turnAngle);

//            if (Math.abs(turnAngle) <= intercept.angleThreshold)
//            {
//                // Ensure that the gun is pointing at the correct angle
//                if (
//                    (intercept.impactPoint.x > 0) &&
//                    (intercept.impactPoint.x < getBattleFieldWidth()) &&
//                    (intercept.impactPoint.y > 0) &&
//                    (intercept.impactPoint.y < getBattleFieldHeight())
//                    )
//                {
//                    // Ensure that the predicted impact point is within 
//                    // the battlefield
//                    fire(bulletPower);
//                }
//            }
//        }
//    }

    public class CircularIntercept : Intercept
    {
        protected Point2D getEstimatedPosition(double time)
        {
            if (Math.Abs(angularVelocity_rad_per_sec)
                <= toRadians(0.1))
            {
                return getEstimatedPosition(time);
            }

            double initialTargetHeading = toRadians(targetHeading);
            double finalTargetHeading = initialTargetHeading +
                                        angularVelocity_rad_per_sec*time;
            double x = targetStartingPoint.x - targetVelocity/
                       angularVelocity_rad_per_sec*(Math.Cos(finalTargetHeading) -
                                                    Math.Cos(initialTargetHeading));
            double y = targetStartingPoint.y - targetVelocity/
                       angularVelocity_rad_per_sec*
                       (Math.Sin(initialTargetHeading) -
                        Math.Sin(finalTargetHeading));
            return new Point2D(x, y);
        }

        public double toRadians(double t)
        {
            return (t/180)*Math.PI;
        }

    }

    public class Intercept
    {
        public Point2D impactPoint = new Point2D(0, 0);
        public double bulletHeading_deg;

        protected Point2D bulletStartingPoint = new Point2D(0, 0);
        protected Point2D targetStartingPoint = new Point2D(0, 0);
        public double targetHeading;
        public double targetVelocity;
        public double bulletPower;
        public double angleThreshold;
        public double distance;

        protected double impactTime;
        protected double angularVelocity_rad_per_sec;

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
            angularVelocity_rad_per_sec =
                toRadians(angularVelocity_deg_per_sec);

            bulletStartingPoint.set(xb, yb);
            targetStartingPoint.set(xt, yt);

            targetHeading = tHeading;
            targetVelocity = vt;
            bulletPower = bPower;
            double vb = 20 - 3*bulletPower;

            double dX, dY;

            // Start with initial guesses at 10 and 20 ticks
            impactTime = getImpactTime(10, 20, 0.01);
            impactPoint = getEstimatedPosition(impactTime);

            dX = (impactPoint.x - bulletStartingPoint.x);
            dY = (impactPoint.y - bulletStartingPoint.y);

            distance = Math.Sqrt(dX*dX + dY*dY);

            bulletHeading_deg = toDegrees(Math.Atan2(dX, dY));
            angleThreshold = toDegrees
                (Math.Atan(100/distance));
        }

        private double toDegrees(double p)
        {
            return (180*p)/Math.PI;
        }

        public double toRadians(double p)
        {
            return (p/180)*Math.PI;
        }

        protected Point2D getEstimatedPosition(double time)
        {

            double x = targetStartingPoint.x +
                       targetVelocity*time*Math.Sin(toRadians(targetHeading));
            double y = targetStartingPoint.y +
                       targetVelocity*time*Math.Cos(toRadians(targetHeading));
            return new Point2D(x, y);
        }

        private double f(double time)
        {

            double vb = 20 - 3*bulletPower;

            Point2D targetPosition = getEstimatedPosition(time);
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

        public class Point2D
        {
            public double x, y;

            public Point2D(double x, double y)
            {
                // Contracts

                this.x = x;
                this.y = y;
            }

            internal void set(double xb, double yb)
            {
                this.x = xb;
                this.y = yb;
            }
        }
    }
}
