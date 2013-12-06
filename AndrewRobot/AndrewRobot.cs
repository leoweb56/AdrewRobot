using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Robocode;
using Robocode.Util;
using System.Drawing;

//Andrew Robot Team
namespace ART
{
    public class AndrewRobot : AdvancedRobot
    {
        double previousEnergy = 102;
        int movementDirection = 1;
        int gunDirection = 1;

        public override void Run()
        {
            IsAdjustGunForRobotTurn = true;
            IsAdjustRadarForGunTurn = true;
            
            
            RadarColor = Color.Green;
            
            //TurnGunLeft(20);
            //TurnGunLeft(Utils.NormalAbsoluteAngleDegrees(Heading));
            //TurnRight(Utils.NormalAbsoluteAngleDegrees(Heading));
            //TurnRight(Heading);
            //SetTurnRadarLeft(360);
            while (true)
            {
                TurnRadarLeft(999999);
                //(100);
                //SetBack(100);
            }
            //if (Heading > 0)
            //{
            //    TurnGunLeft(Heading);
            //}
            //else
            //{
            //    TurnGunLeft(-Heading);
            //}
            //while (true)
            //{
                
            //    TurnRadarRight(360);
            ////    TurnRadarLeftRadians(2 * Math.PI);
            ////    Ahead(100);

            ////    TurnRight(40);

            ////    Ahead(100);

            ////    TurnLeft(90);

            //}
        }

        public override void OnScannedRobot(ScannedRobotEvent e)
        {
            
            TurnRight(90 + e.Bearing);

                SetAhead(100);
                //SetBack(100);

                TurnGunRight(Utils.NormalRelativeAngleDegrees(e.Bearing + (Heading - GunHeading)));
            if (e.Distance < 120)
            {
                Fire(3);
            }
            else if (e.Distance < 130)
            {
                Fire(2);
            }else
            {
                Fire(1);
            }
           
            Scan();
                //double changeInEnergy = previousEnergy - e.Energy;
                //if (changeInEnergy != 0)
                //{
                //    var ang = e.Bearing;
                //    TurnRight(ang + 90);
                //    Ahead(e.Distance);
                //    TurnLeft(ang);
                //    Fire(1);

                //    previousEnergy = e.Energy;
                //}
            //TurnRight(e.Bearing + 90);
            //if (Heading > 0)
            //{
            //    TurnGunLeft(Heading);
            //}
            //else
            //{
            //    TurnGunLeft(-Heading);
            //}
            
            //Fire(1);
                //TurnRight(e.Bearing + 90 - 30 * movementDirection);
                // We fire the gun with bullet power = 1
                // If the bot has small energy drop,
                // assume it fired
                //double changeInEnergy = previousEnergy - e.Energy;
                //if (changeInEnergy != 0 /* > 0 && changeInEnergy <= 3*/)
                //{
                //    TurnGunLeft(e.Bearing + Heading + 90);
                //    Fire(1);
                //    // Dodge!
                //    movementDirection = (-1) * movementDirection;
                //    Ahead((e.Distance / 4 + 25) * movementDirection);

                //    // When a bot is spotted,
                //    // sweep the gun and radar
                //    gunDirection = (-1) * gunDirection;
                //    TurnGunRight(e.Bearing);// * gunDirection);
                //    //TurnRight(e.Bearing);
                //    // Fire directly at target
                    

                //    // Track the energy level
                //    previousEnergy = e.Energy;
                //}
            //else
            //{
            //    Ahead(-e.Distance);
            //    TurnGunLeft(30);
            //    Fire(1);
            //}
        }

        public override void OnHitWall(HitWallEvent evnt)
        {
            TurnRight(90);

            Ahead(100);

            Scan();
        }

        public override void OnHitByBullet(HitByBulletEvent evnt)
        {
            //TurnRight(90);
            
        }
    }
}
