using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using SharpGL.SceneGraph;
using SharpGL;
using Microsoft.Win32;


namespace Projekat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Atributi

        private World myWorld = null;




        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            try
            {
                myWorld = new World(Path.GetFullPath(@".\..\..\Models\\Bure2"), "Barrel N281211.3ds", (int)openGLControl.Width, (int)openGLControl.Height, openGLControl.OpenGL);
            }
            catch (Exception e)
            {
                MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta. Poruka greške: " + e.Message, "Poruka", MessageBoxButton.OK);
                this.Close();
            }

            label1.Content = myWorld.FaktorBuretaSkal +" f";
            label2.Content = myWorld.LightRED + " f";
            label3.Content = myWorld.LightGREEN + " f";
            label4.Content = myWorld.LightBLUE + " f";
            label5.Content = myWorld.PozicijaReflektora;
        }

        /// <summary>
        /// Handles the OpenGLDraw event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            myWorld.Draw(args.OpenGL);
        }

        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            myWorld.Initialize(args.OpenGL);
        }

        /// <summary>
        /// Handles the Resized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            myWorld.Resize(args.OpenGL, (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight);
        }
        //TODO 8 Interakcija
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (myWorld.Press)
            {
                return;
            }
            else
            {
                switch (e.Key)
                {
                    case Key.W: if (myWorld.RotationX > 5)
                        {
                            myWorld.RotationX -= 5.0f;
                            break;
                        }
                        else
                        {
                            break;
                        }
                    case Key.S:
                        if (myWorld.RotationX < 76)
                        {
                            myWorld.RotationX += 5.0f;
                            break;
                        }
                        else
                        {
                            break;
                        }

                    case Key.A: myWorld.RotationY -= 5.0f; break;
                    case Key.D: myWorld.RotationY += 5.0f; break;
                    case Key.Add: if (myWorld.Zoom > 305)
                        {
                            myWorld.Zoom -= 100.0f;
                            break;
                        }
                        else
                        {
                            break;
                        }
                    case Key.Subtract: if (myWorld.Zoom < 1305)
                        {
                            myWorld.Zoom += 100.0f;
                            break;
                        }
                        else
                        {
                            break;
                        }
                    case Key.Q: this.Close(); break;
                    case Key.C:
                        {
                            myWorld.PlayAnimation();
                            break;
                        }
                    //case Key.F2:
                    //    OpenFileDialog opfModel = new OpenFileDialog();
                    //    bool result = (bool)opfModel.ShowDialog();
                    //    if (result)
                    //    {

                    //        try
                    //        {
                    //            World newWorld = new World(Directory.GetParent(opfModel.FileName).ToString(), Path.GetFileName(opfModel.FileName), (int)openGLControl.Width, (int)openGLControl.Height, openGLControl.OpenGL);
                    //            myWorld.Dispose();
                    //            myWorld = newWorld;
                    //            myWorld.Initialize(openGLControl.OpenGL);
                    //        }
                    //        catch (Exception exp)
                    //        {
                    //            MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta:\n" + exp.Message, "GRESKA", MessageBoxButton.OK);
                    //        }
                    //    }
                    //    break;
                }
            }
        }
        //TODO 7 WPF kontrole
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (myWorld.FaktorBuretaSkal < 1.19f)
            {
                myWorld.FaktorBuretaSkal += 0.1f;
                label1.Content = myWorld.FaktorBuretaSkal + " f";
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (myWorld.FaktorBuretaSkal > 0.7f)
            {
                myWorld.FaktorBuretaSkal -= 0.1f;
                label1.Content = myWorld.FaktorBuretaSkal + " f";
            }
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            if (myWorld.LightRED < 1.0f)
            {
                myWorld.LightRED += 0.1f;
                label2.Content = myWorld.LightRED + " f";
            }
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            if (myWorld.LightGREEN < 1.0f)
            {
                myWorld.LightGREEN += 0.1f;
                label3.Content = myWorld.LightGREEN + " f";
            }
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            if (myWorld.LightBLUE < 1.0f)
            {
                myWorld.LightBLUE += 0.1f;
                label4.Content = myWorld.LightBLUE + " f";
            }
        }

        private void button8_Click(object sender, RoutedEventArgs e)
        {
            if (myWorld.LightRED > 0.1f)
            {
                myWorld.LightRED -= 0.1f;
                label2.Content = myWorld.LightRED + " f";
            }
        }

        private void button7_Click(object sender, RoutedEventArgs e)
        {
            if (myWorld.LightGREEN > 0.1f)
            {
                myWorld.LightGREEN -= 0.1f;
                label3.Content = myWorld.LightGREEN + " f";
            }
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            if (myWorld.LightBLUE >0.1f)
            {
                myWorld.LightBLUE -= 0.1f;
                label4.Content = myWorld.LightBLUE + " f";
            }
        }

        private void button9_Click(object sender, RoutedEventArgs e)
        {
            if (myWorld.PozicijaReflektora <1500)
            {
                myWorld.PozicijaReflektora += 50f;
                label5.Content = myWorld.PozicijaReflektora;
            }
        }

        private void button10_Click(object sender, RoutedEventArgs e)
        {
            if (myWorld.PozicijaReflektora >-1500)
            {
                myWorld.PozicijaReflektora -= 50f;
                label5.Content = myWorld.PozicijaReflektora;
            }
        }
    }
}
