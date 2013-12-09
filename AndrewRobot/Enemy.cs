using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Robocode;
using Robocode.Util;

namespace ART
{
    public class Enemy
    {
        public String name;
        public double bearing, heading, speed, x, y, distance, changehead;
        public long ctime; //game time that the scan was produced
        public bool live; //is the enemy alive?


        public Enemy(ScannedRobotEvent eu, AdvancedRobot myRobot)
        {
            // TODO: Complete member initialization
            this.name = eu.Name;
            this.speed = eu.Velocity;
            this.live = eu.Energy > 0;
            this.heading = eu.Heading;
            this.distance = eu.Distance;
            this.ctime = eu.Time;
            this.bearing = eu.Bearing;
            this.changehead = eu.Energy;
            this.x = myRobot.X + (Math.Sin(eu.BearingRadians + myRobot.HeadingRadians) * eu.Distance);
            this.y = myRobot.Y + (Math.Cos(eu.BearingRadians + myRobot.HeadingRadians) * eu.Distance);
        }
    }
}
