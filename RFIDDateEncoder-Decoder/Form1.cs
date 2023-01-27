using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DateEncDec
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        String gsZeros = "0000000000";
        Color gcBackColor = Color.White;

        // Date types
        // This code is here for reference only.
        // It is the index valuae for DSGTIN+ date type.
        String[] DateTypes =
        {
            "11",    // Production
            "13",    // Packaging
            "15",    // Best before
            "16",    // Sell by
            "17",    // Expiration
            "7006",  // 1st freeze
            "7007"   // Harvest
        };


        void ClearErrors()
        {
            cxRaw.BackColor = gcBackColor;
            cxEncoded.BackColor = gcBackColor;
            txErrors.BackColor = gcBackColor;
        }

        String EncodeDate(String sdate, out String errOut)
        {
            int result;
            String sDateX, hexOut;

            errOut = "";
            if (sdate.Length < 6)
            {
                errOut = "Invalid date format. (YYMMDD, Month 1..12)";
                return "Error";    // Warn msg
            }
            if (int.TryParse(sdate, out result))
            {
                if (sdate.Length == 6)
                {
                    int iyear = Convert.ToInt32(sdate.Substring(0, 2), 10);
                    int imonth = Convert.ToInt32(sdate.Substring(2, 2), 10);
                    if ((imonth < 1) || (imonth > 12))
                    {
                        errOut = "Invalid date format. (YYMMDD, Month 1..12)";
                        return "Error";
                    }
                    int iday = Convert.ToInt32(sdate.Substring(4, 2), 10);
                    switch (imonth)
                    {
                        case 1:
                        case 3:
                        case 5:
                        case 7:
                        case 8:
                        case 10:
                        case 12:
                            if ((iday < 1) || (iday > 31))
                            {
                                cxRaw.BackColor = Color.Red;
                                errOut = "Invalid date format. (YYMMDD, Day 1..31)";
                                return "Error";
                            }
                            break;
                        case 4:
                        case 6:
                        case 9:
                        case 11:
                            if ((iday < 1) || (iday > 30))
                            {
                                cxRaw.BackColor = Color.Red;
                                errOut = "Invalid date format. (YYMMDD, Day 1..30)";
                                return "Error";
                            }
                            break;
                        case 2:
                            if ((iday < 1) || (iday > 29))
                            {
                                cxRaw.BackColor = Color.Red;
                                errOut = "Invalid date format. (YYMMDD, Day 1..29)";
                                return "Error";
                            }
                            break;
                        default:
                            cxRaw.BackColor = Color.Red;
                            errOut = "Invalid date format. (YYMMDD)";    // Should never get here
                            return "Error";
                    }
                    int ipackeddate = 0x200 * iyear + 0x20 * imonth + iday;
                    sDateX = ipackeddate.ToString("X");
                    hexOut = gsZeros.Substring(0, 4 - sDateX.Length) + sDateX;
                }
                else
                {
                    cxRaw.BackColor = Color.Red;
                    errOut = "Invalid date format. (YYMMDD)";
                    return "Error";
                }
            }
            else
            {
                cxRaw.BackColor = Color.Red;
                errOut = "Invalid date format. (YYMMDD, all numbers)";
                return "Error";
            }
            return hexOut;
        }

        String DecodeDate(String hdate,out String errOut)
        // Convert binary date to yymmdd
        // hdate - input hex starting with 4-nibble date
        // remOut - remainder of string less date
        // errOut - error message (if any)
        {
            int date, year, month, day;
            String str;

            errOut = "";
            if (hdate.Length < 4)
            {
                errOut = "Date hex too short.";
                return "Error";
            }
            date = Convert.ToInt32(hdate.Substring(0, 4), 16);
            year = date / 0x200;
            month = (date & 0x1E0) / 0x20;
            day = date & 0x1F;
            if (year > 99)
            {
                errOut = "Invalid year format. (YYMMDD, Year 0..99)";
                return "Error";
            }
            str = year.ToString();
            if ((month < 1) || (month > 12))
            {
                errOut = "Invalid month format. (YYMMDD, Month 1..12)";
                return "Error";
            }
            if (month > 9)
            {
                str = str + month.ToString();
            }
            else
            {
                str = str + "0" + month.ToString();
            }
            if ((day < 1) || (day > 31))
            {
                errOut = "Invalid day format. (YYMMDD, Day 1..31)";
                return "Error";
            }
            if (day > 9)
            {
                str = str + day.ToString();
            }
            else
            {
                str = str + "0" + day.ToString();
            }
            return str;
        }

        private void bnEncode_Click(object sender, EventArgs e)
        {
            String str, errOut;

            ClearErrors();
            str = EncodeDate(cxRaw.Text, out errOut);
            if (errOut != "")
                txErrors.AppendText(errOut + Environment.NewLine);
            cxEncoded.Text = str;
        }

        private void bnDecode_Click(object sender, EventArgs e)
        {
            String str, errOut;

            ClearErrors();
            str = DecodeDate(cxEncoded.Text, out errOut);
            if (errOut != "")
                txErrors.AppendText(errOut + Environment.NewLine);
            cxRaw.Text = str;
        }

        private void cxRaw_SelectedIndexChanged(object sender, EventArgs e)
        {
            String str, errOut;

            ClearErrors();
            str = EncodeDate(cxRaw.Text, out errOut);
            if (errOut != "")
                txErrors.AppendText(errOut + Environment.NewLine);
            cxEncoded.Text = str;
        }

        private void cxEncoded_SelectedIndexChanged(object sender, EventArgs e)
        {
            String str, errOut;

            ClearErrors();
            str = DecodeDate(cxEncoded.Text, out errOut);
            cxRaw.Text = str;
        }

        private void cxRaw_Click(object sender, EventArgs e)
        {
            ClearErrors();
        }

        private void cxEncoded_Click(object sender, EventArgs e)
        {
            ClearErrors();
        }
    }
}
