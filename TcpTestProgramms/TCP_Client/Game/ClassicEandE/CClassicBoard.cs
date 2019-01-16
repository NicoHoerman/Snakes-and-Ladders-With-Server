using EandE_ServerModel.EandE.EandEContracts;
using System;
using System.Collections.Generic;

namespace EandE_ServerModel.EandE.ClassicEandE
{

    //Master
    public class ClassicBoard : IBoard
    {
        public int Size { get; } = 30;
        public int MaxWidth { get; } = 8;
        public List<IPawn> Pawns { get; set; }
        public List<IEntity> Entities { get; set; }

        //XD
        public ClassicBoard()
        {
            Pawns = new List<IPawn>();
            Entities = new List<IEntity>();
        }

        public string CreateOutput()
        {

            var fields = Size;
            double ratio = 9.0 / 16.0;

            var fieldCount_x = Math.Min((int)Math.Sqrt(ratio * fields), MaxWidth);
            var fieldCount_y = fields / fieldCount_x; //ratio * fieldCount_y;
            if (fields % fieldCount_x != 0)
                fieldCount_y++;

            string[,] array2D = new string[fieldCount_y, fieldCount_x];
            string result = "";

            var counter = Size;

            for (var y = 0; y < fieldCount_y; ++y)
            {

                for (var x = 0; x < fieldCount_x; ++x)
                {
                    string bottomSpace = "";
                    string topSpace = "";
                    string firstPawnSpace = "";
                    string secondPawnSpace = "";

                    var xOffs = y % 2 == 0 ? x : fieldCount_x - x - 1;
                    var stringDigit = $"      {counter} ";
                    stringDigit = stringDigit.Substring(stringDigit.Length - 4);

                    if (counter > 0 & counter <= fields)
                        array2D[y, xOffs] = $"{stringDigit}[ | , | ] ";
                    else
                        array2D[y, xOffs] = "".PadLeft(14, ' ');

                    Entities.ForEach(entity =>
                    {
                        if (entity.Type == EntityType.Eel & entity.Bottom_location == counter)
                            bottomSpace = "s";
                        else if (entity.Type == EntityType.Escalator & entity.Bottom_location == counter)
                            bottomSpace = "e";                       
                    });

                     if (bottomSpace.Length == 0)
                         bottomSpace = " ";
                     if (bottomSpace.Length == 0)
                         bottomSpace = " ";

                    Entities.ForEach(entity =>
                    {
                        if (entity.Type == EntityType.Eel & entity.Top_location == counter)
                            topSpace = "S";
                        else if (entity.Type == EntityType.Escalator & entity.Top_location == counter)
                            topSpace = "E";
                    });

                    if (topSpace.Length == 0)
                        topSpace = " ";
                    if (topSpace.Length == 0)
                        topSpace = " ";

                    Pawns.ForEach(pawn =>
                    {
                    if (pawn.Location == counter & (firstPawnSpace.Length == 0 || firstPawnSpace == " "))
                            firstPawnSpace = pawn.PlayerID.ToString();
                        else if (pawn.Location == counter & (secondPawnSpace.Length == 0 || secondPawnSpace == " "))
                            secondPawnSpace = pawn.PlayerID.ToString();
                    });

                    if (firstPawnSpace.Length == 0)
                        firstPawnSpace = " ";
                    if (secondPawnSpace.Length == 0 || secondPawnSpace == " ")
                        secondPawnSpace = " ";

                    if (counter > 0 & counter <= fields)
                        array2D[y, xOffs] = $"{stringDigit}[{topSpace}|{firstPawnSpace},{secondPawnSpace}|{bottomSpace}] ";

                    counter--;
                }
            }
            
            for (var y = 0; y < fieldCount_y; ++y)
            {
                for (var x = 0; x < fieldCount_x; ++x)
                    result += array2D[y, x];

                result += "\n";
            }
            return result;
        }
    }
}
