﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace minesweeper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool firstGame; // is first game upon launching the game
        bool firstTurn; // is first turn (used to set mines after first click)
        dif difficulty; // difficulty of current game
        dif choosenDifficulty; // difficulty of game chosen by user
        Board board; // board of current game

        public enum dif
        {
            easy, medium, hard
        }

        // colors of fields
        SolidColorBrush unrevealedColor1 = new SolidColorBrush(Color.FromRgb(185, 221, 119));
        SolidColorBrush unrevealedColor2 = new SolidColorBrush(Color.FromRgb(162, 209, 73));
        SolidColorBrush revealedColor1 = new SolidColorBrush(Color.FromRgb(215, 184, 153));
        SolidColorBrush revealedColor2 = new SolidColorBrush(Color.FromRgb(229, 194, 159));
        
        // colors of numbers representing number of mines around the field
        SolidColorBrush oneMineColor = new SolidColorBrush(Color.FromRgb(25, 118, 210));
        SolidColorBrush twoMinesColor = new SolidColorBrush(Color.FromRgb(56, 142, 60));
        SolidColorBrush threeMinesColor = new SolidColorBrush(Color.FromRgb(211, 47, 47));
        SolidColorBrush moreMinesColor = new SolidColorBrush(Color.FromRgb(123, 31, 162));
        
        // field color when clicked on mine
        SolidColorBrush gameOverColor = new SolidColorBrush(Color.FromRgb(219, 50, 54));
        public MainWindow()
        {
            InitializeComponent();
            difficulty = dif.easy;
            choosenDifficulty = dif.easy;
            firstGame = true;
            firstTurn = true;
        }

        private void newGameButton_Click(object sender, RoutedEventArgs e)
        {
            difficulty = choosenDifficulty;
            if(firstGame) // if this is the first game, adds some text on the top
            {
                TextBlock bl = new TextBlock();
                bl.FontSize = 15;
                bl.Text = "Mines left: 0";
                bl.VerticalAlignment = VerticalAlignment.Center;
                bl.FontWeight = FontWeights.Bold;
                RegisterName("MinesCounter", bl);
                Header.Children.Add(bl);

                TextBlock tl = new TextBlock();
                tl.FontSize = 15;
                tl.Text = "Time: 0";
                tl.VerticalAlignment = VerticalAlignment.Center;
                tl.FontWeight = FontWeights.Bold;
                tl.Margin = new Thickness(30, 0, 0, 0);
                RegisterName("TimeCounter", tl);
                Header.Children.Add(tl);
                firstGame = false;
            }

            board = new Board(difficulty);
            firstTurn = true;
            (GameGrid.FindName("MinesCounter") as TextBlock).Text = $"Mines left: {board.Mines}";
            
            ShowGameBoard(difficulty);
        }

        // Drawing game board on the screen
        private void ShowGameBoard(dif difficulty)
        {
            GameBoardSp.Children.RemoveRange(0, GameBoardSp.Children.Count); // Removes previous board
            
            // Setting width and length for things to look better
            double rectangleWidth;
            double rectangleHeight;
            if (difficulty == dif.easy)
            {
                rectangleWidth = 50;
                rectangleHeight = 50;
                Application.Current.MainWindow.Width = 650;
                Application.Current.MainWindow.Height = 550;
            }
            else if (difficulty == dif.medium)
            {
                rectangleWidth = 35;
                rectangleHeight = 35;
                Application.Current.MainWindow.Width = 750;
                Application.Current.MainWindow.Height = 650;
            }
            else
            {
                rectangleWidth = 33;
                rectangleHeight = 33;
                Application.Current.MainWindow.Width = 900;
                Application.Current.MainWindow.Height = 830;                
            }

            // Showing the board
            for (int i = 0; i < board.Rows; i++)
            {
                StackPanel rowSp = new StackPanel();
                rowSp.Orientation = Orientation.Horizontal;
                for (int j = 0; j < board.Cols; j++)
                {
                    Grid gr = new Grid();
                    Rectangle r = new Rectangle();
                    r.Width = rectangleWidth;
                    r.Height = rectangleHeight;
                    if ((i % 2 == 0 && j % 2 == 0) || (i % 2 != 0 && j % 2 != 0))
                        r.Fill = unrevealedColor1;
                    else
                        r.Fill = unrevealedColor2;

                    gr.Name = $"r{i}c{j}";
                    if (FindName(gr.Name) != null)
                        UnregisterName(gr.Name);
                    RegisterName(gr.Name, gr);

                    gr.MouseLeftButtonDown += new MouseButtonEventHandler(FieldClick);

                    gr.Children.Add(r);
                    rowSp.Children.Add(gr);
                }
                GameBoardSp.Children.Add(rowSp);
            }
        }

        private void FieldClick(object sender, MouseButtonEventArgs e)
        {
            var gr = sender as Grid;
            int index = gr.Name.IndexOf("c");
            int row = int.Parse(gr.Name.Substring(1, index - 1));
            int col = int.Parse(gr.Name.Substring(index + 1));
            if (firstTurn)
            {
                board.SetMines(row, col);
                firstTurn = false;
            }

            bool isGameOver = board.MakeTurn(row, col, true); // returns true if user clicked on mine
            if(isGameOver) // clicked on mine
            {
                for (int i = 0; i < board.Rows; i++)
                {
                    for (int j = 0; j < board.Cols; j++)
                    {
                        if (board.Fields[i, j].IsMine)
                        {
                            Grid tempGr = (FindName($"r{i}c{j}") as Grid);
                            this.Dispatcher.Invoke(() =>
                            {
                                (tempGr.Children[0] as Rectangle).Fill = gameOverColor;
                            });
                            Ellipse mine = new Ellipse();
                            mine.Fill = Brushes.Black;
                            if(difficulty == dif.easy) {
                                mine.Width = 30;
                                mine.Height = 30;
                            }
                            else if (difficulty == dif.medium)
                            {
                                mine.Width = 25;
                                mine.Height = 25;
                            }
                            else
                            {
                                mine.Width = 20;
                                mine.Height = 20;
                            }
                            tempGr.Children.Add(mine);
                        }
                    }
                }
            } else // did not click on mine
            {
                for (int i = 0; i < board.Rows; i++)
                {
                    for (int j = 0; j < board.Cols; j++)
                    {
                        if (board.Fields[i, j].IsRevealed)
                        {
                            Grid tempGr = (FindName($"r{i}c{j}") as Grid);
                            tempGr.Children.RemoveRange(1, tempGr.Children.Count - 1); // removes previous and adds new again (for better performance)
                            this.Dispatcher.Invoke(() =>
                            {
                                if ((i % 2 == 0 && j % 2 == 0) || (i % 2 != 0 && j % 2 != 0))
                                    (tempGr.Children[0] as Rectangle).Fill = revealedColor1;
                                else
                                    (tempGr.Children[0] as Rectangle).Fill = revealedColor2;
                            });
                            TextBlock tb = new TextBlock
                            {
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Center,
                                FontWeight = FontWeights.Bold,
                                Margin = new Thickness(0, 0, 0, 0),
                                Padding = new Thickness(0, 0, 0, 0)
                            };

                            if (difficulty == dif.easy)
                                tb.FontSize = 35;
                            else if (difficulty == dif.medium)
                                tb.FontSize = 25;
                            else
                                tb.FontSize = 23;

                            if (board.Fields[i, j].AdjacentMines > 0)
                            {
                                switch (board.Fields[i, j].AdjacentMines)
                                {
                                    case 1:
                                        tb.Foreground = oneMineColor;
                                        break;
                                    case 2:
                                        tb.Foreground = twoMinesColor;
                                        break;
                                    case 3:
                                        tb.Foreground = threeMinesColor;
                                        break;

                                    default:
                                        tb.Foreground = moreMinesColor;
                                        break;
                                }
                                tb.Text = $"{board.Fields[i, j].AdjacentMines}";
                            }
                            else
                            {
                                tb.Text = "";
                            }
                            tempGr.Children.Add(tb);
                        }
                    }
                }
            }
            
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var checkedBtn = sender as RadioButton;
            if (checkedBtn == null)
                return;
            if (checkedBtn.Name == "Easy")
                choosenDifficulty = dif.easy;
            else if (checkedBtn.Name == "Medium")
                choosenDifficulty = dif.medium;
            else if (checkedBtn.Name == "Hard")
                choosenDifficulty = dif.hard;
        }
    }
}
