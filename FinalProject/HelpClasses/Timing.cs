using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class Timing
    {
        //0
        public double hh;
        //00
        public double mm;
        public double ss;
        public double ff;

        public double midff;
        public double midss;
        public double midmm;
        public double midhh;

        public int mid1;
        public int mid2;

        public Timing()
        {
            hh = 0;
            mm = 0;
            ss = 0;
            ff = 0.0;
        }



        public bool plus(double num)
        {
            mid1 = (int)num;
            mid2 = (int)((num - mid1) * 1000);
            //check if we pass the ff amount
            midff = (mid2 + this.ff);
            if ((int)(midff / 1000) == 1)
            {
                this.ff = midff % 1000;


                midss = (mid1 + 1 + this.ss);
                if ((int)midss / 60 == 1)
                {
                    this.ss = midss % 60;


                    midmm = (1 + this.mm);
                    if ((int)midmm / 60 == 1)
                    {
                        this.mm = midmm % 60;

                        midhh = (1 + this.hh);
                        if ((int)midhh / 10 == 1)
                        {
                            return false;
                        }
                        else
                        {
                            this.hh += 1;
                        }
                    }
                    else
                    {
                        this.mm += 1;
                    }

                }
                else
                {
                    this.ss = midss;
                }
            }
            else
            {
                this.ff = this.ff + mid2;
                midss = (this.ss + mid1);
                if ((int)(midss / 60) == 1)
                {
                    this.mm += 1;
                    this.ss = midss % 60;
                }
                else
                {
                    this.ss = midss;
                }
                //בדיקת חיבור עבור midss
            }

            return true;
        }

        public void copyTime(Timing t1)
        {
            this.hh = t1.hh;
            this.mm = t1.mm;
            this.ss = t1.ss;
            this.ff = t1.ff;
        }

        public string printTime()
        {
            int hh = hhToInt(this);
            int mm = mmToInt(this);
            int ss = ssToInt(this);
            int ff = ffToInt(this);

            this.ff = (double)ff;


            string mmPrint = mm.ToString();
            string ssPrint = ss.ToString();
            string ffPrint = ff.ToString();
            //if (hh / 10 == 0)
            //{
            //    mmPrint = "0" + mm.ToString();
            //}
            if (mm / 10 == 0)
            {
                mmPrint = "0" + mm.ToString();
            }

            if (ss / 10 == 0)
            {
                ssPrint = "0" + ss.ToString();
            }
            if (ff / 100 == 0)
            {
                ffPrint = "0" + ff.ToString() + "0";
            }
            return hh.ToString() + ":" + mmPrint + ":" + ssPrint + "," + ffPrint;
        }
        public int ffToInt(Timing t)
        {

            return (int)t.ff;

        }
        public int ssToInt(Timing t)
        {
            return (int)t.ss;

        }
        public int mmToInt(Timing t)
        {
            return (int)t.mm;

        }
        public int hhToInt(Timing t)
        {
            return (int)t.hh;

        }
    }
}
