using System;
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
using Microsoft.Win32;

namespace Asciify
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
        }


        //GLOBAL VARS
        BitmapMaker _loadedImage;
        string _ImageFilePath;
      
        private void BrowseFileExplorer()
        {
            //browse file explore for image
            OpenFileDialog openFile = new OpenFileDialog();

            if (openFile.ShowDialog() == true)
            {
                _ImageFilePath = openFile.FileName;

                //Turn image into bitmap
                BitmapImage openedImage = new BitmapImage(new Uri(_ImageFilePath));
                RenderOptions.SetBitmapScalingMode(openedImage, BitmapScalingMode.NearestNeighbor);

                //Set GUI Image to found image
                imgLoadImage.Source = openedImage;

                _loadedImage = new BitmapMaker(_ImageFilePath);
            }//end if
        }//end method

        private void Asciitize()
        {
            //LOCAL VARS
            BitmapMaker tempImage = new BitmapMaker(_ImageFilePath);
            string[] asciiImage = new string[tempImage.Height];
            //kernal size
            int kernalHeight = 1;
            int kernalWidth = 1;
                      
            //looping vars
            int heightIndex = 0;
            int widthIndex = 0;
            bool inImage = true;
            double sum = 0.0;
            int arrayIndex = 0;
            double average = 0.0;

            int tempImageHeight = tempImage.Height;
            //if user selects kernal size overwrite current kernal size
            if (kernalHeightCombo.SelectedItem != null)
            {
                kernalHeight = (int)kernalHeightCombo.SelectedItem;
            }
            if (kernalWidthCombo.SelectedItem != null)
            {
                kernalWidth = (int)kernalWidthCombo.SelectedItem;
                
            }
            //ascii image
            string[] kernalImage = new string[(int)(_loadedImage.Height / kernalHeight + 1)];
            int incrementWidth = kernalWidth;
            int incrementHeight = kernalHeight;

            //loop through image 
            while (inImage)
            {
                //loop through height of kernal
                for (int y = heightIndex; y < kernalHeight; y++)
                {
                    //loop through width of kernal
                    for (int x = widthIndex; x < kernalWidth; x++)//how to keep for loop within range of current kernal?
                    {
                        //get pixel color, normalized value, and add to kernal sum
                        Color currentPixelColor = tempImage.GetPixelColor(x, y);
                        double normalizedColorValue = AverageColor(currentPixelColor);
                        sum += normalizedColorValue;

                    }//end for
                }//end for

                //calc average value for kernal, convert to ascii, store to array
                average = sum / ((double)incrementHeight * (double)incrementWidth);                
                kernalImage[arrayIndex]+= GrayToString(average);
                sum = 0;
                //increment to next kernal width range
                widthIndex += incrementWidth;
                if (kernalWidth < _loadedImage.Width)
                {
                    kernalWidth += incrementWidth;
                }
                
                //when Line of Kernals reaches end of Line
                if (widthIndex >= _loadedImage.Width)
                {
                    //go to next line
                    heightIndex += incrementHeight;
                    if (kernalHeight < _loadedImage.Height)
                    {
                        kernalHeight += incrementHeight;
                    }
                    
                    //start at begining of line
                    widthIndex = 0;
                    kernalWidth = incrementWidth;
                    //go to next line of array
                    arrayIndex += 1;
                }
                //when very end of image is reached
                if (heightIndex >= _loadedImage.Height)
                {
                    //trigger end of loop
                    inImage = false;
                }

            }//end of while loop

            //OUTPUT
            OutputToTextBox(kernalImage);
            
        }//end method

        private double AverageColor(Color pixelColor)
        {
            double redValue = (double)pixelColor.R / 255;
            double greenValue = (double)pixelColor.G / 255;
            double blueValue = (double)pixelColor.B / 255;
            double alphaValue = (double)pixelColor.A / 255;
            double normalizedValue = 0.0;
        
            double rawColorAvg = (redValue + blueValue + greenValue) / 3;
           
            if (alphaValue == 0)
            {
                rawColorAvg = 1;
            }//end if
            
            normalizedValue = rawColorAvg;
            return normalizedValue;
        }//end method
        private string GrayToString(double normalizedValue)
        {
            string[] asciiKey= {" ", ".", ":", "+", "%", "#" }; 
            
            if (normalizedValue >= 0 && normalizedValue <= .15)
            {
                return asciiKey[5];
            }else if(normalizedValue > .15 && normalizedValue <= .32)
            {
                return asciiKey[4];
            }else if(normalizedValue > .32 && normalizedValue <= .49)
            {
                return asciiKey[3];
            }else if(normalizedValue > .49 && normalizedValue <= .65)
            {
                return asciiKey[2];
            }else if(normalizedValue > .65 && normalizedValue <= .82)
            {
                return asciiKey[1];
            }else if(normalizedValue > .82 && normalizedValue <= 1)
            {
                return asciiKey[0];
            }//end if
            return asciiKey[0];
        }//end method

        private void OutputToTextBox(string[] asciiImage)
        {
            for (int index = 0; index < asciiImage.Length; index++)
            {
                txtConvertImage.Text += asciiImage[index];
                txtConvertImage.Text += "\n";
            }//end for
        }//end method
        private List<int> NumToList(int num)
        {
            List<int> numList = new List<int>(1);
            int currentNum = 1;
            for (int index = 0; index < num; index++)
            {
                if (num % currentNum == 0)
                {
                    numList.Add(currentNum);

                }

                currentNum++;
            }
            return numList;
        }
        private void LoadImage_Click(object sender, RoutedEventArgs e)
        {
            BrowseFileExplorer();
            if (imgLoadImage.IsLoaded)
            {
                convertButton.Visibility = Visibility.Visible;
                kernalHeightCombo.ItemsSource = NumToList(_loadedImage.Height);
                kernalWidthCombo.ItemsSource = NumToList(_loadedImage.Width);
            }//end if
            
        }//end LoadImage Click
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }//end Exit Click
        private void convertButton_Click(object sender, RoutedEventArgs e)
        {
            txtConvertImage.Text = "";
            Asciitize();
        }//end Convert Click
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            txtConvertImage.Text= "";
        }//end Clear Click
    }//end class
}//end namespace
