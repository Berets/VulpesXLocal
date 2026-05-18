using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Ufp
{
    public partial class ANACERT
    {
        public bool anapeobbBool
        {
            get
            {
                return anapeobb == "S";
            }
            set
            {
                if (value)
                    anapeobb = "S";
                else
                    anapeobb = "N";
            }

        }
    }
}
