using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ChessBoard
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        #region Global Variables _rows is essential, don't delete it
        int _rows;
        const int _iHeight = 55;
        int _iWidth = 55;
        #endregion

        #region Constructor and set up code
        public MainPage() // constructor
        {
            this.InitializeComponent();
            // i+=1
            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            //// create another ellipse
            //Ellipse el2 = new Ellipse();
            //el2.Name = "el2";
            //el2.Height = 200;
            //el2.Width = 300;
            //el2.HorizontalAlignment = HorizontalAlignment.Left;
            //el2.VerticalAlignment = VerticalAlignment.Top;
            //el2.Fill = new SolidColorBrush(Colors.Blue);
            //// add it to a collection of children
            //parentGrid.Children.Add(el2);

            //// add the event handler
            //el2.Tapped += ellipse_Tapped;

        }

        // page loaded function
        // navigated to method
        
        #endregion

        #region Event Handlers for Controls
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            // local variable for the sender
            RadioButton current = (RadioButton)sender;
            // get the number to size the chess board with from the tag of the sender
            // create global var to hold this value
            //_rows = (int)current.Tag;
            _rows = Convert.ToInt32(current.Tag);
            // create method to generate the chess board
            createChessBoard();
            setupThePieces();

        }

        private void setupThePieces()
        {
            // check the size of board and decide how many cats, how many mice
            int numCats = _rows / 2;
            Ellipse cat;
            Grid board = FindName("ChessBoard") as Grid;
            // cats = red ellipse, width = 50, height = 50
            for( int i = 0; i < numCats; i++)
            {
                cat = new Ellipse();
                cat.Name = "cat" + (i + 1).ToString();
                cat.Height = _iHeight * 0.75;
                cat.Width = _iWidth * 0.75;
                cat.HorizontalAlignment = HorizontalAlignment.Center;
                cat.VerticalAlignment = VerticalAlignment.Center;
                cat.Fill = new SolidColorBrush(Colors.Red);
                cat.SetValue(Grid.RowProperty, (_rows - 1));
                if (_rows % 2 == 1)
                {
                    cat.SetValue(Grid.ColumnProperty, i * 2);
                }
                else
                {
                    cat.SetValue(Grid.ColumnProperty, (i * 2) + 1);
                }
                cat.Tapped += El1_Tapped;
                board.Children.Add(cat);
            }
            // mouse = green ellipse, same width
            // create _rows number of ellipses for cats
            // create one for the mouse
            Ellipse mouse = new Ellipse();
            mouse.Name = "theMouse";
            mouse.Height = _iHeight * 0.75;
            mouse.Width = _iWidth * 0.75;
            mouse.HorizontalAlignment = HorizontalAlignment.Center;
            mouse.VerticalAlignment = VerticalAlignment.Center;
            mouse.Fill = new SolidColorBrush(Colors.Green);
            mouse.SetValue(Grid.RowProperty, 0);
            if (_rows % 2 == 1)
            {
                mouse.SetValue(Grid.ColumnProperty, 1);
            }
            else
            {
                mouse.SetValue(Grid.ColumnProperty, 2);
            }
            mouse.Tapped += El1_Tapped;
            board.Children.Add(mouse);

            // decide where to place on the board
            // need one method to move a piece
            //Ellipse cat;    // name = "cat" + _rows

            // add event handler to el1
            foreach (var item in board.Children)
            {
                if( item.GetType() == typeof(Ellipse))
                {

                }

            }


        }

        Ellipse moveMe;
        Border possible1, possible2;
        private void El1_Tapped(object sender, TappedRoutedEventArgs e)
        {
            int toR = 0;
            Ellipse current = (Ellipse)sender;
            moveMe = current;
            //current.Fill = new SolidColorBrush(Colors.Blue);
            // move cats up, mouse down.
            toR = (int)current.GetValue(Grid.RowProperty);

            if ( current.Name == "theMouse")
            {
                // find the squares below this and to the left/rigth
                toR++;
            }
            else
            { // it's a cat
                // square above and left and right
                toR--;  // beware the edge (and the moon)
            }

            // highlight the borders.
            int toC1, toC2;
            toC1 = (int)current.GetValue(Grid.ColumnProperty) - 1;
            toC2 = (int)current.GetValue(Grid.ColumnProperty) + 1;

            Border brdr;
            brdr = FindName(toR.ToString() + toC1.ToString()) as Border;
            brdr.Background = new SolidColorBrush(Colors.Yellow);
            brdr.Tag = "valid";
            brdr.Tapped += Brdr_Tapped;
            possible1 = brdr;

            brdr = FindName(toR.ToString() + toC2.ToString()) as Border;
            brdr.Background = new SolidColorBrush(Colors.Yellow);
            brdr.Tag = "valid";
            brdr.Tapped += Brdr_Tapped;
            possible2 = brdr;
        }

        private void Brdr_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Border current = (Border)sender;

            moveMe.SetValue(Grid.RowProperty, current.GetValue(Grid.RowProperty));
            moveMe.SetValue(Grid.ColumnProperty, current.GetValue(Grid.ColumnProperty));

            possible1.Tapped -= Brdr_Tapped;
            possible1.Background = new SolidColorBrush(Colors.White);
            possible2.Tapped -= Brdr_Tapped;
            possible2.Background = new SolidColorBrush(Colors.White);

        }

        private void createChessBoard()
        {
            /*
             * the try catch block will try to remove any existing chess board so that
             * there is only one on the parent grid at any given time.  It will only fail
             * when there is no grid to remove, so the catch "falls through" and the code
             * executes as expected.
             */
            try
            {
                parentGrid.Children.Remove(FindName("ChessBoard") as Grid);
            }
            catch
            {
            }
            // create a grid object
            Grid grdBoard = new Grid();
            // give it a name, size, horizontal alignment, vertical align
            // give it background colour, margin of 5, Grid.row = 1
            grdBoard.Name = "ChessBoard";
            grdBoard.HorizontalAlignment = HorizontalAlignment.Center;
            grdBoard.VerticalAlignment = VerticalAlignment.Top;
            grdBoard.Height = _iHeight * _rows;
            grdBoard.Width = _iWidth * _rows;
            grdBoard.Background = new SolidColorBrush(Colors.Gray);
            grdBoard.Margin = new Thickness(5);
            grdBoard.SetValue(Grid.ColumnProperty, 1);
            grdBoard.SetValue(Grid.RowProperty, 1);

            // add _rows number of row definitions and column definitions
            for( int i = 0; i < _rows; i++)
            {
                grdBoard.RowDefinitions.Add(new RowDefinition());
                grdBoard.ColumnDefinitions.Add(new ColumnDefinition());
            }

            // add the chessboard to the root grid children collection
            parentGrid.Children.Add(grdBoard);

            // add a border object to each cell on the grid
            // to add one border
            // create the border object
            Border brdr; // don't put this in the loop

            int iR, iC;

            for( iR = 0; iR < _rows; iR++) // on each row
            {
                for(iC = 0; iC < _rows; iC++)  // for each col on that row
                {
                    #region Create one border element and add to the grid
                    brdr = new Border();
                    // give it height, width, horizontal & vertical align in centre
                    brdr.Height = _iHeight * 0.98;
                    brdr.Width = _iWidth * 0.98;
                    // sq_R_C, no duplicates here 
                    brdr.Name = iR.ToString() + iC.ToString();
                    brdr.HorizontalAlignment = HorizontalAlignment.Center;
                    brdr.VerticalAlignment = VerticalAlignment.Center;
                    // set the Grid.col, grid.row property
                    brdr.SetValue(Grid.RowProperty, iR);
                    brdr.SetValue(Grid.ColumnProperty, iC);
                    // give it a background colour
                    brdr.Background = new SolidColorBrush(Colors.Black);
                    if(0 == (iR + iC) % 2 ) // bottom left is black on 8
                    {
                        brdr.Background = new SolidColorBrush(Colors.White);
                    }
                    // add it to the chess board children collection
                    grdBoard.Children.Add(brdr);
                    #endregion
                } // end iC
            } // end of iR


        }

        private void ellipse_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Ellipse current = (Ellipse)sender;

            current.Fill = new SolidColorBrush(Colors.Green);

        }
        #endregion

    }
}
