using System.Collections.Generic;
using System.IO;
using System.Linq;
using SkiaSharp;

namespace Huffman.Drawing
{
    public class TreeDrawer
    {
        public void DrawNodesToFile(string filename, IDictionary<int, DrawNode> drawNodesByOrderNumber)
        {
            ResetBitmapAndCanvas(drawNodesByOrderNumber.Values);

            DrawConnections(drawNodesByOrderNumber);
            DrawShapes(drawNodesByOrderNumber);
            DrawStats(drawNodesByOrderNumber);

            SaveBitmapToFile(filename);
        }

        private void ResetBitmapAndCanvas(IEnumerable<DrawNode> drawNodes)
        {
            var minX = drawNodes.Min(n => n.Position.X);
            var maxX = drawNodes.Max(n => n.Position.X);
            var minY = drawNodes.Min(n => n.Position.Y);
            var maxY = drawNodes.Max(n => n.Position.Y);
            var width = (maxX - minX) + 100;
            var height = (maxY - minY) + 100;

            width += (100 - (width % 100));
            height += (100 - (height % 100));

            bitmap = new SKBitmap((int)width, (int)height);
            canvas = new SKCanvas(bitmap);
            canvas.Clear(SKColors.White);
        }

        private void SaveBitmapToFile(string filename)
        {
            using var fs = File.OpenWrite(filename);

            SKImage.FromBitmap(bitmap)
                .Encode(SKEncodedImageFormat.Png, 100)
                .SaveTo(fs);
        }

        private void DrawConnections(IDictionary<int, DrawNode> drawNodesByOrderNumber)
        {
            using var paint = new SKPaint
            {
                Color = SKColors.Pink,
                StrokeWidth = 4f
            };

            foreach (var keyValue in drawNodesByOrderNumber)
            {
                var drawNode = keyValue.Value;

                if (drawNode.LeftOrderNumber.HasValue)
                {
                    var leftPosition = drawNodesByOrderNumber[drawNode.LeftOrderNumber.Value].Position;

                    canvas.DrawLine(drawNode.Position, leftPosition, paint);
                }

                if (drawNode.RightOrderNumber.HasValue)
                {
                    var rightPosition = drawNodesByOrderNumber[drawNode.RightOrderNumber.Value].Position;

                    canvas.DrawLine(drawNode.Position, rightPosition, paint);
                }
            }
        }

        private void DrawShapes(IDictionary<int, DrawNode> drawNodesByOrderNumber)
        {
            using var paint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
            };

            foreach (var keyValue in drawNodesByOrderNumber)
            {
                var drawNode = keyValue.Value;
                paint.Color = drawNode.Color;

                if (drawNode.Shape == DrawNodeShape.Circle)
                {
                    canvas.DrawCircle(drawNode.Position, 25f, paint);
                }
                else if (drawNode.Shape == DrawNodeShape.Rect)
                {
                    canvas.DrawRect(new SKRect(
                        drawNode.Position.X - 25f,
                        drawNode.Position.Y - 25f,
                        drawNode.Position.X + 25f,
                        drawNode.Position.Y + 25f), paint);
                }
            }
        }

        private void DrawStats(IDictionary<int, DrawNode> drawNodesByOrderNumber)
        {
            using var paint = new SKPaint
            {
                Color = SKColors.Black,
                TextSize = 20f,
                TextAlign = SKTextAlign.Center,
                FakeBoldText = true
            };

            foreach (var keyValue in drawNodesByOrderNumber)
            {
                var drawNode = keyValue.Value;

                var weightPosition = new SKPoint(drawNode.Position.X, drawNode.Position.Y + 6f);
                canvas.DrawText(drawNode.Weight.ToString(), weightPosition, paint);

                var titlePosition = new SKPoint(drawNode.Position.X, drawNode.Position.Y + 40f);
                canvas.DrawText(drawNode.Title, titlePosition, paint);

                var orderPosition = new SKPoint(drawNode.Position.X - 20f, drawNode.Position.Y - 20f);
                canvas.DrawText(drawNode.OrderNumber.ToString(), orderPosition, paint);
            }
        }

        private SKBitmap bitmap;
        private SKCanvas canvas;
    }
}
