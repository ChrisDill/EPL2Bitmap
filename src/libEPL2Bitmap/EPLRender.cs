﻿using System;
using System.Drawing;
using BarcodeLib;

namespace libEPL2Bitmap
{
    public partial class EPL2Bitmap
    {
        /// <summary>
        /// Render barcode string on bitmap
        /// </summary>
        /// <param name="line"></param>
        /// <param name="bmp"></param>
        private static void RenderBarcode(string line, ref Bitmap bmp)
        {
            if (args.Length < 9)
            {
                Log("RenderBarcode failed, 9 arguments required: " + line);
                return;
            }

            // horizontal start position
            // vertical start position
            // rotation
            // barcode selection
            // narrow bar width
            // wide bar width
            // bar code height
            // reverse image
            int x = GetArg(0);
            int y = GetArg(1);
            int rotation = GetArg(2) * 90;
            string selection = args[3];
            int narrow = GetArg(4);
            int wide = GetArg(5);
            int height = GetArg(6);
            char code = args[7].ToCharArray()[0];
            string data = args[8];
            data = data.Replace("\"", "");

            Log("RenderBarcode " + string.Format("Position({0},{1}), Rotation({2}), Selection({3}), Narrow/Wide/Height({4},{5},{6}), Code({6}), Data({7})", x, y, rotation, selection, narrow, wide, height, code, data));

            // using selection, narrow, wide to determine which font to use(page 51 EPL spec)

            // p4 value select font
            switch (selection)
            {
                case "3":
                    break;
                case "3C":
                    break;
                case "9":
                    break;
                case "0":
                    break;
                case "1":
                    break;
                case "1A":
                    break;
                case "1B":
                    break;
                case "1C":
                    break;
                case "1D":
                    break;
                case "K":
                    break;
                case "E80":
                    break;
                case "E82":
                    break;
                case "E85":
                    break;
                case "E30":
                    break;
                case "E32":
                    break;
                case "E35":
                    break;
                case "2G":
                    break;
                case "2":
                    break;
                case "2C":
                    break;
                case "2D":
                    break;
                case "P":
                    break;
                case "PL":
                    break;
                case "J":
                    break;
                case "1E":
                    break;
                case "UA0":
                    break;
                case "UA2":
                    break;
                case "UA5":
                    break;
                case "UE0":
                    break;
                case "UE2":
                    break;
                case "UE5":
                    break;
                case "2U":
                    break;
                case "L":
                    break;
                case "M":
                    break;
            }

            var type = TYPE.EAN13;
            var bar = new Barcode(data, type);
            bar.AlternateLabel = "1-11506-444551";
            bar.IncludeLabel = true;
            bar.LabelFont = fonts[3];
            bar.LabelPosition = LabelPositions.BOTTOMCENTER;
            bar.BarWidth = narrow;
            bar.Height = height;

            var img = bar.Encode(type, data, Color.Black, Color.White);
            graphics.ResetTransform();
            graphics.DrawImage(img, x, y);
        }

        /// <summary>
        /// Draw a string using loaded arguments
        /// Arguments:
        /// horizontal start position
        /// vertical start position
        /// rotation
        /// font selection
        /// horizontal multiplier
        /// vertical multiplier
        /// reverse image
        /// </summary>
        /// <param name="line"></param>
        /// <param name="bmp"></param>
        private static void RenderString(string line, ref Bitmap bmp)
        {
            if (args.Length < 8)
            {
                Log("RenderString failed, 8 arguments required: " + line);
                return;
            }

            int x = GetArg(0);
            int y = GetArg(1);
            int rotation = GetArg(2) * 90;
            int fontId = GetArg(3);
            int scaleX = GetArg(4);
            int scaleY = GetArg(5);
            var type = GetEPLReverseType(args[6].ToCharArray()[0]);
            string data = args[7];
            data = data.Replace("\"", "");

            Brush back = null, text = null;
            switch (type)
            {
                case EPLReverseTypeEnum.BlackOnWhite:
                    back = Brushes.White;
                    text = Brushes.Black;
                    break;
                case EPLReverseTypeEnum.WhiteOnBlack:
                    back = Brushes.Black;
                    text = Brushes.White;
                    break;
                case EPLReverseTypeEnum.Unknown:
                    Log("EPLReverseTypeEnum Unknown. Cannot select brush type for string.");
                    throw (new ArgumentException());
            }

            Log("RenderString " + string.Format("Position({0},{1}), Rotation({2}), Font({3}), Scale({4},{5}), Type({6}), Data({7})", x, y, rotation, fontId, scaleX, scaleY, type, data));

            // use size of text for background
            SetTransform(x, y, rotation, scaleX, scaleY);
            var font = fonts[fontId - 1];

            var size = graphics.MeasureString(data, font);
            graphics.FillRectangle(back, x, y, size.Width, size.Height);
            graphics.DrawString(data, font, text, x, y);
        }

        /// <summary>
        /// Draw a box using arguments
        /// </summary>
        /// <param name="line"></param>
        private void RenderBox(string line)
        {
            if (args.Length < 5)
            {
                Log("RenderBox failed, 5 arguments required: " + line);
                return;
            }

            int x1 = GetArg(0);
            int y1 = GetArg(1);
            int thickness = GetArg(2);
            int x2 = GetArg(3);
            int y2 = GetArg(4);

            // draw top left to bottom right
            Rectangle box = new Rectangle();
            box.X = Math.Min(x1, x2);
            box.Y = Math.Min(y1, y2);
            box.Width = Math.Abs(x1 - x2);
            box.Height = Math.Abs(y1 - y2);

            Pen pen = new Pen(Color.Black, thickness);
            graphics.DrawRectangle(pen, box);
        }

        /// <summary>
        /// Draw a line using arguments
        /// </summary>
        /// <param name="line"></param>
        private void RenderLine(string line)
        {
            if (args.Length < 4)
            {
                Log("RenderLine failed, 4 arguments required: " + line);
                return;
            }

            int x = GetArg(0);
            int y = GetArg(1);
            int lengthX = GetArg(2);
            int lengthY = GetArg(3);

            // LE xor
            // LO draw black
            // LS draw diagonal
            // LW draw white

            // temp check for type
            // xor test
            if (line.Contains("LE"))
            {
                Rectangle rect = new Rectangle(x, y, lengthX, lengthY);
                Region region = new Region();
                region.MakeEmpty();
                region.Xor(rect);

                graphics.FillRegion(Brushes.Black, region);
            }
            // normal line
            else if (line.Contains("LO"))
            {
                graphics.FillRectangle(Brushes.Black, x, y, lengthX, lengthY);
            }
            // diagonal line
            else
            {
                Pen pen = new Pen(Color.Black, 1);
                graphics.DrawLine(pen, new Point(x, y), new Point(lengthX, lengthY));
            }
        }
    }
}
