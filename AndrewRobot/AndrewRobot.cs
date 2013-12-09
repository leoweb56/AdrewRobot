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
        private StrategyManager strategyManager;


        public override void Run()
        {
            this.strategyManager = new StrategyManager(this);

            while (true)
            {
                strategyManager.GetStrategy().ActionMoveRadar();

                strategyManager.GetStrategy().ActionMove();

                Execute();
            }
        }

        public override void OnScannedRobot(ScannedRobotEvent eu)
        {
            strategyManager.GetStrategy().ActionFire(new Enemy(eu, this));

            Execute();
        }

    }
}

