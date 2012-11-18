using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using AddOneGame.Lib;
using Microsoft.Phone.Controls;

namespace AddOneGame
{
    public partial class BlockControl : UserControl
    {
        public AddOneGame.Lib.Block Block { get; set; }

        public BlockControl(AddOneGame.Lib.Block block)
        {
            Block = block;
            DataContext = Block;

            Block.BeginIsTapped += new Lib.Block.BeginIsTappedDelegate(Block_BeginIsTapped);
            Block.BeginExplodes += new Lib.Block.BeginExplodesDelegate(Block_BeginExplodes);

            InitializeComponent();

            ToBigAnimation.Completed += new EventHandler(ToBigAnimation_Completed);
            ExplodeAnimation.Completed += new EventHandler(ExplodeAnimation_Completed);

        }



        void Block_BeginExplodes(AddOneGame.Lib.Block block, Color color)
        {
            ExplodeAnimation.Begin();
        }

        void ExplodeAnimation_Completed(object sender, EventArgs e)
        {
            ForegroundGrid.Opacity = 0;
            Block.ExplodeFinished();
        }

        void Block_BeginIsTapped()
        {
            ToBigAnimation.Begin();
        }

        void ToBigAnimation_Completed(object sender, EventArgs e)
        {
            Block.TapFinished();
        }
      

        public void OnOrientationChanged(PageOrientation Orientation)
        {
            // Switch to the visual state that corresponds to our target orientation 
            VisualStateManager.GoToState(this, Orientation.ToString(), true);
        }


        private void LayoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (GameController.Instance.IsFinished)
                GameController.Instance.Reset();
            else
                Block.Tap(GameController.Instance.CurrentColor, true);
        }
    }
}
