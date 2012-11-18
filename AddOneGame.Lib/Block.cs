using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace AddOneGame.Lib
{
    public class Block : INotifyPropertyChanged
    {
        private int _tapCount;
        private Color? _color;
        public bool IsFinished { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        public int TapCount
        {
            get { return _tapCount; }
            set
            {
                _tapCount = value;
                NotifyPropertyChanged("BindingTapCount");
                NotifyPropertyChanged("TapCount");
            }
        }

        public string BindingTapCount
        {
            get {
                if (_tapCount != 0)
                    return _tapCount.ToString();
                else
                    return string.Empty;
            }
        }

        public Color? TapColor
        {
            get {
                return _color;
            }
            set
            {
                _color = value;
                NotifyPropertyChanged("Color");
                NotifyPropertyChanged("BindingTapColor");
            }
        }

        public SolidColorBrush BindingTapColor
        {
            get
            {
                if (_color.HasValue)
                    return new SolidColorBrush(_color.Value);
                else
                    return new SolidColorBrush(Colors.Black);
            }
        }



        public delegate void IsTappedDelegate(Block block);
        public event IsTappedDelegate IsTapped;

        public delegate void BeginIsTappedDelegate();
        public event BeginIsTappedDelegate BeginIsTapped;

        public delegate void ExplodesDelegate(Block block, Color color);
        public event ExplodesDelegate Explodes;

        public delegate void BeginExplodesDelegate(Block block, Color color);
        public event BeginExplodesDelegate BeginExplodes;

        public Block(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void Tap(Color color, bool userAction)
        {
            if (!IsFinished && (this.TapColor == null || this.TapColor == color))
            {
                TapCount++;
                TapColor = color;

                if (BeginIsTapped != null)
                    BeginIsTapped();

                if (userAction)
                    GameController.Instance.SwitchPlayer();
            }
        }

        public void TapFinished()
        {
            if (IsTapped != null)
                IsTapped(this);
        }

        public void Explode(int count)
        {
            if (BeginExplodes != null)
                BeginExplodes(this, TapColor.Value);

            TapCount = TapCount - count;
        }

        public void ExplodeFinished()
        {
            if (Explodes != null)
                Explodes(this, TapColor.Value);
            
            if(TapCount == 0)
                TapColor = null;

        }

        public void Overtake(Color color)
        {
            TapColor = color;

            Tap(color, false);
        }

        public void Reset()
        {
            TapCount = 0;
            TapColor = null;
            IsFinished = false;
        }



        #region INotifyPropertyChanged Members

        private void NotifyPropertyChanged(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion








        
    }
}
