using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobTest
{
    class SyncRecordList
    {
        private int count;
        public int Count
        {
            private set
            {
                count = names.Count;
            }
            get
            {
                return count;
            }
        }
        public List<string> names;
        public List<int> keys;
        public SyncRecordList()
        {
            names = new List<string>();
            keys = new List<int>();
        }

        public void AddRecord(string name, int key)
        {
            names.Add(name);
            keys.Add(key);
        }

        public void RemoveAt(int index)
        {
            names.RemoveAt(index);
            keys.RemoveAt(index);
        }
    }
}
