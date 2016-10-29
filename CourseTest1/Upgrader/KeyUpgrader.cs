namespace CourseTest1
{
    class KeyUpgrader : IUpgrade
    {
        private bool Shift;
        public string Upgrade(string sym)
        {
            string temp = "";
            if (sym == "OemPeriod")
                temp = ".";
            else if (sym == "Oemplus")
                temp = "+";
            else if (sym == "OemMinus")
                temp = "-";
            else if (sym == "LButton" || sym == "Enter")
                temp = "mark";
            else if ((sym.Contains("D") || sym.Contains("NumPad")) && sym.Length > 1)
            {
                if (sym == "D2" && Shift)
                    temp = "@";
                else if (!Shift)
                    temp = sym[sym.Length - 1].ToString();
            }
            else if (sym.Length == 1 && char.IsLetter(sym[0]))
                if (!Shift)
                    temp = sym.ToLower();
                else
                    temp = sym;
            if (sym == "LShiftKey" || sym == "RShiftKey")
                Shift = true;
            else
                Shift = false;
            return temp;
        }
    }
}