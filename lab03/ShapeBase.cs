using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab03
{
    public abstract class ShapeBase
    {
        #region Class Members
        protected float m_fRot;
        float m_fRotInc;
        float m_fXSpeed;
        float m_fYSpeed;
        public static Random s_rnd = new Random();
        public static int _TILESIZE = 50;
        #endregion

        #region Properties
        public PointF Pos { get; protected set; } //Protected Set to allow children object access
        public bool IsMarkedForDeath { get; set; }
        #endregion

        #region Constructor
        //Class Constructor: ShapeBase, takes a Position and initializes the transform of a shape starting with a variant Rotation and Velocity
        public ShapeBase(PointF _iPos)
        {
            Pos = _iPos;
            m_fRot = 0;
            m_fRotInc = (float)s_rnd.Next(-300, 300) / 100; //Returns Value from -3.00 - 3.00 with accuracy to the hundredth decimal
            m_fXSpeed = (float)s_rnd.Next(-25, 25) / 10; //Returns Value from -2.5 - 2.5 with accuracy to the tenth decimal
            m_fYSpeed = (float)s_rnd.Next(-25, 25) / 10; // ^^ '' ^^
        }
        #endregion

        //Method Render: Fills geometric area on graphics object with specified color
        public void Render(Graphics _graphics, Color _color)
        {
            _graphics.FillPath(new SolidBrush(_color), GetPath());
        }

        //Method Tick: Moves shape, inverses speed if OOB
        public void Tick(Size _size)
        {
            Pos = new PointF(Pos.X + m_fXSpeed, Pos.Y + m_fYSpeed);
            m_fRot += m_fRotInc;

            if (Pos.X <= 0 || Pos.X >= _size.Width) m_fXSpeed *= -1;
            if (Pos.Y <= 0 || Pos.Y >= _size.Height) m_fYSpeed *= -1;
        }

        //Method GenModel: Given a number of vertices and a variance, returns a GraphicsPath (Geometry Data) 
        //                  with vertices placed at regular angular intervals around an orgin, with a random amount of variance in the distance vector from the orgin
        public static GraphicsPath GenModel(int _vertexCount, float _variance)
        {
            //Polygons can be generated using GraphicsPath ctor taking a PointF[] representing vertex coords
            PointF[] vertices = new PointF[_vertexCount];
            byte[] vType = new byte[_vertexCount]; //Array to store vertex type

            //First point 'special', placed on positive x-axis and given the START vertex type
            vertices[0] = new PointF((float)(_TILESIZE - (_TILESIZE * (s_rnd.NextDouble() * _variance))), 0);
            vType[0] = 0;
            
            //Angle to increment on each pass
            double angleInc = (2*Math.PI) / _vertexCount;
            double angle = angleInc;

            //Repeat for remaining vertices
            for (int i = 1; i < _vertexCount; i++)
            {
                //Place vertex a random distance along a directional vector, direction calculated using angle along unit circle
                vertices[i] = new PointF((float)((_TILESIZE - (_TILESIZE * (s_rnd.NextDouble() * _variance))) * Math.Cos(angle)),
                    (float)((_TILESIZE - (_TILESIZE * (s_rnd.NextDouble() * _variance))) * Math.Sin(angle)));
                vType[i] = 1; //LINE TYPE Vertex
                angle += angleInc; //Move to next angle around circle
            }
             //return the GraphicsPath generated from Vertex Array
            return new GraphicsPath(vertices, vType);
        }

        //Abstract Method GetPath: Child Class applies Transformation and returns its geometry
        public abstract GraphicsPath GetPath();
    }
}
