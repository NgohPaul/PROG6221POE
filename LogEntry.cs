using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberBotWPF
{
    internal class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return $"{Timestamp:yyyy-MM-dd HH:mm} - {Description}";
        }
    }
}
