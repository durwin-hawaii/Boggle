using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Boggle.Models
{
    public class Word
    {
        [JsonIgnore]
        public string original { get; set; }
        public string random { get; set; }
        public string guess { get; set; }
        public int width { get; set; }
        public string cell { get; set; }
        public string answers { get; set; } = "";
        public string errors { get; set; } = "";
        public int errors_count { get; set; } = 0;
        public int answers_count { get; set; } = 0;
        public int count { get; set; } = 4;

        public string alert { get; set; }

        private static Random rand = new Random();

        public Word()
        {

        }
        public Word (string word)
        {
            original = word;
            width = 100 / original.Length;
            Scramble();
        }
        private void Scramble()
        {
            var list = new SortedList<int, char>();
            foreach (var c in original)
                list.Add(rand.Next(), c);
            random = new string(list.Values.ToArray());
        }
    }
}
