using System;
using System.Collections.Generic;
using System.Text;

namespace CBasicModel
{
    public class BooleanItem
    {
        public int byteAdr;
        public int bitAdr;
        Boolean state;
        public Boolean force_state;

        public BooleanItem(int byteA, int bitA, Boolean s)
        {
            byteAdr = byteA;
            bitAdr = bitA;
            state = s;
        }

        public BooleanItem(int byteA, int bitA)
        {
            byteAdr = byteA;
            bitAdr = bitA;
        }

        public void setState(Boolean b)
        {
            if (force_state) state = true;
            else state = b;
        }

        public void forceState(Boolean b)
        {
            force_state = b;
            state = b;
        }

        public Boolean getState()
        {
            return state;
        }
        /*
        public String ToString()
        {
            return "Boolean Item (" + byteAdr + "." + bitAdr + ")=" + state;
        }
         */
    }
}
