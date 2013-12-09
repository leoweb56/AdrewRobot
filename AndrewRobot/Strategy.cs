using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Robocode;

namespace ART
{
    public abstract partial class Strategy
    {
        public AdvancedRobot MyRobot { get; set; }

        public virtual void InitializeRobot(){}
        public virtual void ActionMoveRadar() { }
        public virtual void ActionMove() { }
        public virtual void ActionFire(Enemy e) { }
        public virtual void ActionHitMe(Enemy e) { }
        public virtual void ActionHitWall() { }
    }
}
