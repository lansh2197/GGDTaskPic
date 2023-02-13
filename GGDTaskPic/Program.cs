using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace GGDTaskPic
{
    class Program
    {

        static int ColorDiff(Color l, Color r)
        {
            int res = 0;
            int diff = (int)(l.R) - (int)(r.R);
            res += diff * diff;
            diff = (int)(l.G) - (int)(r.G);
            res += diff * diff;
            diff = (int)(l.B) - (int)(r.B);
            res += diff * diff;
            return res;
        }

        static void LocateImageIdentificationAnkorPoint()
        {
            //Color x1 = Color.FromKnownColor(KnownColor.Black);
            //Color y1 = Color.FromKnownColor(KnownColor.White);

            //int xxx = ColorDiff(x1, y1);

            //List<String> path = new List<string> { "S:/GGD/A.bmp", "S:/GGD/B.bmp" , "S:/GGD/C.bmp" };
            List<String> path = new List<string> { "S:/GGD/A.bmp", "S:/GGD/B.bmp" };

            List<Bitmap> bitmaps = new List<Bitmap>();
            for (int i = 0; i < path.Count; ++i)
            {
                bitmaps.Add((Bitmap)Image.FromFile(path[i]));
            }

            /*
            left
            int lx = 400;
            int ly = 200;

            int rx = 475;
            int ry = 280;
            */

            /*
            center
                int lx = 400;
                int ly = 200;

                int rx = 475;
                int ry = 280;
             */

            /*
            right
                int lx = 907;
                int ly = 194;

                int rx = 977;
                int ry = 280;
             */

            /*
             up
            int lx = 258;
            int ly = 309;

            int rx = 324;
            int ry = 389;
            */

            int lx = 271;
            int ly = 436;

            int rx = 311;
            int ry = 488;

            Console.WriteLine("D CF");

            for (int p = 0; p < path.Count; ++p)
            {
                int resDiff = 0;
                int resX = -1;
                int resY = -1;
                string hex = "";
                for (int x = lx; x <= rx; ++x)
                {
                    for (int y = ly; y < ry; ++y)
                    {
                        int diff = int.MaxValue;
                        Color targetColor = bitmaps[p].GetPixel(x, y);

                        for (int k = 0; k < path.Count; ++k)
                        {
                            if (k == p)
                            {
                                continue;
                            }
                            Color color = bitmaps[k].GetPixel(x, y);
                            int cdiff = ColorDiff(targetColor, color);
                            diff = Math.Min(diff, cdiff);
                        }

                        if (diff > resDiff)
                        {
                            resX = x;
                            resY = y;
                            resDiff = diff;
                            hex = ColorTranslator.ToHtml(targetColor);
                        }
                    }
                }

                Console.WriteLine(p);
                Console.WriteLine(resX + ", " + resY + ", " + hex);
            }

            Console.WriteLine();
        }

        static List<string> DirSearch(string sDir)
        {
            List<string> res = new List<string>();
            try
            {
                foreach (string f in Directory.GetFiles(sDir))
                {
                    res.Add(f);
                }
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    DirSearch(d);
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
            return res;
        }

        private static double CalculateColorDistance(Color l, Color r)
        {
            double d = Math.Sqrt(Math.Pow((int)l.R - (int)r.R, 2) + Math.Pow((int)l.G - (int)r.G, 2) + Math.Pow((int)l.B - (int)r.B, 2));
            return d;
        }

        private static double CalculateColorDistance(Byte lr, Byte lg, Byte lb, Byte rr, Byte rg, Byte rb)
        {
            int R1 = Convert.ToInt32(lr);
            int G1 = Convert.ToInt32(lg);
            int B1 = Convert.ToInt32(lb);
            int R2 = Convert.ToInt32(rr);
            int G2 = Convert.ToInt32(rg);
            int B2 = Convert.ToInt32(rb);
            int dist = (R1 - R2) * (R1 - R2) + (G1 - G2) * (G1 - G2) + (B1 - B2) * (B1 - B2);
            //double d = Math.Sqrt(Math.Pow((int)l.R - (int)r.R, 2) + Math.Pow((int)l.G - (int)r.G, 2) + Math.Pow((int)l.B - (int)r.B, 2));
            return dist;
        }

        private static void DisplayColorList(List<Color> colors, int colCount = -1, string saveName = "")
        {
            if (-1 == colCount)
            {
                colCount = (int)Math.Sqrt(colors.Count) + 1;
            }
            int count = colors.Count;
            int xCount = colCount;
            int yCount = (count - 1) / xCount + 1;
            int squareSize = 100;
            int bitmapSizeX = xCount * squareSize;
            int bitmapSizeY = yCount * squareSize;
            Bitmap b = new Bitmap(bitmapSizeX, bitmapSizeY);
            int idx = 0;
            for (int i = 0; i < bitmapSizeX; ++i)
            {
                for (int j = 0; j < bitmapSizeY; ++j)
                {
                    b.SetPixel(i, j, System.Drawing.ColorTranslator.FromHtml("#000000"));
                }
            }
            for (int y = 0; y < yCount; ++y)
            {
                for (int x = 0; x < xCount; ++x)
                {
                    if (idx >= count)
                    {
                        break;
                    }
                    for (int ki = 0; ki < squareSize; ++ki)
                    {
                        for (int kj = 0; kj < squareSize; ++kj)
                        {
                            b.SetPixel(x * squareSize + ki, y * squareSize + kj, colors[idx]);
                        }
                    }
                    idx++;

                }
            }

            Form form = new Form();
            form.AutoSize = true;
            PictureBox pixturebox = new PictureBox();
            pixturebox.SizeMode = PictureBoxSizeMode.AutoSize;
            pixturebox.ClientSize = new System.Drawing.Size(bitmapSizeX, bitmapSizeX);
            pixturebox.Image = (Image)b;
            pixturebox.Show();
            form.Controls.Add(pixturebox);

            pixturebox.Dock = DockStyle.None; pixturebox.AutoSize = true; form.AutoSize = true; form.Dock = DockStyle.Fill;
            form.ShowDialog();
            Console.WriteLine();

            if (saveName.Length > 0)
            {
                b.Save(saveName);
            }
        }
        private static void DisplayColorList(List<String> colors, int colCount = -1, string saveName = "")
        {
            DisplayColorList(colors.Select((String c) => System.Drawing.ColorTranslator.FromHtml(c)).ToList(), colCount, saveName);
        }

        private static string GetClosestColorName(Color color)
        {

            //PropertyInfo[] predefined = typeof(Color).GetProperties(BindingFlags.Public | BindingFlags.Static);
            KnownColor[] predefined = (KnownColor[])Enum.GetValues(typeof(KnownColor));

            //DisplayColorList(predefined.Select((KnownColor c) => Color.FromKnownColor(c)).ToList());

            double[] dist = new double[predefined.Length];
            for (int i = 0; i < predefined.Length; ++i)
            {
                Color l = Color.FromKnownColor(predefined[i]);
                Color r = color;
                dist[i] = CalculateColorDistance(l.R, l.G, l.B, r.R, r.G, r.B);
            }
            int minIdx = Array.IndexOf(dist, dist.Min());
            return predefined[minIdx].ToString();
        }

        private static KnownColor ToClosestKnownColor(Color color)
        {
            //PropertyInfo[] predefined = typeof(Color).GetProperties(BindingFlags.Public | BindingFlags.Static);
            KnownColor[] predefined = (KnownColor[])Enum.GetValues(typeof(KnownColor));
            double[] dist = new double[predefined.Length];
            for (int i = 0; i < predefined.Length; ++i)
            {
                Color l = Color.FromKnownColor(predefined[i]);
                Color r = color;
                dist[i] = CalculateColorDistance(l.R, l.G, l.B, r.R, r.G, r.B);
            }
            int minIdx = Array.IndexOf(dist, dist.Min());
            return predefined[minIdx];
        }

        static void GetAllTaskPositions()
        {
            // Captured Maps
            string folder = "S:\\GGD\\Map";
            List<string> pathes = DirSearch(folder);
            List<Bitmap> images = new List<Bitmap>();
            for (int i = 0; i < pathes.Count; ++i)
            {
                images.Add((Bitmap)Image.FromFile(pathes[i]));
            }

            // Interested area
            int lx = 247;
            int ly = 144;

            int rx = 1032;
            int ry = 730;

            Bitmap b = images[0];
            Bitmap res = b.Clone(new Rectangle(0, 0, b.Width, b.Height), b.PixelFormat);

            for (int y = ly; y < ry; ++y)
            {
                for (int x = lx; x < rx; ++x)
                {
                    /* With this dictionary, we can get distrubition of color at pixel
                     * across all map captures. In this way we can decide the typical
                     * decision threshold of distance to color yellow
                    Dictionary<string, int> countMap = new Dictionary<string, int>();
                    for (int k = 0; k < images.Count; ++k)
                    {

                        Color c = images[k].GetPixel(x, y);
                        string hex = ColorTranslator.ToHtml(c);
                        if (!countMap.ContainsKey(hex))
                        {
                            countMap[hex] = 0;
                        }
                        countMap[hex]++;
                    }
                    */
                    bool found = false;
                    int decisionThreshold1 = 30000;
                   
                    for (int k = 0; k < images.Count; ++k)
                    {
                        Color c = images[k].GetPixel(x, y);
                        double dist = CalculateColorDistance(c.R, c.G, c.B, Color.Yellow.R, Color.Yellow.G, Color.Yellow.B);
                        if (dist < decisionThreshold1)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (found)
                    {
                        res.SetPixel(x, y, Color.Yellow);
                    }

                }
                Console.WriteLine(y);
            }

            List<int[]> tasks = FindCenterOfMapClusters(ref res);

            foreach (int[] x in tasks)
            {
                Console.WriteLine("{" + x[0] + ", " + x[1] + "},");
            }

            /*
            int[,] tasks = new int[,] {
                {327, 590},
                {426, 329},
                {431, 282},
                {472, 353},
                {478, 194},
                {477, 232},
                {500, 238},
                {503, 337},
                {516, 550},
                {519, 286},
                {521, 505},
                {522, 183},
                {532, 356},
                {533, 264},
                {564, 471},
                {565, 346},
                {568, 627},
                {576, 239},
                {583, 545},
                {593, 194},
                {643, 367},
                {663, 532},
                {668, 286},
                {692, 255},
                {706, 184},
                {731, 459},
                {736, 337},
                {742, 244},
                {761, 334},
                {763, 365},
                {765, 561},
                {795, 298},
                {817, 262},
                {824, 624},
                {831, 662},
                {845, 372},
                {868, 649},
                {874, 292},
                {938, 322}
            };
            */

            // This threshold can be calculated by getting all distances of all task
            // points on a maps, get the decision threshold between 5th and 6th ranked
            // distance
            int decisionThreshold = 30000;

            for (int k = 0; k < pathes.Count; ++k)
            {
                string path = pathes[k];
                Bitmap image = (Bitmap)Image.FromFile(path);

                List<int[]> foundTasks = new List<int[]>();
                for (int i = 0; i < tasks.Count; ++i)
                {
                    int x = tasks[i][0];
                    int y = tasks[i][1];
                    Color c = image.GetPixel(x, y);
                    double dist = CalculateColorDistance(c.R, c.G, c.B, Color.Yellow.R, Color.Yellow.G, Color.Yellow.B);

                    if (dist < decisionThreshold)
                    {
                        foundTasks.Add(new int[2] { x, y });
                    }
                }

                /*
                foreach (var x in foundTasks)
                {
                    Console.Write("{" + x[0] + ", " + x[1] + "},");
                }
                */

                if (foundTasks.Count == 5)
                {
                    Bitmap clonedImage = image.Clone(new Rectangle(0, 0, image.Width, image.Height), image.PixelFormat);
                    foreach (int[] task in foundTasks)
                    {
                        for (int i = -2; i <= 2; ++i)
                        {
                            for (int j = -2; j <= 2; ++j)
                            {
                                Color ccc = Color.Red;
                                clonedImage.SetPixel(task[0] + i, task[1] + j, ccc);
                            }
                        }

                        RectangleF rectf = new RectangleF(task[0] - 3, task[1] + 7, 90, 50);

                        Graphics g = Graphics.FromImage(clonedImage);

                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        //g.DrawString("{" + task[0] + ", " + task[1] + "}", new Font("Tahoma", 8), Brushes.White, rectf);
                        g.DrawString("{" + task[0] + ", " + task[1] + "}", new Font("Tahoma", 7, FontStyle.Bold), new SolidBrush(Color.FromArgb(6, 216, 6)), rectf);


                        g.Flush();
                    }

                    string filename = Path.GetFileName(path);
                    string outputPath = folder + "\\output\\" + filename;
                    clonedImage.Save(outputPath);
                    clonedImage.Dispose();
                }
                else
                {
                    ;
                }
                //Console.WriteLine("{0} {1} {2} {3} {4} {5} ", dists[0], dists[1], dists[2], dists[3], dists[4], dists[5]);
            }
            res.Save("S:\\GGD\\map\\output\\all_tasks_position.bmp");
            Console.WriteLine();
        }

        static bool ColorEqual(Color l, Color r)
        {
            return l.R == r.R && l.G == r.G && l.B == r.B;
        }

        static void GetClusterFromPos(ref Bitmap b, ref Color p, int x, int y, int width, int height, ref List<int[]> cluster, ref HashSet<string> visited)
        {
            string pos = "" + x + " " + y;
            if (visited.Contains(pos) || x < 0 || y < 0 || x >= width || y >= height)
            {
                return;
            }
            visited.Add(pos);

            Color n = b.GetPixel(x, y);
            if (!ColorEqual(p, n))
            {
                return;
            }

            cluster.Add(new int[2] { x, y });
            int[,] dir = new int[,] { { 1, 0 }, { 0, 1 }, { -1, 0 }, { 0, -1 } };

            for (int i = 0; i < 4; ++i)
            {
                int xn = x + dir[i, 0];
                int yn = y + dir[i, 1];

                GetClusterFromPos(ref b, ref n, xn, yn, width, height, ref cluster, ref visited);

            }
            return;
        }
        static List<int[]> FindCenterOfMapClusters(ref Bitmap b)
        {
            List<int[]> ret = new List<int[]>();
            int w = b.Width;
            int h = b.Height;
            HashSet<string> visited = new HashSet<string>();
            for (int i = 0; i < w; ++i)
            {
                for (int j = 0; j < h; ++j)
                {
                    string pos = "" + i + " " + j;
                    if (visited.Contains(pos))
                    {
                        continue;
                    }
                    Color c = b.GetPixel(i, j);
                    if (c.R == 255 && c.G == 255 && c.B == 0)
                    {
                        // it is yellow
                        List<int[]> cluster = new List<int[]>();
                        GetClusterFromPos(ref b, ref c, i, j, w, h, ref cluster, ref visited);
                        double xt = 0;
                        double yt = 0;
                        foreach (int[] pp in cluster)
                        {
                            xt += pp[0];
                            yt += pp[1];
                        }
                        xt /= (double)cluster.Count;
                        yt /= (double)cluster.Count;

                        xt = Math.Round(xt, MidpointRounding.AwayFromZero);
                        yt = Math.Round(yt, MidpointRounding.AwayFromZero);
                        ret.Add(new int[2] { (int)xt, (int)yt });
                    }
                }
            }
            return ret;
        }

        static void PrintClusterCenter()
        {
            Bitmap b = (Bitmap)Image.FromFile("S:\\GGD\\Map\\output\\all_tasks_position.bmp");
            List<int[]> foundTasks = FindCenterOfMapClusters(ref b);
/*            foreach (int[] x in ret)
            {
                Console.WriteLine("{" + x[0] + ", " + x[1] + "},");
            }*/
            Bitmap output = b.Clone(new Rectangle(0, 0, b.Width, b.Height), b.PixelFormat);
/*            foreach (int[] x in ret)
            {
                for (int i = -2; i <= 2; ++i)
                {
                    for (int j = -2; j <= 2; ++j)
                    {
                        Color ccc = Color.Red;
                        output.SetPixel(x[0] + i, x[1] + j, ccc);
                    }
                }
            }*/

            foreach (int[] task in foundTasks)
            {
/*
                for (int i = -2; i <= 2; ++i)
                {
                    for (int j = -2; j <= 2; ++j)
                    { 
                        output.SetPixel(task[0] + i, task[1] + j, Color.Red);
                    }
                }
*/
                output.SetPixel(task[0], task[1], Color.Red);

                RectangleF rectf = new RectangleF(task[0] - 3, task[1] + 7, 90, 50);

                Graphics graphic = Graphics.FromImage(output);

                graphic.SmoothingMode = SmoothingMode.AntiAlias;
                graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;

                graphic.DrawString("{" + task[0] + ", " + task[1] + "}", new Font("Tahoma", 7, FontStyle.Bold), new SolidBrush(Color.FromArgb(6, 216, 6)), rectf);

                graphic.Flush();
            }

            output.Save("S:\\GGD\\Map\\output\\all_tasks_position_marked.bmp");
        }


        static void IdentifyTasks()
        {
            int[,] tasks = new int[,] {
                {327, 590},
                {426, 329},
                {431, 282},
                {472, 353},
                {478, 194},
                {477, 232},
                {500, 238},
                {503, 337},
                {516, 550},
                {519, 286},
                {521, 505},
                {522, 183},
                {532, 356},
                {533, 264},
                {564, 471},
                {565, 346},
                {568, 627},
                {576, 239},
                {583, 545},
                {593, 194},
                {643, 367},
                {663, 532},
                {668, 286},
                {692, 255},
                {706, 184},
                {731, 459},
                {736, 337},
                {742, 244},
                {761, 334},
                {763, 365},
                {765, 561},
                {795, 298},
                {817, 262},
                {824, 624},
                {831, 662},
                {845, 372},
                {868, 649},
                {874, 292},
                {938, 322}
            };

            string path = "S:\\GGD\\Map";
            List<String> pathes = DirSearch(path);

            for (int k = 0; k < pathes.Count; ++k)
            {
                Bitmap image = (Bitmap)Image.FromFile(pathes[k]);

                List<int[]> foundTasks = new List<int[]>();
                List<double> dists = new List<double>();
                for (int i = 0; i < tasks.GetLength(0); ++i)
                {
                    int x = tasks[i, 0];
                    int y = tasks[i, 1];
                    Color ccc = image.GetPixel(x, y);
                    double dist = CalculateColorDistance(ccc.R, ccc.G, ccc.B, 255, 255, 0);
                    dists.Add(dist);
                    dists.Sort();
                    // 67704
                    if (dist < 30000)
                    {
                        foundTasks.Add(new int[2] { x, y });
                    }
                }
                string ppp = pathes[k];
                Console.WriteLine(pathes[k]);
                Console.WriteLine(foundTasks.Count);
                foreach (var x in foundTasks)
                {
                    Console.Write("{" + x[0] + ", " + x[1] + "},");
                }
                Console.WriteLine();
                if (foundTasks.Count == 5)
                {
                    Bitmap bb = image.Clone(new Rectangle(0, 0, image.Width, image.Height), image.PixelFormat);
                    foreach (int[] x in foundTasks)
                    {
                        for (int i = -2; i <= 2; ++i)
                        {
                            for (int j = -2; j <= 2; ++j)
                            {
                                Color ccc = Color.Red;
                                bb.SetPixel(x[0] + i, x[1] + j, ccc);
                            }
                        }

                        RectangleF rectf = new RectangleF(x[0] - 3, x[1] + 7, 90, 50);

                        Graphics g = Graphics.FromImage(bb);

                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        g.DrawString("{" + x[0] + ", " + x[1] + "}", new Font("Tahoma", 8), Brushes.White, rectf);

                        g.Flush();

                    }


                    string filename = Path.GetFileName(pathes[k]);
                    string path1 = path + "\\output\\" + filename;
                    bb.Save(path1);
                    bb.Dispose();
                }
                else
                {
                    ;
                }
                Console.WriteLine("{0} {1} {2} {3} {4} {5} ", dists[0], dists[1], dists[2], dists[3], dists[4], dists[5]);
            }
            Console.WriteLine();
        }

        static void LocateImageIdentificationAnkorPoint2()
        {
            // lx, ly, rx, ry
            //int[,] interestedArea = new int[,] { { 400, 185, 100, 100 }, { 640, 185, 100, 100 } , { 890, 185, 100, 100 }, { 270, 320, 50, 65 }, { 270, 435, 50, 65 } };

            // Flask Shelf
            //int[,] interestedArea = new int[,] { { 300, 140, 100, 100 }, { 300, 260, 100, 100 }, { 300, 390, 100, 100 }, { 910, 140, 100, 100 }, { 910, 260, 100, 100 }, { 910, 390, 100, 100 } };

            // Flask Bench
            int[,] interestedArea = new int[,] { { 330, 600, 100, 100 }, { 450, 600, 100, 100 }, { 550, 600, 100, 100 }, { 670, 600, 100, 100 }, { 780, 600, 100, 100 }, { 890, 600, 100, 100 } };

            List<int> resX = new List<int>();
            List<int> resY = new List<int>();
            List<String> resH = new List<string>();
            List<Bitmap> resBitmap = new List<Bitmap>();

            string path = "S:\\GGD\\Flaskes";
            string outputInterestedAreaPath = path + "\\output";
            List<String> pathes = DirSearch(path + "\\input");

            List<Bitmap> images = new List<Bitmap>();
            for (int i = 0; i < pathes.Count; ++i)
            {
                images.Add((Bitmap)Image.FromFile(pathes[i]));
            }

            // Process each interested area
            for (int i = 0; i < interestedArea.GetLength(0); ++i)
            {
                // Step 1: cluster
                List<Bitmap> subImageList = new List<Bitmap>();
                Dictionary<string, Bitmap> bitmapHashDict = new Dictionary<string, Bitmap>();
                for (int k = 0; k < images.Count; ++k)
                {
                    Bitmap image = images[k];
                    Rectangle rect = new Rectangle(interestedArea[i, 0], interestedArea[i, 1], interestedArea[i, 2], interestedArea[i, 3]);
                    System.Drawing.Imaging.PixelFormat format = image.PixelFormat;
                    Bitmap subImage = image.Clone(rect, format);
                    subImageList.Add(subImage);

                    //Convert each image to a byte array
                    System.Drawing.ImageConverter ic = new System.Drawing.ImageConverter();
                    byte[] btImage1 = new byte[1];
                    btImage1 = (byte[])ic.ConvertTo(subImage, btImage1.GetType());

                    //Compute a hash for each image
                    SHA256Managed shaM = new SHA256Managed();
                    byte[] subbitmapHash = shaM.ComputeHash(btImage1);
                    string result = System.Text.Encoding.UTF8.GetString(subbitmapHash);
                    bitmapHashDict[result] = subImage;
                }

                List<Bitmap> distinctBitmaps = new List<Bitmap>(bitmapHashDict.Values);

                for (int dIdx = 0; dIdx < distinctBitmaps.Count; ++dIdx)
                {
                    int lx = interestedArea[i, 0];
                    int ly = interestedArea[i, 1];
                    int w = interestedArea[i, 2];
                    int h = interestedArea[i, 3];

                    int resDiff = 0;
                    int rX = -1;
                    int rY = -1;
                    string hex = "";
                    for (int x = 0; x < w; ++x)
                    {
                        for (int y = 0; y < h; ++y)
                        {
                            int diff = int.MaxValue;
                            Color targetColor = distinctBitmaps[dIdx].GetPixel(x, y);

                            for (int k = 0; k < distinctBitmaps.Count; ++k)
                            {
                                if (k == dIdx)
                                {
                                    continue;
                                }
                                Color color = distinctBitmaps[k].GetPixel(x, y);
                                int cdiff = ColorDiff(targetColor, color);
                                diff = Math.Min(diff, cdiff);
                            }

                            if (diff > resDiff)
                            {
                                rX = lx + x;
                                rY = ly + y;
                                resDiff = diff;
                                hex = ColorTranslator.ToHtml(targetColor);
                            }
                        }
                    }

                    Console.WriteLine(dIdx);
                    Console.WriteLine(rX + ", " + rY + ", " + hex);
                    resX.Add(rX);
                    resY.Add(rY);
                    // revert RGB to BGR to fix with autokey
                    string BGRHex = hex.Substring(5, 2) + hex.Substring(3, 2) + hex.Substring(1, 2);
                    resH.Add(BGRHex);
                    resBitmap.Add(distinctBitmaps[dIdx]);
                }
            }

            String ListJoin<T>(List<T> list, bool quote = false)
            {
                String ret = "";
                for (int i = 0; i < list.Count; ++i)
                {

                    if (i == 0)
                    {
                        ret += "(";
                    }

                    if (quote)
                    {
                        ret += "\"";
                    }
                    ret += list[i];
                    if (quote)
                    {
                        ret += "\"";
                    }
                    if (i != resX.Count - 1)
                    {
                        ret += ", ";
                    }
                    else
                    {
                        ret += ")";
                    }
                }
                return ret;
            }

            Console.WriteLine(ListJoin(resX));
            Console.WriteLine(ListJoin(resY));
            Console.WriteLine(ListJoin(resH, true));

            if (!Directory.Exists(outputInterestedAreaPath))
            {
                Directory.CreateDirectory(outputInterestedAreaPath);
            }

            for (int i = 0; i < resBitmap.Count; ++i)
            {
                resBitmap[i].Save(outputInterestedAreaPath + "\\" + i + ".bmp");
            }
            Console.WriteLine();
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateDC(
         string lpszDriver,         // driver name驱动名
         string lpszDevice,         // device name设备名
         string lpszOutput,         // not used; should be NULL
         IntPtr lpInitData   // optional printer data
         );
        [DllImport("gdi32.dll")]
        public static extern int BitBlt(
         IntPtr hdcDest, // handle to destination DC目标设备的句柄
         int nXDest,   // x-coord of destination upper-left corner目标对象的左上角的X坐标
         int nYDest,   // y-coord of destination upper-left corner目标对象的左上角的Y坐标
         int nWidth,   // width of destination rectangle目标对象的矩形宽度
         int nHeight, // height of destination rectangle目标对象的矩形长度
         IntPtr hdcSrc,   // handle to source DC源设备的句柄
         int nXSrc,    // x-coordinate of source upper-left corner源对象的左上角的X坐标
         int nYSrc,    // y-coordinate of source upper-left corner源对象的左上角的Y坐标
         UInt32 dwRop   // raster operation code光栅的操作值
         );

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(
         IntPtr hdc // handle to DC
         );

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(
         IntPtr hdc,         // handle to DC
         int nWidth,      // width of bitmap, in pixels
         int nHeight      // height of bitmap, in pixels
         );

        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(
         IntPtr hdc,           // handle to DC
         IntPtr hgdiobj    // handle to object
         );

        [DllImport("gdi32.dll")]
        public static extern int DeleteDC(
         IntPtr hdc           // handle to DC
         );

        [DllImport("user32.dll")]
        public static extern bool PrintWindow(
         IntPtr hwnd,                // Window to copy,Handle to the window that will be copied.
         IntPtr hdcBlt,              // HDC to print into,Handle to the device context.
         UInt32 nFlags               // Optional flags,Specifies the drawing options. It can be one of the following values.
         );

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(
         IntPtr hwnd
         );

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr hWndChildAfter, string className, string windowTitle);

        public static Bitmap GetWindowCapture(IntPtr hWnd)
        {
            IntPtr hscrdc = GetWindowDC(hWnd);
            Rect windowRect = new Rect();
            GetWindowRect(hWnd, ref windowRect);
            int width = (int)(windowRect.Right - windowRect.Left);
            int height = (int)(windowRect.Bottom - windowRect.Top);
            width = 200;
            height = 200;

            IntPtr hbitmap = CreateCompatibleBitmap(hscrdc, width, height);
            IntPtr hmemdc = CreateCompatibleDC(hscrdc);
            SelectObject(hmemdc, hbitmap);
            PrintWindow(hWnd, hmemdc, 0);
            Bitmap bmp = Bitmap.FromHbitmap(hbitmap);
            DeleteDC(hscrdc);//删除用过的对象
            DeleteDC(hmemdc);//删除用过的对象
            return bmp;
        }

        [DllImport("User32.dll")]
        public static extern Int32 SendMessage(IntPtr hWnd, int Msg, int wParam, [MarshalAs(UnmanagedType.LPStr)] string lParam);


        //static IntPtr GetWindowHandle(string windowName)
        //{
        //IntPtr hWnd = (IntPtr)FindWindowEx(null, 0, "MozillaUIWindowClass", "");
        //return hWnd;
        //return null;
        //}

        public static IntPtr WinGetHandle(string wName)
        {
            IntPtr hWnd = IntPtr.Zero;
            foreach (Process pList in Process.GetProcesses())
            {
                if (pList.MainWindowTitle.Contains(wName))
                {
                    hWnd = pList.MainWindowHandle;
                }
            }
            return hWnd; //Should contain the handle but may be zero if the title doesn't match
        }

        static void ScreenCaptureTest()
        {
            Image objImage = new Bitmap(400, 300);
            Graphics g = Graphics.FromImage(objImage);
            g.CopyFromScreen(new System.Drawing.Point(Cursor.Position.X - 150, Cursor.Position.Y - 25), new System.Drawing.Point(0, 0), new System.Drawing.Size(400, 300));
            IntPtr dc1 = g.GetHdc();
            g.ReleaseHdc(dc1);

            objImage.Save("S:\\test.bmp");
        }

        static void Foo()
        {
            var notepad = FindWindow("Chrome", null);

            var edit = FindWindowEx(notepad, IntPtr.Zero, "edit", null);

            SendMessage(edit, 0x000C, 0, "A");
            SendMessage(edit, 0x000C, 0, "E");
            SendMessage(edit, 0x000C, 0, "F");
        }


        static void Foo2()
        {
            IntPtr p = (IntPtr)FindWindow("Everything", null);
            Bitmap b = GetWindowCapture(p);
            b.Save("S:\\test.bmp", ImageFormat.Bmp);
            Console.WriteLine();
        }


        static void Foo3()
        {
            String file = "S:\\GGD\\Audio\\1.bmp";
            Bitmap b = (Bitmap)Image.FromFile(file);
            int x = 394;
            int y = 430;
            for (int i = 0; i < 100; ++i)
            {
                Color targetColor = b.GetPixel(x + i, y);
                String hex = ColorTranslator.ToHtml(targetColor);
                Console.WriteLine(x + i);
                Console.WriteLine(hex);
            }
        }


        [DllImport("User32.Dll")]
        public static extern long SetCursorPos(int x, int y);

        [DllImport("User32.Dll")]
        public static extern bool ClientToScreen(IntPtr hWnd, ref POINT point);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;

            public POINT(int X, int Y)
            {
                x = X;
                y = Y;
            }
        }

        static void Drag(uint startX, uint startY, uint endX, uint endY)
        {
            endX = endX - startX;
            endY = endY - startY;
            SetCursorPos((int)startX, (int)startY);
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_MOVE, endX, endY, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

        public static void DoMouseClick()
        {

            for (int i = 50; i < 100; ++i)
            {
                for (int j = 200; j < 250; ++j)
                {
                    SetCursorPos(i, j);
                    uint XX = (uint)i;
                    uint YY = (uint)j;
                    Thread.Sleep(1000);

                }
            }
            mouse_event(MOUSEEVENTF_LEFTUP, 50, 200, 0, 0);
        }

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        public const int KEYEVENTF_EXTENDEDKEY = 0x0001; //Key down flag
        public const int KEYEVENTF_KEYUP = 0x0002; //Key up flag
        public const int VK_RCONTROL = 0xA3; //Right Control key code

        public const int VK_A = 0x41;
        public const int VK_CONTROL = 0x11;



        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
        //Mouse actions
        private const int MOUSEEVENTF_MOVE = 0x01;
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        private static ushort WM_SYSKEYDOWN = 0x0104;
        private static ushort WM_CHAR = 0x0102;
        private static ushort WM_KEYDOWN = 0x0100;



        public static void KeyPress(int keycode, int delay = 0)
        {
            keybd_event((byte)keycode, 0x0, 0, 0);// presses
            System.Threading.Thread.Sleep(delay);
            keybd_event((byte)keycode, 0x0, 2, 0); //releases
        }

        static void Foo5()
        {
            var notepad = FindWindow("Notepad", null);

            var edit = FindWindowEx(notepad, IntPtr.Zero, "edit", null);

            KeyPress(VK_A, 1000000); //5 second

            /*
            SendMessage(edit, 0x000C, 0, "A");

            SendMessage(edit, 0x000C, 0, "AAAA");
            */
            Thread.Sleep(100000);
        }

        static void Foo6()
        {
            while (true)
            {
                Process[] processes = Process.GetProcessesByName("Notepad");

                foreach (Process proc in processes)
                {
                    SetForegroundWindow(proc.MainWindowHandle);
                    KeyPress(0x41, 10000);
                }

                Thread.Sleep(5000);
            }
        }
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern short VkKeyScan(char ch);

        static void Foo7()
        {
            Process[] processes = Process.GetProcessesByName("Notepad");

            foreach (Process proc in processes)
            {
                SetForegroundWindow(proc.MainWindowHandle);
                Thread.Sleep(1000);
                /*
                Thread.Sleep(5000);
                keybd_event(VK_CONTROL, 0x9d, 0, 0); // Ctrl Press
                keybd_event((byte)VkKeyScan('A'), 0x9e, 0, 0); // ‘A’ Press
                keybd_event((byte)VkKeyScan('A'), 0x9e, KEYEVENTF_KEYUP, 0); // ‘A’ Release
                keybd_event(VK_CONTROL, 0x9d, KEYEVENTF_KEYUP, 0); // Ctrl Releas
                */
                keybd_event((byte)VkKeyScan('A'), 0x9e, 0, 0); // ‘A’ Press
                Thread.Sleep(10000);
            }
        }

        static void Main(string[] args)
        {
            //GenerateBackGroundImage();
            //PrintClusterCenter();
            //IdentifyTasks();
            GetAllTaskPositions();
            PrintClusterCenter();
            /*
            SetCursorPos(50, 200);
            mouse_event(MOUSEEVENTF_LEFTDOWN, 50, 200, 0, 0);
            SetCursorPos(100, 200);
            */
        }
    }
}
