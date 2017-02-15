using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace xox_final
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        const int szyrzka = 15, wyszka = 15;
        const int iloscPol = szyrzka * wyszka;
        static int szyrzkapb, wyszkapb;
        const int numOfThreads = 4;
        static int[,,] xypole = new int[numOfThreads, szyrzka, wyszka];
        static double[,] xyskore = new double[szyrzka, wyszka];
        static int[,] xyLewel = new int[szyrzka, wyszka];
        static int[, ,] xyIloscKolek = new int[szyrzka, wyszka, 8];
        static int[, ,] xyWolneMiyjscaNaKoncachKolko = new int[szyrzka, wyszka, 8];
        static int[, ,] xyOdleglosc = new int[szyrzka, wyszka, 8];
        static int[, ,] xyPusteMiyjscaOgolneKolko = new int[szyrzka, wyszka, 4];
        static int[, ,] xyMjynszeSkore = new int[szyrzka, wyszka, 2];
        int[] xpoleTah;
        int[] ypoleTah;
        double[] wartoscTah;
        static bool graPrzebiego = true;
        int depth = 5;
        static int tah = 0;
        static int iloscTahowAlfabety = -99;
        int wygraneX = 0;
        int wygraneO = 0;
        static int[] ostatniTah = new int[2];
        int gdoWygrolOstatniRoz = 0;
        Bitmap bmp;
        Graphics g;
        Thread threadGrej;
        Thread[] workers = new Thread[12];
        Stopwatch[] stopwatches = new Stopwatch[numOfThreads];
        bool rachuje;
        bool hasWon;
        int time;

        //0 - nic, 1 - krzizik, 2 - kolko

        public void namaluj(int thread, bool wymazac)
        {
            if (wymazac)
            {
                g.Clear(Color.White);
            }
            for (int a = 0; a <= szyrzkapb - 1; a++)
            {
                for (int b = wyszkapb / wyszka; b <= wyszkapb - 1; b += wyszkapb / wyszka)
                {
                    bmp.SetPixel(a, b, Color.Black);
                }
            }
            for (int a = szyrzkapb / szyrzka; a <= szyrzkapb - 1; a += szyrzkapb / szyrzka)
            {
                for (int b = 0; b <= wyszkapb - 1; b++)
                {
                    bmp.SetPixel(a, b, Color.Black);
                }
            }
            if (tah > 0)
            {
                namalujzielonomkostke(ostatniTah[0], ostatniTah[1]);
            }
            
            for (int a = 0; a <= szyrzka - 1; a++)
            {
                for (int b = 0; b <= wyszka - 1; b++)
                {
                    //namalujSkore(a, b);
                    //namalujLewel(a, b);
                    if (xypole[thread, a, b] == 2)
                    {
                        namalujkolko(a, b);
                    }
                    else if (xypole[thread, a, b] == 1)
                    {
                        namalujkrzizik(a, b);
                    }
                    else if (xypole[thread, a, b] == 0)
                    {
                        //namalujIloscKolek(a, b);
                        //namalujMjynszeSkore(a, b);
                        //namalujWolneMiyjsca(a, b);
                        //namalujPusteMiyjscaOgolneKolko(a, b);
                        //namalujOdleglosc(a, b;
                    }
                    if (a == 0 || b == 0)
                    {
                        namalujSourzadnice(a, b);
                    }
                }
            }
        }

        public void namalujkolko(int a, int b)
        {
            int x1 = a * szyrzkapb / szyrzka;
            int y1 = b * wyszkapb / wyszka;
            int x2 = (a + 1) * szyrzkapb / szyrzka;
            int y2 = (b + 1) * wyszkapb / wyszka;
            int x12 = (x1 + x2) / 2;
            int y12 = (y1 + y2) / 2;
            float prumer = (float)(szyrzkapb / szyrzka / 2.4);
            g.DrawEllipse(Pens.Black, x12 - prumer, y12 - prumer, prumer * 2, prumer * 2);
            /*
            for (int c = x1; c <= x2; c++)
            {
                for (int d = y1; d <= y2; d++)
                {
                    if (Math.Round(Math.Sqrt((c - x12) * (c - x12) + (d - y12) * (d - y12))) <= Math.Round(prumer)
                        && Math.Sqrt((c - x12) * (c - x12) + (d - y12) * (d - y12)) >= prumer - 0.6)
                    {
                        bmp.SetPixel(c, d, Color.Black);
                    }
                }
            }*/
        }

        public void namalujkrzizik(int a, int b)
        {
            int x1 = a * szyrzkapb / szyrzka;
            int y1 = b * wyszkapb / wyszka;
            int x2 = (a + 1) * szyrzkapb / szyrzka;
            int y2 = (b + 1) * wyszkapb / wyszka;
            int x12 = (x1 + x2) / 2;
            int y12 = (y1 + y2) / 2;
            double prumer = szyrzkapb / szyrzka / 1.9;
            for (int c = x1; c <= x2; c++)
            {
                for (int d = y1; d <= y2; d++)
                {
                    if (Math.Round(Math.Sqrt(Math.Abs(c - x12) * Math.Abs(c - x12) + Math.Abs(d - y12) * Math.Abs(d - y12))) <= Math.Round(prumer) && (
                        Math.Abs(c - x12) == Math.Abs(d - y12)
                        || Math.Abs(c - x12 + 1) == Math.Abs(d - y12)
                        || Math.Abs(c - x12) == Math.Abs(d - y12)
                        || Math.Abs(c - x12) == Math.Abs(d - y12)))
                    {
                        bmp.SetPixel(c, d, Color.Black);
                    }
                }
            }
        }

        public void namalujSkore(int a, int b)
        {
            PointF punkt = new PointF(a * szyrzkapb / szyrzka + 1, b * wyszkapb / wyszka + 3);
            Brush brush = Brushes.Gray;
            /*if (Math.Log10(xyskore[a, b]) > 0)
            {
                g.DrawString(Math.Round(Math.Log10(xyskore[a, b]), 3).ToString(), DefaultFont, brush, punkt);
            }*/
            if(xyskore[a,b] == double.PositiveInfinity)
            {
                g.DrawString("plus ∞", DefaultFont, brush, punkt);
            }
            else if (xyskore[a, b] == double.NegativeInfinity)
            {
                g.DrawString("minus ∞", DefaultFont, brush, punkt);
            }
            else
            {
                g.DrawString(xyskore[a, b].ToString(), DefaultFont, brush, punkt);
            }
        }

        public void namalujLewel(int a, int b)
        {
            PointF punkt = new PointF(a * szyrzkapb / szyrzka + 1, b * wyszkapb / wyszka + 3);
            Brush brush = Brushes.Gray;
            if (xyLewel[a, b] > -200)
            {
                g.DrawString(xyLewel[a, b].ToString(), DefaultFont, brush, punkt);
            }
        }

        public void namalujSourzadnice(int a, int b)
        {
            PointF punkt = new PointF(a * szyrzkapb / szyrzka + 1, b * wyszkapb / wyszka + 2);
            Brush brush = Brushes.LightGray;
            int czislo = 0;
            if(a > 0)
            {
                czislo = a;
            }
            else if(b > 0)
            {
                czislo = b;
            }
            czislo++;
            g.DrawString((czislo - 1).ToString(), DefaultFont, brush, punkt);
        }

        public void namalujIloscKolek(int a, int b)
        {
            PointF[] punkt = new PointF[8];
            punkt[0] = new PointF(a * szyrzkapb / szyrzka + 15, b * wyszkapb / wyszka + 3);
            punkt[1] = new PointF(a * szyrzkapb / szyrzka + 15, b * wyszkapb / wyszka + 28);
            punkt[2] = new PointF(a * szyrzkapb / szyrzka + 3, b * wyszkapb / wyszka + 15);
            punkt[3] = new PointF(a * szyrzkapb / szyrzka + 28, b * wyszkapb / wyszka + 15);
            punkt[4] = new PointF(a * szyrzkapb / szyrzka + 3, b * wyszkapb / wyszka + 3);
            punkt[5] = new PointF(a * szyrzkapb / szyrzka + 28, b * wyszkapb / wyszka + 28);
            punkt[6] = new PointF(a * szyrzkapb / szyrzka + 3, b * wyszkapb / wyszka + 28);
            punkt[7] = new PointF(a * szyrzkapb / szyrzka + 28, b * wyszkapb / wyszka + 3);
            Brush brush = Brushes.Gray;
            for (int i = 0; i < 8; i++)
            {
                if (xyIloscKolek[a, b, i] > 0)
                {
                    g.DrawString(xyIloscKolek[a, b, i].ToString(), DefaultFont, brush, punkt[i]);
                }
                else
                {
                    //g.DrawString(0.ToString(), DefaultFont, brush, punkt[i]);
                }
            }
        }

        public void namalujWolneMiyjsca(int a, int b)
        {
            PointF[] punkt = new PointF[8];
            punkt[0] = new PointF(a * szyrzkapb / szyrzka + 15, b * wyszkapb / wyszka + 3);
            punkt[1] = new PointF(a * szyrzkapb / szyrzka + 15, b * wyszkapb / wyszka + 28);
            punkt[2] = new PointF(a * szyrzkapb / szyrzka + 3, b * wyszkapb / wyszka + 15);
            punkt[3] = new PointF(a * szyrzkapb / szyrzka + 28, b * wyszkapb / wyszka + 15);
            punkt[4] = new PointF(a * szyrzkapb / szyrzka + 3, b * wyszkapb / wyszka + 3);
            punkt[5] = new PointF(a * szyrzkapb / szyrzka + 28, b * wyszkapb / wyszka + 28);
            punkt[6] = new PointF(a * szyrzkapb / szyrzka + 3, b * wyszkapb / wyszka + 28);
            punkt[7] = new PointF(a * szyrzkapb / szyrzka + 28, b * wyszkapb / wyszka + 3);
            Brush brush = Brushes.Gray;
            for (int i = 0; i < 8; i++)
            {
                if (xyWolneMiyjscaNaKoncachKolko[a, b, i] > 0)
                {
                    g.DrawString(xyWolneMiyjscaNaKoncachKolko[a, b, i].ToString(), DefaultFont, brush, punkt[i]);
                }
                else
                {
                    //g.DrawString(0.ToString(), DefaultFont, brush, punkt[i]);
                }
            }
        }

        public void namalujOdleglosc(int a, int b)
        {
            PointF[] punkt = new PointF[8];
            punkt[0] = new PointF(a * szyrzkapb / szyrzka + 15, b * wyszkapb / wyszka + 3);
            punkt[1] = new PointF(a * szyrzkapb / szyrzka + 15, b * wyszkapb / wyszka + 28);
            punkt[2] = new PointF(a * szyrzkapb / szyrzka + 3, b * wyszkapb / wyszka + 15);
            punkt[3] = new PointF(a * szyrzkapb / szyrzka + 28, b * wyszkapb / wyszka + 15);
            punkt[4] = new PointF(a * szyrzkapb / szyrzka + 3, b * wyszkapb / wyszka + 3);
            punkt[5] = new PointF(a * szyrzkapb / szyrzka + 28, b * wyszkapb / wyszka + 28);
            punkt[6] = new PointF(a * szyrzkapb / szyrzka + 3, b * wyszkapb / wyszka + 28);
            punkt[7] = new PointF(a * szyrzkapb / szyrzka + 28, b * wyszkapb / wyszka + 3);
            Brush brush = Brushes.Gray;
            for (int i = 0; i < 8; i++)
            {
                if (xyOdleglosc[a, b, i] > 0)
                {
                    g.DrawString(xyOdleglosc[a, b, i].ToString(), DefaultFont, brush, punkt[i]);
                }
                else
                {
                    //g.DrawString(0.ToString(), DefaultFont, brush, punkt[i]);
                }
            }
        }

        public void namalujPusteMiyjscaOgolneKolko(int a, int b)
        {
            PointF[] punkt = new PointF[8];
            punkt[0] = new PointF(a * szyrzkapb / szyrzka + 15, b * wyszkapb / wyszka + 3);
            punkt[1] = new PointF(a * szyrzkapb / szyrzka + 3, b * wyszkapb / wyszka + 15);
            punkt[2] = new PointF(a * szyrzkapb / szyrzka + 3, b * wyszkapb / wyszka + 3);
            punkt[3] = new PointF(a * szyrzkapb / szyrzka + 28, b * wyszkapb / wyszka + 3);
            Brush brush = Brushes.Gray;
            for (int i = 0; i < 4; i++)
            {
                if (xyPusteMiyjscaOgolneKolko[a, b, i] > 0)
                {
                    g.DrawString(xyPusteMiyjscaOgolneKolko[a, b, i].ToString(), DefaultFont, brush, punkt[i]);
                }
                else
                {
                    g.DrawString(0.ToString(), DefaultFont, brush, punkt[i]);
                }
            }
        }

        public void namalujMjynszeSkore(int a, int b)
        {
            PointF[] punkt = new PointF[8];
            punkt[0] = new PointF(a * szyrzkapb / szyrzka + 3, b * wyszkapb / wyszka + 3);
            punkt[1] = new PointF(a * szyrzkapb / szyrzka + 3, b * wyszkapb / wyszka + 28);
            Brush brush = Brushes.Gray;
            for (int i = 0; i < 2; i++)
            {
                if (xyMjynszeSkore[a, b, i] > 0)
                {
                    g.DrawString(xyMjynszeSkore[a, b, i].ToString(), DefaultFont, brush, punkt[i]);
                }
            }
        }

        public void namalujzielonomkostke(int a, int b)
        {
            int x1 = a * szyrzkapb / szyrzka;
            int y1 = b * wyszkapb / wyszka;
            int x2 = (a + 1) * szyrzkapb / szyrzka;
            int y2 = (b + 1) * wyszkapb / wyszka;
            for (int c = x1 + 1; c <= x2 - 1; c++)
            {
                for (int d = y1 + 1; d <= y2 - 1; d++)
                {
                    bmp.SetPixel(c, d, Color.FromArgb(75, 0, 200, 0));
                }
            }
        }

        public void namalujczyrwonomkostke(int a, int b)
        {
            int x1 = a * szyrzkapb / szyrzka;
            int y1 = b * wyszkapb / wyszka;
            int x2 = (a + 1) * szyrzkapb / szyrzka;
            int y2 = (b + 1) * wyszkapb / wyszka;
            for (int c = x1 + 1; c <= x2 - 1; c++)
            {
                for (int d = y1 + 1; d <= y2 - 1; d++)
                {
                    bmp.SetPixel(c, d, Color.FromArgb(45, 255, 0, 0));
                }
            }
        }

        public void namalujSilnomKostke(int a, int b, double nejwartosc)
        {
            int x1 = a * szyrzkapb / szyrzka;
            int y1 = b * wyszkapb / wyszka;
            int x2 = (a + 1) * szyrzkapb / szyrzka;
            int y2 = (b + 1) * wyszkapb / wyszka;
            int srodekX = (x1 + x2) / 2;
            int srodekY = (y1 + y2) / 2;
            for (int c = x1; c <= x2; c++)
            {
                for (int d = y1; d <= y2; d++)
                {
                    if (Math.Sqrt((c - srodekX) * (c - srodekX) + (d - srodekY) * (d - srodekY)) < 10)
                    {
                        if (nejwartosc >= 99)
                        {
                            this.Invoke((MethodInvoker)delegate { bmp.SetPixel(c, d, Color.Green); });
                        }
                        else if (nejwartosc <= -99)
                        {
                            this.Invoke((MethodInvoker)delegate { bmp.SetPixel(c, d, Color.Black); });
                        }
                        else if ((int)Math.Floor(nejwartosc) != 0)
                        {
                            nejwartosc = (int)Math.Floor(nejwartosc);
                            Color kolor;
                            if(nejwartosc == 10)
                            {
                                kolor = Color.Red;
                            }
                            else if (nejwartosc == 8)
                            {
                                kolor = Color.Orange;
                            }
                            else if (nejwartosc == 6)
                            {
                                kolor = Color.Yellow;
                            }
                            else if (nejwartosc == 4)
                            {
                                kolor = Color.FromArgb(255, 153, 0, 76);
                            }
                            else if (nejwartosc == 2)
                            {
                                kolor = Color.FromArgb(255, 255, 102, 178);
                            }
                            else if (nejwartosc == -1)
                            {
                                kolor = Color.DarkBlue;
                            }
                            else if (nejwartosc == -3)
                            {
                                kolor = Color.Blue;
                            }
                            else if (nejwartosc <= -5)
                            {
                                kolor = Color.LightBlue;
                            }
                            else
                            {
                                kolor = Color.Brown;
                            }
                            //bmp.SetPixel(c, d, Color.FromArgb(100, (int)((nejwartosc * 455 + 200) % 255), (int)((nejwartosc * 1000 + 150) % 255), (int)((nejwartosc * 100 + 135) % 255)));
                            this.Invoke((MethodInvoker)delegate { bmp.SetPixel(c, d, kolor); });
                        }
                        else
                        {
                            this.Invoke((MethodInvoker)delegate { bmp.SetPixel(c, d, Color.FromArgb(255, 200, 200, 200)); });
                        }
                    }
                }
            }
        }

        public int zmjynLewo1(int thread)
        {
            int lewo = 0;
            for (int a = 0; a < szyrzka; a++)
            {
                for (int b = 0; b < wyszka; b++)
                {
                    if (xypole[thread, a, b] != 0)
                    {
                        lewo = a;
                        return lewo;
                    }
                }
            }
            return lewo;
        }

        public int zmjynPrawo1(int thread)
        {
            int prawo = 0;
            for (int a = szyrzka - 1; a >= 0; a--)
            {
                for (int b = 0; b < wyszka; b++)
                {
                    if (xypole[thread, a, b] != 0)
                    {
                        prawo = a;
                        return prawo;
                    }
                }
            }
            return prawo;
        }

        public int zmjynGorno1(int thread)
        {
            int gorno = 0;
            for (int b = 0; b < wyszka; b++)
            {
                for (int a = 0; a < wyszka; a++)
                {
                    if (xypole[thread, a, b] != 0)
                    {
                        gorno = b;
                        return gorno;
                    }
                }
            }
            return gorno;
        }

        public int zmjynSpodnio1(int thread)
        {
            int spodnio = 0;
            for (int b = wyszka - 1; b >= 0; b--)
            {
                for (int a = 0; a < szyrzka; a++)
                {
                    if (xypole[thread, a, b] != 0)
                    {
                        spodnio = b;
                        return spodnio;
                    }
                }
            }
            return spodnio;
        }

        public bool dejKolkoJesliMosz4(int thread)
        {
            for (int a = 0; a < szyrzka; a++)
            {
                for (int b = 0; b < wyszka - 4; b++)
                {
                    int iloscKolekW5 = 0;
                    int iloscKrzizkowW5 = 0;
                    for (int c = 0; c < 5; c++)
                    {
                        if (xypole[thread, a, b + c] == 1)
                        {
                            iloscKrzizkowW5++;
                            iloscKolekW5 = int.MinValue;
                        }
                        else if (xypole[thread, a, b + c] == 2)
                        {
                            iloscKolekW5++;
                            iloscKrzizkowW5 = int.MinValue;
                        }
                    }
                    if (iloscKolekW5 == 4)
                    {
                        for (int c = 0; c < 5; c++)
                        {
                            if (xypole[thread, a, b + c] == 0)
                            {
                                xypole[thread, a, b + c] = 2;
                                ostatniTah[0] = a;
                                ostatniTah[1] = b + c;
                                goto end;
                            }
                        }
                    }
                }
            }
            for (int b = 0; b < wyszka; b++)
            {
                for (int a = 0; a < szyrzka - 4; a++)
                {
                    int iloscKolekW5 = 0;
                    int iloscKrzizkowW5 = 0;
                    for (int c = 0; c < 5; c++)
                    {
                        if (xypole[thread, a + c, b] == 1)
                        {
                            iloscKrzizkowW5++;
                            iloscKolekW5 = int.MinValue;
                        }
                        else if (xypole[thread, a + c, b] == 2)
                        {
                            iloscKolekW5++;
                            iloscKrzizkowW5 = int.MinValue;
                        }
                    }
                    if (iloscKolekW5 == 4)
                    {
                        for (int c = 0; c < 5; c++)
                        {
                            if (xypole[thread, a + c, b] == 0)
                            {
                                xypole[thread, a + c, b] = 2;
                                ostatniTah[0] = a + c;
                                ostatniTah[1] = b;
                                goto end;
                            }
                        }
                    }
                }
            }
            for (int a = -szyrzka + 5; a < szyrzka - 4; a++)
            {
                for (int b = 0; b < wyszka - 4; b++)
                {
                    if (a + b < szyrzka - 4 && a + b >= 0 && a + b < szyrzka)
                    {
                        int iloscKolekW5 = 0;
                        int iloscKrzizkowW5 = 0;
                        for (int c = 0; c < 5; c++)
                        {
                            if (xypole[thread, a + b + c, b + c] == 1)
                            {
                                iloscKrzizkowW5++;
                                iloscKolekW5 = int.MinValue;
                            }
                            else if (xypole[thread, a + b + c, b + c] == 2)
                            {
                                iloscKolekW5++;
                                iloscKrzizkowW5 = int.MinValue;
                            }
                        }
                        if (iloscKolekW5 == 4)
                        {
                            for (int c = 0; c < 5; c++)
                            {
                                if (xypole[thread, a + b + c, b + c] == 0)
                                {
                                    xypole[thread, a + b + c, b + c] = 2;
                                    ostatniTah[0] = a + b + c;
                                    ostatniTah[1] = b + c;
                                    goto end;
                                }
                            }
                        }
                    }
                }
            }
            for (int a = 4; a < 2 * szyrzka - 5; a++)
            {
                for (int b = 0; b < wyszka - 4; b++)
                {
                    if (a - b >= 4 && a - b >= 0 && a - b < szyrzka)
                    {
                        int iloscKolekW5 = 0;
                        int iloscKrzizkowW5 = 0;
                        for (int c = 0; c < 5; c++)
                        {
                            if (xypole[thread, a - (b + c), b + c] == 1)
                            {
                                iloscKrzizkowW5++;
                                iloscKolekW5 = int.MinValue;
                            }
                            else if (xypole[thread, a - (b + c), b + c] == 2)
                            {
                                iloscKolekW5++;
                                iloscKrzizkowW5 = int.MinValue;
                            }
                        }
                        if (iloscKolekW5 == 4)
                        {
                            for (int c = 0; c < 5; c++)
                            {
                                if (xypole[thread, a - (b + c), b + c] == 0)
                                {
                                    xypole[thread, a - (b + c), b + c] = 2;
                                    ostatniTah[0] = a - (b + c);
                                    ostatniTah[1] = b + c;
                                    goto end;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        end: return true;
        }

        public bool dejKolkoJesliKrzizikMo4(int thread)
        {
            for (int a = 0; a < szyrzka; a++)
            {
                for (int b = 0; b < wyszka - 4; b++)
                {
                    int iloscKolekW5 = 0;
                    int iloscKrzizkowW5 = 0;
                    for (int c = 0; c < 5; c++)
                    {
                        if (xypole[thread, a, b + c] == 1)
                        {
                            iloscKrzizkowW5++;
                            iloscKolekW5 = int.MinValue;
                        }
                        else if (xypole[thread, a, b + c] == 2)
                        {
                            iloscKolekW5++;
                            iloscKrzizkowW5 = int.MinValue;
                        }
                    }
                    if (iloscKrzizkowW5 == 4)
                    {
                        for (int c = 0; c < 5; c++)
                        {
                            if (xypole[thread, a, b + c] == 0)
                            {
                                xypole[thread, a, b + c] = 2;
                                ostatniTah[0] = a;
                                ostatniTah[1] = b + c;
                                goto end;
                            }
                        }
                    }
                }
            }
            for (int b = 0; b < wyszka; b++)
            {
                for (int a = 0; a < szyrzka - 4; a++)
                {
                    int iloscKolekW5 = 0;
                    int iloscKrzizkowW5 = 0;
                    for (int c = 0; c < 5; c++)
                    {
                        if (xypole[thread, a + c, b] == 1)
                        {
                            iloscKrzizkowW5++;
                            iloscKolekW5 = int.MinValue;
                        }
                        else if (xypole[thread, a + c, b] == 2)
                        {
                            iloscKolekW5++;
                            iloscKrzizkowW5 = int.MinValue;
                        }
                    }
                    if (iloscKrzizkowW5 == 4)
                    {
                        for (int c = 0; c < 5; c++)
                        {
                            if (xypole[thread, a + c, b] == 0)
                            {
                                xypole[thread, a + c, b] = 2;
                                ostatniTah[0] = a + c;
                                ostatniTah[1] = b;
                                goto end;
                            }
                        }
                    }
                }
            }
            for (int a = -szyrzka + 5; a < szyrzka - 4; a++)
            {
                for (int b = 0; b < wyszka - 4; b++)
                {
                    if (a + b < szyrzka - 4 && a + b >= 0 && a + b < szyrzka)
                    {
                        int iloscKolekW5 = 0;
                        int iloscKrzizkowW5 = 0;
                        for (int c = 0; c < 5; c++)
                        {
                            if (xypole[thread, a + b + c, b + c] == 1)
                            {
                                iloscKrzizkowW5++;
                                iloscKolekW5 = int.MinValue;
                            }
                            else if (xypole[thread, a + b + c, b + c] == 2)
                            {
                                iloscKolekW5++;
                                iloscKrzizkowW5 = int.MinValue;
                            }
                        }
                        if (iloscKrzizkowW5 == 4)
                        {
                            for (int c = 0; c < 5; c++)
                            {
                                if (xypole[thread, a + b + c, b + c] == 0)
                                {
                                    xypole[thread, a + b + c, b + c] = 2;
                                    ostatniTah[0] = a + b + c;
                                    ostatniTah[1] = b + c;
                                    goto end;
                                }
                            }
                        }
                    }
                }
            }
            for (int a = 4; a < 2 * szyrzka - 5; a++)
            {
                for (int b = 0; b < wyszka - 4; b++)
                {
                    if (a - b >= 4 && a - b >= 0 && a - b < szyrzka)
                    {
                        int iloscKolekW5 = 0;
                        int iloscKrzizkowW5 = 0;
                        for (int c = 0; c < 5; c++)
                        {
                            if (xypole[thread, a - (b + c), b + c] == 1)
                            {
                                iloscKrzizkowW5++;
                                iloscKolekW5 = int.MinValue;
                            }
                            else if (xypole[thread, a - (b + c), b + c] == 2)
                            {
                                iloscKolekW5++;
                                iloscKrzizkowW5 = int.MinValue;
                            }
                        }
                        if (iloscKrzizkowW5 == 4)
                        {
                            for (int c = 0; c < 5; c++)
                            {
                                if (xypole[thread, a - (b + c), b + c] == 0)
                                {
                                    xypole[thread, a - (b + c), b + c] = 2;
                                    ostatniTah[0] = a - (b + c);
                                    ostatniTah[1] = b + c;
                                    goto end;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        end: return true;
        }

        public int lewelSkore(int lewel, double skore)
        {
            if (skore >= Math.Pow(10, 90))
            {
                lewel = 10;
            }
            else if (skore <= Math.Pow(10, 80) * -1 && lewel < 9)
            {
                lewel = 9;
            }
            else if (skore >= Math.Pow(10, 70) && lewel < 8)
            {
                lewel = 8;
            }
            else if (skore <= Math.Pow(10, 60) * -1 && lewel < 7)
            {
                lewel = 7;
            }
            else if (skore >= Math.Pow(10, 50) && lewel < 6)
            {
                lewel = 6;
            }
            else if (skore <= Math.Pow(10, 40) * -1 && lewel < 5)
            {
                lewel = 5;
            }
            else if ((skore >= Math.Pow(10, 30) || skore <= Math.Pow(10, 30) * -1) && lewel < 4)
            {
                lewel = 4;
            }
            else if (skore <= Math.Pow(10, 20) * -1 && lewel < 3)
            {
                lewel = 3;
            }
            else if (lewel < 2)
            {
                if (skore > 0)
                {
                    lewel = 2;
                }
                else lewel = 1;
            }
            return lewel;
        }

        public double przirzadzSkoreAlfabeta(int gdoJedzie, int x, int y, int thread)
        {
            //   !!!
            //NIEPOUZYWAC
            //   !!!

            double skore = 0;
            int[] pusteMiyjscaOgolneKolko = new int[4] { 1, 1, 1, 1 };
            int[] pusteMiyjscaOgolneKrzizik = new int[4] {1,1,1,1};
            int[] pusteMiyjscaKolko = new int[8] {0,0,0,0,0,0,0,0};
            int[] pusteMiyjscaKrzizik = new int[8] {0,0,0,0,0,0,0,0};
            int[] pusteMiyjscaMjyndzy = new int[8] {0,0,0,0,0,0,0,0};
            int[] pusteMiyjscaMjyndzyActual = new int[8] {0,0,0,0,0,0,0,0};
            bool[] zarachowaneKolka = new bool[8] {false,false,false,false,false,false,false,false};
            bool[] zarachowaneKrzizki = new bool[8] {false,false,false,false,false,false,false,false};
            int[] iloscKolek = new int[8]  {0,0,0,0,0,0,0,0};
            int[] iloscKrzizkow = new int[8] {0,0,0,0,0,0,0,0};
            int[] iloscKolekOgolnie = new int[4] {0,0,0,0};
            int[] iloscKrzizkowOgolnie = new int[4] {0,0,0,0};
            int[] wolneMiyjscaNaKoncachKolko = new int[8] {0,0,0,0,0,0,0,0};
            int[] wolneMiyjscaNaKoncachKrzizik = new int[8] {0,0,0,0,0,0,0,0};
            int[] odlegloscKolko = new int[8] {0,0,0,0,0,0,0,0};
            int[] odlegloscKrzizik = new int[8] {0,0,0,0,0,0,0,0};
            int[] coByloOstatni = new int[8] {0,0,0,0,0,0,0,0};
            if (gdoJedzie == 1)
            {
                for (int j = 0; j < 8; ++j)
                {
                    coByloOstatni[j] = 1;
                }
            }
            else
            {
                for (int j = 0; j < 8; ++j)
                {
                    coByloOstatni[j] = 2;
                }
            }
            int smer = -1;
            int smerOgolny = -1;
            for (int odleglosc = 1; odleglosc <= 4; odleglosc++)
            {
                for (int c = -odleglosc; c <= odleglosc; c++)
                {
                    for (int d = -odleglosc; d <= odleglosc; d++)
                    {
                        if ((Math.Abs(c) == odleglosc || Math.Abs(d) == odleglosc) && !(c == 0 && d == 0) && x + c >= 0 && x + c < szyrzka && y + d >= 0 && y + d < wyszka)
                        {
                            if (c == 0 && d < 0)
                            {
                                smer = 0;
                                smerOgolny = 0;
                            }
                            else if (c == 0 && d > 0)
                            {
                                smer = 1;
                                smerOgolny = 0;
                            }
                            else if (d == 0 && c < 0)
                            {
                                smer = 2;
                                smerOgolny = 1;
                            }
                            else if (d == 0 && c > 0)
                            {
                                smer = 3;
                                smerOgolny = 1;
                            }
                            else if (c == d && c < 0)
                            {
                                smer = 4;
                                smerOgolny = 2;
                            }
                            else if (c == d && c > 0)
                            {
                                smer = 5;
                                smerOgolny = 2;
                            }
                            else if (c == -d && c < 0)
                            {
                                smer = 6;
                                smerOgolny = 3;
                            }
                            else if (c == -d && c > 0)
                            {
                                smer = 7;
                                smerOgolny = 3;
                            }
                            if (c == 0 || d == 0 || c == d || c == -d)
                            {
                                if (odleglosc == 1)
                                {
                                    if (xypole[thread, x + c, y + d] == 0)
                                    {
                                        wolneMiyjscaNaKoncachKolko[smer % 2 == 0 ? smer + 1 : smer - 1]++;
                                        wolneMiyjscaNaKoncachKrzizik[smer % 2 == 0 ? smer + 1 : smer - 1]++;
                                    }
                                    else if (xypole[thread, x + c, y + d] == 1)
                                    {
                                        wolneMiyjscaNaKoncachKrzizik[smer % 2 == 0 ? smer + 1 : smer - 1]++;
                                    }
                                    else if (xypole[thread, x + c, y + d] == 2)
                                    {
                                        wolneMiyjscaNaKoncachKolko[smer % 2 == 0 ? smer + 1 : smer - 1]++;
                                    }
                                }
                                if (xypole[thread, x + c, y + d] == 0)
                                {
                                    pusteMiyjscaKolko[smer]++;
                                    pusteMiyjscaKrzizik[smer]++;
                                    pusteMiyjscaMjyndzy[smer]++;
                                    coByloOstatni[smer] = 0;
                                }
                                else if (xypole[thread, x + c, y + d] == 1)
                                {
                                    pusteMiyjscaKrzizik[smer]++;
                                    pusteMiyjscaMjyndzyActual[smer] = pusteMiyjscaMjyndzy[smer];
                                    if (pusteMiyjscaMjyndzyActual[smer] < 2 && !zarachowaneKrzizki[smer] && odleglosc <= 4)
                                    {
                                        iloscKrzizkow[smer] += 1;
                                        odlegloscKrzizik[smer] += odleglosc;
                                        iloscKrzizkowOgolnie[smerOgolny]++;
                                    }
                                    if (coByloOstatni[smer] == 0 && !zarachowaneKolka[smer])
                                    {
                                        wolneMiyjscaNaKoncachKolko[smer]++;
                                    }
                                    zarachowaneKolka[smer] = true;
                                    coByloOstatni[smer] = 1;
                                }
                                else if (xypole[thread, x + c, y + d] == 2)
                                {
                                    pusteMiyjscaKolko[smer]++;
                                    pusteMiyjscaMjyndzyActual[smer] = pusteMiyjscaMjyndzy[smer];
                                    if (pusteMiyjscaMjyndzyActual[smer] < 2 && !zarachowaneKolka[smer] && odleglosc <= 4)
                                    {
                                        iloscKolek[smer] += 1;
                                        odlegloscKolko[smer] += odleglosc;
                                        iloscKolekOgolnie[smerOgolny]++;
                                    }
                                    if (coByloOstatni[smer] == 0 && !zarachowaneKrzizki[smer])
                                    {
                                        wolneMiyjscaNaKoncachKrzizik[smer]++;
                                    }
                                    zarachowaneKrzizki[smer] = true;
                                    coByloOstatni[smer] = 2;
                                }
                                if (pusteMiyjscaKolko[smer] >= 0 && (xypole[thread, x + c, y + d] == 1 || odleglosc == 4 || (c > 0 ? x + c + 1 >= szyrzka : c < 0 ? x + c - 1 < 0 : false) || (d > 0 ? y + d + 1 >= wyszka : d < 0 ? y + d - 1 < 0 : false)))
                                {
                                    pusteMiyjscaOgolneKolko[smerOgolny] += pusteMiyjscaKolko[smer];
                                    pusteMiyjscaKolko[smer] = int.MinValue;
                                }
                                if (pusteMiyjscaKrzizik[smer] >= 0 && (xypole[thread, x + c, y + d] == 2 || odleglosc == 4 || (c > 0 ? x + c + 1 >= szyrzka : c < 0 ? x + c - 1 < 0 : false) || (d > 0 ? y + d + 1 >= wyszka : d < 0 ? y + d - 1 < 0 : false)))
                                {
                                    pusteMiyjscaOgolneKrzizik[smerOgolny] += pusteMiyjscaKrzizik[smer];
                                    pusteMiyjscaKrzizik[smer] = int.MinValue;
                                }
                                if ((pusteMiyjscaMjyndzyActual[smer] >= 2 || odleglosc == 4 || (c > 0 ? x + c + 1 >= szyrzka : c < 0 ? x + c - 1 < 0 : false) || (d > 0 ? y + d + 1 >= wyszka : d < 0 ? y + d - 1 < 0 : false)) && !zarachowaneKolka[smer])
                                {
                                    if ((coByloOstatni[smer] == 0 || (coByloOstatni[smer] == 2 && odleglosc == 4)) && !(xypole[thread, x + c, y + d] == 2 && odleglosc < 4 && ((c > 0 ? x + c + 1 >= szyrzka : c < 0 ? x + c - 1 < 0 : false) || (d > 0 ? y + d + 1 >= wyszka : d < 0 ? y + d - 1 < 0 : false))))
                                    {
                                        wolneMiyjscaNaKoncachKolko[smer]++;
                                    }
                                    zarachowaneKolka[smer] = true;
                                }
                                if ((pusteMiyjscaMjyndzyActual[smer] >= 2 || odleglosc == 4 || (c > 0 ? x + c + 1 >= szyrzka : c < 0 ? x + c - 1 < 0 : false) || (d > 0 ? y + d + 1 >= wyszka : d < 0 ? y + d - 1 < 0 : false)) && !zarachowaneKrzizki[smer])
                                {
                                    if ((coByloOstatni[smer] == 0 || (coByloOstatni[smer] == 1 && odleglosc == 4)) && !(xypole[thread, x + c, y + d] == 1 && odleglosc < 4 && ((c > 0 ? x + c + 1 >= szyrzka : c < 0 ? x + c - 1 < 0 : false) || (d > 0 ? y + d + 1 >= wyszka : d < 0 ? y + d - 1 < 0 : false))))
                                    {
                                        wolneMiyjscaNaKoncachKrzizik[smer]++;
                                    }
                                    zarachowaneKrzizki[smer] = true;
                                }
                            }
                        }
                    }
                }
            }

            if (gdoJedzie == 2)
            {
                for (int a = 0; a < 4; a++)
                {
                    if (pusteMiyjscaOgolneKolko[a] < 5)
                    {
                        iloscKolekOgolnie[a] = int.MinValue;
                        iloscKolek[2 * a] = int.MinValue;
                        iloscKolek[2 * a + 1] = int.MinValue;
                    }
                    if (pusteMiyjscaOgolneKrzizik[a] < 5)
                    {
                        iloscKrzizkowOgolnie[a] = int.MinValue;
                        iloscKrzizkow[2 * a] = int.MinValue;
                        iloscKrzizkow[2 * a + 1] = int.MinValue;
                    }
                }
                for (int a = 0; a < 4; a++)
                {
                    if ((pusteMiyjscaMjyndzyActual[a * 2] + pusteMiyjscaMjyndzyActual[a * 2 + 1] == 0) || ((iloscKolek[a * 2] == 4 || iloscKolek[a * 2] == 3) && pusteMiyjscaMjyndzyActual[a * 2] == 0) || ((iloscKolek[a * 2 + 1] == 4 || iloscKolek[a * 2 + 1] == 3) && pusteMiyjscaMjyndzyActual[a * 2 + 1] == 0) || ((iloscKrzizkow[a * 2] == 4 || iloscKrzizkow[a * 2] == 3) && pusteMiyjscaMjyndzyActual[a * 2] == 0) || ((iloscKrzizkow[a * 2 + 1] == 4 || iloscKrzizkow[a * 2 + 1] == 3) && pusteMiyjscaMjyndzyActual[a * 2 + 1] == 0))
                    {
                        if (iloscKolekOgolnie[a] >= 4)
                        {
                            skore += Math.Pow(10, 95); // 1.
                        }
                        else if (iloscKolekOgolnie[a] == 3 && wolneMiyjscaNaKoncachKolko[a * 2] != 1 && wolneMiyjscaNaKoncachKolko[a * 2 + 1] != 1 && odlegloscKolko[a * 2] + odlegloscKolko[a * 2 + 1] <= 6)
                        {
                            skore += Math.Pow(10, 75); // 3.
                        }
                        if (iloscKrzizkowOgolnie[a] >= 4)
                        {
                            skore += Math.Pow(10, 85); // 2.
                        }
                        else if (iloscKrzizkowOgolnie[a] == 3 && wolneMiyjscaNaKoncachKrzizik[a * 2] != 1 && wolneMiyjscaNaKoncachKrzizik[a * 2 + 1] != 1 && odlegloscKrzizik[a * 2] + odlegloscKrzizik[a * 2 + 1] <= 6)
                        {
                            skore += Math.Pow(10, 65); // 4.
                        }
                    }
                    if (iloscKolekOgolnie[a] == 3 || iloscKolek[2 * a] == 3 || iloscKolek[2 * a + 1] == 3)
                    {
                        skore += Math.Pow(10, 35); // 7.
                    }
                    if (iloscKrzizkowOgolnie[a] == 3 || iloscKrzizkow[2 * a] == 3 || iloscKrzizkow[2 * a + 1] == 3)
                    {
                        skore += Math.Pow(10, 25); // 8.
                    }
                }
                double skoreDwojekKolka = 0;
                int sumaDwojekKolka = 0;
                for (int a = 0; a < 4; a++)
                {
                    if (iloscKolekOgolnie[a] >= 2 && !(iloscKolek[2 * a] > 0 && wolneMiyjscaNaKoncachKolko[2 * a] != 2) && !(iloscKolek[2 * a + 1] > 0 && wolneMiyjscaNaKoncachKolko[2 * a + 1] != 2))
                    {
                        skoreDwojekKolka += iloscKolekOgolnie[a] * 10 - odlegloscKolko[2 * a] - odlegloscKolko[2 * a + 1];
                        sumaDwojekKolka++;
                    }
                }
                if (sumaDwojekKolka >= 2)
                {
                    skore += Math.Pow(10, 55) * skoreDwojekKolka; // 5.
                }
                double skoreDwojekKrzizki = 0;
                int sumaDwojekKrzizki = 0;
                for (int a = 0; a < 4; a++)
                {
                    if (iloscKrzizkowOgolnie[a] >= 2 && !(iloscKrzizkow[2 * a] > 0 && wolneMiyjscaNaKoncachKrzizik[2 * a] != 2) && !(iloscKrzizkow[2 * a + 1] > 0 && wolneMiyjscaNaKoncachKrzizik[2 * a + 1] != 2))
                    {
                        skoreDwojekKrzizki += iloscKrzizkowOgolnie[a] * 10 - odlegloscKrzizik[2 * a] - odlegloscKrzizik[2 * a + 1];
                        sumaDwojekKrzizki++;
                    }
                }
                if (sumaDwojekKrzizki >= 2)
                {
                    skore += Math.Pow(10, 45) * skoreDwojekKrzizki; // 6.
                }
                int sumaKolek = 0;
                int sumaKrzizkow = 0;
                for (int a = 0; a < 8; a++)
                {
                    if (iloscKolek[a] > 0)
                    {
                        sumaKolek += ((int)Math.Pow(iloscKolek[a], 2) * 10 - odlegloscKolko[a]) * (iloscKolek[a] == 3 && wolneMiyjscaNaKoncachKolko[a] >= 1 ? 1 : (int)Math.Pow(wolneMiyjscaNaKoncachKolko[a], 2));
                    }
                    if(iloscKrzizkow[a] > 0)
                    {
                        sumaKrzizkow += ((int)Math.Pow(iloscKrzizkow[a], 2) * 10 - odlegloscKrzizik[a]) * (iloscKrzizkow[a] == 3 && wolneMiyjscaNaKoncachKrzizik[a] >= 1 ? 1 : (int)Math.Pow(wolneMiyjscaNaKoncachKrzizik[a], 2));
                    }
                }
                if(sumaKolek > 0)
                {
                    skore += Math.Pow(10, 15) * sumaKolek; // 9.
                }
                if (sumaKrzizkow > 0)
                {
                    skore += Math.Pow(10, 15) * sumaKrzizkow / 2; // 10.
                }
                /*xyMjynszeSkore[x, y, 0] = sumaKolek;
                xyMjynszeSkore[x, y, 1] = sumaKrzizkow;*/
            }
            else if (gdoJedzie == 1)
            {
                for (int a = 0; a < 4; a++)
                {
                    if (pusteMiyjscaOgolneKolko[a] < 5)
                    {
                        iloscKolekOgolnie[a] = int.MinValue;
                        iloscKolek[2 * a] = int.MinValue;
                        iloscKolek[2 * a + 1] = int.MinValue;
                    }
                    if (pusteMiyjscaOgolneKrzizik[a] < 5)
                    {
                        iloscKrzizkowOgolnie[a] = int.MinValue;
                        iloscKrzizkow[2 * a] = int.MinValue;
                        iloscKrzizkow[2 * a + 1] = int.MinValue;
                    }
                }
                for (int a = 0; a < 4; a++)
                {
                    if ((pusteMiyjscaMjyndzyActual[a * 2] + pusteMiyjscaMjyndzyActual[a * 2 + 1] == 0) || ((iloscKolek[a * 2] == 4 || iloscKolek[a * 2] == 3) && pusteMiyjscaMjyndzyActual[a * 2] == 0) || ((iloscKolek[a * 2 + 1] == 4 || iloscKolek[a * 2 + 1] == 3) && pusteMiyjscaMjyndzyActual[a * 2 + 1] == 0) || ((iloscKrzizkow[a * 2] == 4 || iloscKrzizkow[a * 2] == 3) && pusteMiyjscaMjyndzyActual[a * 2] == 0) || ((iloscKrzizkow[a * 2 + 1] == 4 || iloscKrzizkow[a * 2 + 1] == 3) && pusteMiyjscaMjyndzyActual[a * 2 + 1] == 0))
                    {
                        if (iloscKolekOgolnie[a] >= 4)
                        {
                            skore += Math.Pow(10, 85); // 2.
                        }
                        else if (iloscKolekOgolnie[a] == 3 && wolneMiyjscaNaKoncachKolko[a * 2] != 1 && wolneMiyjscaNaKoncachKolko[a * 2 + 1] != 1 && odlegloscKolko[a * 2] + odlegloscKolko[a * 2 + 1] <= 6)
                        {
                            skore += Math.Pow(10, 65); // 4.
                        }
                        if (iloscKrzizkowOgolnie[a] >= 4)
                        {
                            skore += Math.Pow(10, 95); // 1.
                        }
                        else if (iloscKrzizkowOgolnie[a] == 3 && wolneMiyjscaNaKoncachKrzizik[a * 2] != 1 && wolneMiyjscaNaKoncachKrzizik[a * 2 + 1] != 1 && odlegloscKrzizik[a * 2] + odlegloscKrzizik[a * 2 + 1] <= 6)
                        {
                            skore += Math.Pow(10, 75); // 3.
                        }
                    }
                    if (iloscKolekOgolnie[a] == 3 || iloscKolek[2 * a] == 3 || iloscKolek[2 * a + 1] == 3)
                    {
                        skore += Math.Pow(10, 25); // 8.
                    }
                    if (iloscKrzizkowOgolnie[a] == 3 || iloscKrzizkow[2 * a] == 3 || iloscKrzizkow[2 * a + 1] == 3)
                    {
                        skore += Math.Pow(10, 35); // 7.
                    }
                }
                double skoreDwojekKolka = 0;
                int sumaDwojekKolka = 0;
                for (int a = 0; a < 4; a++)
                {
                    if (iloscKolekOgolnie[a] >= 2 && !(iloscKolek[2 * a] > 0 && wolneMiyjscaNaKoncachKolko[2 * a] != 2) && !(iloscKolek[2 * a + 1] > 0 && wolneMiyjscaNaKoncachKolko[2 * a + 1] != 2))
                    {
                        skoreDwojekKolka += iloscKolekOgolnie[a] * 10 - odlegloscKolko[2 * a] - odlegloscKolko[2 * a + 1];
                        sumaDwojekKolka++;
                    }
                }
                if (sumaDwojekKolka >= 2)
                {
                    skore += Math.Pow(10, 45) * skoreDwojekKolka; // 6.
                }
                double skoreDwojekKrzizki = 0;
                int sumaDwojekKrzizki = 0;
                for (int a = 0; a < 4; a++)
                {
                    if (iloscKrzizkowOgolnie[a] >= 2 && !(iloscKrzizkow[2 * a] > 0 && wolneMiyjscaNaKoncachKrzizik[2 * a] != 2) && !(iloscKrzizkow[2 * a + 1] > 0 && wolneMiyjscaNaKoncachKrzizik[2 * a + 1] != 2))
                    {
                        skoreDwojekKrzizki += iloscKrzizkowOgolnie[a] * 10 - odlegloscKrzizik[2 * a] - odlegloscKrzizik[2 * a + 1];
                        sumaDwojekKrzizki++;
                    }
                }
                if (sumaDwojekKrzizki >= 2)
                {
                    skore += Math.Pow(10, 55) * skoreDwojekKrzizki; // 5.
                }
                int sumaKolek = 0;
                int sumaKrzizkow = 0;
                for (int a = 0; a < 8; a++)
                {
                    if (iloscKolek[a] > 0)
                    {
                        sumaKolek += ((int)Math.Pow(iloscKolek[a], 2) * 10 - odlegloscKolko[a]) * (iloscKolek[a] == 3 && wolneMiyjscaNaKoncachKolko[a] >= 1 ? 1 : (int)Math.Pow(wolneMiyjscaNaKoncachKolko[a], 2));
                    }
                    if (iloscKrzizkow[a] > 0)
                    {
                        sumaKrzizkow += ((int)Math.Pow(iloscKrzizkow[a], 2) * 10 - odlegloscKrzizik[a]) * (iloscKrzizkow[a] == 3 && wolneMiyjscaNaKoncachKrzizik[a] >= 1 ? 1 : (int)Math.Pow(wolneMiyjscaNaKoncachKrzizik[a], 2));
                    }
                }
                if (sumaKolek > 0)
                {
                    skore += Math.Pow(10, 15) * sumaKolek / 2; // 10.
                }
                if (sumaKrzizkow > 0)
                {
                    skore += Math.Pow(10, 15) * sumaKrzizkow; // 9.
                }
            }
            
            return skore;
        }

        public double przirzadzSkoreAlfabeta2(int gdoJedzie, int x, int y, int thread)
        {
            double skore = 0;
            int[] pusteMiyjscaOgolneKolko = new int[4] { 1, 1, 1, 1 };
            int[] pusteMiyjscaOgolneKrzizik = new int[4] {1,1,1,1};
            int[] pusteMiyjscaKolko = new int[8] {0,0,0,0,0,0,0,0};
            int[] pusteMiyjscaKrzizik = new int[8] {0,0,0,0,0,0,0,0};
            int[] pusteMiyjscaMjyndzy = new int[8] {0,0,0,0,0,0,0,0};
            int[] pusteMiyjscaMjyndzyActual = new int[8] {0,0,0,0,0,0,0,0};
            bool[] zarachowaneKolka = new bool[8] {false,false,false,false,false,false,false,false};
            bool[] zarachowaneKrzizki = new bool[8] {false,false,false,false,false,false,false,false};
            bool[] zarachowaneKolkaHnedObok = new bool[8] { false, false, false, false, false, false, false, false };
            bool[] zarachowaneKrzizkiHnedObok = new bool[8] { false, false, false, false, false, false, false, false };
            int[] iloscKolekHnedObok = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] iloscKrzizkowHnedObok = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] iloscKolek = new int[8]  {0,0,0,0,0,0,0,0};
            int[] iloscKrzizkow = new int[8] {0,0,0,0,0,0,0,0};
            int[] iloscKolekOgolnie = new int[4] {0,0,0,0};
            int[] iloscKrzizkowOgolnie = new int[4] {0,0,0,0};
            int[] wolneMiyjscaNaKoncachKolko = new int[8] {0,0,0,0,0,0,0,0};
            int[] wolneMiyjscaNaKoncachKrzizik = new int[8] {0,0,0,0,0,0,0,0};
            int[] maxOdlegloscKolko = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] maxOdlegloscKrzizik = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] odlegloscKolko = new int[8] {0,0,0,0,0,0,0,0};
            int[] odlegloscKrzizik = new int[8] {0,0,0,0,0,0,0,0};
            int[] coByloOstatni = new int[8] {0,0,0,0,0,0,0,0};
            if (gdoJedzie == 1)
            {
                for (int j = 0; j < 8; ++j)
                {
                    coByloOstatni[j] = 1;
                }
            }
            else
            {
                for (int j = 0; j < 8; ++j)
                {
                    coByloOstatni[j] = 2;
                }
            }
            int smer = -1;
            int smerOgolny = -1;
            for (int odleglosc = 1; odleglosc <= 4; odleglosc++)
            {
                for (int c = -odleglosc; c <= odleglosc; c++)
                {
                    for (int d = -odleglosc; d <= odleglosc; d++)
                    {
                        if ((Math.Abs(c) == odleglosc || Math.Abs(d) == odleglosc) && !(c == 0 && d == 0) && x + c >= 0 && x + c < szyrzka && y + d >= 0 && y + d < wyszka)
                        {
                            if (c == 0 && d < 0)
                            {
                                smer = 0;
                                smerOgolny = 0;
                            }
                            else if (c == 0 && d > 0)
                            {
                                smer = 1;
                                smerOgolny = 0;
                            }
                            else if (d == 0 && c < 0)
                            {
                                smer = 2;
                                smerOgolny = 1;
                            }
                            else if (d == 0 && c > 0)
                            {
                                smer = 3;
                                smerOgolny = 1;
                            }
                            else if (c == d && c < 0)
                            {
                                smer = 4;
                                smerOgolny = 2;
                            }
                            else if (c == d && c > 0)
                            {
                                smer = 5;
                                smerOgolny = 2;
                            }
                            else if (c == -d && c < 0)
                            {
                                smer = 6;
                                smerOgolny = 3;
                            }
                            else if (c == -d && c > 0)
                            {
                                smer = 7;
                                smerOgolny = 3;
                            }
                            if (c == 0 || d == 0 || c == d || c == -d)
                            {
                                if (odleglosc == 1)
                                {
                                    if (xypole[thread, x + c, y + d] == 0)
                                    {
                                        wolneMiyjscaNaKoncachKolko[smer % 2 == 0 ? smer + 1 : smer - 1]++;
                                        wolneMiyjscaNaKoncachKrzizik[smer % 2 == 0 ? smer + 1 : smer - 1]++;
                                    }
                                    else if (xypole[thread, x + c, y + d] == 1)
                                    {
                                        wolneMiyjscaNaKoncachKrzizik[smer % 2 == 0 ? smer + 1 : smer - 1]++;
                                    }
                                    else if (xypole[thread, x + c, y + d] == 2)
                                    {
                                        wolneMiyjscaNaKoncachKolko[smer % 2 == 0 ? smer + 1 : smer - 1]++;
                                    }
                                }
                                if (xypole[thread, x + c, y + d] == 0)
                                {
                                    zarachowaneKolkaHnedObok[smer] = true;
                                    zarachowaneKrzizkiHnedObok[smer] = true;
                                    pusteMiyjscaKolko[smer]++;
                                    pusteMiyjscaKrzizik[smer]++;
                                    pusteMiyjscaMjyndzy[smer]++;
                                    coByloOstatni[smer] = 0;
                                }
                                else if (xypole[thread, x + c, y + d] == 1)
                                {
                                    zarachowaneKolkaHnedObok[smer] = true;
                                    if (!zarachowaneKrzizkiHnedObok[smer])
                                    {
                                        iloscKrzizkowHnedObok[smer]++;
                                    }
                                    pusteMiyjscaKrzizik[smer]++;
                                    if (!zarachowaneKrzizki[smer] && !zarachowaneKolka[smer])
                                    {
                                        pusteMiyjscaMjyndzyActual[smer] = pusteMiyjscaMjyndzy[smer];
                                    }
                                    if (pusteMiyjscaMjyndzyActual[smer] < 2 && !zarachowaneKrzizki[smer] && odleglosc <= 4)
                                    {
                                        iloscKrzizkow[smer] += 1;
                                        odlegloscKrzizik[smer] += odleglosc;
                                        iloscKrzizkowOgolnie[smerOgolny]++;
                                    }
                                    if (coByloOstatni[smer] == 0 && !zarachowaneKolka[smer])
                                    {
                                        wolneMiyjscaNaKoncachKolko[smer]++;
                                    }
                                    zarachowaneKolka[smer] = true;
                                    coByloOstatni[smer] = 1;
                                    maxOdlegloscKrzizik[smer] = odleglosc;
                                }
                                else if (xypole[thread, x + c, y + d] == 2)
                                {
                                    zarachowaneKrzizkiHnedObok[smer] = true;
                                    if (!zarachowaneKolkaHnedObok[smer])
                                    {
                                        iloscKolekHnedObok[smer]++;
                                    }
                                    pusteMiyjscaKolko[smer]++;
                                    if (!zarachowaneKrzizki[smer] && !zarachowaneKolka[smer])
                                    {
                                        pusteMiyjscaMjyndzyActual[smer] = pusteMiyjscaMjyndzy[smer];
                                    }
                                    if (pusteMiyjscaMjyndzyActual[smer] < 2 && !zarachowaneKolka[smer] && odleglosc <= 4)
                                    {
                                        iloscKolek[smer] += 1;
                                        odlegloscKolko[smer] += odleglosc;
                                        iloscKolekOgolnie[smerOgolny]++;
                                    }
                                    if (coByloOstatni[smer] == 0 && !zarachowaneKrzizki[smer])
                                    {
                                        wolneMiyjscaNaKoncachKrzizik[smer]++;
                                    }
                                    zarachowaneKrzizki[smer] = true;
                                    coByloOstatni[smer] = 2;
                                    maxOdlegloscKolko[smer] = odleglosc;
                                }
                                if (pusteMiyjscaKolko[smer] >= 0 && (xypole[thread, x + c, y + d] == 1 || odleglosc == 4 || (c > 0 ? x + c + 1 >= szyrzka : c < 0 ? x + c - 1 < 0 : false) || (d > 0 ? y + d + 1 >= wyszka : d < 0 ? y + d - 1 < 0 : false)))
                                {
                                    pusteMiyjscaOgolneKolko[smerOgolny] += pusteMiyjscaKolko[smer];
                                    pusteMiyjscaKolko[smer] = int.MinValue;
                                }
                                if (pusteMiyjscaKrzizik[smer] >= 0 && (xypole[thread, x + c, y + d] == 2 || odleglosc == 4 || (c > 0 ? x + c + 1 >= szyrzka : c < 0 ? x + c - 1 < 0 : false) || (d > 0 ? y + d + 1 >= wyszka : d < 0 ? y + d - 1 < 0 : false)))
                                {
                                    pusteMiyjscaOgolneKrzizik[smerOgolny] += pusteMiyjscaKrzizik[smer];
                                    pusteMiyjscaKrzizik[smer] = int.MinValue;
                                }
                                if ((pusteMiyjscaMjyndzyActual[smer] >= 2 || odleglosc == 5 || (c > 0 ? x + c + 1 >= szyrzka : c < 0 ? x + c - 1 < 0 : false) || (d > 0 ? y + d + 1 >= wyszka : d < 0 ? y + d - 1 < 0 : false)) && !zarachowaneKolka[smer])
                                {
                                    if ((coByloOstatni[smer] == 0 || (coByloOstatni[smer] == 2 && odleglosc == 4)) && !(xypole[thread, x + c, y + d] == 2 && odleglosc < 4 && ((c > 0 ? x + c + 1 >= szyrzka : c < 0 ? x + c - 1 < 0 : false) || (d > 0 ? y + d + 1 >= wyszka : d < 0 ? y + d - 1 < 0 : false))))
                                    {
                                        wolneMiyjscaNaKoncachKolko[smer]++;
                                    }
                                    zarachowaneKolka[smer] = true;
                                }
                                if ((pusteMiyjscaMjyndzyActual[smer] >= 2 || odleglosc == 5 || (c > 0 ? x + c + 1 >= szyrzka : c < 0 ? x + c - 1 < 0 : false) || (d > 0 ? y + d + 1 >= wyszka : d < 0 ? y + d - 1 < 0 : false)) && !zarachowaneKrzizki[smer])
                                {
                                    if ((coByloOstatni[smer] == 0 || (coByloOstatni[smer] == 1 && odleglosc == 4)) && !(xypole[thread, x + c, y + d] == 1 && odleglosc < 4 && ((c > 0 ? x + c + 1 >= szyrzka : c < 0 ? x + c - 1 < 0 : false) || (d > 0 ? y + d + 1 >= wyszka : d < 0 ? y + d - 1 < 0 : false))))
                                    {
                                        wolneMiyjscaNaKoncachKrzizik[smer]++;
                                    }
                                    zarachowaneKrzizki[smer] = true;
                                }
                            }
                        }
                    }
                }
            }

            if (gdoJedzie == 2)
            {
                for (int a = 0; a < 4; a++)
                {
                    if (pusteMiyjscaOgolneKolko[a] < 5)
                    {
                        iloscKolekOgolnie[a] = int.MinValue;
                        iloscKolek[2 * a] = int.MinValue;
                        iloscKolek[2 * a + 1] = int.MinValue;
                    }
                    if (pusteMiyjscaOgolneKrzizik[a] < 5)
                    {
                        iloscKrzizkowOgolnie[a] = int.MinValue;
                        iloscKrzizkow[2 * a] = int.MinValue;
                        iloscKrzizkow[2 * a + 1] = int.MinValue;
                    }
                }
                for (int a = 0; a < 4; a++)
                {
                    if ((pusteMiyjscaMjyndzyActual[a * 2] + pusteMiyjscaMjyndzyActual[a * 2 + 1] == 0) || (iloscKolek[a * 2] == 4 && pusteMiyjscaMjyndzyActual[a * 2] == 0) || (iloscKolek[a * 2 + 1] == 4 && pusteMiyjscaMjyndzyActual[a * 2 + 1] == 0) || (iloscKrzizkow[a * 2] == 4 && pusteMiyjscaMjyndzyActual[a * 2] == 0) || (iloscKrzizkow[a * 2 + 1] == 4 && pusteMiyjscaMjyndzyActual[a * 2 + 1] == 0))
                    {
                        if (iloscKolekHnedObok[a * 2] + iloscKolekHnedObok[a * 2 + 1] >= 4)
                        {
                            skore += Math.Pow(10, 95); // 1.
                        }
                        else if (iloscKolekOgolnie[a] == 3 && wolneMiyjscaNaKoncachKolko[a * 2] != 1 && wolneMiyjscaNaKoncachKolko[a * 2 + 1] != 1 && odlegloscKolko[a * 2] + odlegloscKolko[a * 2 + 1] <= 6)
                        {
                            skore += Math.Pow(10, 75) + Math.Pow(10, 74) * (10 - (odlegloscKolko[a * 2] + odlegloscKolko[a * 2 + 1])); // 3.
                        }
                        if (iloscKrzizkowHnedObok[a * 2] + iloscKrzizkowHnedObok[a * 2 + 1] >= 4)
                        {
                            skore -= Math.Pow(10, 85); // 2.
                        }
                        else if (iloscKrzizkowOgolnie[a] == 3 && wolneMiyjscaNaKoncachKrzizik[a * 2] != 1 && wolneMiyjscaNaKoncachKrzizik[a * 2 + 1] != 1 && odlegloscKrzizik[a * 2] + odlegloscKrzizik[a * 2 + 1] <= 6)
                        {
                            skore -= Math.Pow(10, 65) + Math.Pow(10, 64) * (10 - (odlegloscKrzizik[a * 2] + odlegloscKrzizik[a * 2 + 1])); // 4.
                        }
                    }
                    if (iloscKolekOgolnie[a] == 3 || iloscKolek[2 * a] == 3 || iloscKolek[2 * a + 1] == 3)
                    {
                        skore += Math.Pow(10, 35) + Math.Pow(10, 34) * (10 - (odlegloscKolko[a * 2] + odlegloscKolko[a * 2 + 1])); // 7.
                    }
                    if (iloscKrzizkowOgolnie[a] == 3 || iloscKrzizkow[2 * a] == 3 || iloscKrzizkow[2 * a + 1] == 3)
                    {
                        if (iloscKrzizkow[2 * a] == 3 && maxOdlegloscKrzizik[2 * a] == 4 && maxOdlegloscKolko[2 * a] == 0 && (odlegloscKrzizik[2 * a] == 7 || odlegloscKrzizik[2 * a] == 8))
                        {
                            skore -= Math.Pow(10, 34); // 7b.
                        }
                        if (iloscKrzizkow[2 * a + 1] == 3 && maxOdlegloscKrzizik[2 * a + 1] == 4 && maxOdlegloscKolko[2 * a + 1] == 0 && (odlegloscKrzizik[2 * a + 1] == 7 || odlegloscKrzizik[2 * a + 1] == 8))
                        {
                            skore -= Math.Pow(10, 34); // 7b.
                        }
                        skore -= Math.Pow(10, 25) + Math.Pow(10, 24) * (10 - (odlegloscKrzizik[a * 2] + odlegloscKrzizik[a * 2 + 1])); // 8.
                    }
                }
                double skoreDwojekKolka = 0;
                int sumaDwojekKolka = 0;
                for (int a = 0; a < 4; a++)
                {
                    if (iloscKolekOgolnie[a] >= 2 && !(iloscKolek[2 * a] > 0 && (wolneMiyjscaNaKoncachKolko[2 * a] != 2 || maxOdlegloscKolko[2 * a] >= 4)) && !(iloscKolek[2 * a + 1] > 0 && (wolneMiyjscaNaKoncachKolko[2 * a + 1] != 2 || maxOdlegloscKolko[2 * a + 1] >= 4)))
                    {
                        skoreDwojekKolka += iloscKolekOgolnie[a] * 10 - odlegloscKolko[2 * a] - odlegloscKolko[2 * a + 1];
                        sumaDwojekKolka++;
                    }
                }
                if (sumaDwojekKolka >= 2)
                {
                    skore += Math.Pow(10, 55) * skoreDwojekKolka; // 5.
                }
                double skoreDwojekKrzizki = 0;
                int sumaDwojekKrzizki = 0;
                for (int a = 0; a < 4; a++)
                {
                    if (iloscKrzizkowOgolnie[a] >= 2 && !(iloscKrzizkow[2 * a] > 0 && (wolneMiyjscaNaKoncachKrzizik[2 * a] != 2 || maxOdlegloscKrzizik[a * 2] >= 4)) && !(iloscKrzizkow[2 * a + 1] > 0 && (wolneMiyjscaNaKoncachKrzizik[2 * a + 1] != 2 || maxOdlegloscKrzizik[a * 2 + 1] >= 4)))
                    {
                        skoreDwojekKrzizki += iloscKrzizkowOgolnie[a] * 10 - odlegloscKrzizik[2 * a] - odlegloscKrzizik[2 * a + 1];
                        sumaDwojekKrzizki++;
                    }
                }
                if (sumaDwojekKrzizki >= 2)
                {
                    skore -= Math.Pow(10, 45) * skoreDwojekKrzizki; // 6.
                }
                int sumaKolek = 0;
                int sumaKrzizkow = 0;
                for (int a = 0; a < 8; a++)
                {
                    if (iloscKolek[a] > 0)
                    {
                        sumaKolek += ((int)Math.Pow(iloscKolek[a], 2) * 10 - odlegloscKolko[a]) * (int)Math.Pow(wolneMiyjscaNaKoncachKolko[a], 2);
                    }
                    if (iloscKrzizkow[a] > 0)
                    {
                        sumaKrzizkow += ((int)Math.Pow(iloscKrzizkow[a], 2) * 10 - odlegloscKrzizik[a]) * (int)Math.Pow(wolneMiyjscaNaKoncachKrzizik[a], 2);// (iloscKrzizkow[a] == 3 && wolneMiyjscaNaKoncachKrzizik[a] >= 1 ? 1 : (int)Math.Pow(wolneMiyjscaNaKoncachKrzizik[a], 2));
                    }
                }
                if (sumaKolek > 0)
                {
                    skore += Math.Pow(10, 15) * sumaKolek; // 9.
                }
                if (sumaKrzizkow > 0)
                {
                    skore -= Math.Pow(10, 15) * sumaKrzizkow / 2; // 10.
                }
                /*xyMjynszeSkore[x, y, 0] = sumaKolek;
                xyMjynszeSkore[x, y, 1] = sumaKrzizkow;*/
            }
            else if (gdoJedzie == 1)
            {
                for (int a = 0; a < 4; a++)
                {
                    if (pusteMiyjscaOgolneKolko[a] < 5)
                    {
                        iloscKolekOgolnie[a] = int.MinValue;
                        iloscKolek[2 * a] = int.MinValue;
                        iloscKolek[2 * a + 1] = int.MinValue;
                    }
                    if (pusteMiyjscaOgolneKrzizik[a] < 5)
                    {
                        iloscKrzizkowOgolnie[a] = int.MinValue;
                        iloscKrzizkow[2 * a] = int.MinValue;
                        iloscKrzizkow[2 * a + 1] = int.MinValue;
                    }
                }
                for (int a = 0; a < 4; a++)
                {
                    if ((pusteMiyjscaMjyndzyActual[a * 2] + pusteMiyjscaMjyndzyActual[a * 2 + 1] == 0) || (iloscKolek[a * 2] == 4 && pusteMiyjscaMjyndzyActual[a * 2] == 0) || (iloscKolek[a * 2 + 1] == 4 && pusteMiyjscaMjyndzyActual[a * 2 + 1] == 0) || (iloscKrzizkow[a * 2] == 4 && pusteMiyjscaMjyndzyActual[a * 2] == 0) || (iloscKrzizkow[a * 2 + 1] == 4 && pusteMiyjscaMjyndzyActual[a * 2 + 1] == 0))
                    {
                        if (iloscKolekHnedObok[a * 2] + iloscKolekHnedObok[a * 2 + 1] >= 4)
                        {
                            skore -= Math.Pow(10, 85); // 2.
                        }
                        else if (iloscKolekOgolnie[a] == 3 && wolneMiyjscaNaKoncachKolko[a * 2] != 1 && wolneMiyjscaNaKoncachKolko[a * 2 + 1] != 1 && odlegloscKolko[a * 2] + odlegloscKolko[a * 2 + 1] <= 6)
                        {
                            skore -= Math.Pow(10, 65) + Math.Pow(10, 64) * (10 - (odlegloscKolko[a * 2] + odlegloscKolko[a * 2 + 1])); // 4.
                        }
                        if (iloscKrzizkowHnedObok[a * 2] + iloscKrzizkowHnedObok[a * 2 + 1] >= 4)
                        {
                            skore += Math.Pow(10, 95); // 1.
                        }
                        else if (iloscKrzizkowOgolnie[a] == 3 && wolneMiyjscaNaKoncachKrzizik[a * 2] != 1 && wolneMiyjscaNaKoncachKrzizik[a * 2 + 1] != 1 && odlegloscKrzizik[a * 2] + odlegloscKrzizik[a * 2 + 1] <= 6)
                        {
                            skore += Math.Pow(10, 75) + Math.Pow(10, 74) * (10 - (odlegloscKrzizik[a * 2] + odlegloscKrzizik[a * 2 + 1])); // 3.
                        }
                    }
                    if (iloscKolekOgolnie[a] == 3 || iloscKolek[2 * a] == 3 || iloscKolek[2 * a + 1] == 3)
                    {
                        if (iloscKolek[2 * a] == 3 && maxOdlegloscKolko[2 * a] == 4 && maxOdlegloscKrzizik[2 * a] == 0 && (odlegloscKolko[2 * a] == 7 || odlegloscKolko[2 * a] == 8))
                        {
                            skore -= Math.Pow(10, 34); // 7b.
                        }
                        if (iloscKolek[2 * a + 1] == 3 && maxOdlegloscKolko[2 * a + 1] == 4 && maxOdlegloscKrzizik[2 * a + 1] == 0 && (odlegloscKolko[2 * a + 1] == 7 || odlegloscKolko[2 * a + 1] == 8))
                        {
                            skore -= Math.Pow(10, 34); // 7b.
                        }
                        skore -= Math.Pow(10, 25) + Math.Pow(10, 24) * (10 - (odlegloscKolko[a * 2] + odlegloscKolko[a * 2 + 1])); // 8.
                    }
                    if (iloscKrzizkowOgolnie[a] == 3 || iloscKrzizkow[2 * a] == 3 || iloscKrzizkow[2 * a + 1] == 3)
                    {
                        skore += Math.Pow(10, 35) + Math.Pow(10, 34) * (10 - (odlegloscKrzizik[a * 2] + odlegloscKrzizik[a * 2 + 1]));// 7.
                    }
                }
                double skoreDwojekKolka = 0;
                int sumaDwojekKolka = 0;
                for (int a = 0; a < 4; a++)
                {
                    if (iloscKolekOgolnie[a] >= 2 && !(iloscKolek[2 * a] > 0 && (wolneMiyjscaNaKoncachKolko[2 * a] != 2 || maxOdlegloscKolko[2 * a] >= 4)) && !(iloscKolek[2 * a + 1] > 0 && (wolneMiyjscaNaKoncachKolko[2 * a + 1] != 2 || maxOdlegloscKolko[2 * a + 1] >= 4)))
                    {
                        skoreDwojekKolka += iloscKolekOgolnie[a] * 10 - odlegloscKolko[2 * a] - odlegloscKolko[2 * a + 1];
                        sumaDwojekKolka++;
                    }
                }
                if (sumaDwojekKolka >= 2)
                {
                    skore -= Math.Pow(10, 45) * skoreDwojekKolka; // 6.
                }
                double skoreDwojekKrzizki = 0;
                int sumaDwojekKrzizki = 0;
                for (int a = 0; a < 4; a++)
                {
                    if (iloscKrzizkowOgolnie[a] >= 2 && !(iloscKrzizkow[2 * a] > 0 && (wolneMiyjscaNaKoncachKrzizik[2 * a] != 2 || maxOdlegloscKrzizik[a * 2] >= 4)) && !(iloscKrzizkow[2 * a + 1] > 0 && (wolneMiyjscaNaKoncachKrzizik[2 * a + 1] != 2 || maxOdlegloscKrzizik[a * 2 + 1] >= 4)))
                    {
                        skoreDwojekKrzizki += iloscKrzizkowOgolnie[a] * 10 - odlegloscKrzizik[2 * a] - odlegloscKrzizik[2 * a + 1];
                        sumaDwojekKrzizki++;
                    }
                }
                if (sumaDwojekKrzizki >= 2)
                {
                    skore += Math.Pow(10, 55) * skoreDwojekKrzizki; // 5.
                }
                int sumaKolek = 0;
                int sumaKrzizkow = 0;
                for (int a = 0; a < 8; a++)
                {
                    if (iloscKolek[a] > 0)
                    {
                        sumaKolek += ((int)Math.Pow(iloscKolek[a], 2) * 10 - odlegloscKolko[a]) * (int)Math.Pow(wolneMiyjscaNaKoncachKolko[a], 2);
                    }
                    if (iloscKrzizkow[a] > 0)
                    {
                        sumaKrzizkow += ((int)Math.Pow(iloscKrzizkow[a], 2) * 10 - odlegloscKrzizik[a]) * (int)Math.Pow(wolneMiyjscaNaKoncachKrzizik[a], 2);
                    }
                }
                if (sumaKolek > 0)
                {
                    skore -= Math.Pow(10, 15) * sumaKolek / 2;
                }
                if (sumaKrzizkow > 0)
                {
                    skore += Math.Pow(10, 15) * sumaKrzizkow;
                }
            }
            return skore;
        }

        public double przirzadzSkoreAlfabeta3(int thread)
        {
            int lewel = 0;
            int lewo = 1;
            int prawo = 13;
            int gorno = 1;
            int spodnio = 13;
            lewo = zmjynLewo1(thread);
            prawo = zmjynPrawo1(thread);
            gorno = zmjynGorno1(thread);
            spodnio = zmjynSpodnio1(thread);
            double skore = double.NegativeInfinity;
            for (int x = lewo - 1; x < prawo + 1; x++)
            {
                for (int y = gorno - 1; y < spodnio + 1; y++)
                {
                    if (x >= 0 && x < szyrzka && y >= 0 && y < wyszka)
                    {
                        skore = przirzadzSkoreAlfabeta2(2, x, y, thread);
                        lewel = lewelSkore(lewel, skore);
                    }
                }
            }
            double score = 0;
            if (lewel % 2 == 0)
            {
                score = lewel + 1/(double)(Math.Log10(Math.Abs(skore)));
            }
            else
            {
                score = -lewel - 1 / (double)(Math.Log10(Math.Abs(skore)));
            }
            return score;
        }

        public double[] przirzadzSkoreAlfabeta4(int gdoJedzie, int x, int y, int thread)
        {
            double[] skore2 = new double[2];
            int[] pusteMiyjscaOgolneKolko = new int[4] { 1, 1, 1, 1 };
            int[] pusteMiyjscaOgolneKrzizik = new int[4] {1,1,1,1};
            int[] pusteMiyjscaKolko = new int[8] {0,0,0,0,0,0,0,0};
            int[] pusteMiyjscaKrzizik = new int[8] {0,0,0,0,0,0,0,0};
            int[] pusteMiyjscaMjyndzy = new int[8] {0,0,0,0,0,0,0,0};
            int[] pusteMiyjscaMjyndzyActual = new int[8] {0,0,0,0,0,0,0,0};
            bool[] zarachowaneKolka = new bool[8] {false,false,false,false,false,false,false,false};
            bool[] zarachowaneKrzizki = new bool[8] {false,false,false,false,false,false,false,false};
            bool[] zarachowaneKolkaHnedObok = new bool[8] { false, false, false, false, false, false, false, false };
            bool[] zarachowaneKrzizkiHnedObok = new bool[8] { false, false, false, false, false, false, false, false };
            int[] iloscKolekHnedObok = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] iloscKrzizkowHnedObok = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] iloscKolek = new int[8]  {0,0,0,0,0,0,0,0};
            int[] iloscKrzizkow = new int[8] {0,0,0,0,0,0,0,0};
            int[] iloscKolekOgolnie = new int[4] {0,0,0,0};
            int[] iloscKrzizkowOgolnie = new int[4] {0,0,0,0};
            int[] wolneMiyjscaNaKoncachKolko = new int[8] {0,0,0,0,0,0,0,0};
            int[] wolneMiyjscaNaKoncachKrzizik = new int[8] {0,0,0,0,0,0,0,0};
            int[] maxOdlegloscKolko = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] maxOdlegloscKrzizik = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] odlegloscKolko = new int[8] {0,0,0,0,0,0,0,0};
            int[] odlegloscKrzizik = new int[8] {0,0,0,0,0,0,0,0};
            int[] coByloOstatni = new int[8];
            if (gdoJedzie == 1)
            {
                for (int j = 0; j < 8; ++j)
                {
                    coByloOstatni[j] = 1;
                }
            }
            else
            {
                for (int j = 0; j < 8; ++j)
                {
                    coByloOstatni[j] = 2;
                }
            }
            int smer = -1;
            int smerOgolny = -1;
            for (int odleglosc = 1; odleglosc <= 5; odleglosc++)
            {
                for (int c = -odleglosc; c <= odleglosc; c++)
                {
                    for (int d = -odleglosc; d <= odleglosc; d++)
                    {
                        if ((Math.Abs(c) == odleglosc || Math.Abs(d) == odleglosc) && !(c == 0 && d == 0) && x + c >= 0 && x + c < szyrzka && y + d >= 0 && y + d < wyszka)
                        {
                            if (c == 0 && d < 0)
                            {
                                smer = 0;
                                smerOgolny = 0;
                            }
                            else if (c == 0 && d > 0)
                            {
                                smer = 1;
                                smerOgolny = 0;
                            }
                            else if (d == 0 && c < 0)
                            {
                                smer = 2;
                                smerOgolny = 1;
                            }
                            else if (d == 0 && c > 0)
                            {
                                smer = 3;
                                smerOgolny = 1;
                            }
                            else if (c == d && c < 0)
                            {
                                smer = 4;
                                smerOgolny = 2;
                            }
                            else if (c == d && c > 0)
                            {
                                smer = 5;
                                smerOgolny = 2;
                            }
                            else if (c == -d && c < 0)
                            {
                                smer = 6;
                                smerOgolny = 3;
                            }
                            else if (c == -d && c > 0)
                            {
                                smer = 7;
                                smerOgolny = 3;
                            }
                            if (c == 0 || d == 0 || c == d || c == -d)
                            {
                                if (odleglosc == 1)
                                {
                                    if (xypole[thread, x + c, y + d] == 0)
                                    {
                                        wolneMiyjscaNaKoncachKolko[smer % 2 == 0 ? smer + 1 : smer - 1]++;
                                        wolneMiyjscaNaKoncachKrzizik[smer % 2 == 0 ? smer + 1 : smer - 1]++;
                                    }
                                    else if (xypole[thread, x + c, y + d] == 1)
                                    {
                                        wolneMiyjscaNaKoncachKrzizik[smer % 2 == 0 ? smer + 1 : smer - 1]++;
                                    }
                                    else if (xypole[thread, x + c, y + d] == 2)
                                    {
                                        wolneMiyjscaNaKoncachKolko[smer % 2 == 0 ? smer + 1 : smer - 1]++;
                                    }
                                }
                                if (xypole[thread, x + c, y + d] == 0)
                                {
                                    zarachowaneKolkaHnedObok[smer] = true;
                                    zarachowaneKrzizkiHnedObok[smer] = true;
                                    pusteMiyjscaKolko[smer]++;
                                    pusteMiyjscaKrzizik[smer]++;
                                    pusteMiyjscaMjyndzy[smer]++;
                                    coByloOstatni[smer] = 0;
                                }
                                else if (xypole[thread, x + c, y + d] == 1)
                                {
                                    zarachowaneKolkaHnedObok[smer] = true;
                                    if(!zarachowaneKrzizkiHnedObok[smer])
                                    {
                                        iloscKrzizkowHnedObok[smer]++;
                                    }
                                    pusteMiyjscaKrzizik[smer]++;
                                    if(!zarachowaneKrzizki[smer] && !zarachowaneKolka[smer])
                                    {
                                        pusteMiyjscaMjyndzyActual[smer] = pusteMiyjscaMjyndzy[smer];
                                    }
                                    if (pusteMiyjscaMjyndzyActual[smer] < 2 && !zarachowaneKrzizki[smer] && odleglosc <= 4)
                                    {
                                        iloscKrzizkow[smer] += 1;
                                        odlegloscKrzizik[smer] += odleglosc;
                                        iloscKrzizkowOgolnie[smerOgolny]++;
                                    }
                                    if (coByloOstatni[smer] == 0 && !zarachowaneKolka[smer])
                                    {
                                        wolneMiyjscaNaKoncachKolko[smer]++;
                                    }
                                    zarachowaneKolka[smer] = true;
                                    coByloOstatni[smer] = 1;
                                    maxOdlegloscKrzizik[smer] = odleglosc;
                                }
                                else if (xypole[thread, x + c, y + d] == 2)
                                {
                                    zarachowaneKrzizkiHnedObok[smer] = true;
                                    if (!zarachowaneKolkaHnedObok[smer])
                                    {
                                        iloscKolekHnedObok[smer]++;
                                    }
                                    pusteMiyjscaKolko[smer]++;
                                    if (!zarachowaneKrzizki[smer] && !zarachowaneKolka[smer])
                                    {
                                        pusteMiyjscaMjyndzyActual[smer] = pusteMiyjscaMjyndzy[smer];
                                    }
                                    if (pusteMiyjscaMjyndzyActual[smer] < 2 && !zarachowaneKolka[smer] && odleglosc <= 4)
                                    {
                                        iloscKolek[smer] += 1;
                                        odlegloscKolko[smer] += odleglosc;
                                        iloscKolekOgolnie[smerOgolny]++;
                                    }
                                    if (coByloOstatni[smer] == 0 && !zarachowaneKrzizki[smer])
                                    {
                                        wolneMiyjscaNaKoncachKrzizik[smer]++;
                                    }
                                    zarachowaneKrzizki[smer] = true;
                                    coByloOstatni[smer] = 2;
                                    maxOdlegloscKolko[smer] = odleglosc;
                                }
                                if (pusteMiyjscaKolko[smer] >= 0 && (xypole[thread, x + c, y + d] == 1 || odleglosc == 4 || (c > 0 ? x + c + 1 >= szyrzka : c < 0 ? x + c - 1 < 0 : false) || (d > 0 ? y + d + 1 >= wyszka : d < 0 ? y + d - 1 < 0 : false)))
                                {
                                    pusteMiyjscaOgolneKolko[smerOgolny] += pusteMiyjscaKolko[smer];
                                    pusteMiyjscaKolko[smer] = int.MinValue;
                                }
                                if (pusteMiyjscaKrzizik[smer] >= 0 && (xypole[thread, x + c, y + d] == 2 || odleglosc == 4 || (c > 0 ? x + c + 1 >= szyrzka : c < 0 ? x + c - 1 < 0 : false) || (d > 0 ? y + d + 1 >= wyszka : d < 0 ? y + d - 1 < 0 : false)))
                                {
                                    pusteMiyjscaOgolneKrzizik[smerOgolny] += pusteMiyjscaKrzizik[smer];
                                    pusteMiyjscaKrzizik[smer] = int.MinValue;
                                }
                                if ((pusteMiyjscaMjyndzyActual[smer] >= 2 || odleglosc == 5 || (c > 0 ? x + c + 1 >= szyrzka : c < 0 ? x + c - 1 < 0 : false) || (d > 0 ? y + d + 1 >= wyszka : d < 0 ? y + d - 1 < 0 : false)) && !zarachowaneKolka[smer])
                                {
                                    if ((coByloOstatni[smer] == 0 || (coByloOstatni[smer] == 2 && odleglosc == 4)) && !(xypole[thread, x + c, y + d] == 2 && odleglosc < 4 && ((c > 0 ? x + c + 1 >= szyrzka : c < 0 ? x + c - 1 < 0 : false) || (d > 0 ? y + d + 1 >= wyszka : d < 0 ? y + d - 1 < 0 : false))))
                                    {
                                        wolneMiyjscaNaKoncachKolko[smer]++;
                                    }
                                    zarachowaneKolka[smer] = true;
                                }
                                if ((pusteMiyjscaMjyndzyActual[smer] >= 2 || odleglosc == 5 || (c > 0 ? x + c + 1 >= szyrzka : c < 0 ? x + c - 1 < 0 : false) || (d > 0 ? y + d + 1 >= wyszka : d < 0 ? y + d - 1 < 0 : false)) && !zarachowaneKrzizki[smer])
                                {
                                    if ((coByloOstatni[smer] == 0 || (coByloOstatni[smer] == 1 && odleglosc == 4)) && !(xypole[thread, x + c, y + d] == 1 && odleglosc < 4 && ((c > 0 ? x + c + 1 >= szyrzka : c < 0 ? x + c - 1 < 0 : false) || (d > 0 ? y + d + 1 >= wyszka : d < 0 ? y + d - 1 < 0 : false))))
                                    {
                                        wolneMiyjscaNaKoncachKrzizik[smer]++;
                                    }
                                    zarachowaneKrzizki[smer] = true;
                                }
                            }
                        }
                    }
                }
            }
            
            if (gdoJedzie == 2)
            {
                for (int a = 0; a < 4; a++)
                {
                    if (pusteMiyjscaOgolneKolko[a] < 5)
                    {
                        iloscKolekOgolnie[a] = int.MinValue;
                        iloscKolek[2 * a] = int.MinValue;
                        iloscKolek[2 * a + 1] = int.MinValue;
                    }
                    if (pusteMiyjscaOgolneKrzizik[a] < 5)
                    {
                        iloscKrzizkowOgolnie[a] = int.MinValue;
                        iloscKrzizkow[2 * a] = int.MinValue;
                        iloscKrzizkow[2 * a + 1] = int.MinValue;
                    }
                }
                for (int a = 0; a < 4; a++)
                {
                    if ((pusteMiyjscaMjyndzyActual[a * 2] + pusteMiyjscaMjyndzyActual[a * 2 + 1] == 0) || (iloscKolek[a * 2] == 4 && pusteMiyjscaMjyndzyActual[a * 2] == 0) || (iloscKolek[a * 2 + 1] == 4 && pusteMiyjscaMjyndzyActual[a * 2 + 1] == 0) || (iloscKrzizkow[a * 2] == 4 && pusteMiyjscaMjyndzyActual[a * 2] == 0) || (iloscKrzizkow[a * 2 + 1] == 4 && pusteMiyjscaMjyndzyActual[a * 2 + 1] == 0))
                    {
                        if (iloscKolekHnedObok[a * 2] + iloscKolekHnedObok[a * 2 + 1] >= 4)
                        {
                            skore2[0] += Math.Pow(10, 95); // 1.
                            skore2[1] += Math.Pow(10, 95); // 1.
                        }
                        else if (iloscKolekOgolnie[a] == 3 && wolneMiyjscaNaKoncachKolko[a * 2] != 1 && wolneMiyjscaNaKoncachKolko[a * 2 + 1] != 1 && odlegloscKolko[a * 2] + odlegloscKolko[a * 2 + 1] <= 6)
                        {
                            skore2[0] += Math.Pow(10, 75) + Math.Pow(10, 74) * (10 - (odlegloscKolko[a * 2] + odlegloscKolko[a * 2 + 1])); // 3.
                            skore2[1] += Math.Pow(10, 75) + Math.Pow(10, 74) * (10 - (odlegloscKolko[a * 2] + odlegloscKolko[a * 2 + 1])); // 3.
                        }
                        if (iloscKrzizkowHnedObok[a * 2] + iloscKrzizkowHnedObok[a * 2 + 1] >= 4)
                        {
                            skore2[0] -= Math.Pow(10, 85); // 2.
                            skore2[1] += Math.Pow(10, 85); // 2.
                        }
                        else if (iloscKrzizkowOgolnie[a] == 3 && wolneMiyjscaNaKoncachKrzizik[a * 2] != 1 && wolneMiyjscaNaKoncachKrzizik[a * 2 + 1] != 1 && odlegloscKrzizik[a * 2] + odlegloscKrzizik[a * 2 + 1] <= 6)
                        {
                            skore2[0] -= Math.Pow(10, 65) + Math.Pow(10, 64) * (10 - (odlegloscKrzizik[a * 2] + odlegloscKrzizik[a * 2 + 1])); // 4.
                            skore2[1] += Math.Pow(10, 65) + Math.Pow(10, 64) * (10 - (odlegloscKrzizik[a * 2] + odlegloscKrzizik[a * 2 + 1])); // 4.
                        }
                    }
                    if (iloscKolekOgolnie[a] == 3 || iloscKolek[2 * a] == 3 || iloscKolek[2 * a + 1] == 3)
                    {
                        skore2[0] += Math.Pow(10, 35) + Math.Pow(10, 34) * (10 - (odlegloscKolko[a * 2] + odlegloscKolko[a * 2 + 1])); // 7.
                        skore2[1] += Math.Pow(10, 35) + Math.Pow(10, 34) * (10 - (odlegloscKolko[a * 2] + odlegloscKolko[a * 2 + 1])); // 7.
                    }
                    if (iloscKrzizkowOgolnie[a] == 3 || iloscKrzizkow[2 * a] == 3 || iloscKrzizkow[2 * a + 1] == 3)
                    {
                        if (iloscKrzizkow[2 * a] == 3 && maxOdlegloscKrzizik[2 * a] == 4 && maxOdlegloscKolko[2 * a] == 0 && (odlegloscKrzizik[2 * a] == 7 || odlegloscKrzizik[2 * a] == 8))
                        {
                            skore2[0] -= Math.Pow(10, 34); // 7b.
                            skore2[1] += Math.Pow(10, 34); // 7b.
                        }
                        if (iloscKrzizkow[2 * a + 1] == 3 && maxOdlegloscKrzizik[2 * a + 1] == 4 && maxOdlegloscKolko[2 * a + 1] == 0 && (odlegloscKrzizik[2 * a + 1] == 7 || odlegloscKrzizik[2 * a + 1] == 8))
                        {
                            skore2[0] -= Math.Pow(10, 34); // 7b.
                            skore2[1] += Math.Pow(10, 34); // 7b.
                        }
                        skore2[0] -= Math.Pow(10, 25) + Math.Pow(10, 24) * (10 - (odlegloscKrzizik[a * 2] + odlegloscKrzizik[a * 2 + 1])); // 8.
                        skore2[1] += Math.Pow(10, 25) + Math.Pow(10, 24) * (10 - (odlegloscKrzizik[a * 2] + odlegloscKrzizik[a * 2 + 1])); // 8.
                    }
                }
                double skoreDwojekKolka = 0;
                int sumaDwojekKolka = 0;
                for (int a = 0; a < 4; a++)
                {
                    if (iloscKolekOgolnie[a] >= 2 && !(iloscKolek[2 * a] > 0 && (wolneMiyjscaNaKoncachKolko[2 * a] != 2 || maxOdlegloscKolko[2 * a] >= 4)) && !(iloscKolek[2 * a + 1] > 0 && (wolneMiyjscaNaKoncachKolko[2 * a + 1] != 2 || maxOdlegloscKolko[2 * a + 1] >= 4)))
                    {
                        skoreDwojekKolka += iloscKolekOgolnie[a] * 10 - odlegloscKolko[2 * a] - odlegloscKolko[2 * a + 1];
                        sumaDwojekKolka++;
                    }
                }
                if (sumaDwojekKolka >= 2)
                {
                    skore2[0] += Math.Pow(10, 55) * skoreDwojekKolka; // 5.
                    skore2[1] += Math.Pow(10, 55) * skoreDwojekKolka; // 5.
                }
                double skoreDwojekKrzizki = 0;
                int sumaDwojekKrzizki = 0;
                for (int a = 0; a < 4; a++)
                {
                    if (iloscKrzizkowOgolnie[a] >= 2 && !(iloscKrzizkow[2 * a] > 0 && (wolneMiyjscaNaKoncachKrzizik[2 * a] != 2 || maxOdlegloscKrzizik[a * 2] >= 4)) && !(iloscKrzizkow[2 * a + 1] > 0 && (wolneMiyjscaNaKoncachKrzizik[2 * a + 1] != 2 || maxOdlegloscKrzizik[a * 2 + 1] >= 4)))
                    {
                        skoreDwojekKrzizki += iloscKrzizkowOgolnie[a] * 10 - odlegloscKrzizik[2 * a] - odlegloscKrzizik[2 * a + 1];
                        sumaDwojekKrzizki++;
                    }
                }
                if (sumaDwojekKrzizki >= 2)
                {
                    skore2[0] -= Math.Pow(10, 45) * skoreDwojekKrzizki; // 6.
                    skore2[1] += Math.Pow(10, 45) * skoreDwojekKrzizki; // 6.
                }
                int sumaKolek = 0;
                int sumaKrzizkow = 0;
                for (int a = 0; a < 8; a++)
                {
                    if (iloscKolek[a] > 0)
                    {
                        sumaKolek += ((int)Math.Pow(iloscKolek[a], 2) * 10 - odlegloscKolko[a]) * (int)Math.Pow(wolneMiyjscaNaKoncachKolko[a], 2);
                    }
                    if (iloscKrzizkow[a] > 0)
                    {
                        sumaKrzizkow += ((int)Math.Pow(iloscKrzizkow[a], 2) * 10 - odlegloscKrzizik[a]) * (int)Math.Pow(wolneMiyjscaNaKoncachKrzizik[a], 2);
                    }
                    
                }
                if (sumaKolek > 0)
                {
                    skore2[0] += Math.Pow(10, 15) * sumaKolek; // 9.
                    skore2[1] += Math.Pow(10, 15) * sumaKolek; // 9.
                }
                if (sumaKrzizkow > 0)
                {
                    skore2[0] -= Math.Pow(10, 15) * sumaKrzizkow / 2; // 10.
                    skore2[1] += Math.Pow(10, 15) * sumaKrzizkow / 2; // 10.
                }
                /*xyMjynszeskore2[x, y, 0] = sumaKolek;
                xyMjynszeskore2[x, y, 1] = sumaKrzizkow;*/
            }
            else if (gdoJedzie == 1)
            {
                for (int a = 0; a < 4; a++)
                {
                    if (pusteMiyjscaOgolneKolko[a] < 5)
                    {
                        iloscKolekOgolnie[a] = int.MinValue;
                        iloscKolek[2 * a] = int.MinValue;
                        iloscKolek[2 * a + 1] = int.MinValue;
                    }
                    if (pusteMiyjscaOgolneKrzizik[a] < 5)
                    {
                        iloscKrzizkowOgolnie[a] = int.MinValue;
                        iloscKrzizkow[2 * a] = int.MinValue;
                        iloscKrzizkow[2 * a + 1] = int.MinValue;
                    }
                }
                for (int a = 0; a < 4; a++)
                {
                    if ((pusteMiyjscaMjyndzyActual[a * 2] + pusteMiyjscaMjyndzyActual[a * 2 + 1] == 0) || (iloscKolek[a * 2] == 4 && pusteMiyjscaMjyndzyActual[a * 2] == 0) || (iloscKolek[a * 2 + 1] == 4 && pusteMiyjscaMjyndzyActual[a * 2 + 1] == 0) || (iloscKrzizkow[a * 2] == 4 && pusteMiyjscaMjyndzyActual[a * 2] == 0) || (iloscKrzizkow[a * 2 + 1] == 4 && pusteMiyjscaMjyndzyActual[a * 2 + 1] == 0))
                    {
                        if (iloscKolekHnedObok[a * 2] + iloscKolekHnedObok[a * 2 + 1] >= 4)
                        {
                            skore2[0] -= Math.Pow(10, 85); // 2.
                            skore2[1] += Math.Pow(10, 85); // 2.
                        }
                        else if (iloscKolekOgolnie[a] == 3 && wolneMiyjscaNaKoncachKolko[a * 2] != 1 && wolneMiyjscaNaKoncachKolko[a * 2 + 1] != 1 && odlegloscKolko[a * 2] + odlegloscKolko[a * 2 + 1] <= 6)
                        {
                            skore2[0] -= Math.Pow(10, 65) + Math.Pow(10, 64) * (10 - (odlegloscKolko[a * 2] + odlegloscKolko[a * 2 + 1])); // 4.
                            skore2[1] += Math.Pow(10, 65) + Math.Pow(10, 64) * (10 - (odlegloscKolko[a * 2] + odlegloscKolko[a * 2 + 1])); // 4.
                        }
                        if (iloscKrzizkowHnedObok[a * 2] + iloscKrzizkowHnedObok[a * 2 + 1] >= 4)
                        {
                            skore2[0] += Math.Pow(10, 95); // 1.
                            skore2[1] += Math.Pow(10, 95); // 1.
                        }
                        else if (iloscKrzizkowOgolnie[a] == 3 && wolneMiyjscaNaKoncachKrzizik[a * 2] != 1 && wolneMiyjscaNaKoncachKrzizik[a * 2 + 1] != 1 && odlegloscKrzizik[a * 2] + odlegloscKrzizik[a * 2 + 1] <= 6)
                        {
                            skore2[0] += Math.Pow(10, 75) + Math.Pow(10, 74) * (10 - (odlegloscKrzizik[a * 2] + odlegloscKrzizik[a * 2 + 1])); // 3.
                            skore2[1] += Math.Pow(10, 75) + Math.Pow(10, 74) * (10 - (odlegloscKrzizik[a * 2] + odlegloscKrzizik[a * 2 + 1])); // 3.
                        }
                    }
                    if (iloscKolekOgolnie[a] == 3 || iloscKolek[2 * a] == 3 || iloscKolek[2 * a + 1] == 3)
                    {
                        if (iloscKolek[2 * a] == 3 && maxOdlegloscKolko[2 * a] == 4 && maxOdlegloscKrzizik[2 * a] == 0 && (odlegloscKolko[2 * a] == 7 || odlegloscKolko[2 * a] == 8))
                        {
                            skore2[0] -= Math.Pow(10, 34); // 7b.
                            skore2[1] += Math.Pow(10, 34); // 7b.
                        }
                        if (iloscKolek[2 * a + 1] == 3 && maxOdlegloscKolko[2 * a + 1] == 4 && maxOdlegloscKrzizik[2 * a + 1] == 0 && (odlegloscKolko[2 * a + 1] == 7 || odlegloscKolko[2 * a + 1] == 8))
                        {
                            skore2[0] -= Math.Pow(10, 34); // 7b.
                            skore2[1] += Math.Pow(10, 34); // 7b.
                        }
                        skore2[0] -= Math.Pow(10, 25) + Math.Pow(10, 24) * (10 - (odlegloscKolko[a * 2] + odlegloscKolko[a * 2 + 1])); // 8.
                        skore2[1] += Math.Pow(10, 25) + Math.Pow(10, 24) * (10 - (odlegloscKolko[a * 2] + odlegloscKolko[a * 2 + 1])); // 8.
                    }
                    if (iloscKrzizkowOgolnie[a] == 3 || iloscKrzizkow[2 * a] == 3 || iloscKrzizkow[2 * a + 1] == 3)
                    {
                        skore2[0] += Math.Pow(10, 35) + Math.Pow(10, 34) * (10 - (odlegloscKrzizik[a * 2] + odlegloscKrzizik[a * 2 + 1])); // 7.
                        skore2[1] += Math.Pow(10, 35) + Math.Pow(10, 34) * (10 - (odlegloscKrzizik[a * 2] + odlegloscKrzizik[a * 2 + 1])); // 7.
                    }
                }
                double skoreDwojekKolka = 0;
                int sumaDwojekKolka = 0;
                for (int a = 0; a < 4; a++)
                {
                    if (iloscKolekOgolnie[a] >= 2 && !(iloscKolek[2 * a] > 0 && (wolneMiyjscaNaKoncachKolko[2 * a] != 2 || maxOdlegloscKolko[2 * a] >= 4)) && !(iloscKolek[2 * a + 1] > 0 && (wolneMiyjscaNaKoncachKolko[2 * a + 1] != 2 || maxOdlegloscKolko[2 * a + 1] >= 4)))
                    {
                        skoreDwojekKolka += iloscKolekOgolnie[a] * 10 - odlegloscKolko[2 * a] - odlegloscKolko[2 * a + 1];
                        sumaDwojekKolka++;
                    }
                }
                if (sumaDwojekKolka >= 2)
                {
                    skore2[0] -= Math.Pow(10, 45) * skoreDwojekKolka; // 6.
                    skore2[1] += Math.Pow(10, 45) * skoreDwojekKolka; // 6.
                }
                double skoreDwojekKrzizki = 0;
                int sumaDwojekKrzizki = 0;
                for (int a = 0; a < 4; a++)
                {
                    if (iloscKrzizkowOgolnie[a] >= 2 && !(iloscKrzizkow[2 * a] > 0 && (wolneMiyjscaNaKoncachKrzizik[2 * a] != 2 || maxOdlegloscKrzizik[a * 2] >= 4)) && !(iloscKrzizkow[2 * a + 1] > 0 && (wolneMiyjscaNaKoncachKrzizik[2 * a + 1] != 2 || maxOdlegloscKrzizik[a * 2 + 1] >= 4)))
                    {
                        skoreDwojekKrzizki += iloscKrzizkowOgolnie[a] * 10 - odlegloscKrzizik[2 * a] - odlegloscKrzizik[2 * a + 1];
                        sumaDwojekKrzizki++;
                    }
                }
                if (sumaDwojekKrzizki >= 2)
                {
                    skore2[0] += Math.Pow(10, 55) * skoreDwojekKrzizki; // 5.
                    skore2[1] += Math.Pow(10, 55) * skoreDwojekKrzizki; // 5.
                }
                int sumaKolek = 0;
                int sumaKrzizkow = 0;
                for (int a = 0; a < 8; a++)
                {
                    if (iloscKolek[a] > 0)
                    {
                        sumaKolek += ((int)Math.Pow(iloscKolek[a], 2) * 10 - odlegloscKolko[a]) * (int)Math.Pow(wolneMiyjscaNaKoncachKolko[a], 2);
                    }
                    if (iloscKrzizkow[a] > 0)
                    {
                        sumaKrzizkow += ((int)Math.Pow(iloscKrzizkow[a], 2) * 10 - odlegloscKrzizik[a]) * (int)Math.Pow(wolneMiyjscaNaKoncachKrzizik[a], 2);
                    }
                }
                if (sumaKolek > 0)
                {
                    skore2[0] -= Math.Pow(10, 15) * sumaKolek / 2; // 10.
                    skore2[1] += Math.Pow(10, 15) * sumaKolek / 2; // 10.
                }
                if (sumaKrzizkow > 0)
                {
                    skore2[0] += Math.Pow(10, 15) * sumaKrzizkow; // 9.
                    skore2[1] += Math.Pow(10, 15) * sumaKrzizkow; // 9.
                }
            }
            double[] skore = new double[2];
            skore[0] = skore2[0];
            skore[1] = skore2[1];
            /*for (int a = 0; a < 8; a++)
            {
                if (iloscKolek[a] <= 0)
                {
                    wolneMiyjscaNaKoncachKolko[a] = 0;
                }
                if (iloscKrzizkow[a] <= 0)
                {
                    wolneMiyjscaNaKoncachKrzizik[a] = 0;
                }
            }
            for (int i = 0; i < 8; i++ )
            {
                xyIloscKolek[x, y, i] = (int)iloscKolek[i];
                xyWolneMiyjscaNaKoncachKolko[x, y, i] = wolneMiyjscaNaKoncachKolko[i];
                xyOdleglosc[x, y, i] = odlegloscKolko[i];
                if(i < 4)
                {
                    xyPusteMiyjscaOgolneKolko[x, y, i] = pusteMiyjscaOgolneKolko[i];
                }
            }*/
            return skore;
        }

        public double przirzadzSkoreMinimax(int gdoJedzie, int thread)
        {
            double skore = 0;
            for (int a = 0; a < szyrzka; a++)
            {
                for (int b = 0; b < wyszka - 4; b++)
                {
                    int iloscKolekW5 = 0;
                    int iloscKrzizkowW5 = 0;
                    for (int c = 0; c < 5; c++)
                    {
                        if (xypole[thread, a, b + c] == 1)
                        {
                            iloscKrzizkowW5++;
                            iloscKolekW5 = int.MinValue;
                        }
                        else if (xypole[thread, a, b + c] == 2)
                        {
                            iloscKolekW5++;
                            iloscKrzizkowW5 = int.MinValue;
                        }
                    }
                    if (iloscKolekW5 > 1)
                    {
                        skore += Math.Pow(100, iloscKolekW5);
                    }
                    if (iloscKrzizkowW5 > 1)
                    {
                        skore -= Math.Pow(100, iloscKrzizkowW5) * 2;
                    }
                }
            }
            for (int b = 0; b < wyszka; b++)
            {
                for (int a = 0; a < szyrzka - 4; a++)
                {
                    int iloscKolekW5 = 0;
                    int iloscKrzizkowW5 = 0;
                    for (int c = 0; c < 5; c++)
                    {
                        if (xypole[thread, a + c, b] == 1)
                        {
                            iloscKrzizkowW5++;
                            iloscKolekW5 = int.MinValue;
                        }
                        else if (xypole[thread, a + c, b] == 2)
                        {
                            iloscKolekW5++;
                            iloscKrzizkowW5 = int.MinValue;
                        }
                    }
                    if (iloscKolekW5 > 1)
                    {
                        skore += Math.Pow(100, iloscKolekW5);
                    }
                    if (iloscKrzizkowW5 > 1)
                    {
                        skore -= Math.Pow(100, iloscKrzizkowW5) * 2;
                    }
                }
            }
            for (int a = -szyrzka + 5; a < szyrzka - 4; a++)
            {
                for (int b = 0; b < wyszka - 4; b++)
                {
                    if (a + b < szyrzka - 4 && a + b >= 0 && a + b < szyrzka)
                    {
                        int iloscKolekW5 = 0;
                        int iloscKrzizkowW5 = 0;
                        for (int c = 0; c < 5; c++)
                        {
                            if (xypole[thread, a + b + c, b + c] == 1)
                            {
                                iloscKrzizkowW5++;
                                iloscKolekW5 = int.MinValue;
                            }
                            else if (xypole[thread, a + b + c, b + c] == 2)
                            {
                                iloscKolekW5++;
                                iloscKrzizkowW5 = int.MinValue;
                            }
                        }
                        if (iloscKolekW5 > 1)
                        {
                            skore += Math.Pow(100, iloscKolekW5);
                        }
                        if (iloscKrzizkowW5 > 1)
                        {
                            skore -= Math.Pow(100, iloscKrzizkowW5) * 2;
                        }
                    }
                }
            }
            for (int a = 4; a < 2 * szyrzka - 5; a++)
            {
                for (int b = 0; b < wyszka - 4; b++)
                {
                    if (a - b >= 4 && a - b >= 0 && a - b < szyrzka)
                    {
                        int iloscKolekW5 = 0;
                        int iloscKrzizkowW5 = 0;
                        for (int c = 0; c < 5; c++)
                        {
                            if (xypole[thread, a - (b + c), b + c] == 1)
                            {
                                iloscKrzizkowW5++;
                                iloscKolekW5 = int.MinValue;
                            }
                            else if (xypole[thread, a - (b + c), b + c] == 2)
                            {
                                iloscKolekW5++;
                                iloscKrzizkowW5 = int.MinValue;
                            }
                        }
                        if (iloscKolekW5 > 1)
                        {
                            skore += Math.Pow(100, iloscKolekW5);
                        }
                        if (iloscKrzizkowW5 > 1)
                        {
                            skore -= Math.Pow(100, iloscKrzizkowW5) * 2;
                        }
                    }
                }
            }
            return skore;
        }

        public double alfabeta(int depth2, int maximizingPlayer/*2 - kolko, 1 - krzizik*/,
                            int xpole, int ypole, double alfa, double beta, int iloscTahow, int poczontkowyLewelO, int poczontkowyLewelX, int thread, int cTah)
        {
            if (depth2 == 0 || (stopwatches[thread].ElapsedMilliseconds > 15000 && depth - depth2 >= 7) || hasWon)
            {
                return przirzadzSkoreAlfabeta3(thread);
            }
            else if (maximizingPlayer == 2)
            {
                int lewo = 1;
                int prawo = 13;
                int gorno = 1;
                int spodnio = 13;
                lewo = zmjynLewo1(thread);
                prawo = zmjynPrawo1(thread);
                gorno = zmjynGorno1(thread);
                spodnio = zmjynSpodnio1(thread);
                double v = double.NegativeInfinity;
                int nejLewel = int.MinValue;
                double[,] tahi = new double[25, 4];
                int lewel = 0;
                for (int c = 0; c < iloscTahow; c++)
                {
                    tahi[c, 2] = double.NegativeInfinity;
                }
                for (int a = lewo - 1; a <= prawo + 1; a++)
                {
                    for (int b = gorno - 1; b <= spodnio + 1; b++)
                    {
                        if (a >= 0 && a < szyrzka && b >= 0 && b < wyszka && xypole[thread, a, b] == 0)
                        {
                            double[] arrSkore = przirzadzSkoreAlfabeta4(maximizingPlayer, a, b, thread);
                            double skore = arrSkore[1];
                            double skore2 = arrSkore[0];
                            lewel = lewelSkore(0, skore2);
                            if (lewel > nejLewel)
                            {
                                nejLewel = lewel;
                            }
                            for (int c = 0; c < iloscTahow; c++)
                            {
                                if (skore > tahi[c, 2])
                                {
                                    for (int d = iloscTahow - 1; d > c; d--)
                                    {
                                        tahi[d, 0] = tahi[d - 1, 0];
                                        tahi[d, 1] = tahi[d - 1, 1];
                                        tahi[d, 2] = tahi[d - 1, 2];
                                        tahi[d, 3] = tahi[d - 1, 3];
                                    }
                                    tahi[c, 0] = a;
                                    tahi[c, 1] = b;
                                    tahi[c, 2] = skore;
                                    tahi[c, 3] = lewel;
                                    break;
                                }
                            }
                        }
                    }
                }
                if (iloscTahow == 0)
                {
                    return nejLewel;
                }
                else if (nejLewel == 10)
                {
                    iloscTahow = 1;
                }
                else if (nejLewel == 9)
                {
                    iloscTahow = 2;
                }
                else if (nejLewel >= 7)
                {
                    for (int c = 0; c < iloscTahow; c++)
                    {
                        if (tahi[c, 2] > double.NegativeInfinity)
                        {
                            if (tahi[c, 3] < 4)
                            {
                                tahi[c, 2] = double.NegativeInfinity;
                            }
                        }
                    }
                    int nowoIloscTahow = 0;
                    for (int c = 0; c < iloscTahow; c++)
                    {
                        if (tahi[c, 2] > double.NegativeInfinity)
                        {
                            nowoIloscTahow++;
                        }
                    }
                    iloscTahow = nowoIloscTahow;
                }
                else if (nejLewel >= 3 || (tah <= 8 && depth2 >= depth - 6))
                {
                    for (int c = 0; c < iloscTahow; c++)
                    {
                        if (tahi[c, 2] > double.NegativeInfinity)
                        {
                            if (tahi[c, 3] < 2)
                            {
                                tahi[c, 2] = double.NegativeInfinity;
                            }
                        }
                    }
                    int nowoIloscTahow = 0;
                    for (int c = 0; c < iloscTahow; c++)
                    {
                        if (tahi[c, 2] > double.NegativeInfinity)
                        {
                            nowoIloscTahow++;
                        }
                    }
                    iloscTahow = nowoIloscTahow;
                }
                else
                {
                    return nejLewel;
                }
                for (int c = 0; c < iloscTahow; c++)
                {
                    if (tahi[c, 2] > double.NegativeInfinity)
                    {
                        int a = (int)tahi[c, 0];
                        int b = (int)tahi[c, 1];
                        xypole[thread, a, b] = 2;
                        /*this.Invoke((MethodInvoker)delegate
                        {
                            namaluj(thread, true);
                            pictureBox1.Image = bmp;
                            pictureBox1.Update();
                        });
                        System.Threading.Thread.Sleep(1500);*/
                        if (moO5(thread))
                        {
                            xypole[thread, a, b] = 0;
                            return double.PositiveInfinity;
                        }
                        v = Math.Max(v, alfabeta(depth2 - 1, 1, a, b, alfa, beta, iloscTahow >= 1 || iloscTahow <= 6 ? 7 : iloscTahow, poczontkowyLewelO, poczontkowyLewelX, thread, cTah));
                        alfa = Math.Max(alfa, v);
                        xypole[thread, a, b] = 0;
                        if (beta <= alfa)
                        {
                            return v;// beta cut-off 
                        }
                    }
                }
                return v;
            }
            else
            {
                int lewo = 1;
                int prawo = 13;
                int gorno = 1;
                int spodnio = 13;
                lewo = zmjynLewo1(thread);
                prawo = zmjynPrawo1(thread);
                gorno = zmjynGorno1(thread);
                spodnio = zmjynSpodnio1(thread);
                int lewel = 0;
                double v = double.PositiveInfinity;
                int nejLewel = int.MinValue;
                double[,] tahi = new double[25, 4];
                for (int c = 0; c < iloscTahow; c++)
                {
                    tahi[c, 2] = double.NegativeInfinity;
                }
                for (int a = lewo - 1; a <= prawo + 1; a++)
                {
                    for (int b = gorno - 1; b <= spodnio + 1; b++)
                    {
                        if (a >= 0 && a < szyrzka && b >= 0 && b < wyszka && xypole[thread, a, b] == 0)
                        {
                            double[] arrSkore = przirzadzSkoreAlfabeta4(maximizingPlayer, a, b, thread);
                            double skore = arrSkore[1];
                            double skore2 = arrSkore[0];
                            lewel = lewelSkore(0, skore2);
                            if (lewel > nejLewel)
                            {
                                nejLewel = lewel;
                            }
                            for (int c = 0; c < iloscTahow; c++)
                            {
                                if (skore > tahi[c, 2])
                                {
                                    for (int d = iloscTahow - 1; d > c; d--)
                                    {
                                        tahi[d, 0] = tahi[d - 1, 0];
                                        tahi[d, 1] = tahi[d - 1, 1];
                                        tahi[d, 2] = tahi[d - 1, 2];
                                        tahi[d, 3] = tahi[d - 1, 3];
                                    }
                                    tahi[c, 0] = a;
                                    tahi[c, 1] = b;
                                    tahi[c, 2] = skore;
                                    tahi[c, 3] = lewel;
                                    break;
                                }
                            }
                        }
                    }
                }
                if (iloscTahow == 0)
                {
                    return nejLewel;
                }
                else if (nejLewel == 10)
                {
                    iloscTahow = 1;
                }
                else if (nejLewel == 9)
                {
                    iloscTahow = 2;
                }
                else if (nejLewel >= 7)
                {
                    for (int c = 0; c < iloscTahow; c++)
                    {
                        if (tahi[c, 2] > double.NegativeInfinity)
                        {
                            if (tahi[c, 3] < 4)
                            {
                                tahi[c, 2] = double.NegativeInfinity;
                            }
                        }
                    }
                    int nowoIloscTahow = 0;
                    for (int c = 0; c < iloscTahow; c++)
                    {
                        if (tahi[c, 2] > double.NegativeInfinity)
                        {
                            nowoIloscTahow++;
                        }
                    }
                    iloscTahow = nowoIloscTahow;
                }
                else if (nejLewel >= 3 || (tah >= 4 && tah <= 8 && depth2 >= depth - 6))
                {
                    for (int c = 0; c < iloscTahow; c++)
                    {
                        if (tahi[c, 2] > double.NegativeInfinity)
                        {
                            if (tahi[c, 3] < 2)
                            {
                                tahi[c, 2] = double.NegativeInfinity;
                            }
                        }
                    }
                    int nowoIloscTahow = 0;
                    for (int c = 0; c < iloscTahow; c++)
                    {
                        if (tahi[c, 2] > double.NegativeInfinity)
                        {
                            nowoIloscTahow++;
                        }
                    }
                    iloscTahow = nowoIloscTahow;
                }
                else
                {
                    return nejLewel;
                }

                for (int c = 0; c < iloscTahow; c++)
                {
                    int a = (int)tahi[c, 0];
                    int b = (int)tahi[c, 1];
                    xypole[thread, a, b] = 1;
                    /*this.Invoke((MethodInvoker)delegate
                    {
                        namaluj(thread, true);
                        pictureBox1.Image = bmp;
                        pictureBox1.Update();
                    });
                    System.Threading.Thread.Sleep(1500);*/
                    if (moX5(thread))
                    {
                        xypole[thread, a, b] = 0;
                        return double.NegativeInfinity;
                    }
                    v = Math.Min(v, alfabeta(depth2 - 1, 2, a, b, alfa, beta, iloscTahow >= 1 || iloscTahow <= 6 ? 7 : iloscTahow, poczontkowyLewelO, poczontkowyLewelX, thread, cTah));
                    beta = Math.Min(beta, v);
                    xypole[thread, a, b] = 0;
                    if (beta <= alfa)
                    {
                        return v; ; // alfa cut-off 
                    }
                }
                return v;
            }
        }

        public double minimax2(int depth2, int maximizingPlayer/*2 - kolko, 1 - krzizik*/,
                            int xpole, int ypole, int thread)
        {
            if (depth2 == 0)
            {
                return przirzadzSkoreAlfabeta(maximizingPlayer, xpole, ypole, thread);
            }
            else if (maximizingPlayer == 2)
            {
                double bestvalue = double.NegativeInfinity;
                int lewo = 1;
                int prawo = 13;
                int gorno = 1;
                int spodnio = 13;
                lewo = zmjynLewo1(thread);
                prawo = zmjynPrawo1(thread);
                gorno = zmjynGorno1(thread);
                spodnio = zmjynSpodnio1(thread);
                int iloscTahow = 5;
                double[,] tahi = new double[iloscTahow, 3];
                for (int c = 0; c < iloscTahow; c++)
                {
                    tahi[c, 2] = double.NegativeInfinity;
                }
                for (int a = lewo - 1; a <= prawo + 1; a++)
                {
                    for (int b = gorno - 1; b <= spodnio + 1; b++)
                    {
                        if (a >= 0 && a < szyrzka && b >= 0 && b < wyszka && xypole[thread, a, b] == 0)
                        {
                            double skore = przirzadzSkoreAlfabeta(maximizingPlayer, xpole, ypole, thread);
                            for (int c = 0; c < iloscTahow; c++)
                            {
                                if (skore > tahi[c, 2])
                                {
                                    for (int d = iloscTahow - 1; d > c; d--)
                                    {
                                        tahi[d, 0] = tahi[d - 1, 0];
                                        tahi[d, 1] = tahi[d - 1, 1];
                                        tahi[d, 2] = tahi[d - 1, 2];
                                    }
                                    tahi[c, 0] = a;
                                    tahi[c, 1] = b;
                                    tahi[c, 2] = skore;
                                    break;
                                }
                            }
                        }
                    }
                }
                for (int c = 0; c < iloscTahow; c++)
                {
                    if (tahi[c, 2] > double.NegativeInfinity)
                    {
                        int a = (int)tahi[c, 0];
                        int b = (int)tahi[c, 1];
                        if (depth2 > 1)
                        {
                            xypole[thread, a, b] = 2;
                        }
                        double bestp = bestvalue;
                        bestvalue = Math.Max(bestvalue, minimax(depth2 - 1, depth2 == 1 ? 2 : 1, a, b, thread));
                        if (depth2 > 1)
                        {
                            xypole[thread, a, b] = 0;
                        }
                    }
                }
            return bestvalue;
            }
            else
            {
                double bestvalue = double.NegativeInfinity;
                int lewo = 1;
                int prawo = 13;
                int gorno = 1;
                int spodnio = 13;
                lewo = zmjynLewo1(thread);
                prawo = zmjynPrawo1(thread);
                gorno = zmjynGorno1(thread);
                spodnio = zmjynSpodnio1(thread);
                int iloscTahow = 5;
                double[,] tahi = new double[iloscTahow, 3];
                for (int c = 0; c < iloscTahow; c++)
                {
                    tahi[c, 2] = double.NegativeInfinity;
                }
                for (int a = lewo - 1; a <= prawo + 1; a++)
                {
                    for (int b = gorno - 1; b <= spodnio + 1; b++)
                    {
                        if (a >= 0 && a < szyrzka && b >= 0 && b < wyszka && xypole[thread, a, b] == 0)
                        {
                            double skore = przirzadzSkoreAlfabeta(maximizingPlayer, xpole, ypole, thread);
                            for (int c = 0; c < iloscTahow; c++)
                            {
                                if (skore > tahi[c, 2])
                                {
                                    for (int d = iloscTahow - 1; d > c; d--)
                                    {
                                        tahi[d, 0] = tahi[d - 1, 0];
                                        tahi[d, 1] = tahi[d - 1, 1];
                                        tahi[d, 2] = tahi[d - 1, 2];
                                    }
                                    tahi[c, 0] = a;
                                    tahi[c, 1] = b;
                                    tahi[c, 2] = skore;
                                    break;
                                }
                            }
                        }
                    }
                }
                for (int c = 0; c < iloscTahow; c++)
                {
                    if (tahi[c, 2] > double.NegativeInfinity)
                    {
                        int a = (int)tahi[c, 0];
                        int b = (int)tahi[c, 1];
                        if (depth2 > 1)
                        {
                            xypole[thread, a, b] = 1;
                        }
                        double bestp = bestvalue;
                        bestvalue = Math.Max(bestvalue, minimax(depth2 - 1, depth2 == 1 ? 1 : 2, a, b, thread));
                        if (depth2 > 1)
                        {
                            xypole[thread, a, b] = 0;
                        }
                    }
                }
            return bestvalue;
            }
        }

        public double minimax(int depth2, int maximizingPlayer/*2 - kolko, 1 - krzizik*/,
                            int xpole, int ypole, int thread)
        {
            if (depth2 == 0)
            {
                return przirzadzSkoreMinimax(maximizingPlayer, thread);
            }
            else if (maximizingPlayer == 2)
            {
                double bestvalue = double.NegativeInfinity;
                int lewo = 1;
                int prawo = 13;
                int gorno = 1;
                int spodnio = 13;
                lewo = zmjynLewo1(thread);
                prawo = zmjynPrawo1(thread);
                gorno = zmjynGorno1(thread);
                spodnio = zmjynSpodnio1(thread);
                for (int a = lewo - 1; a <= prawo + 1; a++)
                {
                    for (int b = gorno - 1; b <= spodnio + 1; b++)
                    {
                        if (a >= 0 && a < szyrzka && b >= 0 && b < wyszka && xypole[thread, a, b] == 0 && ((a - 1 > 0 && b - 1 > 0 && xypole[thread, a - 1, b - 1] != 0) || (a - 1 > 0 && xypole[thread, a - 1, b] != 0) || (a - 1 > 0 && b + 1 < wyszka && xypole[thread, a - 1, b + 1] != 0) || (b - 1 > 0 && xypole[thread, a, b - 1] != 0) || (b + 1 < wyszka && xypole[thread, a, b + 1] != 0) || (a + 1 < szyrzka && b - 1 > 0 && xypole[thread, a + 1, b - 1] != 0) || (a + 1 < szyrzka && xypole[thread, a + 1, b] != 0) || (a + 1 < szyrzka && b + 1 < wyszka && xypole[thread, a + 1, b + 1] != 0)))
                        {
                            xypole[thread, a, b] = 2;
                            double val = minimax(depth2 - 1, 1, a, b, thread);
                            bestvalue = Math.Max(bestvalue, val);
                            xypole[thread, a, b] = 0;
                        }
                    }
                }
                return bestvalue;
            }
            else
            {
                double bestvalue = double.PositiveInfinity;
                int lewo = 1;
                int prawo = 13;
                int gorno = 1;
                int spodnio = 13;
                lewo = zmjynLewo1(thread);
                prawo = zmjynPrawo1(thread);
                gorno = zmjynGorno1(thread);
                spodnio = zmjynSpodnio1(thread);
                for (int a = lewo - 1; a <= prawo + 1; a++)
                {
                    for (int b = gorno - 1; b <= spodnio + 1; b++)
                    {
                        if (a >= 0 && a < szyrzka && b >= 0 && b < wyszka && xypole[thread, a, b] == 0 && ((a - 1 > 0 && b - 1 > 0 && xypole[thread, a - 1, b - 1] != 0) || (a - 1 > 0 && xypole[thread, a - 1, b] != 0) || (a - 1 > 0 && b + 1 < wyszka && xypole[thread, a - 1, b + 1] != 0) || (b - 1 > 0 && xypole[thread, a, b - 1] != 0) || (b + 1 < wyszka && xypole[thread, a, b + 1] != 0) || (a + 1 < szyrzka && b - 1 > 0 && xypole[thread, a + 1, b - 1] != 0) || (a + 1 < szyrzka && xypole[thread, a + 1, b] != 0) || (a + 1 < szyrzka && b + 1 < wyszka && xypole[thread, a + 1, b + 1] != 0)))
                        {
                            xypole[thread, a, b] = 1;
                            double val = minimax(depth2 - 1, 2, a, b, thread);
                            bestvalue = Math.Min(bestvalue, val);
                            xypole[thread, a, b] = 0;
                        }
                    }
                }
                return bestvalue;
            }
        }

        public bool openbook(int thread)
        {
            bool znalezione = false;
            int lewo = 1;
            int prawo = 13;
            int gorno = 1;
            int spodnio = 13;
            lewo = zmjynLewo1(thread);
            prawo = zmjynPrawo1(thread);
            gorno = zmjynGorno1(thread);
            spodnio = zmjynSpodnio1(thread);
            int iloscX = 0;
            int iloscO = 0;
            for (int a = lewo; a <= prawo; a++)
            {
                for (int b = gorno; b <= spodnio; b++)
                {
                    if (a >= 0 && a < szyrzka && b >= 0 && b < wyszka)
                    {
                        if (xypole[thread, a, b] == 1)
                        {
                            iloscX++;
                        }
                        else if (xypole[thread, a, b] == 2)
                        {
                            iloscO++;
                        }
                    }
                }
            }
            if(iloscX == 4 && iloscO == 3 && prawo - lewo == 4 && spodnio - gorno == 2)
            {
                if(xypole[thread, lewo + 0, gorno + 0] == 1 && 
                   xypole[thread, lewo + 1, gorno + 0] == 0 && 
                   xypole[thread, lewo + 2, gorno + 0] == 2 && 
                   xypole[thread, lewo + 3, gorno + 0] == 0 && 
                   xypole[thread, lewo + 4, gorno + 0] == 1 && 

                   xypole[thread, lewo + 0, gorno + 1] == 0 && 
                   xypole[thread, lewo + 1, gorno + 1] == 2 && 
                   xypole[thread, lewo + 2, gorno + 1] == 1 &&
                   xypole[thread, lewo + 3, gorno + 1] == 1 && 
                   xypole[thread, lewo + 4, gorno + 1] == 0 &&
                    
                   xypole[thread, lewo + 0, gorno + 2] == 0 && 
                   xypole[thread, lewo + 1, gorno + 2] == 0 && 
                   xypole[thread, lewo + 2, gorno + 2] == 2 && 
                   xypole[thread, lewo + 3, gorno + 2] == 0 && 
                   xypole[thread, lewo + 4, gorno + 2] == 0)
                {
                    for (int t = 0; t < numOfThreads; t++)
                    {
                        xypole[t, lewo + 1, gorno + 2] = 2;
                    }
                    znalezione = true;
                }
            }
            else if (iloscX == 3 && iloscO == 2 && prawo - lewo == 2 && spodnio - gorno == 3 && prawo != 14)
            {
                if(xypole[thread, lewo + 0, gorno + 0] == 0 && 
                   xypole[thread, lewo + 1, gorno + 0] == 1 && 
                   xypole[thread, lewo + 2, gorno + 0] == 0 &&

                   xypole[thread, lewo + 0, gorno + 1] == 2 && 
                   xypole[thread, lewo + 1, gorno + 1] == 0 && 
                   xypole[thread, lewo + 2, gorno + 1] == 1 &&
                    
                   xypole[thread, lewo + 0, gorno + 2] == 0 && 
                   xypole[thread, lewo + 1, gorno + 2] == 1 && 
                   xypole[thread, lewo + 2, gorno + 2] == 0 &&

                   xypole[thread, lewo + 0, gorno + 3] == 2 && 
                   xypole[thread, lewo + 1, gorno + 3] == 0 && 
                   xypole[thread, lewo + 2, gorno + 3] == 0)
                {
                    for (int t = 0; t < numOfThreads; t++)
                    {
                        xypole[t, lewo + 3, gorno + 2] = 2;
                    }
                    znalezione = true;
                }
            }
            else if (iloscX == 1 && iloscO == 0 && prawo - lewo == 0 && spodnio - gorno == 0)
            {
                if(xypole[thread, lewo + 0, gorno + 0] == 1)
                {
                    if(lewo < 7)
                    {
                        if (gorno < 7)
                        {
                            if(lewo < gorno)
                            {
                                for (int t = 0; t < numOfThreads; t++)
                                {
                                    xypole[t, lewo + 1, gorno + 0] = 2;
                                }
                            }
                            else
                            {
                                for (int t = 0; t < numOfThreads; t++)
                                {
                                    xypole[t, lewo + 0, gorno + 1] = 2;
                                }
                            }
                        }
                        else if (gorno > 7)
                        {
                            if (lewo < 14 - gorno)
                            {
                                for (int t = 0; t < numOfThreads; t++)
                                {
                                    xypole[t, lewo + 1, gorno + 0] = 2;
                                }
                            }
                            else
                            {
                                for (int t = 0; t < numOfThreads; t++)
                                {
                                    xypole[t, lewo + 0, gorno - 1] = 2;
                                }
                            }
                        }
                        else
                        {
                            for (int t = 0; t < numOfThreads; t++)
                            {
                                xypole[t, lewo + 1, gorno + 0] = 2;
                            }
                        }
                    }
                    else if(lewo > 7)
                    {
                        if (gorno < 7)
                        {
                            if (14 - lewo < gorno)
                            {
                                for (int t = 0; t < numOfThreads; t++)
                                {
                                    xypole[t, lewo - 1, gorno + 0] = 2;
                                }
                            }
                            else
                            {
                                for (int t = 0; t < numOfThreads; t++)
                                {
                                    xypole[t, lewo + 0, gorno + 1] = 2;
                                }
                            }
                        }
                        else if (gorno > 7)
                        {
                            if (14 - lewo < 14 - gorno)
                            {
                                for (int t = 0; t < numOfThreads; t++)
                                {
                                    xypole[t, lewo - 1, gorno + 0] = 2;
                                }
                            }
                            else
                            {
                                for (int t = 0; t < numOfThreads; t++)
                                {
                                    xypole[t, lewo + 0, gorno - 1] = 2;
                                }
                            }
                        }
                        else
                        {
                            for (int t = 0; t < numOfThreads; t++)
                            {
                                xypole[t, lewo - 1, gorno + 0] = 2;
                            }
                        }
                    }
                    else
                    {
                        if(gorno <= 7)
                        {
                            for (int t = 0; t < numOfThreads; t++)
                            {
                                xypole[t, lewo + 0, gorno + 1] = 2;
                            }
                        }
                        else
                        {
                            for (int t = 0; t < numOfThreads; t++)
                            {
                                xypole[t, lewo + 0, gorno - 1] = 2;
                            }
                        }
                    }
                }
                znalezione = true;
            }

            return znalezione;
        }

        public void resetStatystyk()
        {
            for(int x = 0; x < szyrzka; x++)
            {
                for(int y = 0; y < wyszka; y++)
                {
                    for(int i = 0; i < 8; i++)
                    {
                        xyWolneMiyjscaNaKoncachKolko[x, y, i] = 0;
                    }
                }
            }
        }

        public void zapiszDoTxt()
        {
            File.WriteAllText("C:\\xox\\xypole.txt", String.Empty);
            using (System.IO.StreamWriter file2 = new System.IO.StreamWriter("C:\\xox\\xypole.txt", true))
            {
                for (int b = 0; b < wyszka; b++)
                {
                    for (int a = 0; a < szyrzka; a++)
                    {
                        file2.Write(xypole[0, a, b]);
                    }
                    file2.WriteLine();
                }
                file2.WriteLine();
                file2.Close();
            }
        }

        public void nacztiZTxt()
        {
            System.IO.StreamReader file = new System.IO.StreamReader("C:\\xox\\xypole.txt");
            for(int y = 0; y < wyszka; y++)
            {
                string line = file.ReadLine();
                for (int x = 0; x < szyrzka; x++)
                {
                    for (int t = 0; t < numOfThreads; t++)
                    {
                        xypole[t, x, y] = Convert.ToInt32(line[x]) - 48;
                        
                    }
                }
                
            }
            file.Close();
            namaluj(0, true);
            pictureBox1.Image = bmp;
            pictureBox1.Update();
            pictureBox1.Show();
        }

        public void koniec(int gdoWygrol)
        {
            //zapiszDoTxt();
            namaluj(0, false);
            pictureBox1.Image = bmp;
            if (graPrzebiego)
            {
                if (gdoWygrol == 1)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        label2.Text = "Wygrałeś";
                        label2.Update();
                    });
                    wygraneX++;
                    gdoWygrolOstatniRoz = 1;
                }
                else
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        label2.Text = "Przegrałeś";
                        label2.Update();
                    });
                    wygraneO++;
                    gdoWygrolOstatniRoz = 2;
                }
            }
            graPrzebiego = false;
            this.Invoke((MethodInvoker)delegate
            {
                label4.Text = wygraneX.ToString() + " : " + wygraneO.ToString();
                button1.Enabled = true;
            });        
        }

        public void wygrolO(int thread)
        {
            for (int a = 0; a <= szyrzka - 5; a++)
            {
                for (int b = 0; b <= wyszka - 5; b++)
                {
                    if (xypole[thread, a, b] == 2 && xypole[thread, a + 1, b + 1] == 2 && xypole[thread, a + 2, b + 2] == 2 && xypole[thread, a + 3, b + 3] == 2 && xypole[thread, a + 4, b + 4] == 2 && graPrzebiego)
                    {
                        g.Clear(Color.White);
                        namalujzielonomkostke(a, b);
                        namalujzielonomkostke(a + 1, b + 1);
                        namalujzielonomkostke(a + 2, b + 2);
                        namalujzielonomkostke(a + 3, b + 3);
                        namalujzielonomkostke(a + 4, b + 4);
                        koniec(2);
                    }
                    else if (xypole[thread, a + 4, b] == 2 && xypole[thread, a + 3, b + 1] == 2 && xypole[thread, a + 2, b + 2] == 2 && xypole[thread, a + 1, b + 3] == 2 && xypole[thread, a, b + 4] == 2 && graPrzebiego)
                    {
                        g.Clear(Color.White);
                        namalujzielonomkostke(a + 4, b);
                        namalujzielonomkostke(a + 3, b + 1);
                        namalujzielonomkostke(a + 2, b + 2);
                        namalujzielonomkostke(a + 1, b + 3);
                        namalujzielonomkostke(a, b + 4);
                        koniec(2);
                    }
                }
            }
            for (int a = 0; a <= szyrzka - 1; a++)
            {
                for (int b = 0; b <= wyszka - 5; b++)
                {
                    if (xypole[thread, a, b] == 2 && xypole[thread, a, b + 1] == 2 && xypole[thread, a, b + 2] == 2 && xypole[thread, a, b + 3] == 2 && xypole[thread, a, b + 4] == 2 && graPrzebiego)
                    {
                        g.Clear(Color.White);
                        namalujzielonomkostke(a, b);
                        namalujzielonomkostke(a, b + 1);
                        namalujzielonomkostke(a, b + 2);
                        namalujzielonomkostke(a, b + 3);
                        namalujzielonomkostke(a, b + 4);
                        koniec(2);
                    }
                }
            }
            for (int a = 0; a <= szyrzka - 5; a++)
            {
                for (int b = 0; b <= wyszka - 1; b++)
                {
                    if (xypole[thread, a, b] == 2 && xypole[thread, a + 1, b] == 2 && xypole[thread, a + 2, b] == 2 && xypole[thread, a + 3, b] == 2 && xypole[thread, a + 4, b] == 2 && graPrzebiego)
                    {
                        g.Clear(Color.White);
                        namalujzielonomkostke(a, b);
                        namalujzielonomkostke(a + 1, b);
                        namalujzielonomkostke(a + 2, b);
                        namalujzielonomkostke(a + 3, b);
                        namalujzielonomkostke(a + 4, b);
                        koniec(2);
                    }
                }
            }
        }

        public void wygrolX(int thread)
        {
            for (int a = 0; a <= szyrzka - 5; a++)
            {
                for (int b = 0; b <= wyszka - 5; b++)
                {
                    if (xypole[thread, a, b] == 1 && xypole[thread, a + 1, b + 1] == 1 && xypole[thread, a + 2, b + 2] == 1 && xypole[thread, a + 3, b + 3] == 1 && xypole[thread, a + 4, b + 4] == 1 && graPrzebiego)
                    {
                        g.Clear(Color.White);
                        namalujzielonomkostke(a, b);
                        namalujzielonomkostke(a + 1, b + 1);
                        namalujzielonomkostke(a + 2, b + 2);
                        namalujzielonomkostke(a + 3, b + 3);
                        namalujzielonomkostke(a + 4, b + 4);
                        koniec(1);
                    }
                    else if (xypole[thread, a + 4, b] == 1 && xypole[thread, a + 3, b + 1] == 1 && xypole[thread, a + 2, b + 2] == 1 && xypole[thread, a + 1, b + 3] == 1 && xypole[thread, a, b + 4] == 1 && graPrzebiego)
                    {
                        g.Clear(Color.White);
                        namalujzielonomkostke(a + 4, b);
                        namalujzielonomkostke(a + 3, b + 1);
                        namalujzielonomkostke(a + 2, b + 2);
                        namalujzielonomkostke(a + 1, b + 3);
                        namalujzielonomkostke(a, b + 4);
                        koniec(1);
                    }
                }
            }
            for (int a = 0; a <= szyrzka - 1; a++)
            {
                for (int b = 0; b <= wyszka - 5; b++)
                {
                    if (xypole[thread, a, b] == 1 && xypole[thread, a, b + 1] == 1 && xypole[thread, a, b + 2] == 1 && xypole[thread, a, b + 3] == 1 && xypole[thread, a, b + 4] == 1 && graPrzebiego)
                    {
                        g.Clear(Color.White);
                        namalujzielonomkostke(a, b);
                        namalujzielonomkostke(a, b + 1);
                        namalujzielonomkostke(a, b + 2);
                        namalujzielonomkostke(a, b + 3);
                        namalujzielonomkostke(a, b + 4);
                        koniec(1);
                    }
                }
            }
            for (int a = 0; a <= szyrzka - 5; a++)
            {
                for (int b = 0; b <= wyszka - 1; b++)
                {
                    if (xypole[thread, a, b] == 1 && xypole[thread, a + 1, b] == 1 && xypole[thread, a + 2, b] == 1 && xypole[thread, a + 3, b] == 1 && xypole[thread, a + 4, b] == 1 && graPrzebiego)
                    {
                        g.Clear(Color.White);
                        namalujzielonomkostke(a, b);
                        namalujzielonomkostke(a + 1, b);
                        namalujzielonomkostke(a + 2, b);
                        namalujzielonomkostke(a + 3, b);
                        namalujzielonomkostke(a + 4, b);
                        koniec(1);
                    }
                }
            }
        }

        public bool moO5(int thread)
        {
            for (int a = 0; a <= szyrzka - 5; a++)
            {
                for (int b = 0; b <= wyszka - 5; b++)
                {
                    if (xypole[thread, a, b] == 2 && xypole[thread, a + 1, b + 1] == 2 && xypole[thread, a + 2, b + 2] == 2 && xypole[thread, a + 3, b + 3] == 2 && xypole[thread, a + 4, b + 4] == 2)
                    {
                        return true;
                    }
                    else if (xypole[thread, a + 4, b] == 2 && xypole[thread, a + 3, b + 1] == 2 && xypole[thread, a + 2, b + 2] == 2 && xypole[thread, a + 1, b + 3] == 2 && xypole[thread, a, b + 4] == 2)
                    {
                        return true;
                    }
                }
            }
            for (int a = 0; a <= szyrzka - 1; a++)
            {
                for (int b = 0; b <= wyszka - 5; b++)
                {
                    if (xypole[thread, a, b] == 2 && xypole[thread, a, b + 1] == 2 && xypole[thread, a, b + 2] == 2 && xypole[thread, a, b + 3] == 2 && xypole[thread, a, b + 4] == 2)
                    {
                        return true;
                    }
                }
            }
            for (int a = 0; a <= szyrzka - 5; a++)
            {
                for (int b = 0; b <= wyszka - 1; b++)
                {
                    if (xypole[thread, a, b] == 2 && xypole[thread, a + 1, b] == 2 && xypole[thread, a + 2, b] == 2 && xypole[thread, a + 3, b] == 2 && xypole[thread, a + 4, b] == 2)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool moX5(int thread)
        {
            for (int a = 0; a <= szyrzka - 5; a++)
            {
                for (int b = 0; b <= wyszka - 5; b++)
                {
                    if (xypole[thread, a, b] == 1 && xypole[thread, a + 1, b + 1] == 1 && xypole[thread, a + 2, b + 2] == 1 && xypole[thread, a + 3, b + 3] == 1 && xypole[thread, a + 4, b + 4] == 1)
                    {
                        return true;
                    }
                    else if (xypole[thread, a + 4, b] == 1 && xypole[thread, a + 3, b + 1] == 1 && xypole[thread, a + 2, b + 2] == 1 && xypole[thread, a + 1, b + 3] == 1 && xypole[thread, a, b + 4] == 1)
                    {
                        return true;
                    }
                }
            }
            for (int a = 0; a <= szyrzka - 1; a++)
            {
                for (int b = 0; b <= wyszka - 5; b++)
                {
                    if (xypole[thread, a, b] == 1 && xypole[thread, a, b + 1] == 1 && xypole[thread, a, b + 2] == 1 && xypole[thread, a, b + 3] == 1 && xypole[thread, a, b + 4] == 1)
                    {
                        return true;
                    }
                }
            }
            for (int a = 0; a <= szyrzka - 5; a++)
            {
                for (int b = 0; b <= wyszka - 1; b++)
                {
                    if (xypole[thread, a, b] == 1 && xypole[thread, a + 1, b] == 1 && xypole[thread, a + 2, b] == 1 && xypole[thread, a + 3, b] == 1 && xypole[thread, a + 4, b] == 1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void poczontekGry()
        {
            timer1.Stop();
            rachuje = false;
            szyrzkapb = pictureBox1.Width;
            wyszkapb = pictureBox1.Height;
            bmp = new Bitmap(szyrzkapb, wyszkapb);
            g = Graphics.FromImage(bmp);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(Color.White);
            if(gdoWygrolOstatniRoz == 1)
            {
                for (int t = 0; t < numOfThreads; t++)
                {
                    xypole[t, szyrzka / 2, wyszka / 2] = 2;
                }
            }
            for (int t = 0; t < numOfThreads; t++)
            {
                stopwatches[t] = new Stopwatch();
            }
            label2.Text = "Krzyżyk";
            namaluj(0, true);
            pictureBox1.Image = bmp;
            threadGrej = new Thread(nacztiZTxt);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            poczontekGry();
        }

        public void GrejThread(int thread, int iloscTahow, double[,] tahi, int poczontkowyLewel)
        {
            double wartosc = 0;
            for (int c = thread; c < iloscTahow; c += numOfThreads)
            {
                if (tahi[c, 2] > double.NegativeInfinity)
                {
                    int a = (int)tahi[c, 0];
                    int b = (int)tahi[c, 1];
                    this.Invoke((MethodInvoker)delegate
                    {
                        label7.Text = "Nic :(";
                    });
                    namalujSilnomKostke(a, b, 0);
                    this.Invoke((MethodInvoker)delegate
                    {
                        pictureBox1.Image = bmp;
                        pictureBox1.Show();
                        pictureBox1.Update();
                    });
                    if (depth > 0)
                    {
                        xypole[thread, a, b] = 2;
                    }
                    if (iloscTahow == 1)
                    {
                        if (depth > 0)
                        {
                            xypole[thread, a, b] = 0;
                        }
                        xpoleTah[c] = a;
                        ypoleTah[c] = b;
                        break;
                    }
                    iloscTahowAlfabety = 12;
                    stopwatches[thread].Reset();
                    stopwatches[thread].Start();
                    wartosc = alfabeta(depth, depth == 0 ? 2 : 1, a, b, double.NegativeInfinity, double.PositiveInfinity, iloscTahowAlfabety, poczontkowyLewel, int.MinValue, thread, c);
                    stopwatches[thread].Stop();
                    xpoleTah[c] = a;
                    ypoleTah[c] = b;
                    wartoscTah[c] = wartosc;
                    xyskore[a, b] = wartosc;
                    
                    this.Invoke((MethodInvoker)delegate
                    {
                        namalujSilnomKostke(a, b, wartosc);
                        pictureBox1.Image = bmp;
                        pictureBox1.Show();
                        pictureBox1.Update();
                    });
                    if (depth > 0)
                    {
                        xypole[thread, a, b] = 0;
                    }
                    if (wartosc >= 99)
                    {
                        hasWon = true;
                        wartosc = 99;
                        wartoscTah[c] = wartosc;
                        break;
                    }
                }
            }
        }

        public void Grej(int xpole, int ypole)
        {
            this.Invoke((MethodInvoker)delegate
            {
                progressBar1.Value = 0;
                progressBar1.Update();
            });
            // let AI place the O/X on the board
            if (xypole[0, xpole, ypole] == 0 && graPrzebiego && checkBox1.Checked == true)
            {
                //initializing
                for (int t = 0; t < numOfThreads; t++)
                {
                    xypole[t, xpole, ypole] = 1;
                }
                tah++;
                this.Invoke((MethodInvoker)delegate
                {
                    textBox6.Text = tah.ToString();
                    label2.Text = "Kółko";
                    label2.Update();
                });
                ostatniTah[0] = xpole;
                ostatniTah[1] = ypole;
                
                namaluj(0, true);
                this.Invoke((MethodInvoker)delegate
                {
                    pictureBox1.Image = bmp;
                    pictureBox1.Show();
                    pictureBox1.Update();
                });
                wygrolX(0);
                int nejxpole = -99;
                int nejypole = -99;
                int iloscTahow = 12;
                xpoleTah = new int[iloscTahow];
                ypoleTah = new int[iloscTahow];
                for (int t = 0; t < iloscTahow; t++)
                {
                    xpoleTah[t] = -99;
                    ypoleTah[t] = -99;
                }
                wartoscTah = new double[iloscTahow];
                for (int t = 0; t < iloscTahow; t++)
                {
                    wartoscTah[t] = double.NegativeInfinity;
                }
                int[] dalszy = new int[2];
                if (graPrzebiego)
                {
                    //initializing lewy, prawy, gorny, dolny
                    int lewo = 1;
                    int prawo = 13;
                    int gorno = 1;
                    int spodnio = 13;
                    lewo = zmjynLewo1(0);
                    prawo = zmjynPrawo1(0);
                    gorno = zmjynGorno1(0);
                    spodnio = zmjynSpodnio1(0);
                    if (openbook(0))
                    {
                        g.Clear(Color.White);
                        tah++;
                        this.Invoke((MethodInvoker)delegate
                        {
                            textBox6.Text = tah.ToString();
                            label2.Text = "Krzyżyk";
                            label2.Update();
                        });
                    }
                    else
                    {
                        //getting moves to analyze
                        int lewel = 0;
                        int nejLewel = int.MinValue;
                        int iloscNejLewlow = 0;
                        resetStatystyk();
                        double[,] tahi = new double[iloscTahow, 4];
                        for (int c = 0; c < iloscTahow; c++)
                        {
                            tahi[c, 2] = double.NegativeInfinity;
                        }
                        for (int a = lewo - (tah == 1 ? 1 : 2); a <= prawo + (tah == 1 ? 1 : 2); a++)
                        {
                            for (int b = gorno - (tah == 1 ? 1 : 2); b <= spodnio + (tah == 1 ? 1 : 2); b++)
                            {
                                if (a >= 0 && a < szyrzka && b >= 0 && b < wyszka && xypole[0, a, b] == 0)
                                {
                                    double[] arrSkore = przirzadzSkoreAlfabeta4(2, a, b, 0);
                                    double skore = arrSkore[1];
                                    double skore2 = arrSkore[0];
                                    lewel = lewelSkore(0, skore2);
                                    xyLewel[a, b] = lewel;
                                    if (lewel > nejLewel)
                                    {
                                        nejLewel = lewel;
                                        iloscNejLewlow = 1;
                                    }
                                    else if (lewel == nejLewel)
                                    {
                                        iloscNejLewlow++;
                                    }
                                    for (int c = 0; c < iloscTahow; c++)
                                    {
                                        if (skore > tahi[c, 2])
                                        {
                                            for (int d = iloscTahow - 1; d > c; d--)
                                            {
                                                tahi[d, 0] = tahi[d - 1, 0];
                                                tahi[d, 1] = tahi[d - 1, 1];
                                                tahi[d, 2] = tahi[d - 1, 2];
                                                tahi[d, 3] = tahi[d - 1, 3];
                                            }
                                            tahi[c, 0] = a;
                                            tahi[c, 1] = b;
                                            tahi[c, 2] = skore;
                                            tahi[c, 3] = lewel;
                                            break;
                                        }
                                    }
                                }
                                else if (a >= 0 && a < szyrzka && b >= 0 && b < wyszka)
                                {
                                    xyskore[a, b] = 0;
                                    xyLewel[a, b] = 0;
                                }
                            }
                        }
                        namaluj(0, true);
                        this.Invoke((MethodInvoker)delegate
                        {
                            pictureBox1.Image = bmp;
                            pictureBox1.Show();
                            pictureBox1.Update();
                        });
                        this.Invoke((MethodInvoker)delegate
                        {
                            label6.Text = "";
                            label8.Text = "";
                        });
                        int poczontkowyLewel = nejLewel;
                        if (nejLewel == 10)
                        {
                            iloscTahow = 1;
                        }
                        else if (nejLewel == 9)
                        {
                            iloscTahow = 1;
                        }
                        else if (nejLewel >= 7)
                        {
                            for (int c = 0; c < iloscTahow; c++)
                            {
                                if (tahi[c, 2] > double.NegativeInfinity)
                                {
                                    if (tahi[c, 3] < 4)
                                    {
                                        tahi[c, 2] = double.NegativeInfinity;
                                    }
                                }
                            }
                            int nowoIloscTahow = 0;
                            for (int c = 0; c < iloscTahow; c++)
                            {
                                if (tahi[c, 2] > double.NegativeInfinity)
                                {
                                    nowoIloscTahow++;
                                }
                            }
                            iloscTahow = nowoIloscTahow;
                        }
                        else if (nejLewel >= 5)
                        {
                            for (int c = 0; c < iloscTahow; c++)
                            {
                                if (tahi[c, 2] > double.NegativeInfinity)
                                {
                                    if (tahi[c, 3] < 2)
                                    {
                                        tahi[c, 2] = double.NegativeInfinity;
                                    }
                                }
                            }
                            int nowoIloscTahow = 0;
                            for (int c = 0; c < iloscTahow; c++)
                            {
                                if (tahi[c, 2] > double.NegativeInfinity)
                                {
                                    nowoIloscTahow++;
                                }
                            }
                            iloscTahow = nowoIloscTahow;
                        }
                        this.Invoke((MethodInvoker)delegate
                        {
                            for (int c = 0; c < iloscTahow; c++)
                            {
                                label6.Text += (tahi[c, 0] + 1) + ", " + (tahi[c, 1] + 1) + ", " + tahi[c, 2] + "\n";
                            }

                            label8.Text = iloscTahow.ToString();
                            label8.Update();
                        });
                        hasWon = false;
                        workers[0] = new Thread(() => GrejThread(0, iloscTahow, tahi, poczontkowyLewel));
                        workers[1] = new Thread(() => GrejThread(1, iloscTahow, tahi, poczontkowyLewel));
                        workers[2] = new Thread(() => GrejThread(2, iloscTahow, tahi, poczontkowyLewel));
                        workers[3] = new Thread(() => GrejThread(3, iloscTahow, tahi, poczontkowyLewel));
                        workers[4] = new Thread(() => GrejThread(4, iloscTahow, tahi, poczontkowyLewel));
                        workers[5] = new Thread(() => GrejThread(5, iloscTahow, tahi, poczontkowyLewel));
                        workers[6] = new Thread(() => GrejThread(6, iloscTahow, tahi, poczontkowyLewel));
                        workers[7] = new Thread(() => GrejThread(7, iloscTahow, tahi, poczontkowyLewel));
                        workers[8] = new Thread(() => GrejThread(8, iloscTahow, tahi, poczontkowyLewel));
                        workers[9] = new Thread(() => GrejThread(9, iloscTahow, tahi, poczontkowyLewel));
                        workers[10] = new Thread(() => GrejThread(10, iloscTahow, tahi, poczontkowyLewel));
                        workers[11] = new Thread(() => GrejThread(11, iloscTahow, tahi, poczontkowyLewel));
                        for (int t = 0; t < numOfThreads; t++)
                        {
                            workers[t].Start();
                        }
                        while(true)
                        {
                            bool canContinue = true;
                            for (int t = 0; t < numOfThreads && canContinue; t++)
                            {
                                if(workers[t].IsAlive)
                                {
                                    canContinue = false;
                                }
                            }
                            if(canContinue)
                            {
                                break;
                            }
                            else
                            {
                                Thread.Sleep(1);
                            }
                        }
                        resetStatystyk();
                        int nejThread = 0;
                        double nejThreadwartosc = double.NegativeInfinity;
                        for (int t = 0; t < iloscTahow; t++)
                        {
                            if (wartoscTah[t] > nejThreadwartosc)
                            {
                                nejThreadwartosc = wartoscTah[t];
                                nejThread = t;
                            }
                        }
                        nejxpole = xpoleTah[nejThread];
                        nejypole = ypoleTah[nejThread];
                        this.Invoke((MethodInvoker)delegate
                        {
                            textBox3.Text = Convert.ToString(nejThreadwartosc);
                            textBox4.Text = Convert.ToString(nejxpole + 1);
                            textBox5.Text = Convert.ToString(nejypole + 1);
                        });
                        
                        for (int t = 0; t < numOfThreads; t++)
                        {
                            if (xypole[t, nejxpole, nejypole] == 0)
                            {
                                xypole[t, nejxpole, nejypole] = 2;
                            }
                        }
                        ostatniTah[0] = nejxpole;
                        ostatniTah[1] = nejypole;
                        g.Clear(Color.White);
                        tah++;
                        this.Invoke((MethodInvoker)delegate
                        {
                            textBox6.Text = tah.ToString();
                            label2.Text = "Krzyżyk";
                            label2.Update();
                        });
                    }
                }
            }
            // place O/X on the board
            else if (xypole[0, xpole, ypole] == 0 && graPrzebiego)
            {
                if (tah % 2 == 0)
                {
                    for (int t = 0; t < numOfThreads; t++)
                    {
                        xypole[t, xpole, ypole] = 1;
                    }
                }
                else
                {
                    for (int t = 0; t < numOfThreads; t++)
                    {
                        xypole[t, xpole, ypole] = 2;
                    }
                }
                ostatniTah[0] = xpole;
                ostatniTah[1] = ypole;
                tah++;
            }
            timer1.Stop();
            if (graPrzebiego)
            {
                rachuje = false;
                namaluj(0, true);
                wygrolX(0);
                wygrolO(0);
                this.Invoke((MethodInvoker)delegate
                {
                    pictureBox1.Image = bmp;
                    pictureBox1.Update();
                    pictureBox1.Show();
                });
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (!rachuje)
            {
                rachuje = true;
                labelTime.Text = "";
                labelTime.Update();
                time = 0;
                timer1.Start();
                double xx = e.X;
                double yy = e.Y;
                textBox1.Text = Convert.ToString(xx);
                textBox2.Text = Convert.ToString(yy);
                int xpole = Convert.ToInt32(Math.Floor(Convert.ToDecimal(xx / szyrzkapb * szyrzka)));
                int ypole = Convert.ToInt32(Math.Floor(Convert.ToDecimal(yy / wyszkapb * wyszka)));
                threadGrej = new Thread(() => Grej(xpole, ypole));
                threadGrej.Start();
            }
            if(numericUpDown1.Enabled)
            {
                depth = (int)numericUpDown1.Value;
                numericUpDown1.Enabled = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            for (int a = 0; a < szyrzka; a++)
            {
                for (int b = 0; b < wyszka; b++)
                {
                    for (int t = 0; t < numOfThreads; t++)
                    {
                        xypole[t, a, b] = 0;
                    }
                    xyskore[a, b] = 0;
                    xyLewel[a, b] = 0;
                }
            }
            tah = 0;
            graPrzebiego = true;
            poczontekGry();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (threadGrej.IsAlive)
            {
                for (int t = 0; t < numOfThreads; t++)
                {
                    workers[t].Abort();
                }
            }
            threadGrej.Abort();
            Thread.Sleep(150);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            zapiszDoTxt();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            nacztiZTxt();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            time++;
            labelTime.Text = (time).ToString() + " s";
            labelTime.Update();
        }

    }
}
