using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab03
{
    //Rock Class: Inherits from ShapeBase
    //Contains geometry data for Rock model and position
    class Rock : ShapeBase
    {
        GraphicsPath _sRockModel = GenModel(s_rnd.Next(4, 13), 0.2f); //Rock Model contains vertex variance, 20% with respect to size

        public Rock(PointF _iPos) : base(_iPos)
        {
            Pos = _iPos;
        }
        //Method GetPath(): Return the Transformed Rock model
        public override GraphicsPath GetPath()
        {
            GraphicsPath path = (GraphicsPath)_sRockModel.Clone();

            //From Demo - Pedro Method
            Matrix matTransform = new Matrix();
            matTransform.Translate(Pos.X, Pos.Y);
            matTransform.Rotate(m_fRot);
            path.Transform(matTransform);

            return path;
        }
    }
}
