namespace Ofc.LZMA.Common
{
    using System.Collections;
    using System.Collections.Generic;

    public class SwitchResult
    {
        public bool ThereIs;
        public bool WithMinus;
        public List<object> PostStrings = new List<object>();
        public int PostCharIndex;
        public SwitchResult()
        {
            ThereIs = false;
        }
    }
}