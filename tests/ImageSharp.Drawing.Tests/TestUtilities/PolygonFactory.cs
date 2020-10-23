// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using Newtonsoft.Json;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Drawing.Tests.Shapes.Scan;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Drawing.Tests
{
    internal static class PolygonFactory
    {
        private const float Inf = 10000;

        private static readonly IBrush TestBrush = Brushes.Solid(Color.Red);

        private static readonly IPen GridPen = Pens.Solid(Color.Aqua, 0.5f);

        // based on:
        // https://github.com/SixLabors/ImageSharp.Drawing/issues/15#issuecomment-521061283
        public static IReadOnlyList<PointF[]> GetGeoJsonPoints(IGeometryObject geometry, Matrix3x2 transform)
        {
            var result = new List<PointF[]>();
            if (geometry is GeoJSON.Net.Geometry.Polygon p)
            {
                AddGeoJsonPolygon(p);
            }
            else if (geometry is MultiPolygon mp)
            {
                foreach (GeoJSON.Net.Geometry.Polygon subPolygon in mp.Coordinates)
                {
                    AddGeoJsonPolygon(subPolygon);
                }
            }

            return result;

            void AddGeoJsonPolygon(GeoJSON.Net.Geometry.Polygon polygon)
            {
                foreach (LineString lineString in polygon.Coordinates)
                {
                    PointF[] points = lineString.Coordinates.Select(PositionToPointF).ToArray();
                    result.Add(points);
                }
            }

            PointF PositionToPointF(IPosition pos)
            {
                float lon = (float)pos.Longitude + 180f;
                if (lon > 180)
                {
                    lon = 360 - lon;
                }

                float lat = (float)pos.Latitude + 90;
                if (lat > 90)
                {
                    lat = 180 - lat;
                }

                return Vector2.Transform(new Vector2(lon, lat), transform);
            }
        }

        public static PointF[][] GetGeoJsonPoints(string geoJsonContent, Matrix3x2 transform)
        {
            FeatureCollection features = JsonConvert.DeserializeObject<FeatureCollection>(geoJsonContent);
            return features.Features.SelectMany(f => GetGeoJsonPoints(f.Geometry, transform)).ToArray();
        }

        public static PointF[][] GetGeoJsonPoints(string geoJsonContent) =>
            GetGeoJsonPoints(geoJsonContent, Matrix3x2.Identity);

        public static Polygon CreatePolygon(params (float x, float y)[] coords)
            => new Polygon(new LinearLineSegment(CreatePointArray(coords)))
            {
                // The default epsilon is too large for test code, we prefer the vertices not to be changed
                RemoveCloseAndCollinearPoints = false
            };

        public static (PointF Start, PointF End) CreateHorizontalLine(float y)
            => (new PointF(-Inf, y), new PointF(Inf, y));

        public static PointF[] CreatePointArray(params (float x, float y)[] coords) =>
            coords.Select(c => new PointF(c.x, c.y)).ToArray();
        public static T[] CloneArray<T>(this T[] points)
        {
            T[] result = new T[points.Length];
            Array.Copy(points, result, points.Length);
            return result;
        }
    }
}
