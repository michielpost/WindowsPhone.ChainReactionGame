using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace AddOneGame.Lib
{
    public class BlocksCollection : List<Block>
    {
        public int Rows { get; set; }
        public int Columns { get; set; }
        public bool IsFinished { get; set; }

        public delegate void GameIsFinishedDelegate(Color color);
        public event GameIsFinishedDelegate GameIsFinished;
              
        public BlocksCollection(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;

            FillCollection();
        }

        private void FillCollection()
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    AddBlock(c, r);
                }
            }
        }

        private void AddBlock(int x, int y)
        {
            Block block = new Block(x, y);
            block.IsTapped += new Block.IsTappedDelegate(block_IsTapped);
            block.Explodes += new Block.ExplodesDelegate(block_Explodes);

            this.Add(block);
        }

        void block_Explodes(Block block, Color color)
        {
            var neighbours = GetNeighbours(block);

            Color? winnerColor = GetWinner();

            if (!winnerColor.HasValue)
            {
                foreach (Block neighbour in neighbours)
                {
                    neighbour.Overtake(color);
                }
            }
        }

        void block_IsTapped(Block block)
        {
            Color? winnerColor = GetWinner();

            int maxCount = GetNeighbours(block).Count();

            if (block.TapCount >= maxCount)
            {
                block.Explode(maxCount);
            }
        }


        private IEnumerable<Block> GetNeighbours(Block block)
        {
            int x = block.X;
            int y = block.Y;

            List<Block> list = new List<Block>();

            Block left = GetBlock(x - 1, y);
            Block right = GetBlock(x + 1, y);
            Block top = GetBlock(x, y - 1);
            Block bottom = GetBlock(x, y + 1);

            if (left != null)
                list.Add(left);

            if (right != null)
                list.Add(right);

            if (top != null)
                list.Add(top);

            if (bottom != null)
                list.Add(bottom);

            return list;
        }

        private Block GetBlock(int x, int y)
        {
            return this.Where(b => b.X == x).Where(b => b.Y == y).FirstOrDefault();
        }


        public Color? GetWinner()
        {
            Color? color = null;

            var colors = GetActivePlayers();

            if (colors.Count() >= 2)
            {
                var distinct = colors.Distinct();
                    
                if (distinct.Count() == 1)
                {
                    color = distinct.First();

                    if (!IsFinished)
                    {
                        IsFinished = true;

                        foreach (Block b in this)
                        {
                            b.IsFinished = true;
                        }

                        if (GameIsFinished != null)
                            GameIsFinished(color.Value);
                    }
                }
            }

            return color;
        }

        public IEnumerable<Color> GetActivePlayers()
        {
            var colors = this.Where(b => b.TapColor.HasValue).Select(b => b.TapColor.Value);
            return colors;
        }


        public void Reset()
        {
            IsFinished = false;

            foreach (Block b in this)
            {
                b.Reset();
            }
        }
    }
}
