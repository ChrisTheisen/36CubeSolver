using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace _36Cube
{
    class Utility
    {
        public static byte[,] HeightMap { get { return _heightMap; } }
        private static byte[,] _heightMap = new byte[,] { { 1, 2, 5, 4, 6, 3 },
                                                   { 5, 3, 6, 1, 4, 2 },
                                                   { 4, 6, 3, 5, 2, 1 },
                                                   { 2, 1, 4, 3, 5, 6 },
                                                   { 3, 5, 2, 6, 1, 4 },
                                                   { 6, 4, 1, 2, 3, 5 }};

        public static byte[] Heights = new byte[] { 1, 2, 3, 4, 5, 6 };

        public static Color[] Colors { get { return _colors; } }
        private static Color[] _colors = new Color[] { Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Purple };
        //Optimized order to find the fist solution faster.
        //private static Color[] _colors = new Color[] { Color.Yellow, Color.Red, Color.Green, Color.Blue, Color.Purple, Color.Orange };

        public static IEnumerable<Block> FullBlockPool
        {
            get
            {
                List<Block> output = new List<Block>();

                foreach (byte H in Heights)
                {
                    foreach (Color C in Colors)
                    {
                        output.Add(new Block(C, H));
                    }
                }

                return output;
            }
        }

        public static IEnumerable<Block> UsedBlocks(Node input)
        {
            List<Block> output = new List<Block>();

            while (input.Parent != null)
            {
                output.Add(input.Block);
                input = input.Parent;
            }

            return output;
        }

        public static IEnumerable<Block> UnusedBlocks(Node input)
        {
            IEnumerable<Block> usedBlocks = UsedBlocks(input);

            IEnumerable<Block> output = from block in FullBlockPool
                                        where !BucketContains(usedBlocks, block)
                                        select block;
            return output;
        }

        public static IEnumerable<Block> GetNextBlockOptions(Node input)
        {
            bPoint loc = GetNextLocation(input);
            byte H = HeightMap[loc.Y, loc.X];
            Block[,] bs = GetBoardState(input);
            IEnumerable<Block> output = UnusedBlocks(input);
            output = output.Where(x => x._Height == H);

            #region special block/location combos
            Block B1 = new Block(Color.Orange, 6);
            Block B2 = new Block(Color.Yellow, 5);
            if (loc.X == 3 && loc.Y == 2 && BucketContains(UnusedBlocks(input), B1))
            {
                output = output.Concat(new[] { B1 });
            }
            if (loc.X == 3 && loc.Y == 4 && BucketContains(UnusedBlocks(input), B2))
            {
                output = output.Concat(new[] { B2 });
            }
            #endregion

            //remove invalid colors
            for (int i = 0; i < Heights.Count() - 1; i++)
            {
                Color C1 = bs[i, loc.Y]._Color;
                Color C2 = bs[loc.X, i]._Color;
                output = output.Where(x => x._Color != C1 && x._Color != C2);
            }

            return output;
        }

        public static bool BucketContains(IEnumerable<Block> bucket, Block input)
        {
            return bucket.Where(x => x._Color == input._Color && x._Height == input._Height).Count() > 0;
        }

        public static Block[,] GetBoardState(Node input)
        {
            Block[,] output = new Block[HeightMap.GetLength(0),HeightMap.GetLength(1)];
            for (int i = 0; i < output.GetLength(0); i++)
            {
                for (int j = 0; j < output.GetLength(1); j++)
                {
                    output[i, j] = new Block(Color.Black, 0);
                }
            }

            while (input.Parent != null)
            {
                output[input.Location.X, input.Location.Y] = input.Block;
                input = input.Parent;
            }

            return output;
        }

        public static bPoint GetNextLocation(Node input)
        {
            sbyte x = (sbyte)((input.Location.X + 1) % HeightMap.GetLength(0));
            sbyte y = (sbyte)(input.Location.Y + (x == 0 ? 1 : 0));

            return new bPoint(x, y);
        }

        public static string SolutionsToText(List<Block[,]> Solutions)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < Solutions.Count; i++)
            {
                sb.AppendLine("Solution:" + i);
                sb.AppendLine(Matrix(Solutions[i]));
            }

            return sb.ToString();
        }

        private static string Matrix(Block[,] Input)
        {
            StringBuilder sb = new StringBuilder();

            int rowLength = Input.GetLength(0);
            int colLength = Input.GetLength(1);

            for (int i = 0; i < rowLength; i++)
            {
                for (int j = 0; j < colLength; j++)
                {
                    sb.Append(Input[i, j]);
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }

    public class Block
    {
        public Color _Color;
        public byte _Height;

        public Block(Color _Color, byte _Height)
        {
            this._Color = _Color;
            this._Height = _Height;
        }

        public bool Equals(Block Input)
        {
            return this._Color == Input._Color && this._Height == Input._Height;
        }

        public override string ToString()
        {
            return this._Color + ":" + this._Height;
        }
    }

    public class bPoint
    {
        public sbyte X;
        public sbyte Y;

        public bPoint() { }

        public bPoint(sbyte X, sbyte Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }
}
