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
using Microsoft.Phone.Controls;
using AddOneGame.Lib;
using System.Windows.Navigation;

namespace AddOneGame
{
    public partial class PlayPage : PhoneApplicationPage
    {
        public delegate void ChangeOrientationDelegate(PageOrientation Orientation);
        public event ChangeOrientationDelegate ChangeOrientation;

        // Constructor
        public PlayPage()
        {
            InitializeComponent();

        }

        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            //base.OnOrientationChanged(e);
            if (ChangeOrientation != null)
                ChangeOrientation(e.Orientation);
        }
      

        // When page is navigated to, set data context to selected item in list
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string rowString = "";
            string columnString = "";

            if (NavigationContext.QueryString.TryGetValue("rows", out rowString))
            {
                int rows = int.Parse(rowString);

                if (NavigationContext.QueryString.TryGetValue("columns", out columnString))
                {
                    int columns = int.Parse(columnString);

                    FillGrid(rows, columns);
                }
            }

            GameController.Instance.GameIsFinished += new BlocksCollection.GameIsFinishedDelegate(Instance_GameIsFinished);
            
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            GameController.Instance.GameIsFinished -= Instance_GameIsFinished;
        }

        private void FillGrid(int rows, int columns)
        {
            if (ContentPanel.Children.Count == 0)
            {
                GameController.Instance.Init(rows, columns);

                BlocksCollection collection = GameController.Instance.BlocksCollection;

                for (int i = 0; i < collection.Columns; i++)
                {
                    ContentPanel.ColumnDefinitions.Add(new ColumnDefinition());
                }
                for (int i = 0; i < collection.Rows; i++)
                {
                    ContentPanel.RowDefinitions.Add(new RowDefinition());
                }

                foreach (AddOneGame.Lib.Block b in collection)
                {
                    BlockControl block = new BlockControl(b);
                    block.SetValue(Grid.ColumnProperty, b.X);
                    block.SetValue(Grid.RowProperty, b.Y);

                    this.ChangeOrientation += new ChangeOrientationDelegate(block.OnOrientationChanged);

                    ContentPanel.Children.Add(block);
                }
            }
        }


        void Instance_GameIsFinished(Color color)
        {
            MessageBox.Show("You Won!");
        }

        private void ResetMenuIcon_Click(object sender, EventArgs e)
        {
            GameController.Instance.Reset();
        }

        private void HelpMenuIcon_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/HelpPage.xaml", UriKind.Relative));
        }

       
    }
}