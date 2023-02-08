using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scanner.Structures
{
    public class CassieMessage
    {
        public string CassieText { get; set; } = string.Empty;
        public string CaptionText { get; set; } = string.Empty;

        public CassieMessage(string cassieText, string captionText)
        {
            CassieText = cassieText;
            CaptionText = captionText;
        }
    }
}
