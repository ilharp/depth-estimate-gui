using System.Collections.Generic;
using Avalonia.Controls;

namespace DepthEstimateGui.Utils
{
    public static class FormatFilter
    {
        public static readonly List<FileDialogFilter> PilOpenFormatFilter = new()
        {
            new()
            {
                Name = "All Picture Files",
                Extensions = new()
                {
                    "png",
                    "bmp",
                    "dib",
                    "jpg",
                    "jpeg",
                    "jpe",
                    "jfif",
                    "dds",
                    "tif",
                    "tiff",
                    "webp"
                }
            },
            new()
            {
                Name = "PNG",
                Extensions = new()
                {
                    "png"
                }
            },
            new()
            {
                Name = "Bitmap Files",
                Extensions = new()
                {
                    "bmp",
                    "dib"
                }
            },
            new()
            {
                Name = "JPEG",
                Extensions = new()
                {
                    "jpg",
                    "jpeg",
                    "jpe",
                    "jfif"
                }
            },
            new()
            {
                Name = "DDS",
                Extensions = new()
                {
                    "dds"
                }
            },
            new()
            {
                Name = "TIFF",
                Extensions = new()
                {
                    "tif",
                    "tiff"
                }
            },
            new()
            {
                Name = "WEBP",
                Extensions = new()
                {
                    "webp"
                }
            },
            new()
            {
                Name = "All Files",
                Extensions = new()
                {
                    "*"
                }
            }
        };
    }
}
