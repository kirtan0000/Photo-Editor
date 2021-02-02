using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Drawing.Imaging;
using Microsoft.VisualBasic;


namespace PhotoEditor
{


    public partial class Form1 : Form
    {

        Point lastPoint = Point.Empty;//Point.Empty represents null for a Point object

        bool isMouseDown = new Boolean();//this is used to evaluate whether our mousebutton is down or not

        public static class Globals
        {
            // Global variables

            public static bool checkIfMoveFalse = false;
            public static bool checkIfTextFalse = false;
            public static string pathDat = "";
            public static string defpathDat = "";
            public static bool checkifDrawFalse = false;
            public static int lineThickness = 3;
            public static Brush lineColor = Brushes.Black;
            public static bool checkIfFile = false;


        }
        Point location = Point.Empty;
        public Form1()
        {
            InitializeComponent();

        }



        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Call the function
            OpenFileOnPress();


        }

        // When form Closes
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (e.CloseReason != CloseReason.UserClosing)
                return;


            DialogResult result;


            result = MessageBox.Show("Do you want to save the file?", "Unsaved Chnages",
                   MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);


            if (Globals.checkIfFile == true)
            {
                if (result == DialogResult.Yes)
                {
                    // Cancel the exit
                    e.Cancel = true;

                    // Save the image
                    SaveFull();
                } else if (result == DialogResult.Cancel)
                {

                    // Cancel the exit
                    e.Cancel = true;

                    
                }
            }
        }

        private void OpenFileOnPress()
        {
            // The function to open explorer an select a file

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "Browse Images";
            openFileDialog1.Filter = "BMP|*.bmp|GIF|*.gif|JPG|*.jpg;*.jpeg|PNG|*.png|TIFF|*.tif;*.tiff|"
       + "All Image Types|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff";


            //Console.WriteLine(RandomString(11));

            if (openFileDialog1.ShowDialog() == DialogResult.OK)

            {
                Globals.checkIfFile = true;


                label1.Visible = true;

                pictureBox1.ImageLocation = openFileDialog1.FileName;

                FileInfo file = new FileInfo(openFileDialog1.FileName);
                var sizeInBytes = file.Length;

                Bitmap img = new Bitmap(openFileDialog1.FileName.ToString());

                Globals.pathDat = openFileDialog1.FileName.ToString();
                var imageHeight = img.Height;
                var imageWidth = img.Width;

                pictureBox1.Image = img;



                label1.Text = imageWidth.ToString() + " x " + imageHeight.ToString();

                button7.Visible = false;

                pictureBox1.Visible = true;


                //pictureBox1.BorderStyle = BorderStyle.Fixed3D;
                pictureBox1.Size = new Size(img.Width, img.Height);

            }


        }

        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());

        }



        public void SaveFull()
        {
            // Generate a random string

            string saveName = RandomString(5);
            //Console.WriteLine(saveName);
            pictureBox1.Image.Save(@"C:\Users\" + Environment.UserName + "\\Desktop\\" + "\\" + saveName + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            MessageBox.Show("Saved to: " + @"C:\Users\" + Environment.UserName + "\\Desktop" + "\\" + saveName + ".bmp");

            Globals.checkIfFile = false;


            string filePath = @"C:\Users\" + Environment.UserName + "\\Desktop\\" + "\\" + saveName + ".bmp";


            // Open the file
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            string _path = filePath;
            startInfo.Arguments = string.Format("/C start {0}", _path);
            process.StartInfo = startInfo;
            process.Start();

        }


        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {

            // Call the function
            SaveFull();

        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            // Move the image on button press
            if (location != Point.Empty)
            {
                Point newlocation = this.pictureBox1.Location;
                newlocation.X += e.X - location.X;
                newlocation.Y += e.Y - location.Y;
                this.pictureBox1.Location = newlocation;

            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                location = new Point(e.X, e.Y);
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            location = Point.Empty;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.S))
            {
                SaveFull();
                return true;
            }
            else if (keyData == (Keys.Control | Keys.O))
            {
                OpenFileOnPress();
                return true;
            }
            else if (keyData == (Keys.F10))
            {
                zoomin();
                return true;
            }
            else if (keyData == (Keys.F11))
            {
                zoomout();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
        }



        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            // Zoom
            if (e.Delta > 0)
            {

                zoomin();


            }
            else
            {

                zoomout();


            }

        }

       


        private void Form1_Load(object sender, EventArgs e)
        {
            this.pictureBox1.MouseWheel += new MouseEventHandler(pictureBox1_MouseWheel);


            // Change window title
            this.Text = "Cool Photo Editor";

            // Make numeric up and down read only
            numericUpDown1.ReadOnly = true;

            // Chnage brush color to black
            comboBox1.SelectedIndex = comboBox1.FindStringExact("Black");



        }

        public void zoomout()
{
    if (pictureBox1.Width > 308)
    {
        Bitmap bm_source = new Bitmap(pictureBox1.Image);
        // Make a bitmap for the result.  
        Bitmap bm_dest = new Bitmap(Convert.ToInt32(pictureBox1.Width / 1.2), Convert.ToInt32(pictureBox1.Height / 1.2));
        // Make a Graphics object for the result Bitmap.  
        Graphics gr_dest = Graphics.FromImage(bm_dest);
        // Copy the source image into the destination bitmap.  
        gr_dest.DrawImage(bm_source, 0, 0, bm_dest.Width + 1, bm_dest.Height + 1);
        // Display the result.  
        pictureBox1.Image = bm_dest;
        pictureBox1.Width = bm_dest.Width;
        pictureBox1.Height = bm_dest.Height;
        //MessageBox.Show(pictureBox1.Width.ToString());
    }
    else
    {

        MessageBox.Show("You have reached the max zoom out limit!");

    }

}

public void zoomin()
{
    if (pictureBox1.Width < 3958)
    {
        Bitmap bm_source = new Bitmap(pictureBox1.Image);
        // Make a bitmap for the result.  
        Bitmap bm_dest = new Bitmap(Convert.ToInt32(pictureBox1.Width * 1.2), Convert.ToInt32(pictureBox1.Height * 1.2));
        // Make a Graphics object for the result Bitmap.  
        Graphics gr_dest = Graphics.FromImage(bm_dest);
        // Copy the source image into the destination bitmap.  
        gr_dest.DrawImage(bm_source, 0, 0, bm_dest.Width + 1, bm_dest.Height + 1);
        // Display the result.  
        pictureBox1.Image = bm_dest;
        pictureBox1.Width = bm_dest.Width;
        pictureBox1.Height = bm_dest.Height;
        //MessageBox.Show(pictureBox1.Width.ToString());
    }
    else
    {

        MessageBox.Show("You have reached the max zoom in limit!");

    }

}




        private void pictureBox1_Click_1(object sender, EventArgs e)
        {
            if (Globals.checkIfMoveFalse == true)
            {
                Cursor.Current = Cursors.SizeAll;
            }



        }


        // Green
private void greenToolStripMenuItem_Click(object sender, EventArgs e)
{
    // we pull the bitmap from the image
    Bitmap bmp = (Bitmap)pictureBox1.Image;

    // we change some picels
    for (int y = 0; y < bmp.Height; y++)
        for (int x = 0; x < bmp.Width; x++)
        {
            Color c = bmp.GetPixel(x, y);
            bmp.SetPixel(x, y, Color.FromArgb(0, 0, 0, 0));

            bmp.SetPixel(x, y, Color.FromArgb(120, c.R, 255, c.B));
        }
    // we need to re-assign the changed bitmap
    pictureBox1.Image = (Bitmap)bmp;
}



// Blue
private void blueToolStripMenuItem_Click(object sender, EventArgs e)
{
    // we pull the bitmap from the image
    Bitmap bmp = (Bitmap)pictureBox1.Image;

    // we change some picels
    for (int y = 0; y < bmp.Height; y++)
        for (int x = 0; x < bmp.Width; x++)
        {
            Color c = bmp.GetPixel(x, y);
            bmp.SetPixel(x, y, Color.FromArgb(0, 0, 0, 0));

            bmp.SetPixel(x, y, Color.FromArgb(120, c.R, c.G, 255));
        }
    // we need to re-assign the changed bitmap
    pictureBox1.Image = (Bitmap)bmp;
}



// Yellow
private void yellowToolStripMenuItem_Click(object sender, EventArgs e)
{
    // we pull the bitmap from the image
    Bitmap bmp = (Bitmap)pictureBox1.Image;

    // we change some picels
    for (int y = 0; y < bmp.Height; y++)
        for (int x = 0; x < bmp.Width; x++)
        {
            Color c = bmp.GetPixel(x, y);
            bmp.SetPixel(x, y, Color.FromArgb(0, 0, 0, 0));

            bmp.SetPixel(x, y, Color.FromArgb(120, 255, c.G, c.B));
        }
    // we need to re-assign the changed bitmap
    pictureBox1.Image = (Bitmap)bmp;
}



// Yellow button 
private void button2_Click(object sender, EventArgs e)
{
    // we pull the bitmap from the image
    Bitmap bmp = (Bitmap)pictureBox1.Image;

    // we change some picels
    for (int y = 0; y < bmp.Height; y++)
        for (int x = 0; x < bmp.Width; x++)
        {
            Color c = bmp.GetPixel(x, y);
            bmp.SetPixel(x, y, Color.FromArgb(0, 0, 0, 0));

            bmp.SetPixel(x, y, Color.FromArgb(120, 255, c.G, c.B));
        }
    // we need to re-assign the changed bitmap
    pictureBox1.Image = (Bitmap)bmp;
}


// Blue button
private void button3_Click(object sender, EventArgs e)
{
    // we pull the bitmap from the image
    Bitmap bmp = (Bitmap)pictureBox1.Image;

    // we change some picels
    for (int y = 0; y < bmp.Height; y++)
        for (int x = 0; x < bmp.Width; x++)
        {
            Color c = bmp.GetPixel(x, y);
            bmp.SetPixel(x, y, Color.FromArgb(0, 0, 0, 0));

            bmp.SetPixel(x, y, Color.FromArgb(120, c.R, c.G, 255));
        }
    // we need to re-assign the changed bitmap
    pictureBox1.Image = (Bitmap)bmp;
}


// Green button
private void button1_Click(object sender, EventArgs e)
{
    // we pull the bitmap from the image
    Bitmap bmp = (Bitmap)pictureBox1.Image;

    // we change some picels
    for (int y = 0; y < bmp.Height; y++)
        for (int x = 0; x < bmp.Width; x++)
        {
            Color c = bmp.GetPixel(x, y);
            bmp.SetPixel(x, y, Color.FromArgb(0, 0, 0, 0));

            bmp.SetPixel(x, y, Color.FromArgb(120, c.R, 255, c.B));
        }
    // we need to re-assign the changed bitmap
    pictureBox1.Image = (Bitmap)bmp;
}

// Red button
private void button4_Click(object sender, EventArgs e)
{

    // we pull the bitmap from the image
    Bitmap bmp = (Bitmap)pictureBox1.Image;

    // we change some picels
    for (int y = 0; y < bmp.Height; y++)
        for (int x = 0; x < bmp.Width; x++)
        {
            Color c = bmp.GetPixel(x, y);
            bmp.SetPixel(x, y, Color.FromArgb(0, 0, 0, 0));

            bmp.SetPixel(x, y, Color.FromArgb(120, 255, c.G, c.B));
        }
    // we need to re-assign the changed bitmap
    pictureBox1.Image = (Bitmap)bmp;
}

        // Red
        private void redToolStripMenuItem_Click(object sender, EventArgs e)
        {

            // we pull the bitmap from the image
            Bitmap bmp = (Bitmap)pictureBox1.Image;

            // we change some picels
            for (int y = 0; y < bmp.Height; y++)
                for (int x = 0; x < bmp.Width; x++)
                {
                    Color c = bmp.GetPixel(x, y);
                    bmp.SetPixel(x, y, Color.FromArgb(0, 0, 0, 0));

                    bmp.SetPixel(x, y, Color.FromArgb(120, 255, c.G, c.B));
                }
            // we need to re-assign the changed bitmap
            pictureBox1.Image = (Bitmap)bmp;


        }


        

        private void pictureBox1_MouseUp_1(object sender, MouseEventArgs e)
        {
            if (Globals.checkIfMoveFalse == true)
            {
                location = Point.Empty;
            }

            isMouseDown = false;

            lastPoint = Point.Empty;




        }

        public System.Drawing.Drawing2D.SmoothingMode SmoothingMode { get; set; }


        private void pictureBox1_MouseMove_1(object sender, MouseEventArgs e)
        {
            if (Globals.checkIfMoveFalse == true)
            {

                if (location != Point.Empty)
                {
                    Point newlocation = this.pictureBox1.Location;
                    newlocation.X += e.X - location.X;
                    newlocation.Y += e.Y - location.Y;
                    this.pictureBox1.Location = newlocation;
                }
            }
            else if (Globals.checkifDrawFalse == true)
            {

                if (isMouseDown == true)//check to see if the mouse button is down

                {

                    if (lastPoint != null)//if our last point is not null, which in this case we have assigned above

                    {

                        if (pictureBox1.Image == null)//if no available bitmap exists on the picturebox to draw on

                        {
                            //create a new bitmap
                            Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);

                            pictureBox1.Image = bmp; //assign the picturebox.Image property to the bitmap created

                        }

                        using (Graphics g = Graphics.FromImage(pictureBox1.Image))

                        {//we need to create a Graphics object to draw on the picture box, its our main tool

                            //when making a Pen object, you can just give it color only or give it color and pen size

                            g.DrawLine(new Pen(Globals.lineColor, Globals.lineThickness), lastPoint, e.Location);
                            g.SmoothingMode = SmoothingMode.AntiAlias;

                            //this is to give the drawing a more smoother, less sharper look

                        }

                        pictureBox1.Invalidate();//refreshes the picturebox

                        lastPoint = e.Location;//keep assigning the lastPoint to the current mouse position

                    }
                }

            }
        }

        private void pictureBox1_MouseDown_1(object sender, MouseEventArgs e)
        {
            if (Globals.checkIfMoveFalse == true)
            {
                if (e.Button == MouseButtons.Left)
                {
                    location = new Point(e.X, e.Y);
                }
                Cursor.Current = Cursors.SizeAll;

            }
            else if (Globals.checkifDrawFalse == true)
            {


                lastPoint = e.Location;//we assign the lastPoint to the current mouse position: e.Location ('e' is from the MouseEventArgs passed into the MouseDown event)

                isMouseDown = true;//we set to true because our mouse button is down (clicked)
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {

        }

        private void process1_Exited(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (Globals.checkIfMoveFalse == false)
            {
                Globals.checkIfMoveFalse = true;
                Globals.checkifDrawFalse = false;
                button5.Text = "Stop";
                button9.Text = "Draw";
                numericUpDown1.Visible = false;
                label2.Visible = false;
                label3.Visible = false;
                comboBox1.Visible = false;

            }
            else if (Globals.checkIfMoveFalse == true)
            {

                Globals.checkIfMoveFalse = false;
                button5.Text = "Move";


            }


        }

        private void pictureBox1_MouseHover(object sender, EventArgs e)
        {


            if (Globals.checkIfMoveFalse == true)
            {

                Cursor.Current = Cursors.SizeAll;

            }

        }

        private void pictureBox1_Paint_1(object sender, PaintEventArgs e)
        {

            using (Font myFont = new Font("Arial", 14))
            {
                e.Graphics.DrawString("Error", myFont, Brushes.Black, new Point(2, 2));

            }

        }


        // Reset colors(didn't work so I just reset the path)
        private void button6_Click(object sender, EventArgs e)
        {


            //Console.WriteLine(pictureBox1.Image.ToString());
            pictureBox1.Image = new Bitmap(Globals.pathDat.ToString());


        }


        private void button7_Click(object sender, EventArgs e)
        {

            OpenFileOnPress();

        }


        private void button8_Click(object sender, EventArgs e)
        {
            // Save the image
            SaveFull();

        }

        private void button9_Click(object sender, EventArgs e)
        {
            startDraw();
        }

        public void startDraw()
        {
            if (Globals.checkifDrawFalse == false)
            {
                Globals.checkifDrawFalse = true;
                Globals.checkIfMoveFalse = false;

                button9.Text = "Stop Draw";
                button5.Text = "Move";
                numericUpDown1.Visible = true;
                label2.Visible = true;
                label3.Visible = true;
                comboBox1.Visible = true;

            }
            else if (Globals.checkifDrawFalse == true)
            {

                Globals.checkifDrawFalse = false;
                button9.Text = "Draw";
                numericUpDown1.Visible = false;
                label2.Visible = false;
                label3.Visible = false;
                comboBox1.Visible = false;

            }


        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            // Don't call base.OnMouseWheel(e)
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            using (Graphics g = Graphics.FromImage(pictureBox1.Image))

            {

                int mainCount = Convert.ToInt32(Math.Round(numericUpDown1.Value, 0));


                Globals.lineThickness = mainCount;


            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            string selected = this.comboBox1.GetItemText(this.comboBox1.SelectedItem);




            if (selected == "Black")
            {

                Globals.lineColor = Brushes.Black;

            }
            else if (selected == "Red")
            {

                Globals.lineColor = Brushes.Red;

            }
            else if (selected == "Orange")
            {

                Globals.lineColor = Brushes.Orange;

            }
            else if (selected == "Yellow")
            {

                Globals.lineColor = Brushes.Yellow;

            }
            else if (selected == "Green")
            {

                Globals.lineColor = Brushes.Green;

            }
            else if (selected == "Blue")
            {

                Globals.lineColor = Brushes.Blue;

            }
            else if (selected == "Purple")
            {

                Globals.lineColor = Brushes.Purple;

            }
            else if (selected == "White")
            {

                Globals.lineColor = Brushes.White;

            }



            //MessageBox.Show(selected);
            //Console.WriteLine(Globals.lineColor.ToString());

        }

        public void rotatePic90()
        {

            Image img = pictureBox1.Image;
            img.RotateFlip(RotateFlipType.Rotate90FlipNone);
            pictureBox1.Image = img;

        }


        private void rotate90ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            rotatePic90();

        }

        private void button10_Click(object sender, EventArgs e)
        {

            rotatePic90();

        }

        private void startDrawingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            startDraw();
        }

        private void zoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            zoomin();
        }

        private void zoomOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            zoomout();
        }
    }
}