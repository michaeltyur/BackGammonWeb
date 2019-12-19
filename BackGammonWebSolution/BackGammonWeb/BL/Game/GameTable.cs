using Common.Data;
using Common.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BLServer.Game
{
    public class GameTable
    {

        private static readonly object padlock = new object();
        //Checkers collection
        public List<Collection<Checker>> ListAllCollections { get; set; }
        //List Of Bar Collection
        public List<Collection<Checker>> ListBarCollections { get; set; }

        //Bars
        public Collection<Checker> WhiteBar { get; set; }
        public Collection<Checker> BlackBar { get; set; }

        public int WhiteCounter;
        public int BlackCounter;

        //Colors
        public CheckerColors _black;
        public CheckerColors Black
        {
            get { return _black; }
            set { _black = value; }
        }
        public CheckerColors _white;
        public CheckerColors White
        {
            get { return _white; }
            set { _white = value; }
        }
        public CheckerColors Selected { get; set; }

        public GameTable()
        {
            lock (padlock)
            {
                _black = CheckerColors.black;
                _white = CheckerColors.white;
                ListAllCollections = new List<Collection<Checker>>();
                ListBarCollections = new List<Collection<Checker>>();

                //  Black = new SolidColorBrush(Colors.Black);
                //  White = new SolidColorBrush(Colors.Moccasin);
                Selected = CheckerColors.brown;
                SetCollection();

                CheckerStartPosition();
                //TestCheckerStartPosition();

                WhiteBar = ListBarCollections[0];
                BlackBar = ListBarCollections[1];

            }
            WhiteCounter = 15;
            BlackCounter = 15;

        }

        private void SetCollection()
        {
            //Checkers collection
            for (int i = 0; i < 24; i++)
            {
                ListAllCollections.Add(new Collection<Checker>());
            }

            for (int i = 0; i < 2; i++)
            {
                ListBarCollections.Add(new Collection<Checker>());
            }
        }

        //Start Position
        public void CheckerStartPosition()
        {
            //5 checkers
            for (int i = 0; i < 5; i++)
            {
                ListAllCollections[5].Add(new Checker { Color = CheckerColors.black });
                ListAllCollections[12].Add(new Checker { Color = CheckerColors.black });

                ListAllCollections[11].Add(new Checker { Color = CheckerColors.white });
                ListAllCollections[18].Add(new Checker { Color = CheckerColors.white });
            }
            //3 checkers 
            for (int i = 0; i < 3; i++)
            {
                ListAllCollections[16].Add(new Checker { Color = CheckerColors.white });
                ListAllCollections[7].Add(new Checker { Color = CheckerColors.black });
            }
            //2 checkers
            for (int i = 0; i < 2; i++)
            {
                ListAllCollections[0].Add(new Checker { Color = CheckerColors.white });

                ListAllCollections[23].Add(new Checker { Color = CheckerColors.black });
            }
        }

        public void TestCheckerStartPosition()
        {

            ListAllCollections[0].Add(new Checker { Color = CheckerColors.black });
            ListAllCollections[0].Add(new Checker { Color = CheckerColors.black });

            //ListAllCollections[1].Add(new Checker { Color = Black });
            //ListAllCollections[1].Add(new Checker { Color = Black });

            //ListAllCollections[2].Add(new Checker { Color = Black });
            //ListAllCollections[2].Add(new Checker { Color = Black });

            //ListAllCollections[3].Add(new Checker { Color = Black });
            //ListAllCollections[3].Add(new Checker { Color = Black });

            //ListAllCollections[4].Add(new Checker { Color = Black });
            //ListAllCollections[4].Add(new Checker { Color = Black });

            //ListAllCollections[5].Add(new Checker { Color = Black });
            //ListAllCollections[5].Add(new Checker { Color = Black });

            //ListAllCollections[6].Add(new Checker { Color = Black });
            //ListAllCollections[6].Add(new Checker { Color = Black });


            ListAllCollections[16].Add(new Checker { Color = CheckerColors.white });
            ListAllCollections[16].Add(new Checker { Color = CheckerColors.white });

            ListAllCollections[17].Add(new Checker { Color = CheckerColors.white });
            ListAllCollections[17].Add(new Checker { Color = CheckerColors.white });

            ListAllCollections[18].Add(new Checker { Color = CheckerColors.white });
            ListAllCollections[18].Add(new Checker { Color = CheckerColors.white });

            ListAllCollections[19].Add(new Checker { Color = CheckerColors.white });
            ListAllCollections[19].Add(new Checker { Color = CheckerColors.white });

            ListAllCollections[20].Add(new Checker { Color = CheckerColors.white });
            ListAllCollections[20].Add(new Checker { Color = CheckerColors.white });

            ListAllCollections[21].Add(new Checker { Color = CheckerColors.white });
            ListAllCollections[21].Add(new Checker { Color = CheckerColors.white });

            ListAllCollections[22].Add(new Checker { Color = CheckerColors.white });
            ListAllCollections[22].Add(new Checker { Color = CheckerColors.white });

            ListAllCollections[23].Add(new Checker { Color = CheckerColors.white });
            ListAllCollections[23].Add(new Checker { Color = CheckerColors.white });
        }

        /// <summary>
        /// Returns array contains white checkers positions
        /// </summary>
        /// <returns></returns>
        public int[] GetWhiteCollArray()
        {
            int[] array = new int[24];
            var listCollectons = ListAllCollections;
            for (int i = 0; i < listCollectons.Count; i++)
            {
                var collection = listCollectons[i];
                if (collection.Count > 0 && collection.First().Color == CheckerColors.white)
                {
                    array[i] = collection.Count;
                }
            }
            return array;
        }

        /// <summary>
        /// Returns array contains black checkers positions
        /// </summary>
        /// <returns></returns>
        public int[] GetBlackCollArray()
        {
            int[] array = new int[24];
            var listCollectons = ListAllCollections;
            for (int i = 0; i < listCollectons.Count; i++)
            {
                var collection = listCollectons[i];
                if (collection.Count > 0 && collection.First().Color == CheckerColors.black)
                {
                    array[i] = collection.Count;
                }
            }
            return array;
        }

        /// <summary>
        /// Returns array contains bar checkers positions
        /// </summary>
        /// <returns></returns>
        public int[] GetBarCollArray()
        {
            int[] array = new int[2];
            var listBarectons = ListBarCollections;
            for (int i = 0; i < listBarectons.Count; i++)
            {
                var collection = listBarectons[i];

                array[i] = collection.Count;

            }
            return array;
        }

        public int[][] GetGameTableForSend()
        {
            var bigArray = new int[3][];

            bigArray[0] = GetWhiteCollArray();
            bigArray[1] = GetBlackCollArray();
            bigArray[2] = GetBarCollArray();
            return bigArray;
        }
        public bool IsGameOver()
        {
            if (WhiteCounter == 0 || BlackCounter == 0)
                return true;
            else return false;
        }
    }

}
