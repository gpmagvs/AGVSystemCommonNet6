using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.MAP.Geometry
{
    public class MapCircleArea
    {
        public float Width { get; private set; }
        public float Length { get; private set; }
        public float RotationRadius { get; private set; }
        public PointF Center { get; private set; } // 圆形区域的中心坐标

        public MapCircleArea(float length, float width, PointF center)
        {
            Width = width;
            Length = length;
            Center = center;
            CalculateRotationRadius();
        }
        public void SetCenter(double x, double y)
        {
            this.Center = new PointF((float)x, (float)y);
        }
        // 计算旋转半径
        private void CalculateRotationRadius()
        {
            RotationRadius = (float)Math.Sqrt((Width / 2) * (Width / 2) + (Length / 2) * (Length / 2));
        }

        // 显示旋转半径
        public void DisplayRotationArea()
        {
            Console.WriteLine($"车辆旋转360度时覆盖的半径: {RotationRadius}");
        }
        public bool IsIntersectionTo(MapRectangle rectangle)
        {
            float minX = rectangle.CornersSet.Select(pt=>pt.X).Min();
            float maxX = rectangle.CornersSet.Select(pt => pt.X).Max();
            float minY = rectangle.CornersSet.Select(pt => pt.Y).Min();
            float maxY = rectangle.CornersSet.Select(pt => pt.Y).Max();

            float closestX = Math.Max(minX, Math.Min(Center.X, maxX));
            float closestY = Math.Max(minY, Math.Min(Center.Y, maxY));

            float distanceX = Center.X - closestX;
            float distanceY = Center.Y - closestY;

            return distanceX * distanceX + distanceY * distanceY <= RotationRadius * RotationRadius;

        }

        public bool IsIntersectionTo(MapCircleArea circle)
        {  
            // 1. 计算两个圆心之间的距离
            float distanceX = Center.X - circle.Center.X;
            float distanceY = Center.Y - circle.Center.Y;
            float distance = (float)Math.Sqrt(distanceX * distanceX + distanceY * distanceY);
            // 2. 判断是否重叠
            return distance <= RotationRadius + circle.RotationRadius;
        }
    }
}
