using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MDS
{

    public partial class MainForm : Form
    {

        string source, source1;
        bool isMouseDown = false;
        Point position, positionCl;
        double stepX = 0, stepY = 0;
        double sizeX;
        double sizeMin;
        double sizeY;
        double k;
        double sizeMinY;

        List<List<Line>> Lines;
        List<List<int>> res;
        // bool smacof, landmark;
        public MainForm()
        {
            InitializeComponent();
            source = "log.txt";
            source1 = "Distance.txt";
            k = 1;
            this.MouseWheel += new MouseEventHandler(Form1_MouseWheel);
            res = new List<List<int>>();
            Lines = new List<List<Line>>();
        }
        void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            // trackBar1.Value += e.Delta/10;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            StreamReader myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            List<Articles> tempList = new List<Articles>();
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = new StreamReader(openFileDialog1.OpenFile(), System.Text.Encoding.GetEncoding(1251))) != null)
                    {
                        using (myStream)
                        {
                            source = openFileDialog1.FileName;
                        }
                        myStream.Close();

                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }
      

        List<PointArt> pointsAtr;
        double[,] distancesMap;
        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < chartOut.Series.Count; i++)
            {
                chartOut.Series[i].Points.Clear();
            }
            for (int i = 0; i < chartLinear.Series.Count; i++)
            {
                chartLinear.Series[i].Points.Clear();
            }
            StreamReader sr = new StreamReader(source, System.Text.Encoding.GetEncoding(1251));
            List<string> stringsArticles = new List<string>();
            while (!sr.EndOfStream)
            {
                string temp = sr.ReadLine();
                if (temp.Split(':').Length < 13)
                    break;
                stringsArticles.Add(temp);
            }
            distancesMap = null;
            try
            {
                distancesMap = GetDistances(stringsArticles);
                //double[,] ex = { { 0.0, 11.4, 3.4 }, { 11.4, 0.0, 9.4 }, { 3.4, 9.4, 0.0 } };
            }
            catch
            {
                MessageBox.Show("Файл некоректний!");
            }
            double[,] output;
            if (distancesMap.Length > 1)
            {
                MessageBox.Show("Файл оброблено!");
                
                if (SMACOFradio.Checked)
                    output = MDS.distanceScaling(distancesMap, 2);
                else
                    output = MDS.distanceScalingLandmark(distancesMap, 3);
                art = GetArticles(stringsArticles);
                Draw(art, output);
                DrawLinear(art, output);

            }
            else
            {
                MessageBox.Show("Файл некоректний!");
            }
            sr.Close();
        }
        List<Articles> art;
        public void Draw(List<Articles> art, double[,] output)
        {

            pointsAtr = new List<PointArt>();
            for (int i = 0; i < output.GetLength(1); i++)
            {
                pointsAtr.Add(new PointArt(output[0, i], output[1, i], art[i]));
            }

            foreach (var item in pointsAtr)
            {
                System.Windows.Forms.DataVisualization.Charting.DataPoint tempDataPoint = new System.Windows.Forms.DataVisualization.Charting.DataPoint(item.X, item.Y);
                tempDataPoint.Color = item.Color;
                int r = item.ArticleName.Length < 11 ? item.ArticleName.Length - 1 : 10;
                if (item.ArticleName.Length > 11)
                    tempDataPoint.Label = item.ArticleName.Remove(r) + "..";
                else
                    tempDataPoint.Label = item.ArticleName;
                chartOut.Series[item.IdCat].Points.Add(tempDataPoint);
            }
            sizeY = (int)pointsAtr.OrderByDescending(x => x.Y).First().Y;
            sizeX = (int)pointsAtr.OrderByDescending(x => x.X).First().X;
            sizeMinY = (int)Math.Abs(pointsAtr.OrderBy(x => x.Y).First().Y);
            sizeMin = (int)Math.Abs(pointsAtr.OrderBy(x => x.X).First().X);
        }
        public void DrawLinear(List<Articles> art, double[,] output)
        { // 0 7 8 12
            int[] a = { 0, 7, 8, 12 };
            pointsAtr = new List<PointArt>();
            for (int i = 0; i < output.GetLength(1); i++)
            {
                pointsAtr.Add(new PointArt(output[0, i], output[1, i], art[i]));
            }
            List<PointArt> point = new List<PointArt>();

            for (int i = 0; i < a.Length; i++)
            {
                point.AddRange(pointsAtr.Where(x => x.IdCat.Equals(a[i])).ToList());
            }
            foreach (var item in point)
            {

                System.Windows.Forms.DataVisualization.Charting.DataPoint tempDataPoint = new System.Windows.Forms.DataVisualization.Charting.DataPoint(item.X, item.Y);
                tempDataPoint.Color = item.Color;
                int r = item.ArticleName.Length < 11 ? item.ArticleName.Length - 1 : 10;
                if (item.ArticleName.Length > 11)
                    tempDataPoint.Label = item.ArticleName.Remove(r) + "..";
                else
                    tempDataPoint.Label = item.ArticleName;
                chartLinear.Series[0].Points.Add(tempDataPoint);
            }
            //sizeY = (int)pointsAtr.OrderByDescending(x => x.Y).First().Y;
            //sizeX = (int)pointsAtr.OrderByDescending(x => x.X).First().X;
            //sizeMinY = (int)Math.Abs(pointsAtr.OrderBy(x => x.Y).First().Y);
            //sizeMin = (int)Math.Abs(pointsAtr.OrderBy(x => x.X).First().X);
        }
        public List<Articles> GetArticles(List<string> stringsArt)
        {
            List<Articles> tempList = new List<Articles>();
            double[,] input = GetValues(stringsArt);
            for (int i = 0; i < input.GetLength(0); i++)
            {
                List<double> t = new List<double>();
                for (int j = 0; j < input.GetLength(1); j++)
                {
                    t.Add(input[i, j]);
                }
                tempList.Add(new Articles(t, (stringsArt[i].Split(':'))[0]));
            }
            return tempList;
        }
        double[,] GetValues(List<string> stringsArt)
        {
            double[,] input = new double[stringsArt.Count, 13];
            for (int i = 0; i < stringsArt.Count; i++)
            {
                string[] t = stringsArt[i].Split(':');
                for (int j = 1; j < t.Length; j++)
                    input[i, j - 1] = Convert.ToDouble(t[j].Replace('.', ','));
            }
            return input;
        }
        double[,] GetValuesW(List<string> stringsArt)
        {
            double[,] input = new double[stringsArt.Count, 13];
            for (int i = 0; i < stringsArt.Count; i++)
            {
                string[] t = stringsArt[i].Split(':');
                for (int j = 0; j < t.Length; j++)
                    input[i, j] = Convert.ToDouble(t[j].Replace('.', ','));
            }
            return input;
        }

        public double[,] GetDistances(List<string> stringsArt)
        {
            double[,] input = GetValues(stringsArt);
            double[,] distances = new double[stringsArt.Count, stringsArt.Count];
            for (int i = 0; i < stringsArt.Count; i++)
            {
                for (int j = i; j < stringsArt.Count; j++)
                {
                    double temp = 0;
                    for (int k = 0; k < 13; k++)
                    {
                        temp += Math.Pow(input[i, k] - input[j, k], 2);
                    }
                    distances[i, j] = Math.Round(Math.Sqrt(temp), 3);
                    distances[j, i] = Math.Round(Math.Sqrt(temp), 3);
                }
            }
            return distances;
        }
        public PointArt FindPoint(Point p)
        {
            foreach (var item in pointsAtr)
            {
                if (Math.Abs(item.X - (double)p.X) < 1.5 & Math.Abs(item.Y - (double)p.Y) < 1.5)
                {
                    MessageBox.Show(String.Format("{0}", item.ArticleName));
                    isMouseDown = false;
                    break;
                }
            }
            return null;
        }
        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void chartOut_MouseDown(object sender, MouseEventArgs e)
        {

            isMouseDown = true;
            position = new Point((int)chartOut.ChartAreas[0].AxisX.PixelPositionToValue(e.X), (int)chartOut.ChartAreas[0].AxisY.PixelPositionToValue(e.Y));
            positionCl = new Point((int)chartOut.ChartAreas[0].AxisX.PixelPositionToValue(e.X), (int)chartOut.ChartAreas[0].AxisY.PixelPositionToValue(e.Y));
           // isMouseClick = true;
            FindPoint(positionCl);
        }

        private void chartOut_MouseMove(object sender, MouseEventArgs e)
        {

            if (isMouseDown)
            {
               // isMouseClick = false;
                try
                {
                    if (position.X < chartOut.ChartAreas[0].AxisX.PixelPositionToValue(e.X))//переміщення вліво
                        stepX -= 0.6;
                    if (position.X > chartOut.ChartAreas[0].AxisX.PixelPositionToValue(e.X))//вправо
                        stepX += 0.6;
                    if (position.Y < chartOut.ChartAreas[0].AxisY.PixelPositionToValue(e.Y))//вниз
                        stepY -= 0.5;
                    if (position.Y > chartOut.ChartAreas[0].AxisY.PixelPositionToValue(e.Y))//вверз
                        stepY += 0.5;
                    position = new Point((int)chartOut.ChartAreas[0].AxisX.PixelPositionToValue(e.X), (int)chartOut.ChartAreas[0].AxisY.PixelPositionToValue(e.Y));
                }
                catch { }
                chartOut.ChartAreas[0].AxisY.Maximum = sizeY * k + stepY;
                chartOut.ChartAreas[0].AxisX.Maximum = (sizeX * k + stepX);
                chartOut.ChartAreas[0].AxisY.Minimum = -sizeMinY * k + stepY;
                chartOut.ChartAreas[0].AxisX.Minimum = -((sizeMin * k - stepX));

            }
        }

        private void chartOut_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            k = -(double)trackBar1.Value / 100.0;
            stepX = 0;
            stepY = 0;
            chartOut.ChartAreas[0].AxisY.Maximum = sizeY * k + stepY;
            chartOut.ChartAreas[0].AxisX.Maximum = (sizeX * k + stepX);
            chartOut.ChartAreas[0].AxisY.Minimum = -sizeMinY * k + stepY;
            chartOut.ChartAreas[0].AxisX.Minimum = -((sizeMin * k - stepX));
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            bool[] cat = new bool[13];
            for (int i = groupBox2.Controls.Count; i > 0; i--)
            {
                cat[groupBox2.Controls.Count - i] = (groupBox2.Controls[i - 1] as CheckBox).Checked;
                chartOut.Series[groupBox2.Controls.Count - i].Enabled = (groupBox2.Controls[i - 1] as CheckBox).Checked;
            }
        }
        bool isAddCat;
        private void addCatButton_Click(object sender, EventArgs e)
        {
            if (!isAddCat)
            {
                addCatButton.Text = "Завершити";
                System.Windows.Forms.DataVisualization.Charting.Series tempSeries = new System.Windows.Forms.DataVisualization.Charting.Series();
                tempSeries.ChartArea = "ChartArea1";
                tempSeries.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                tempSeries.Legend = "Legend1";
                CounterOfCat++;
                tempSeries.Name = String.Format("Категорія {0}", CounterOfCat);
                tempSeries.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
                chartLinear.Series.Add(tempSeries);
                addArtButton.Enabled = false;
               
                isCalc = false;
                isAddCat = true;
            }
            else
            {
                addCatButton.Text = "Виділити категорію";
                MessageBox.Show("Клікніть в зоні категорії!!!!");
                isAddCat = false;
                addCatButton.Enabled = false;
            }
            // isAddCat = !isAddCat;

        }

        public void InitLines(MouseEventArgs e)
        {

            List<int> res = new List<int>();
            List<Line> temp = new List<Line>();            
            for (int i = 0; i < chartLinear.Series.Last().Points.Count - 1; i++)
            {
                temp.Add(new Line(new Point((int)chartLinear.Series.Last().Points[i].XValue, (int)chartLinear.Series.Last().Points[i].YValues[0]), new Point((int)chartLinear.Series.Last().Points[i + 1].XValue, (int)chartLinear.Series.Last().Points[i + 1].YValues[0]), CounterOfCat, chartLinear.ChartAreas[0].AxisX.PixelPositionToValue(e.X), chartLinear.ChartAreas[0].AxisY.PixelPositionToValue(e.Y)));     
            }
            Lines.Add(temp);
             
        }
        bool isCalc = true;
        int CounterOfCat = 0;
        private void chartLinear_MouseDown(object sender, MouseEventArgs e)
        {
            if (isAddCat)
            {
                chartLinear.Series.Last().Points.AddXY(chartLinear.ChartAreas[0].AxisX.PixelPositionToValue(e.X), chartLinear.ChartAreas[0].AxisY.PixelPositionToValue(e.Y));
            }
            else if (!isCalc)
            {
                InitLines(e);
                addCatButton.Enabled = true;
                isCalc = true;
                addArtButton.Enabled = true;
            }
            else if (isAddArt)
            {
                int cat = GetCatNum(chartLinear.ChartAreas[0].AxisX.PixelPositionToValue(e.X), chartLinear.ChartAreas[0].AxisY.PixelPositionToValue(e.Y));
                chartLinear.Series[1].Points.AddXY(chartLinear.ChartAreas[0].AxisX.PixelPositionToValue(e.X), chartLinear.ChartAreas[0].AxisY.PixelPositionToValue(e.Y));
                chartLinear.Series[1].Points.Last().Label = String.Format("Стаття №{0} Категорія: {1}", chartLinear.Series[1].Points.Count, cat);
                chartLinear.Series[1].Points.Last().Color = chartLinear.Series[cat + 1].Points.Last().Color;
            }
        }
        public int GetCatNum(double x, double y) 
        {
            foreach (var item in Lines)
            {
                bool flag = true;
                foreach (var val in item)
                {
                    //k[l] * chart1.Series[i].Points[j].XValue + b[l] - chart1.Series[i].Points[j].YValues[0]).CompareTo(0) != res[l];
                    if ((val.Angle * x + val.B - y).CompareTo(0) != val.Coord)
                    {
                        flag = false;
                        break;
                    }
                    
                }
                if (flag)
                {
                    return item.Last().Id_tr;
                }
            }
            
            return 0;
        }
        private void chartLinear_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void calcButton_Click(object sender, EventArgs e)
        {

        }
        public bool InteresectOfLine(Point A1, Point B1, Point B2, Point A2)
        {
            bool f = false;
            double x1 = A1.X;
            double x2 = B1.X;
            double y1 = A1.Y;
            double y2 = B1.Y;
            double x11 = A2.X;
            double x21 = B2.X;
            double y11 = A2.Y;
            double y21 = B2.Y;
            double k1, k2;
            if (y2 - y1 != 0)
            {
                k1 = (y2 - y1) / (x2 - x1);
            }
            else
            {
                k1 = 0;
            }
            if (y21 - y11 != 0)
            {
                k2 = (y21 - y11) / (x21 - x11);
            }
            else
            {
                k2 = 0;
            }
            double b1 = y1 - k1 * x1;
            double b2 = y11 - k2 * x11;
            double x = (b2 - b1) / (k1 - k2);
            double y = k1 * x + b1;

            if (y1 > y2)
            {
                double temp = y1;
                y1 = y2;
                y2 = temp;
            }
            if (k1 == k2)
            {
                f = false;
            }
            else if ((x > x1) && (x < x2) && (y > y1) && (y < y2))
            {
                f = true;
            }


            return f;
        }
        bool isAddArt;
        private void addArtButton_Click(object sender, EventArgs e)
        {
            isAddArt = true;
        }

        private void настройкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<PointArt3d> pointsAtr = new List<PointArt3d>();
            double[,] output;
            try
            {
                output = MDS.distanceScaling(distancesMap, 3);
                for (int i = 0; i < output.GetLength(1); i++)
                {
                    pointsAtr.Add(new PointArt3d(output[0, i], output[1, i], output[2, i], art[i]));
                }
                OpenGlVis.Start(pointsAtr);
            }
            catch 
            {
                MessageBox.Show("Спочатку проведіть аналіз данних натиснувши кнопку OK!!!!");
            }
            
        }

        private void chartOut_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            StreamReader myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            //List<Articles> tempList = new List<Articles>();
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = new StreamReader(openFileDialog1.OpenFile(), System.Text.Encoding.GetEncoding(1251))) != null)
                    {
                        using (myStream)
                        {
                            source1 = openFileDialog1.FileName;
                        }
                        myStream.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }
        List<PointCategory> Result;
        List<Line> ResultL;
        int mainCat,secondCat;
        string [] categoryName = {"Авто","Економіка і бізнес","Розваги","Сім'я","Мода","Медицина","Політика","Комп. ігри","Нерухомість","НіТ","Спорт","Туризм","Кухня"};
        private void button2_Click(object sender, EventArgs e)
        {
            List<List<string>> ListWeStr = new List<List<string>>();
            List<double[,]> ListWei = new List<double[,]>();
            StreamReader sr = new StreamReader(source1);
            List<string> tempList = new List<string>();
            while (!sr.EndOfStream)
            {
                string temp = sr.ReadLine();
                if (temp.Equals("#")) 
                {
                    ListWeStr.Add(tempList);
                    tempList = new List<string>();
                    continue;
                }
                tempList.Add(temp);              
            }
           
            foreach (var item in ListWeStr)
            {
                ListWei.Add(GetValuesW(item));
            }
            countWord = ListWei.Count;
            ResultL = new List<Line>();
            foreach (var item in ListWei)
            {
               
                double temp1 = 0;
                for (int i = 0; i < item.GetLength(0); i++)
                {
                    double temp = 0;
                    for (int j = 0; j < item.GetLength(1); j++)
                    {
                        temp += item[i, j];
                    }
                    if (temp>temp1)
                    {
                        mainCat = i;
                        temp1 = temp;
                    }
                   
                }
                double tempSC;
                int start=0;
                if (mainCat != 0)
                    tempSC = item[0, mainCat];
                else
                {
                    tempSC = item[1, mainCat];
                    start = 1;
                }
                for (int i = start+1; i < item.GetLength(0); i++)
                {
                    if (tempSC > item[i, mainCat] && item[i, mainCat]!=0.0)
                    {
                        secondCat = i;
                    }
                }
            }
             Result = new List<PointCategory>();
            for (int i = 0; i < ListWei.Count; i++)
            {
               Result.Add(new PointCategory(MDS.distanceScaling(ListWei[i],2)));
            }
            chart1.Series[0].Points.Clear();
            for (int i = 0; i < Result[iter - 1].Coordinates.GetLength(1); i++)
            {
                chart1.Series[0].Points.AddXY(Result[iter - 1].Coordinates[0, i], Result[iter - 1].Coordinates[1, i]);
                chart1.Series[0].Points.Last().Label = categoryName[i];
            }
            textBox1.Text = (11-iter).ToString();
        }
        int iter = 1;
        int countWord=1;
        private void button3_Click_1(object sender, EventArgs e)
        {
            if (iter < countWord )
            {
                 iter++;
                 textBox1.Text = (countWord - iter+1).ToString();
                 chart1.Series[0].Points.Clear();
                 for (int i = 0; i < Result[iter - 1].Coordinates.GetLength(1); i++)
                 {
                     chart1.Series[0].Points.AddXY(Result[iter - 1].Coordinates[0, i], Result[iter - 1].Coordinates[1, i]);
                     chart1.Series[0].Points.Last().Label = categoryName[i];
                 }
            }
           
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (iter >1)
            {
                iter--;
                textBox1.Text = (countWord - iter+1).ToString();
            }
            chart1.Series[0].Points.Clear();
            for (int i = 0; i < Result[iter-1].Coordinates.GetLength(1); i++)
            {
                chart1.Series[0].Points.AddXY(Result[iter - 1].Coordinates[0, i], Result[iter - 1].Coordinates[1, i]);
                chart1.Series[0].Points.Last().Label = categoryName[i];
            }
        }

    }
    public struct PointCategory
    {
        double[,] coordinates;
        string name;
        public PointCategory( double[,] arr)
        {
            coordinates = arr;
            name = "";
        }
        public double[,] Coordinates
        {
            get { return coordinates; }
        }
        public string Name
        {
            get { return name; }
        }
    }
    public struct Line
    {
        public Point p1;
        public Point p2;
        int lenght;
        double angle;
        int id_tr;
        double b;
        double coord;
        public Line(Point P1, Point P2, int id, double x, double y)
        {
            p1 = P1;
            p2 = P2;
            lenght = (int)Math.Sqrt(Math.Pow((p2.X - p1.X), 2) + Math.Pow((p2.Y - p1.Y), 2));
            if ((p2.X - p1.X) != 0)
            {
                double k = (double)(p2.Y - p1.Y) / (double)(p2.X - p1.X);
                angle = k;
            }
            else
            {
                angle = Math.PI / 2;
            }
            b = p1.Y - angle * p1.X;
            id_tr = id;
            coord = (angle * x + b - y).CompareTo(0);
        }
        public int Id_tr
        {
            get { return id_tr; }
        }
        public int Lenght
        {
            get { return lenght; }
        }
        public double Angle
        {
            get { return angle; }
        }
        public double B
        {
            get { return b; }
        }
        public double Coord
        {
            get { return coord; }
        } 
    }
}

