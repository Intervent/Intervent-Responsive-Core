using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClaimDataAnalytics.Claims.LayoutTransformer
{
    public abstract class BaseFixedLayoutTransformer
    {
        protected abstract int[] Positions { get; }

        protected abstract bool IsInsert { get; }
        public void Transform(string sourceFilePath, string destinationFilePath)
        {
            string line;
            char delimiter = '|';
            if (!IsInsert)
            {
                using (StreamReader sr = new StreamReader(sourceFilePath))
                using (StreamWriter sw = File.AppendText(destinationFilePath))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        var cArray = line.ToCharArray();
                        foreach (int i in Positions)
                        {
                            cArray[i - 1] = delimiter;
                        }

                        sw.WriteLine(new string(cArray));
                    }
                }
            }
            else
            {

                using (StreamReader sr = new StreamReader(sourceFilePath))
                using (StreamWriter sw = File.AppendText(destinationFilePath))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        var cArray = line.ToCharArray();
                        var totalLength = cArray.Length + Positions.Length;
                        char[] buffer = new char[totalLength];
                        int index = 0;
                        int bufferIndex = 0;
                        foreach (int i in Positions)
                        {
                            while (index <= (i - 1))
                            {
                                buffer[bufferIndex++] = cArray[index++];
                            }
                            buffer[bufferIndex++] = delimiter;
                        }

                        sw.WriteLine(new string(buffer));
                    }
                }
            }
        }
    }
}
