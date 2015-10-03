﻿using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace csmacnz.Monocle
{
    public class Scene
    {
        public static Scene Default()
        {
            return new Scene
            {
                CameraPosition = new Vector3D(0, 0, -10.00),
                CameraFacingAt = new Vector3D(),
                PixelsToUnits = 1,
                DefaultColor = Colors.Firebrick,
                AmbientLight = new LightStrength(0.1, 0.1, 0.1)
            };
        }

        public LightStrength AmbientLight { get; set; }

        public Vector3D CameraFacingAt { get; set; }

        public Vector3D CameraPosition { get; set; }

        public double PixelsToUnits { get; set; }
        public Color DefaultColor { get; set; }
    }
}