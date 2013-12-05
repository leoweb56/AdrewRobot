using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Robocode;
using Robocode.Util;

namespace ART
{
    public class Si2 : AdvancedRobot
    {

        private int deteccion = 0; // cuenta adversarios detectados para cambiar velocidad
        private int modo = 0; // cambio entre los modos de navegaci�n
        private double distancia = 99999;
        private string name;


        //TODO hacer una clase que guarde cuantas veces le di 

        public override void Run()
        {

            SetColors(Color.Yellow, Color.Yellow, Color.Yellow);
            evasivo(); // lo primero es ponerse a salvo alejandose hasta el muro
            IsAdjustRadarForGunTurn = true;
            while (true)
            {
                SetTurnRadarRight(360);
                // BUCLE PRINCIPAL
                if (Others != 1 || modo == 0)
                {
                    // en situaci�n de melee o condiciones
                    conservador(); // de baja energ�a entramos en este modo
                }
                else
                {
                    // para situacion de uno contra uno
                    agresivo(); // y energ�a suficiente
                }

                Execute();
            }
        }

        public override void OnHitWall(HitWallEvent e)
        {
            //gira 90� en caso de impacto con el muro
            TurnRight(90);
        }

        public override void OnHitRobot(HitRobotEvent e)
        {
            TurnGunRight(Utils.NormalRelativeAngleDegrees(e.Bearing + Heading - GunHeading));
            Fire(3);
            //if (e.Bearing > -10 && e.Bearing < 10)
            //{
            //    // Dispara si lo tiene al frente
            //    Fire(3);
            //}
            //if (e.Bearing > -90 && e.Bearing <= 90)
            //{
            //    // retrocede si esta por los lados o detras
            //    SetBack(200);
            //}
            //else
            //{
            //    SetAhead(100);
            //}
            Scan();
        }

        public override void OnBulletHit(BulletHitEvent evnt)
        {

            base.OnBulletHit(evnt);
        }

        public override void OnScannedRobot(ScannedRobotEvent e)
        {
            var defasaje = 0;
            ++deteccion; // ...Incrementamos la cuenta de detectados
            SetAhead(100); // y seguimos avanzando...
            if (Others == 1 && Energy > 80 && e.Distance < 200)
            {
                Console.WriteLine("Energy Fire < 200: " + e.Energy / 4);
                // ...Con �nico adversario cercano
                modo = 1; // cambio a modo agresivo
                SetTurnRight(0 - 90);
                defasaje = e.Velocity >= 0 ? 10 : 0;
                SetTurnGunRight(Utils.NormalRelativeAngleDegrees(e.Bearing + Heading - GunHeading)); // si tenemos suficiente energ�a
                SetFire(3); // perseguimos intentando arrollarlo
                SetTurnRight(0 - 90);
                defasaje = e.Velocity >= 0 ? 10 : 0;
                SetTurnGunRight(Utils.NormalRelativeAngleDegrees(e.Bearing + Heading - GunHeading)); // y disparando...
                SetAhead(100);
                SetFire(3);
                Scan();
                Execute();
            }
            if (e.Distance < 500)
            {
                // ...En distancia corta o media
                defasaje = e.Velocity >= 0 ? 10 : 0;
                TurnGunRight(Utils.NormalRelativeAngleDegrees(e.Bearing + Heading - GunHeading )); // y disparando...
                Console.WriteLine("Energy Fire < 500: " + e.Energy / 4);
                //Fire(e.Energy/4); // dispara con una fracci�n de la energ�a
                Fire(2); 
            } // del adversario para no malgastar la propia...
            else
            {
                defasaje = e.Velocity >= 0 ? 10 : 90;
                TurnGunRight(Utils.NormalRelativeAngleDegrees(e.Bearing + Heading - GunHeading )); // y disparando...
                Console.WriteLine("Energy Fire > 500: " + e.Energy / 20);
                //Fire(Energy/20); // ...En distancias largas dispara una fracci�n peque�a... 
                Fire(1);
            }
            Scan();
            if (deteccion > 5)
            {
                //...Modificamos la velocidad del robot rango[4-6]
                deteccion = 0; //de forma pseudoaleatoria cada cinco detecciones
                MaxVelocity = ((e.Distance%3) + 4); //para dificultar a los adversarios
            } //el c�lculo de nuestra posici�n futura...
        }

        public void evasivo()
        {
            MaxVelocity = (8);
            TurnLeft(Heading%90); // ...Gira a la izquierda hasta ponerse paralelo al muro
            Ahead(800); //avanzando hasta el muro frontal
            MaxVelocity = (5); //minimizamos los da�os en impactos con los muros...
        }

        public void conservador()
        {
            //TurnGunRight(180); // ...mientras avanza por el muro
            SetAhead(800); // gira el ca�on a izquierda y derecha
            //SetTurnGunLeft(180); // para localizar a los adversarios
            //WaitFor(new GunTurnCompleteCondition(this)); // solo barre el campo de batalla	
        } // girando 180�...

        public void agresivo()
        {
            if (Energy > 50)
            {
                // ... con energ�a suficiente
                MaxVelocity = (8); // vamos a tope...
                //TurnGunRight(360);
            }
            else
            {
                modo = 0; // ...Volvemos a modo conservador con poca energ�a
                evasivo(); // alejandonos hacia un muro...
            }
        }

        public override void OnWin(WinEvent e)
        {
            TurnRight(90);
            Ahead(100);
            SetTurnRight(3600);
            TurnGunLeft(3600);
        }
    }
}
