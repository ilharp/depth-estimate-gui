using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia.Media.Imaging;

namespace DepthEstimateGui.Core
{
    public class ColorMap
    {
        #region Data

        public ColorMap(Bitmap image, string name)
        {
            Image = image;
            Name = name;
        }

        public Bitmap Image { get; }

        public string Name { get; }

        #endregion

        #region Static

        private static List<ColorMap>? _mapList;

        public static List<ColorMap> MapList
        {
            get
            {
                if (_mapList is not null) return _mapList;

                _mapList = Directory.EnumerateFiles(
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "cmap"))
                    .Select(x => new ColorMap(new(x), Path.GetFileNameWithoutExtension(x)))
                    .ToList();
                return _mapList;
            }
        }

        #endregion
    }
}
