using System;
using System.Collections.Generic;
using System.Text;

namespace CBasicModel
{
    interface PlcListener
    {
        void stateChanged(String state);

        void outputChanged(BooleanItem bi);

        void lostConnection(String s);
    }
}
