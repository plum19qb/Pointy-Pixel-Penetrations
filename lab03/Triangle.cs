using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab03
{
    //Triangle Class: Inherits from ShapeBase
    //Contains geometry data for triangle model and position
    class Triangle : ShapeBase
    {
        public static GraphicsPath _sTriangleModel = GenModel(3, 0);

        public Triangle(PointF _iPos): base(_iPos)
        {
            Pos = _iPos;
        }

        //Method GetPath(): Return the Transformed triangle model
        public override GraphicsPath GetPath()
        {
            GraphicsPath path = (GraphicsPath)_sTriangleModel.Clone();

            //From Demo - Pedro Method
            Matrix matTransform = new Matrix();
            matTransform.Translate(Pos.X,  Pos.Y);
            matTransform.Rotate(m_fRot);
            path.Transform(matTransform);

            return path;
        }

    }
}
