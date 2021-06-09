using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.UMLS.Classes
{
    public class RankKey : IEquatable<RankKey>
    {
        public string SAB { get; set; }
        public string TTY { get; set; }

        public override int GetHashCode()
        {
            return 17 * SAB.GetHashCode() + TTY.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as RankKey);
        }
        public bool Equals(RankKey obj)
        {
            return this.SAB == obj.SAB && this.TTY == obj.TTY;
        }
    }
}
