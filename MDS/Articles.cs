using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDS
{
    public struct Articles
    {
        List<double> cat;
        string name;
        public string Name
        {
            get { return name; }
        }
        public Articles(List<double> cat, string name)
        {
            this.cat = cat;
            this.name = name;
        }
        public int GetMainCategory()
        {
            return Cat.IndexOf(Cat.Max());
        }
        public double GetMainVal()
        {
            return Cat.Max();
        }
        public List<double> Cat
        {
            get { return cat; }
        }
    }
    public abstract class Shape
    {
        protected Color color;
        protected string article;
    }
    public class PointArt : Shape
    {

        protected double[,] colors = { { 255, 0, 0 }, { 255, 255, 0 }, { 0, 255, 0 },
                                   { 0, 255, 255 }, { 0, 0, 255 }, { 255, 0, 255 },
                                   { 255, 128, 0 }, { 128, 128, 128 }, { 230, 51, 119 },
                                   { 232, 108,120 },{128,64,255},{192,128,64},
                                   {255,0,192}};
        protected double value;
        protected int category;
        protected double x;
        protected double y;
        protected int idCat;
        public int IdCat
        {
            get { return idCat; }
        }
        public PointArt(Point p, Articles articles)
        {
            value = articles.GetMainVal();
            category = articles.GetMainCategory();
            x = p.X;
            y = p.Y;
            idCat = articles.GetMainCategory();
            ArticleName = articles.Name;
            color = Color.FromArgb(((int)(((byte)(colors[category, 0] * value)))), ((int)(((byte)(colors[category, 1] * value)))), ((int)(((byte)(colors[category, 2] * value)))));
        }
        public PointArt(double X, double Y, Articles articles)
        {
            value = 1;
            category = articles.GetMainCategory();
            x = X;
            y = Y;
            idCat = articles.GetMainCategory();
            ArticleName = articles.Name;
            color = Color.FromArgb(((int)(((byte)(colors[category, 0] * value)))), ((int)(((byte)(colors[category, 1] * value)))), ((int)(((byte)(colors[category, 2] * value)))));
        }
        public int Category
        {
            get { return category; }
        }
        public Point Point
        {
            get { return new Point((int)x, (int)y); }
        }
        public double X
        {
            get { return x; }
        }
        public double Y
        {
            get { return y; }
        }
        public Color Color
        {
            get { return color; }
        }
        public string ArticleName
        {
            get { return article; }
            set { article = value; }
        }
    }
    public sealed class PointArt3d : PointArt
    {
        double z;
        public PointArt3d(double X, double Y, double Z, Articles articles)
            : base(X, Y, articles)
        {
            value = 1;
            category = articles.GetMainCategory();
            x = X;
            y = Y;
            z = Z;
            idCat = articles.GetMainCategory();
            ArticleName = articles.Name;
            color = Color.FromArgb(((int)(((byte)(colors[category, 0] * value)))), ((int)(((byte)(colors[category, 1] * value)))), ((int)(((byte)(colors[category, 2] * value)))));
        }
        public double Z
        {
            get { return z; }
        }

    }
}
