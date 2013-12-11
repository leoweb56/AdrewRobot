using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ART
{
    public class EightStrategy : Strategy
    {
        private double turn = 0;
        private int dir = 1;

        public override void ActionMove()
        {
            if (turn%10 == 0)
            {
                dir = -dir;
            }

            MyRobot.SetTurnRight(20*dir);
            MyRobot.SetAhead(20);

            MyRobot.Execute();
        }
    }
}
