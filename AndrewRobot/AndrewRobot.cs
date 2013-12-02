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
                Ahead(5000);

                TurnRight(90);
            }
        }

        public override void OnScannedRobot(ScannedRobotEvent e)
        {
            // We fire the gun with bullet power = 1
            Fire(1);
        }
    }
}
