/* LAB 03 Point Pixel Penetration Submission Code 1211_PPP
 * 
 * November 15th, 2021
 * 
 * By Liam Carroll for CMPE2800 1211
 * 
 * Program demonstrates double buffered graphics, procedural quasii random ploygon generation,
 * and the use of GDI+ Regions for collision detection
 * 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab03
{
    public partial class Form1 : Form
    {
        Random _rng = new Random(); 
        //Collection to hold our shapes, and shape intersections
        List<ShapeBase> shapes = new List<ShapeBase>();
        Dictionary<Region, long> intersections = new Dictionary<Region, long>();
        //Stopwatch allows us to remove certain objects after a specified time
        Stopwatch stopwatch = new Stopwatch();

        long timeToDeath = 3000; //How long until an object is removed after creation

        public Form1()
        {
            InitializeComponent();
            //Couldn't find  MouseWheel event in Form Design UI, so create Handler here
            this.MouseWheel += new MouseEventHandler(Form1_MouseWheel);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //Enable our Timer, start the stopwatch
            ui_Timer.Enabled = true;
            stopwatch.Start();
        }

        //Event Timer_Tick: Run this every 25 milliseconds
        private void ui_Timer_Tick(object sender, EventArgs e)
        {
            //Graphics
            using(BufferedGraphicsContext bgc = new BufferedGraphicsContext())
            {
                using(BufferedGraphics bg = bgc.Allocate(CreateGraphics(), ClientRectangle))
                {
                    //Clear the Back Buffer
                    bg.Graphics.Clear(Color.FromKnownColor(KnownColor.Black));

                    //Render all intersections in dark blue
                    foreach(KeyValuePair<Region,long> intersection in intersections) 
                        bg.Graphics.FillRegion(new SolidBrush(Color.Blue), intersection.Key);
                        
                    //For every shape in our collection
                    for(int i = 0; i < shapes.Count; i++)
                    {
                        #region If Shape Dead
                        if (shapes[i].IsMarkedForDeath) continue;
                        #endregion

                        //Tick each shape
                        shapes[i].Tick(new Size(ClientRectangle.Width, ClientRectangle.Height));
                        //Render each shape
                        shapes[i].Render(bg.Graphics, shapes[i] is Triangle ? Color.Salmon : Color.Aqua); //ternary conditional operator magic 

                        //Check against every other shape
                        
                        for (int j = 0; j < shapes.Count; j++)
                        {
                            #region If Shape Dead Ignore
                            if (shapes[j].IsMarkedForDeath) continue;
                            else if (i == j) continue;
                            #endregion
                            //Check the distance between our shapes using Pythagorean Theorem
                            double distanceVector = Math.Sqrt(Math.Pow(shapes[i].Pos.X - shapes[j].Pos.X, 2) + Math.Pow(shapes[i].Pos.Y - shapes[j].Pos.Y, 2));
                            #region If the shapes COULD BE colliding
                            //If the shapes COULD be colliding
                            if (distanceVector < 2 * ShapeBase._TILESIZE)
                            {
                                //Render a circle around our shape
                                GraphicsPath path = new GraphicsPath();
                                path.AddEllipse(shapes[i].Pos.X - (ShapeBase._TILESIZE), shapes[i].Pos.Y - (ShapeBase._TILESIZE), 2 * ShapeBase._TILESIZE, 2 * ShapeBase._TILESIZE);
                                bg.Graphics.DrawPath(new Pen(Color.White), path);

                                //Create regions for both shapes, and intersect them
                                Region shape1Region = new Region(shapes[i].GetPath());
                                Region shape2Region = new Region(shapes[j].GetPath());
                                shape1Region.Intersect(shape2Region);
                                //If the region is not empty: ie the Shapes were intersecting
                                if (!shape1Region.IsEmpty(bg.Graphics))
                                {
                                    //Add a clone of the region to our collection, and timestamp it for removal
                                    intersections.Add(shape1Region.Clone(), stopwatch.ElapsedMilliseconds + timeToDeath);
                                    //Mark our shapes for removal
                                    shapes[i].IsMarkedForDeath = true;
                                    shapes[j].IsMarkedForDeath = true;
                                }
                            }
                            #endregion
                        }
                        
                    }

                    #region Remove All Dead Shapes & Expired Collisions
                    shapes.RemoveAll(x => x.IsMarkedForDeath);
                    intersections.RemoveAll((k, v) => v < stopwatch.ElapsedMilliseconds);
                    #endregion

                    //Render text debug info to screen
                    bg.Graphics.DrawString($"Shape Count : {shapes.Count} | TILESIZE : {ShapeBase._TILESIZE}"
                       , new Font(FontFamily.GenericSansSerif, 20)
                       , new SolidBrush(Color.Red)
                       , 0, 0);

                    //Flip the back-buffer to the primary surface
                    bg.Render();
                   
                }
            }
        }

        //Event Mouse_Event
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            //Get cursor position
            PointF curPos = new PointF(e.X, e.Y);

            //Change behaviour on type of mouse click
            switch (e.Button)
            {
                case MouseButtons.Left:
                    //If control key, clear all active shapes
                    if (Control.ModifierKeys == Keys.Control) shapes.Clear();
                    //If Shift key, add 1000 Triangles
                    else if ((Control.ModifierKeys == Keys.Shift))
                    {
                        for (int i = 0; i < 1000; i++)
                        {
                            PointF rndPos = new PointF(_rng.Next(0, ClientRectangle.Width), _rng.Next(0, ClientRectangle.Height));
                            shapes.Add(new Triangle(rndPos));
                        }
                    }
                    //If no modifier, add 1 Triangle
                    else shapes.Add(new Triangle(curPos));
                    break;

                case MouseButtons.Right:
                    //If shift key, add 1000 Rocks
                    if (Control.ModifierKeys == Keys.Shift) 
                    {
                        for (int i = 0; i < 1000; i++)
                        {
                            PointF rndPos = new PointF(_rng.Next(0, ClientRectangle.Width), _rng.Next(0, ClientRectangle.Height));
                            shapes.Add(new Rock(rndPos));
                        }
                    }
                    //If no modifier add 1 Rock
                    else shapes.Add(new Rock(curPos));
                    break;
            }
        }

        //Event Mouse_Wheel_Event
        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            //If there are no shapes active
            if (shapes.Count <= 0)
            {
                //If scroll down
                if (e.Delta < 0)
                {
                    //Set shape size by (size - 10), or 5. Which ever is higher 
                    ShapeBase._TILESIZE = Math.Max(5, ShapeBase._TILESIZE - 10);
                }
                else ShapeBase._TILESIZE = Math.Min(500, ShapeBase._TILESIZE + 10); //Set shape size to (size + 10), or 500, whichever is lower

                //Refresh static Triangle Model
                Triangle._sTriangleModel = ShapeBase.GenModel(3, 0);
            }
        }


    }
}
