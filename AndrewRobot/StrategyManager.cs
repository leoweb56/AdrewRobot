using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Robocode;

namespace ART
{
    public enum StrategiesTypes
    {
        SUPER_WALLS,
        EIGHT,
    }

    public class StrategyManager
    {
        private AdvancedRobot myRobot;
        private Dictionary<StrategiesTypes, Strategy> strategies; 

        public StrategyManager(AdvancedRobot myRobot)
        {
            this.myRobot = myRobot;
            this.strategies = new Dictionary<StrategiesTypes, Strategy>();

            //Strategies
            this.strategies.Add(StrategiesTypes.SUPER_WALLS, new SuperWallsStratergy());
            this.strategies.Add(StrategiesTypes.EIGHT, new EightStrategy());

            //Initialize strategies
            InitializeStrategies();
        }

        private void InitializeStrategies()
        {
            foreach (var strategy in strategies.Values)
            {
                strategy.MyRobot = myRobot;
                strategy.InitializeRobot();
            }
        }

        public Strategy GetStrategy()
        {
            //TODO poner logica de cual estrategia devolver
            Strategy strategy = this.strategies[StrategiesTypes.SUPER_WALLS];
            
            if (myRobot.Others > 1 || myRobot.Energy < 60)
            {
                strategy = this.strategies[StrategiesTypes.SUPER_WALLS];
            }
            else
            {
                strategy = this.strategies[StrategiesTypes.EIGHT];
            }

            return strategy;
        }
    }
}
