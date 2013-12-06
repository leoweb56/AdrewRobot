using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Robocode;

namespace ART
{
    internal class AndrewRobotAntiGravity : AdvancedRobot
    {
        /**
	 * run: SnippetBot's default behavior
	 */
        private Hashtable targets; //all enemies are stored in the hashtable
        private Enemy target; //our current enemy
        private double PI = Math.PI; //just a constant
        private int direction = 1; //direction we are heading... 1 = forward, -1 = backwards
        private double firePower; //the power of the shot we will be using
        private double midpointstrength = 0; //The strength of the gravity point in the middle of the field
        private int midpointcount = 0; //Number of turns since that strength was changed.

        public override void Run()
        {
            targets = new Hashtable();
            target = new Enemy();
            target.distance = 100000; //initialise the distance so that we can select a target
            SetColors(Color.Black, Color.Red, Color.Green); //sets the colours of the robot
            //the next two lines mean that the turns of the robot, gun and radar are independant
            IsAdjustGunForRobotTurn = true;
            IsAdjustRadarForGunTurn = true;
            TurnRadarRightRadians(2*PI); //turns the radar right around to get a view of the field
            while (true)
            {
                antiGravMove(); //Move the bot
                doFirePower(); //select the fire power to use
                doScanner(); //Oscillate the scanner over the bot
                doGun();
                Console.WriteLine(target.distance); //move the gun to predict where the enemy will be
                Fire(firePower);
                Execute(); //execute all commands
            }
        }

        private void doFirePower()
        {
            firePower = 1;
            //firePower = 400/target.distance; //selects a bullet power based on our distance away from the target
            //if (firePower > 3)
            //{
            //    firePower = 1;
            //}
        }

        private void antiGravMove()
        {
            double xforce = 0;
            double yforce = 0;
            double force;
            double ang;
            GravPoint p;
            Enemy en;
            IDictionaryEnumerator e = targets.GetEnumerator();
            //cycle through all the enemies.  If they are alive, they are repulsive.  Calculate the force on us
            while (e.MoveNext())
            {
                en = (Enemy) e.Value;
                if (en.live)
                {
                    p = new GravPoint(en.x, en.y, -1000);
                    force = p.power/Math.Pow(getRange(X, Y, p.x, p.y), 2);
                    //Find the bearing from the point to us
                    ang = normaliseBearing(Math.PI/2 - Math.Atan2(Y - p.y, X - p.x));
                    //Add the components of this force to the total force in their respective directions
                    xforce += Math.Sin(ang)*force;
                    yforce += Math.Cos(ang)*force;
                }
            }

            /**The next section adds a middle point with a random (positive or negative) strength.
		The strength changes every 5 turns, and goes between -1000 and 1000.  This gives a better
		overall movement.**/
            midpointcount++;
            if (midpointcount > 2)
            {
                midpointcount = 0;
                midpointstrength = (new Random()).Next(1, 2000) - 1000;
            }
            p = new GravPoint(BattleFieldHeight/2, BattleFieldHeight/2, midpointstrength);
            force = p.power/Math.Pow(getRange(X, Y, p.x, p.y), 1.5);
            ang = normaliseBearing(Math.PI/2 - Math.Atan2(Y - p.y, X - p.x));
            xforce += Math.Sin(ang)*force;
            yforce += Math.Cos(ang)*force;

            /**The following four lines add wall avoidance.  They will only affect us if the bot is close 
	    to the walls due to the force from the walls decreasing at a power 3.**/
            xforce += 5000/Math.Pow(getRange(X, Y, BattleFieldWidth, Y), 3);
            xforce -= 5000/Math.Pow(getRange(X, Y, 0, Y), 3);
            yforce += 5000/Math.Pow(getRange(X, Y, X, BattleFieldHeight), 3);
            yforce -= 5000/Math.Pow(getRange(X, Y, X, 0), 3);

            //Move in the direction of our resolved force.
            goTo(X - xforce, Y - yforce);
        }

        /**Move towards an x and y coordinate**/

        private void goTo(double x, double y)
        {
            double dist = 20;
            double angle = ToDegrees(absbearing(X, Y, x, y));
            double r = turnTo(angle);
            SetAhead(dist*r);
        }

        private double ToDegrees(double p)
        {
            return (180*p)/Math.PI;
        }


        /**Turns the shortest angle possible to come to a heading, then returns the direction the
	the bot needs to move in.**/

        private int turnTo(double angle)
        {
            double ang;
            int dir;
            ang = normaliseBearing(Heading - angle);
            if (ang > 90)
            {
                ang -= 180;
                dir = -1;
            }
            else if (ang < -90)
            {
                ang += 180;
                dir = -1;
            }
            else
            {
                dir = 1;
            }
            SetTurnLeft(ang);
            return dir;
        }

        /**keep the scanner turning**/

        private void doScanner()
        {
            SetTurnRadarLeftRadians(2*PI);
        }

        /**Move the gun to the predicted next bearing of the enemy**/

        private void doGun()
        {
            long time = Time +
                        (int) Math.Round((getRange(X, Y, target.x, target.y)/(20 - (3*firePower))));
            Point2D p = target.guessPosition(time);

            //offsets the gun by the angle to the next shot based on linear targeting provided by the enemy class
            double gunOffset = GunHeadingRadians - (Math.PI/2 - Math.Atan2(p.y - Y, p.x - X));
            SetTurnGunLeftRadians(normaliseBearing(gunOffset));
        }


        //if a bearing is not within the -pi to pi range, alters it to provide the shortest angle
        private double normaliseBearing(double ang)
        {
            if (ang > PI)
                ang -= 2*PI;
            if (ang < -PI)
                ang += 2*PI;
            return ang;
        }

        //if a heading is not within the 0 to 2pi range, alters it to provide the shortest angle
        private double normaliseHeading(double ang)
        {
            if (ang > 2*PI)
                ang -= 2*PI;
            if (ang < 0)
                ang += 2*PI;
            return ang;
        }

        //returns the distance between two x,y coordinates
        public double getRange(double x1, double y1, double x2, double y2)
        {
            double xo = x2 - x1;
            double yo = y2 - y1;
            double h = Math.Sqrt(xo*xo + yo*yo);
            return h;
        }

        //gets the absolute bearing between to x,y coordinates
        public double absbearing(double x1, double y1, double x2, double y2)
        {
            double xo = x2 - x1;
            double yo = y2 - y1;
            double h = getRange(x1, y1, x2, y2);
            if (xo > 0 && yo > 0)
            {
                return Math.Asin(xo/h);
            }
            if (xo > 0 && yo < 0)
            {
                return Math.PI - Math.Asin(xo/h);
            }
            if (xo < 0 && yo < 0)
            {
                return Math.PI + Math.Asin(-xo/h);
            }
            if (xo < 0 && yo > 0)
            {
                return 2.0*Math.PI - Math.Asin(-xo/h);
            }
            return 0;
        }


        /**
	 * onScannedRobot: What to do when you see another robot
	 */

        public override void OnScannedRobot(ScannedRobotEvent e)
        {
            Enemy en;
            if (targets.ContainsKey(e.Name))
            {
                en = (Enemy) targets[e.Name];
            }
            else
            {
                en = new Enemy();
                targets.Add(e.Name, en);
            }
            //the next line gets the absolute bearing to the point where the bot is
            double absbearing_rad = (HeadingRadians + e.BearingRadians)%(2*PI);
            //this section sets all the information about our target
            en.name = e.Name;
            double h = normaliseBearing(e.HeadingRadians - en.heading);
            h = h/(Time - en.ctime);
            en.changehead = h;
            en.x = X + Math.Sin(absbearing_rad)*e.Distance;
            //works out the x coordinate of where the target is
            en.y = Y + Math.Cos(absbearing_rad)*e.Distance;
            //works out the y coordinate of where the target is
            en.bearing = e.BearingRadians;
            en.heading = e.HeadingRadians;
            en.ctime = Time; //game time at which this scan was produced
            en.speed = e.Velocity;
            en.distance = e.Distance;
            en.live = true;
            if ((en.distance < target.distance) || (target.live == false))
            {
                target = en;
            }
        }

        public override void OnRobotDeath(RobotDeathEvent e)
        {
            Enemy en = (Enemy) targets[e.Name];
            en.live = false;
        }
    }

    internal class Enemy
    {
        /*
	 * ok, we should really be using accessors and mutators here,
	 * (i.e getName() and setName()) but life's too short.
	 */
        public String name;
        public double bearing, heading, speed, x, y, distance, changehead;
        public long ctime; //game time that the scan was produced
        public bool live; //is the enemy alive?

        public Point2D guessPosition(long when)
        {
            double diff = when - ctime;
            double newY = y + Math.Cos(heading)*speed*diff;
            double newX = x + Math.Sin(heading)*speed*diff;

            return new Point2D(newX, newY);
        }
    }

/**Holds the x, y, and strength info of a gravity point**/

    internal class GravPoint
    {
        public double x, y, power;

        public GravPoint(double pX, double pY, double pPower)
        {
            x = pX;
            y = pY;
            power = pPower;
        }
    }

    internal class Point2D
    {
        public double x, y;

        public Point2D(double x, double y)
        {
            // Contracts

            this.x = x;
            this.y = y;
        }
    }
}

