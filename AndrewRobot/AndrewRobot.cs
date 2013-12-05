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

                //Ahead(-10);
                //TurnGunLeft(ang);
                
                //isFire = false;
                //TurnGunRight(20);
                SetTurnRight(ang * sentido);
                SetAhead(dist);
                sentido *= -1;
                //Ahead(-50);
                SetTurnRadarLeft(360);
                //TurnRadarLeft(30);
                //TurnGunLeft(45);
                //SetAhead(100);

                Execute();
            }
            

        }

        public override void OnScannedRobot(ScannedRobotEvent eu)
        {
            UpdateTarget(eu);

            var e = GetTarget();
            
            //ang = Utils.NormalRelativeAngleDegrees(e.Bearing + Heading - GunHeading);
            //ang = Utils.NormalRelativeAngleDegrees(e.Bearing + Heading);
            TurnGunRight(Utils.NormalRelativeAngleDegrees(e.Bearing + Heading - GunHeading));
            //Fire(1);

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
            
            //TurnGunRight(ang + 3);
            //Fire(1);
            //TurnGunRight(ang - 3);
            //Fire(1);
            //Scan();
            //TurnRadarLeft(ang);
            //Console.WriteLine(e.Bearing);
            //TurnGunLeft(e.Bearing);

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
            TurnRight(Utils.NormalRelativeAngleDegrees(90 + e.Heading));

            //Ahead(dist);
            dist *= -1;
            //TurnLeft(45);
            Ahead(dist);
            Scan();
        }

        public override void OnHitWall(HitWallEvent evnt)
        {
            TurnLeft(90);
        }
    }
}

