﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Carrom
    {
    //Will handle the life cycle of a single carrom game
    class Game
        {
        public static Player playerOne;
        public static Player playerTwo;
        public static Canvas carromBoard;
        public static Coin[] coins;
        public static Coin queen;
        public static Pocket[] pockets;
        public static Striker striker;

        //Reset game stage here, all coins must be at initial position
        //striker must be in Player 1's hand
        public void BeginGame ()
            {
            //Start PhysicsEngine
            PhysicsEngine.Start ();

            //Initiate two players
            playerOne = new Player ("Rahul");
            playerTwo = new Player ("Ajinkya");

            //Set Pockets
            pockets = new Pocket[4];
            pockets[0] = new Pocket (22, new Point(22,22));
            pockets[1] = new Pocket (22, new Point (718, 22));
            pockets[2] = new Pocket (22, new Point (22, 718));
            pockets[3] = new Pocket (22, new Point (718, 718));

            pockets[0].Update ();
            pockets[0].AddToGame ();
            pockets[1].Update ();
            pockets[1].AddToGame ();
            pockets[2].Update ();
            pockets[2].AddToGame ();
            pockets[3].Update ();
            pockets[3].AddToGame ();

            //Create all coins
            //coins = new Coin[18];
            //for player one
            //for (int i = 0; i <= 9; i++)
            //    {
            //    coins[i] = new Coin (10, Colors.Black);
            //    coins[i].AssignPlayer (playerOne);
            //    coins[i].CoinNumber = i;
            //    }

            ////for player two
            //for (int i = 0; i <= 9; i++)
            //    {
            //    coins[i] = new Coin (10, Colors.WhiteSmoke);
            //    coins[i].AssignPlayer (playerTwo);
            //    coins[i].CoinNumber = i;
            //    }

            //Create Striker
            striker = new Striker (14,Colors.White);
            striker.SetInitialPoint (new Point (370, 633));
            striker.Update ();
            striker.AddToGame ();
            striker.GetBaseElement ().Stroke = new SolidColorBrush (Colors.Black);
            striker.GetBaseElement ().MouseLeftButtonDown += TestMethod ;

            queen = new Coin (14, Colors.BlueViolet);
            queen.SetInitialPoint (new Point (370, 370));
            queen.Update ();
            queen.AddToGame ();
            queen.GetBaseElement ().Stroke = new SolidColorBrush (Colors.Black);

            //Give Striker in Player One's hand
            }

        private void TestMethod (object sender, MouseButtonEventArgs e)
            {
            //Use this statement for Striker to Coin detection and Coin to Pocket detection
            PhysicsEngine engine = new PhysicsEngine ();
            engine.HitStriker (ref striker, 40, 4.6338492);
            Game.striker.initialHitTime = 0;
            //Use this line for Striker to Edge detection
            //PhysicsEngine.HitStriker (ref striker, 70, 5.41052);
            }

        //All standard rules that applies in the turn must apply here
        //PhysicsEngine's HitCarromObject will be called here
        public void Turn ()
            {

            }

        //Set variables for next turn
        // e.g. 1. Switch Player
        //      2. Striker must be in another player's hand or 
        //         in the same player's hand depending on the rules
        public void NextTurn ()
            {
            }
        
        //Game will end here and it shuld return winner Player of the game
        public Player EndGame ()
            {
            
            
            return this.GetWinner();
            }

        // Find max score and return respective Player
        public Player GetWinner ()
            {
            if (playerOne.Score > playerTwo.Score)
                {
                return playerOne;
                }
            else if (playerTwo.Score > playerOne.Score)
                {
                return playerTwo;
                }
            else
                {
                return null;
                }
            }
        }
    }
