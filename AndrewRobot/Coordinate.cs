using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Robocode.Util;

namespace ART
{
    public class Coordinate
    {

        private const double EQUALITY_THRESHOLD = 2.0;
        public double x;
        public double y;

        public Coordinate()
        {
            set(0, 0);
        }


        public Coordinate(double posX, double posY)
        {
            set(posX, posY);
        }

        public Coordinate(Coordinate coord)
        {
            set(coord.x, coord.y);
        }

        public bool isEqualTo(Coordinate coordinate)
        {
            return
                (
                    (Math.Abs(x - coordinate.x) < EQUALITY_THRESHOLD) &&
                    (Math.Abs(y - coordinate.y) < EQUALITY_THRESHOLD)
                    );
        }

        public void set(double posX, double posY)
        {
            x = posX;
            y = posY;
        }

        public void set(Coordinate coordinate)
        {
            set(coordinate.x, coordinate.y);
        }

        public Coordinate minus(Coordinate coordinate)
        {
            return new Coordinate(x - coordinate.x, y - coordinate.y);
        }

        public Coordinate plus(Coordinate coordinate)
        {
            return new Coordinate(x + coordinate.x, y + coordinate.y);
        }

        public Coordinate plus(double dx, double dy)
        {
            return new Coordinate(x + dx, y + dy);
        }

        public double length()
        {
            return Math.Sqrt(x*x + y*y);
        }

        public double distanceFrom(Coordinate coordinate)
        {
            return minus(coordinate).length();
        }

        public Coordinate unit()
        {
            return new Coordinate(x/length(), y/length());
        }

        public Coordinate multiply(double multiplier)
        {
            return new Coordinate(x*multiplier, y*multiplier);
        }

        public double distanceFrom(Coordinate pos0, Coordinate pos1)
        {
            Coordinate difference = pos1.minus(pos0);
            Coordinate u = difference.unit();
            Coordinate x = minus(pos0);

            double d = x.dotProduct(u);

            if (d <= 0.0) return distanceFrom(pos0);
            if (d >= difference.length()) return distanceFrom(pos1);

            Coordinate projectionPoint = pos0.plus(u.multiply(d));

            return distanceFrom(projectionPoint);
        }

        public double dotProduct(Coordinate coordinate)
        {
            return x*coordinate.x + y*coordinate.y;
        }

        public Coordinate getNextPosition(double nextHeading, double nextVelocity, double angularVelocity)
        {
            double dX = nextVelocity*Math.Sin(Utils.ToRadians(nextHeading + angularVelocity));
            double dY = nextVelocity*Math.Cos(Utils.ToRadians(nextHeading + angularVelocity));
            return new Coordinate(x + dX, y + dY);

        }

        public double headingTo(Coordinate targetCoordinate)
        {
            Coordinate difference = targetCoordinate.minus(this);
            return Utils.ToRadians(Math.Atan2(difference.x, difference.y));
        }

        public Coordinate rotate(double angle_deg)
        {
            double angle_rad = -Utils.ToRadians(angle_deg); //due to the strange Coordinate system
            return new Coordinate
                (
                x*Math.Cos(angle_rad) - y*Math.Sin(angle_rad),
                x*Math.Sin(angle_rad) + y*Math.Cos(angle_rad)
                );
        }

        public String toString()
        {
            return "(" + (int) x + ", " + (int) y + ")";
        }
    }
}
