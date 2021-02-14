using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Pvz1
{
    public partial class Form1 : Form
    {
        List<Timer> Timerlist = new List<Timer>();

        public Form1()
        {
            InitializeComponent();
            Initialize();
        }

        // ---------------------------------------------- PUSIAUKIRTOS METODAS ----------------------------------------------

        float x1, x2, xtemp; // izoliacijos intervalo pradžia ir galas, vidurio taškas
        int N = 1000; // maksimalus iteracijų skaičius
        int iii; // iteracijos numeris

        Series Fx, X1X2, XMid; // naudojama atvaizduoti f-jai, šaknų rėžiams ir vidiniams taškams


        /// <summary>
        /// Sprendžiama lygtis F(x) = 0
        /// </summary>
        /// <param name="x">funkcijos argumentas</param>
        /// <returns></returns>
        private double F1(double x)
        {
            return (double)(1.2 * Math.Pow(x, 4) - 11.84 * Math.Pow(x, 3) + 36.35 * Math.Pow(x, 2) - 34.77 * x + 7.23);
        }

        private double F1Isvestine(double x)
        {
            return (double)(4.8 * Math.Pow(x, 3) - 35.52 * Math.Pow(x, 2) + 72.7 * Math.Pow(x, 1) - 34.77);
        }

        private double G(double x)
        {
            return (double)((Math.Pow(x - 2, 2) / 4) + 5 * Math.Sin(x)); // -5 <= x <= 15
        }

        private double GIsvestine(double x)
        {
            return (double)(x / 2 - 1 + 5 * Math.Cos(x)); // -5 <= x <= 15
        }

        private double Z(double h)
        {
            return Math.PI*Math.Pow(h, 3) - 6 * Math.PI * Math.Pow(h, 2) + 6;
        }

        private double ZIsvestine(double h)
        {
            return 3 * Math.Pow(h, 2) - 12 * Math.PI * h;
        }

        private void setFAxis()
        {
            iii = 0;
            x1 = -31.29F;
            x2 = 6.5F;
        }

        private void setGAxis()
        {
            iii = 0;
            x1 = -5F;
            x2 = 15F;
        }

        private void setZodinisAxis()
        {
            iii = 0;
            x1 = -1F; // ne 0 kad rastu taska
            x2 = 3.45F;
        }

        // ---------------------------------------------- PARAMETRINĖS FUNKCIJOS ----------------------------------------------

        List<PointF> data = new List<PointF>();
        Series S1;

        // G 2 Button
        private void button9_Click(object sender, EventArgs e)
        {
            ClearForm(); // išvalomi programos duomenys
            data.Clear();
            PreparareForm(-4, 15, (float)-100, (float)100);
            setGAxis();
            Method2(G, GIsvestine,800);
        }

        // Abstract Function Scanning
        private void scanFunction(Func<double, double> F_G_Word, int times)
        {
            int ii = 0;

            richTextBox1.AppendText(
                "Iteracija         x            F(x)        x1          x2          F(x1)         F(x2)       \n");
            Fx = chart1.Series.Add("F(x)");
            Fx.BorderWidth = 9;
            Fx.ChartType = SeriesChartType.Line;

            double X = x1;

            for (int i = 0; i < times; i++)
            {
                double Y;
                Y = F_G_Word(X);

                Fx.Points.AddXY(X, Y);
                X = X + (2 * Math.PI) / 50; // buvo pagrindiniam kode

            }

            Fx.BorderWidth = 3;
            XMid = chart1.Series.Add("XMid");
            XMid.MarkerStyle = MarkerStyle.Circle;
            XMid.MarkerSize = 8;

            double X2 = x2;
            while (x1 <= X2)
            {
                if (Math.Sign((double)F_G_Word(x1)) != Math.Sign((double)F_G_Word(x1 + 0.1)))
                {
                    x2 = (float)(x1 + (float)( 0.1)); // 0.1 yra musu zingsnis
                    Skenavimo((float)x1, (float)x2, F_G_Word);
                }
                x1 = (float)(x1 + (float)( 0.1));
            }
        }

        // F Scan Button
        private void button13_Click(object sender, EventArgs e)
        {
            ClearForm();
            data.Clear();
            setFAxis();
            PreparareForm(-10, 10, -10, 50);
            scanFunction(F1, 800);
        }

        private void Skenavimo(float a, float b, Func<double, double> F_G)
        {
            float xa, xb;
            float x1 = a, x2 = b;
            // 0.0000001
            
                // 0.00001
                while (Math.Abs(F_G(x1)) > 1e-5 && iii <= N)
                {
                    xa = x1;
                    xb = x2;
                    float dydis = (xb - xa) / 10;
                    float k = (float)F_G(x1);
                    while (xa <= x2)
                    {
                        int i = 1;
                        if ((xa + i * dydis) <= xb)
                        {
                            if (Math.Sign((double)F_G(xa)) != Math.Sign((double)F_G(xa + i * dydis)))
                            {
                                xb = (float)(xa + (float)(i * dydis));
                                break;
                            }
                        }
                        else
                        {
                            xb = (float)(xa + (float)(i * dydis));
                            break;
                        }

                        xa = (float)(xa + (float)(i * dydis));
                        i++;
                    }

                    x1 = xa;
                    x2 = xb;
                    richTextBox1.AppendText(String.Format(
                        " {0,6:d}   {1,12:f7}  {2,12:f7} {3,12:f7} {4,12:f7} {5,12:f7} {6,12:f7}\n",
                        iii, x1, F_G(x1), x1, x2, F_G(x1), F_G(x2)));

                    iii = iii + 1;
                }

                XMid.Points.AddXY(x1, 0);

            
        }


        // ---------------------------------------------- TIESINĖ ALGEBRA ----------------------------------------------

        private void iteraciju(float a, float b, Func<double, double> F_G_Word, Func<double, double> Isvestine)
        {
            float x0;
            float X1, X2 = b;
            float alfa = -100;
           
                X1 = (float)((float)a + 0.1);
                float isvestinesr = (float)Isvestine((double)X1);
                int i = 0;
                while ((((isvestinesr / alfa) < -2) || ((isvestinesr / alfa) > 0)) && i < 10000)
                {
                    alfa = (float)(alfa + 1);
                    i++;

                } // is formules 3 psl 31 skaidre
                float F = (float)F_G_Word(X1);
                x0 = (float)(X1 + F_G_Word(X1) / alfa); // Formule 1
                if (Math.Abs(F_G_Word(x0)) <= 1e-7 & iii <= N)
                {
                    XMid.Points.AddXY(x0, 0);
                    richTextBox1.AppendText(String.Format(" {0,6:d}   {1,12:f7}  {2,12:f7} {3,12:f7} {4,12:f7} {5,12:f7} {6,12:f7}\n",
                        iii, x0, F_G_Word(x0), X1, X2, F_G_Word(X1), F_G_Word(X2)));
                }
                while (Math.Abs(F_G_Word(x0)) > 1e-5 & iii <= N) // sukam cikla kol nera 0
                {
                    x0 = (float)(x0 + F_G_Word(x0) / alfa); // apsk. antra reiksme x0 gretinys
                    float Reiksme = (float)F_G_Word(x0);
                    richTextBox1.AppendText(String.Format(" {0,6:d}   {1,12:f7}  {2,12:f7} {3,12:f7} {4,12:f7} {5,12:f7} {6,12:f7}\n",
                        iii, x0, F_G_Word(x0), X1, X2, F_G_Word(X1), F_G_Word(X2)));
                    if (Math.Sign((double)F_G_Word(X1)) != Math.Sign((double)F_G_Word(x0)))
                    {
                        X2 = x0;
                    }
                    else
                    {
                        X1 = x0;
                    }
                    iii = iii + 1;
                }
                XMid.Points.AddXY(x0, 0);

            
        }

        // 2 Metodas
        private void Method2(Func<double, double> F_G_Word, Func<double, double> Isvestine, int times)
        {
            iii = 0; // iteraciju skaičius
            richTextBox1.AppendText("Iteracija         x            F(x)        x1          x2          F(x1)         F(x2)       \n");
            // Nubraižoma f-ja, kuriai ieskome saknies
            Fx = chart1.Series.Add("F(x)");
            Fx.ChartType = SeriesChartType.Line;
            double x = -5;
            for (int i = 0; i < times; i++)
            {
                Fx.Points.AddXY(x, F_G_Word(x)); x = x + (2 * Math.PI) / 50;
            }
            Fx.BorderWidth = 3;
            XMid = chart1.Series.Add("XMid");
            XMid.MarkerStyle = MarkerStyle.Circle;
            XMid.MarkerSize = 8;

            while (x1 <= 12)
            {
                if (Math.Sign((double)F_G_Word(x1)) != Math.Sign((double)F_G_Word(x1 +  0.2)))
                {
                    x2 = (float)(x1 + (float)( 0.2));
                    iteraciju(x1, x2, F_G_Word, Isvestine);
                }
                x1 = (float)(x1 + (float)( 0.2));
            }
        }

        // F 2 button
        private void button6_Click(object sender, EventArgs e)
        {
            ClearForm();
            data.Clear();

            PreparareForm(-4, 5, (float)-100, (float)100);
            setFAxis();
            Method2(F1, F1Isvestine,80);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            ClearForm();
            data.Clear();
            setGAxis();
            PreparareForm(x1, x2, -100, 100); 
            scanFunction(G,800);
        }

        // 4 Metodas
        private void Method4(Func<double, double> F_G_Word, Func<double, double> Isvestine)
        {
            iii = 0; // iteraciju skaičius
            richTextBox1.AppendText("Iteracija         x            F(x)        x1          x2          F(x1)         F(x2)       \n");
            // Nubraižoma f-ja, kuriai ieskome saknies
            Fx = chart1.Series.Add("F(x)");
            Fx.ChartType = SeriesChartType.Line;
            double x = -5;
            for (int i = 0; i < 800; i++)
            {
                Fx.Points.AddXY(x, F_G_Word(x)); x = x + (2 * Math.PI) / 50;
            }
            Fx.BorderWidth = 3;

            XMid = chart1.Series.Add("XMid");
            XMid.MarkerStyle = MarkerStyle.Circle;
            XMid.MarkerSize = 8;

            while (x1 <= 12)
            {
                int i = 1;
                if (Math.Sign((double)F_G_Word(x1)) != Math.Sign((double)F_G_Word(x1 + i * 0.2)))
                {
                    x2 = (float)(x1 + (float)(i * 0.2));
                    KvaziNiutono(x1, x2, F_G_Word);
                }
                x1 = (float)(x1 + (float)(i * 0.2));
                i++;
            }
        }

        // 5 Metodas
        private void Method5(Func<double, double> F_G_Word, Func<double, double> Isvestine)
        {
            iii = 0; // iteraciju skaičius
            richTextBox1.AppendText("Iteracija         x            F(x)        x1          x2          F(x1)         F(x2)       \n");
            // Nubraižoma f-ja, kuriai ieskome saknies
            Fx = chart1.Series.Add("F(x)");
            Fx.ChartType = SeriesChartType.Line;
            double x = -5;
            for (int i = 0; i < 800; i++)
            {
                Fx.Points.AddXY(x, F_G_Word(x)); x = x + (2 * Math.PI) / 50;
            }
            Fx.BorderWidth = 3;
            XMid = chart1.Series.Add("XMid");
            XMid.MarkerStyle = MarkerStyle.Circle;
            XMid.MarkerSize = 8;

            while (x1 <= 12)
            {
                if (Math.Sign((double)F_G_Word(x1)) != Math.Sign((double)F_G_Word(x1 + 0.2)))
                {
                    x2 = (float)(x1 + (float)( 0.2));
                    iteraciju(x1, x2, F_G_Word, Isvestine);
                }
                x1 = (float)(x1 + (float)( 0.2));
            }
        }

        // F 4 Button
        private void button7_Click_1(object sender, EventArgs e)
        {
            ClearForm(); // išvalomi programos duomenys
            data.Clear();
            PreparareForm(-4, 5, (float)-100, (float)100);
            setFAxis();
            Method4(F1,F1Isvestine);
        }

        // G 4 Button
        private void button10_Click(object sender, EventArgs e)
        {
            ClearForm(); // išvalomi programos duomenys
            data.Clear();
            PreparareForm(-4, 15, (float)-100, (float)100);
            setGAxis();
            Method4(G,GIsvestine);
        }

        // Zodinis Button
        private void button12_Click(object sender, EventArgs e)
        {
            ClearForm(); // išvalomi programos duomenys
            PreparareForm(-2, (float)1, (float)-60, (float)15);
            setZodinisAxis();
            iii = 0; // iteraciju skaičius
            richTextBox1.AppendText("Iteracija         x            F(x)        x1          x2          F(x1)         F(x2)       \n");
            // Nubraižoma f-ja, kuriai ieskome saknies
            Fx = chart1.Series.Add("F(x)");
            Fx.ChartType = SeriesChartType.Line;
            double x = -1;
            for (int i = 0; i < 800; i++)
            {
                Fx.Points.AddXY(x, Z(x)); x = x + 0.01;
            }
            Fx.BorderWidth = 3;

            XMid = chart1.Series.Add("XMid");
            XMid.MarkerStyle = MarkerStyle.Circle;
            XMid.MarkerSize = 8;

            while (x1 <= 5)
            {
                int i = 1;
                if (Math.Sign((double)Z(x1)) != Math.Sign((double)Z(x1 + i * 0.1)))
                {
                    x2 = (float)(x1 + (float)(i * 0.01));
                    KvaziNiutono(x1, x2, Z);
                }
                x1 = (float)(x1 + (float)(i * 0.1));
                i++;
            }
        }

        // F5 Button
        private void button8_Click(object sender, EventArgs e)
        {
            ClearForm(); // išvalomi programos duomenys
            data.Clear();
            PreparareForm(-4, 5, (float)-100, (float)100);
            setFAxis();
            Method5(F1, F1Isvestine);
        }

        // G5 button
        private void button11_Click(object sender, EventArgs e)
        {
            ClearForm(); // išvalomi programos duomenys
            data.Clear();
            PreparareForm(-4, 15, (float)-100, (float)100);
            setGAxis();
            Method5(G, GIsvestine);
        }

        private void KvaziNiutono(float a, float b, Func<double, double> F_G_Word)
        {
            double h = 0.0001; // zingsnio dydis
            float x0;
            float X1 = a, X2 = (float)(X1 + h); // gretinys
            {
                x0 = X2 - (float)(F_G_Word(X2) * (X2 - X1) / (F_G_Word(X2) - F_G_Word(X1))); // Formule 2
                while (Math.Abs(F_G_Word(x0)) > 1e-5 & iii <= N) // ne mazais zingsniai, o zingsniai pagal formule
                {
                    x0 = X2 - (float)(F_G_Word(X2) * (X2 - X1) / (F_G_Word(X2) - F_G_Word(X1))); 

                    richTextBox1.AppendText(String.Format(" {0,6:d}   {1,12:f7}  {2,12:f7} {3,12:f7} {4,12:f7} {5,12:f7} {6,12:f7}\n",
                        iii, x0, F_G_Word(x0), X1, X2, F_G_Word(X1), F_G_Word(b)));

                    if (Math.Sign((double)F_G_Word(X1)) != Math.Sign((double)F_G_Word(x0)))
                    {
                        X2 = x0;
                    }
                    else
                    {
                        X1 = x0;
                    }
                    iii = iii + 1;
                }
                XMid.Points.AddXY(x0, 0);

            }
           
        }


        // ---------------------------------------------- KITI METODAI ----------------------------------------------

        /// <summary>
        /// Uždaroma programa
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Išvalomas grafikas ir consolė
        /// </summary>
        private void button4_Click(object sender, EventArgs e)
        {
            ClearForm();
        }


        public void ClearForm()
        {
            richTextBox1.Clear(); // isvalomas richTextBox1
            // sustabdomi timeriai jei tokiu yra
            foreach (var timer in Timerlist)
            {
                timer.Stop();
            }

            // isvalomos visos nubreztos kreives
            chart1.Series.Clear();
        }
    }
}
