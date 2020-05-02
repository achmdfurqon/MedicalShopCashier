using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalShop.Models
{
    public class ChartJS
    {
        public string[] labels { get; set; }
        public Dataset[] datasets { get; set; } = new Dataset[1];
        public ChartJS() { }
        public ChartJS(int i)
        {
            labels = new string[i];
            datasets[0] = new Dataset(i);
        }
    }

    public class Dataset
    {
        public int[] data { get; set; }
        public string[] backgroundColor { get; set; }
        public Dataset() { }
        public Dataset(int i)
        {
            Random random = new Random();
            data = new int[i];
            backgroundColor = new string[i];
            for(var j = 0; j < i; j++)
            {
                backgroundColor[j] = String.Format("#{0:X6}", random.Next(0x1000000));
            }
        }
    }
}
