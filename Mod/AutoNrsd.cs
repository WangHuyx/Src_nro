using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRO
{
    public class AutoNrsd : IActionListener
    {
        private static AutoNrsd _Instance;
        public static AutoNrsd getInstance()
        {
            if (_Instance == null)
            {
                _Instance = new AutoNrsd();
            }
            return _Instance;
        }
        public void perform(int idAction, object p)
        {
            switch (idAction)
            {
                case 1:
                    break;
                default:
                    GameCanvas.startOKDlg("Chức năng chưa được cài đặt!");
                    break;
            }
        }
    }
}
