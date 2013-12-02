using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Robocode;

//Andrew Robot Team
namespace ART
{
    public class AndrewRobot : Robot
    {
        public override void Run()
        {
            while (true)
            {
                Ahead(100);

                TurnRight(40);

                Ahead(100);

                TurnLeft(90);

            }
        }

        public override void OnScannedRobot(ScannedRobotEvent e)
        {
            // We fire the gun with bullet power = 1
            Fire(1);

            TurnLeft(50);
        }
    }
}
