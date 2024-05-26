using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private List<PointF> setA = new List<PointF>();
        private List<PointF> setB = new List<PointF>();
        private Triangle minimalTriangle;
        private Font drawFont = new Font("Arial", 12);
        private SolidBrush drawBrush = new SolidBrush(Color.Black);

        public Form1()
        {
            InitializeComponent();

            // Инициализация множества точек A и B
            setA.AddRange(new PointF[] {
                new PointF(1, 1),
                new PointF(5, 1),
                new PointF(3, 4),
                new PointF(2, 6)
            });

            setB.AddRange(new PointF[] {
                new PointF(3, 3),
                new PointF(4, 3)
            });

            this.Width = 800;
            this.Height = 600;
            this.BackColor = Color.LightGray; // Устанавливаем серый цвет фона

            FindMinimalTriangle();
        }

        private void FindMinimalTriangle()
        {
            float minArea = float.MaxValue;
            Triangle bestTriangle = null;

            for (int i = 0; i < setA.Count; i++)
            {
                for (int j = i + 1; j < setA.Count; j++)
                {
                    for (int k = j + 1; k < setA.Count; k++)
                    {
                        var triangle = new Triangle(setA[i], setA[j], setA[k]);
                        if (ContainsAllPoints(triangle, setB))
                        {
                            float area = triangle.Area();
                            if (area < minArea)
                            {
                                minArea = area;
                                bestTriangle = triangle;
                            }
                        }
                    }
                }
            }

            if (bestTriangle != null)
            {
                minimalTriangle = bestTriangle;
                Console.WriteLine($"Минимальный треугольник с площадью: {minArea}");
            }
            else
            {
                Console.WriteLine("Решение не найдено.");
            }
        }

        private bool ContainsAllPoints(Triangle triangle, List<PointF> points)
        {
            return points.All(p => triangle.Contains(p));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;

            // Рисуем линии между точками множества A
            for (int i = 0; i < setA.Count; i++)
            {
                for (int j = i + 1; j < setA.Count; j++)
                {
                    g.DrawLine(Pens.Blue, setA[i].X * 50, setA[i].Y * 50, setA[j].X * 50, setA[j].Y * 50);
                }
            }

            // Рисуем линии между точками множества B
            for (int i = 0; i < setB.Count; i++)
            {
                for (int j = i + 1; j < setB.Count; j++)
                {
                    g.DrawLine(Pens.Red, setB[i].X * 50, setB[i].Y * 50, setB[j].X * 50, setB[j].Y * 50);
                }
            }

            // Рисуем точки и буквы из множества A
            char label = 'A';
            foreach (var point in setA)
            {
                g.FillEllipse(Brushes.Blue, point.X * 50 - 2.5f, point.Y * 50 - 2.5f, 5, 5);
                g.DrawString(label.ToString(), drawFont, drawBrush, point.X * 50 + 5, point.Y * 50);
                label++;
            }

            // Рисуем точки и буквы из множества B
            label = 'B';
            foreach (var point in setB)
            {
                g.FillEllipse(Brushes.Red, point.X * 50 - 2.5f, point.Y * 50 - 2.5f, 5, 5);
                g.DrawString(label.ToString(), drawFont, drawBrush, point.X * 50 + 5, point.Y * 50);
                label++;
            }

            // Рисуем минимальный треугольник
            if (minimalTriangle != null)
            {
                g.DrawPolygon(Pens.Green, minimalTriangle.GetPoints().Select(p => new PointF(p.X * 50, p.Y * 50)).ToArray());
            }
        }
    }

    public class Triangle
    {
        public PointF A { get; }
        public PointF B { get; }
        public PointF C { get; }

        public Triangle(PointF a, PointF b, PointF c)
        {
            A = a;
            B = b;
            C = c;
        }

        public float Area()
        {
            return Math.Abs((A.X * (B.Y - C.Y) + B.X * (C.Y - A.Y) + C.X * (A.Y - B.Y)) / 2.0f);
        }

        public bool Contains(PointF p)
        {
            float alpha = ((B.Y - C.Y) * (p.X - C.X) + (C.X - B.X) * (p.Y - C.Y)) /
                          ((B.Y - C.Y) * (A.X - C.X) + (C.X - B.X) * (A.Y - C.Y));
            float beta = ((C.Y - A.Y) * (p.X - C.X) + (A.X - C.X) * (p.Y - C.Y)) /
                         ((B.Y - C.Y) * (A.X - C.X) + (C.X - B.X) * (A.Y - C.Y));
            float gamma = 1.0f - alpha - beta;

            return alpha > 0 && beta > 0 && gamma > 0;
        }

        public PointF[] GetPoints()
        {
            return new PointF[] { A, B, C };
        }
    }
}
