﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Carrom
    {
    public class PhysicsEngine
        {
        //static List<CarromObject> listOfCoins;
        //Striker striker;
        //static List<CarromObject> listOfPockets;
        static DispatcherTimer timer;
        static double strikerForce, strikerAngle;
        static List<Point> allPoints;
        static Point pointOfIntersection = new Point ();
        public static void Start ()
            {
            //listOfCoins = new List<CarromObject> (19);
            //listOfPockets = new List<CarromObject> (4);
            timer = new DispatcherTimer ();
            timer.Interval = TimeSpan.FromMilliseconds (10);
            timer.Tick += UpdateUI;
            timer.Start ();
            allPoints = new List<Point> ();
            }

        private static void UpdateUI (object sender, EventArgs e)
            {
            //Update all coins

            //Update striker
            Game.striker.Update ();
            for (int i = 0; i < 19; i++)
                {
                Game.coins[i].Update ();
                }
            }

        //public void AddObject (CarromObject obj)
        //    {
        //    try
        //        {
        //        if (obj as Coin != null)
        //            {
        //            listOfCoins.Add (obj as Coin);
        //            }
        //        else if (obj as Striker != null)
        //            {
        //            striker = obj as Striker;
        //            }
        //        else if (obj as Pocket != null)
        //            {
        //            listOfPockets.Add( obj as Striker);
        //            }
        //        }
        //    catch (Exception)
        //        {

        //        throw;
        //        }
            
        //    }

        //Will be used to move a CarromObject
        //Two scenarios 
        //  1. Apply force, direction and CarromObject will stop at perticular point
        //  2. Apply force, direction and CarromObject will collide with other CarromObjects (Coins or Striker or Pocket) or Edge

        public void HitStriker (double force, double angle)
            {
            //int x = (int)Math.Round (obj.GetOrigin ().X + GetStoppingDistance (force) * Math.Cos (angle));
            //int y = (int)Math.Round (obj.GetOrigin ().Y + GetStoppingDistance (force) * Math.Sin (angle));
            //allPoints = GetAllPointsBetweenTwoPoints (obj.GetOrigin (), new Point (x, y));

            Game.striker.currentVelocity = force /4;
            Game.striker.intialVelocity = Game.striker.currentVelocity;
            SetStrikerInput (force, angle);


            changeStrikerValues = new DispatcherTimer ();
            changeStrikerValues.Interval = TimeSpan.FromMilliseconds (10);
            changeStrikerValues.Tick += (sender, e) => {
                //Striker displacement code
                try
                    {
                    List<Coin> collidedCoins = new List<Coin> ();
                    List<CollisionResult> collisionResult = isCollided (Game.striker, ref collidedCoins);

                    if (collisionResult[0] == CollisionResult.None)
                        {
                        //Game.striker.SetOrigin (allPoints.ElementAt (i++));
                        if (Game.striker.currentVelocity <= 0.0F)
                            {
                            changeStrikerValues.Stop ();
                            }
                        else
                            {
                            Game.striker.SetOrigin (GetPointFrom (Game.striker.GetOrigin (), strikerForce, strikerAngle, Game.striker.initialHitTime));
                            }
                        }
                    else
                        {
                        changeStrikerValues.Stop ();
                        foreach (var collisionObject in collisionResult)
                            {
                            switch (collisionObject)
                                {
                                case CollisionResult.Queen:
                                    Point strikerOrigin = Game.striker.GetOrigin ();
                                    Point coinOrigin = Game.queen.GetOrigin ();
                                    HitCoin (ref Game.queen, 50, 3.14159 - AngleBetweenTwoLines (strikerOrigin, coinOrigin, new Point (0, 740), new Point (740, 740)));
                                    break;
                                case CollisionResult.Edge:
                                    Game.striker.GetBaseElement ().Fill = new SolidColorBrush (Colors.Blue);
                                    Point initialStrikerPoint = Game.striker.GetInitialPoint ();
                                    double angle1 = AngleBetweenTwoLines (initialStrikerPoint, pointOfIntersection, pointOfIntersection, new Point (740, 740));
                                    i = 0;
                                    HitStriker (ref Game.striker, 50, angle1 + 1.5708); 
                                    break;
                                }
                            }
                        }
                    }
                catch (ArgumentOutOfRangeException)
                    {
                    changeStrikerValues.Stop ();
                    }
                //end

            };
            changeStrikerValues.Start ();
            }

        public void HitCoin (ref Coin obj, double force, double angle)
            {
            //int x = (int)Math.Round (obj.GetOrigin ().X + GetStoppingDistance (force) * Math.Cos (angle));
            //int y = (int)Math.Round (obj.GetOrigin ().Y + GetStoppingDistance (force) * Math.Sin (angle));
            //allPoints = GetAllPointsBetweenTwoPoints (obj.GetOrigin (), new Point (x, y));

            SetCoinInput (force, angle);

            DispatcherTimer changeCoinValues = new DispatcherTimer ();
            changeCoinValues.Interval = TimeSpan.FromMilliseconds (10);
            changeCoinValues.Tick += (sender, e) =>
            {
                try
                    {
                    Game.queen.SetOrigin (allPoints.ElementAt (i++));
                    List<Coin> collidedCoins = new List<Coin> ();
                    List<CollisionResult> collisionResult = isCollided (Game.queen, ref collidedCoins);
                    foreach (var collisionObject in collisionResult)
                        {
                        switch (collisionObject)
                            {
                            case CollisionResult.None:
                                Game.queen.SetOrigin (allPoints.ElementAt (i++));
                                break;

                            case CollisionResult.Pocket:
                                changeCoinValues.Stop ();
                                Game.queen.GetBaseElement ().Fill = new SolidColorBrush (Colors.White);
                                break;

                            case CollisionResult.Edge:
                                changeCoinValues.Stop ();
                                Game.queen.GetBaseElement ().Fill = new SolidColorBrush (Colors.Blue);
                                break;

                            //What if Queen hits one or more coins
                            case CollisionResult.Coin:
                                changeCoinValues.Stop ();
                                for (int i = 0; i < collidedCoins.Count; i++)
                                    {
                                    //HitCoin (ref collidedCoins[i], 30, 5.41052);
                                    //Task.Run (() =>
                                    //{
                                    //    //HitCoin (ref collidedCoins[i], 30, 5.41052);
                                    //});
                                    //Call HitCoin () method for each colliede coins
                                    //HitCoin (ref collidedCoins[i], 30, 5.41052);
                                    }
                                break;
                            }
                        }


                    //}
                    //else
                    //{
                    //  changeCoinValues.Stop ();
                    //Point strikerOrigin = Game.striker.GetOrigin ();
                    //Point coinOrigin = Game.queen.GetOrigin ();
                    //HitStriker (ref Game.queen, 50, AngleBetweenTwoLines (strikerOrigin, coinOrigin, new Point (0, 740), new Point (740, 740)));
                    //}
                    }
                catch (ArgumentOutOfRangeException)
                    {
                    changeCoinValues.Stop ();
                    }
            };
            changeCoinValues.Start ();

            //change
            //foreach (var p in allPoints)
            //    {
            //if (!obj.GetOrigin ().Equals (new Point (x, y)))
            //    {
            //}
            //Thread.Sleep (100);
            //    }
            //while (obj.GetOrigin ().Equals (new Point (x, y)))
            //    {
            //    }
            //Move to perticular direction
            //Check if it collides with others
            //--------If yes, recalculate current objects direction and force
            //--------and find other collided objects and call HitCarromObject with its direction and force
            //--------If no, continue
            }

        public static void SetStrikerInput (double force, double angle)
            {
            strikerForce = force;
            strikerAngle = angle;
            Point strikerOrigin = Game.striker.GetOrigin ();

            int x = (int)Math.Round (strikerOrigin.X + GetStoppingDistance (force) * Math.Cos (angle));
            int y = (int)Math.Round (strikerOrigin.Y + GetStoppingDistance (force) * Math.Sin (angle));
            allPoints = GetAllPointsBetweenTwoPoints (strikerOrigin, new Point (x, y));
            }

        public static void SetCoinInput (double force, double angle)
            {
            coinForce = force;
            coinAngle = angle + 3.14159;
            Point coinOrigin = Game.queen.GetOrigin ();
            allPoints.Clear ();
            i = 0;
            int x = (int)Math.Round (coinOrigin.X + GetStoppingDistance (force) * Math.Cos (coinAngle));
            int y = (int)Math.Round (coinOrigin.Y + GetStoppingDistance (force) * Math.Sin (coinAngle));
            allPoints = GetAllPointsBetweenTwoPoints (coinOrigin, new Point (x, y));
            }

        static int i = 0;
        private static double coinForce;
        private static double coinAngle;

        //private static void ChangeStrikerValues (object sender, EventArgs e)
        //    {
        //    try
        //        {
        //        List<Coin> collidedCoins = new List<Coin> ();
        //        List<CollisionResult> collisionResult = isCollided (Game.striker, ref collidedCoins);

        //        if (collisionResult[0] == CollisionResult.None)
        //            {
        //            //Game.striker.SetOrigin (allPoints.ElementAt (i++));
        //            Game.striker.initialHitTime += 10;
        //            Game.striker.SetOrigin (GetPointFrom (Game.striker.GetOrigin (), strikerForce, strikerAngle, Game.striker.initialHitTime));

        //            }
        //        else
        //            {
        //            changeStrikerValues.Stop ();
        //            foreach (var collisionObject in collisionResult)
        //                {
        //                switch (collisionObject)
        //                    {
        //                    case CollisionResult.Queen:
        //                        Point strikerOrigin = Game.striker.GetOrigin ();
        //                        Point coinOrigin = Game.queen.GetOrigin ();
        //                        HitCoin (ref Game.queen, 50, 3.14159 - AngleBetweenTwoLines (strikerOrigin, coinOrigin, new Point (0, 740), new Point (740, 740)));
        //                        break;
        //                    case CollisionResult.Edge:
        //                        Game.striker.GetBaseElement ().Fill = new SolidColorBrush (Colors.Blue);
        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //    catch (ArgumentOutOfRangeException)
        //        {
        //        changeStrikerValues.Stop ();
        //        }
        //    }

        //private static void ChangeCoinValues (object sender, EventArgs e)
        //    {
        //    try
        //        {
        //        Game.queen.SetOrigin (allPoints.ElementAt (i++));
        //        List<Coin> collidedCoins = new List<Coin> ();
        //        List<CollisionResult> collisionResult = isCollided (Game.queen, ref collidedCoins);
        //        foreach (var collisionObject in collisionResult)
        //            {
        //            switch (collisionObject)
        //                {
        //                case CollisionResult.None:
        //                    Game.queen.SetOrigin (allPoints.ElementAt (i++));
        //                    break;

        //                case CollisionResult.Pocket:
        //                    changeCoinValues.Stop ();
        //                    Game.queen.GetBaseElement ().Fill = new SolidColorBrush (Colors.White);
        //                    break;

        //                case CollisionResult.Edge:
        //                    changeCoinValues.Stop ();
        //                    Game.queen.GetBaseElement ().Fill = new SolidColorBrush (Colors.Blue);
        //                    break;

        //                //What if Queen hits one or more coins
        //                case CollisionResult.Coin:
        //                    changeCoinValues.Stop ();
        //                    for (int i = 0; i < collidedCoins.Count; i++) ;
        //                        {
        //                        //Call HitCoin () method for each colliede coins
        //                        //HitCoin (ref collidedCoins[i], 30, 5.41052);
        //                        }
        //                    break;
        //                }
        //            }


        //            //}
        //        //else
        //            //{
        //          //  changeCoinValues.Stop ();
        //            //Point strikerOrigin = Game.striker.GetOrigin ();
        //            //Point coinOrigin = Game.queen.GetOrigin ();
        //            //HitStriker (ref Game.queen, 50, AngleBetweenTwoLines (strikerOrigin, coinOrigin, new Point (0, 740), new Point (740, 740)));
        //            //}
        //        }
        //    catch (ArgumentOutOfRangeException)
        //        {
        //        changeCoinValues.Stop ();
        //        }

        //    }

        public static Point GetPointFrom (Point p, double force, double angle, int timeInMillis)
            {
            timeInMillis = Game.striker.initialHitTime += 3;
            Point result = new Point ();
            Game.striker.currentVelocity *= 0.9;

            double distance = (0.5) * (Game.striker.currentVelocity + Game.striker.intialVelocity) * timeInMillis;
            result.X = Game.striker.GetOrigin().X + (int)Math.Round (distance * Math.Cos (angle));
            result.Y = Game.striker.GetOrigin().Y + (int)Math.Round (distance * Math.Sin (angle));
            Game.striker.intialVelocity = Game.striker.currentVelocity;
            return result;
            }

        public enum CollisionResult
            {
            None,
            Edge,
            Coin,
            Striker,
            Queen,
            Pocket
            };

        public static List<CollisionResult> isCollided (CarromObject obj, ref List<Coin> collidedCoins)
            {

            List<CollisionResult> result = new List<CollisionResult> ();

            //Check if it is in pocket
            for (int i = 0; i < 4; i++)
                {
                var pocketDx = obj.GetOrigin ().X - Game.pockets[i].GetOrigin ().X;
                var pocketDy = obj.GetOrigin ().Y - Game.pockets[i].GetOrigin ().Y;

                var pocketDistance = Math.Sqrt (pocketDx * pocketDx + pocketDy * pocketDy);

                //This condition will pass if provided CarromObject is pocketed
                if (pocketDistance < Game.pockets[0].Radius - 2)
                    {
                    obj.GetBaseElement ().Fill = new SolidColorBrush (Colors.Black);
                    //changeCoinValues.Stop ();
                    result.Add(CollisionResult.Pocket);
                    break;
                    }
                }

            //Check if is collided with Edge
            Point[] edgePoints = new Point[5];
            edgePoints[0] = new Point (0, 0);
            edgePoints[1] = new Point (740,0);
            edgePoints[2] = new Point (740, 740);
            edgePoints[3] = new Point (0, 740);
            edgePoints[4] = new Point (0, 0);
            for (var i = 0; i < 4; i++)
                {
                var edgePointx1 = edgePoints[i].X;
                var edgePointy1 = edgePoints[i].Y;
                var edgePointx2 = edgePoints[i+1].X;
                var edgePointy2 = edgePoints[i+1].Y;
                Point CarromObjectOrigin = obj.GetOrigin ();
                var xCircle = CarromObjectOrigin.X;
                var yCircle = CarromObjectOrigin.Y;
                var radius = obj.Radius;
                double dx1, dy1, A, B, C, det, t;

                dx1 = edgePointx2 - edgePointx1;
                dy1 = edgePointy2 - edgePointy1;

                A = dx1 * dx1 + dy1 * dy1;
                B = 2 * (dx1 * (edgePointx1 - xCircle) + dy1 * (edgePointy1 - yCircle));
                C = (edgePointx1 - xCircle) * (edgePointx1 - xCircle) + (edgePointy1 - yCircle) * (edgePointy1 - yCircle) - radius * radius;

                det = B * B - 4 * A * C;
                if (det == 0)
                    {
                    result.Add (CollisionResult.Edge);
                    t = -B / (2 * A);
                    pointOfIntersection = new Point (edgePointx1 + t * dx1, edgePointy1 + t * dy1);
                    }

                }

            //Check if it is collided with coins and queen
            double dx, dy, distance;
            for (int i = 0; i < 19; i++)
                {
                dx = obj.GetOrigin ().X - Game.coins[i].GetOrigin ().X;
                dy = obj.GetOrigin ().Y - Game.coins[i].GetOrigin ().Y;

                distance = Math.Sqrt (dx * dx + dy * dy);

                if (distance < obj.Radius + Game.coins[i].Radius)
                    {
                    obj.GetBaseElement ().Fill = new SolidColorBrush (Colors.Red);
                    if (Game.coins[i].IsQueen)
                        {
                        result.Add (CollisionResult.Queen);
                        }
                    else
                        {
                        collidedCoins.Add (Game.coins[i]);
                        result.Add (CollisionResult.Coin);
                        }
                    }
                }
            //var dx = obj.GetOrigin().X - Game.queen.GetOrigin().X;
            //var dy = obj.GetOrigin ().Y - Game.queen.GetOrigin ().Y;

            //var distance = Math.Sqrt (dx * dx + dy * dy);

            //if (distance < obj.Radius + Game.queen.Radius)
            //    {
            //    obj.GetBaseElement ().Fill = new SolidColorBrush(Colors.Red);
            //    result.Add (CollisionResult.Queen);
            //    }

            if (result.Count == 0)
                {
                result.Add (CollisionResult.None);
                
                }
            return result;
            }

        public static double GetStoppingDistance (double force)
            {
            double coefficientOfFriction = 0.25 * 9.8;
            double stoppingDistance = 0.0F;

            stoppingDistance = (force * force) / (2 * coefficientOfFriction);

            return stoppingDistance;
            }

        public static List<Point> GetAllPointsBetweenTwoPoints (Point one, Point two)
            {
            List<Point> allPoints = new List<Point> ();
            int w = (int) Math.Round (two.X - one.X);
            int h = (int)Math.Round (two.Y - one.Y);
            int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
            if (w < 0)
                dx1 = -1;
            else if (w > 0)
                dx1 = 1;
            if (h < 0)
                dy1 = -1;
            else if (h > 0)
                dy1 = 1;
            if (w < 0)
                dx2 = -1;
            else if (w > 0)
                dx2 = 1;
            int longest = Math.Abs (w);
            int shortest = Math.Abs (h);
            if (!(longest > shortest))
                {
                longest = Math.Abs (h);
                shortest = Math.Abs (w);
                if (h < 0)
                    dy2 = -1;
                else if (h > 0)
                    dy2 = 1;
                dx2 = 0;
                }
            int numerator = longest >> 1;
            for (int i = 0; i <= longest; i++)
                {
                allPoints.Add (one);
                numerator += shortest;
                if (!(numerator < longest))
                    {
                    numerator -= longest;
                    one.X += dx1;
                    one.Y += dy1;
                    }
                else
                    {
                    one.X += dx2;
                    one.Y += dy2;
                    }
                }
            return allPoints;
            }

        public static double AngleBetweenTwoLines (Point a, Point b, Point c, Point d)
            {
            //double m1 = (b.Y - a.Y) / (b.X - a.X);
            //double m2 = (d.Y - c.Y) / (d.X - c.X);

            //double angle = Math.Atan (Math.Abs((m2-m1)/(1+(m2*m1))));


            //return angle;

            double theta1 = Math.Atan2 (b.Y - a.Y, b.X - a.X);
            double theta2 = Math.Atan2 (d.Y - c.Y, d.X - c.X);

            double diff = Math.Abs (theta1 - theta2);

            double angle = Math.Min (diff, Math.Abs (180 - diff));
            return angle;
            }
        }
    }
