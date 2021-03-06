﻿using System;
using System.Windows.Media.Media3D;

namespace csmacnz.Monocle
{
    public class Sphere : IGeometry
    {
        public Vector3D CenterPoint { get; set; }
        public double Radius { get; set; }
        public Material Material { get; set; }
        public Sphere(Vector3D center, double radius, Material material)
        {
            CenterPoint = center;
            Radius = radius;
            Material=material;
        }

        public Vector3D NormalAt(Vector3D intersectionPoint)
        {
            //Assert actually an intersection?
            var surfaceNormal = intersectionPoint - CenterPoint;
            surfaceNormal.Normalize();
            return surfaceNormal;
        }

        public double? Test(Vector3D renderPoint, Vector3D direction)
        {
            var lineToCircle = renderPoint - CenterPoint;
            var B = 2 * Vector3D.DotProduct(direction, lineToCircle);
            var C = lineToCircle.LengthSquared - Math.Pow(Radius, 2);

            var determinate = Math.Pow(B, 2) - 4 * C;
            if (determinate >= 0)
            {
                var diff = Math.Sqrt(determinate);
                var tintersect1 = (-B - diff) / 2;
                var tintersect2 = (-B + diff) / 2;

                if (tintersect1 > tintersect2)
                {
                    var temp = tintersect2;
                    tintersect2 = tintersect1;
                    tintersect1 = temp;
                }
                if (tintersect1 < 0)
                {
                    tintersect1 = tintersect2;
                }
                if (tintersect1 >= 0)
                {
                    return tintersect1;
                }
            }
            return null;
        }
    }
}