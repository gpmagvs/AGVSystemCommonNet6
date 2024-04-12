using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.MAP.Geometry
{
    public class MapRectangle
    {
        public MapRectangle() { }
        public PointF CenterPoint
        {
            get
            {
                PointF[] rect = new PointF[4] { Corner1, Corner2, Corner4, Corner3 };
                float sumX = 0, sumY = 0;
                foreach (var point in rect)
                {
                    sumX += point.X;
                    sumY += point.Y;
                }
                return new PointF(sumX / rect.Length, sumY / rect.Length);
            }
        }
        public double Length
        {
            get
            {
                PointF[] rect = new PointF[4] { Corner1, Corner2, Corner4, Corner3 };
                double diffX = Corner2.X - Corner1.X;
                double diffY = Corner2.Y - Corner1.Y;
                return Math.Sqrt(Math.Pow(diffX, 2) + Math.Pow(diffY, 2));
            }
        }


        public double Theta
        {
            get
            {
                float deltaX = Corner2.X - Corner1.X;
                float deltaY = Corner2.Y - Corner1.Y;
                double thetaRadians = Math.Atan2(deltaY, deltaX);//弧度
                double thetaDegrees = thetaRadians * (180.0 / Math.PI);//角度
                return thetaDegrees;
            }
        }

        public MapPoint StartPointTag { get; set; } = new MapPoint();
        public MapPoint EndPointTag { get; set; } = new MapPoint();
        public PointF Corner1 { get; set; } = new PointF();
        public PointF Corner2 { get; set; } = new PointF();
        public PointF Corner3 { get; set; } = new PointF();
        public PointF Corner4 { get; set; } = new PointF();
        /// <summary>
        /// 最上面的角
        /// </summary>
        public PointF[] CornersSet
        {
            get
            {
                PointF centroid = CalculateCentroid();
                PointF[] corners = { Corner1, Corner2, Corner3, Corner4 };
                return corners.OrderByDescending(corner => CalculateAngleFromCentroid(corner, centroid)).ToArray();
            }
        }

        public bool IsIntersectionTo(MapCircleArea rotaionRegion)
        {
            throw new NotImplementedException();
        }
        public bool IsIntersectionTo(MapRectangle rectangle_compare_to)
        {
            
            // 对于每个矩形，计算四个边的法线（这里简化为水平和垂直方向）
            PointF[] axes = CalculateRotatedRectangleNormals((float)this.Theta);
            PointF[] axes2 = CalculateRotatedRectangleNormals((float)rectangle_compare_to.Theta);
            List<PointF> axexes = new List<PointF>();
            axexes.AddRange(axes);
            axexes.AddRange(axes2);
            foreach (PointF axis in axexes)
            {
                if (!OverlapOnAxis(axis, this.CornersSet, rectangle_compare_to.CornersSet))
                {
                    return false; // 找到了一个分离轴，两个矩形不重叠
                }
            }

            return true; // 没有找到分离轴，两个矩形重叠
        }

        PointF CalculateCentroid()
        {
            float centerX = (Corner1.X + Corner2.X + Corner3.X + Corner4.X) / 4;
            float centerY = (Corner1.Y + Corner2.Y + Corner3.Y + Corner4.Y) / 4;
            return new PointF(centerX, centerY);
        }
        float CalculateAngleFromCentroid(PointF point, PointF centroid)
        {
            return (float)Math.Atan2(point.Y - centroid.Y, point.X - centroid.X);
        }
        PointF[] CalculateRotatedRectangleNormals(float angleDegrees)
        {
            // 将角度从度转换为弧度
            float angleRadians = angleDegrees * (float)Math.PI / 180.0f;

            // 计算旋转后的法线向量
            PointF[] normals = new PointF[2];

            // 原始法线向量
            PointF horizontal = new PointF(1, 0); // 对应矩形的宽边法线
            PointF vertical = new PointF(0, 1);   // 对应矩形的长边法线

            // 旋转法线向量
            normals[0] = RotateVector(horizontal, angleRadians); // 宽边法线旋转
            normals[1] = RotateVector(vertical, angleRadians);   // 长边法线旋转

            return normals;
        }
        PointF RotateVector(PointF vector, float angleRadians)
        {
            // 旋转向量
            return new PointF(
                vector.X * (float)Math.Cos(angleRadians) - vector.Y * (float)Math.Sin(angleRadians),
                vector.X * (float)Math.Sin(angleRadians) + vector.Y * (float)Math.Cos(angleRadians)
            );
        }
        bool OverlapOnAxis(PointF axis, PointF[] rect1, PointF[] rect2)
        {
            // 计算每个矩形在轴上的投影的最小值和最大值
            (float minA, float maxA) = ProjectRectangleOnAxis(axis, rect1);
            (float minB, float maxB) = ProjectRectangleOnAxis(axis, rect2);

            // 检查投影是否重叠
            return maxA >= minB && maxB >= minA;
        }
        (float, float) ProjectRectangleOnAxis(PointF axis, PointF[] rectangle)
        {
            // 初始化最小和最大标量值为第一个顶点在轴上的投影
            float min = DotProduct(axis, rectangle[0]);
            float max = min;

            for (int i = 1; i < rectangle.Length; i++)
            {
                // 对每个顶点计算在轴上的投影，并更新最小和最大值
                float projection = DotProduct(axis, rectangle[i]);
                if (projection < min) min = projection;
                else if (projection > max) max = projection;
            }

            return (min, max);
        }
        float DotProduct(PointF vector1, PointF vector2)
        {
            return vector1.X * vector2.X + vector1.Y * vector2.Y;
        }

    }

}
