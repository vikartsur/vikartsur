using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Practical_task_ruby_play
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hi! Follow carefully the instructions.");

            // basic value input
            Console.WriteLine("Enter an integer to define a basic value: ");
            var inputBasVal = Console.ReadLine();
            int basVal;
            while (!int.TryParse(inputBasVal, out basVal))
            {
                //Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error occured!");
                //Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("Please, check the value you have entered: \n 1) Integer \n 2) No special symbols (except \"+\" and \"-\")");
                inputBasVal = Console.ReadLine();
            }

            // tolerance value input
            Console.WriteLine("Enter a positive non-integer to define a tolerance value ");
            var inputTolVal = Console.ReadLine();
            double tolVal;
            while (!double.TryParse(inputTolVal, out tolVal) || tolVal <= 0) //integers are 
            {
                Console.WriteLine("Error occured!");
                Console.WriteLine("Please, check the value you have entered: \n 1) Non-integer \n 2) Positive \n 3) No special symbols (except \"+\" and \".\")");
                inputTolVal = Console.ReadLine();
            }

            // temporary values for fluctuations
            var dotPosition = inputTolVal.IndexOf('.');
            var elemAfterDot = inputTolVal.Length - dotPosition - 1;
            int newTolVal;
            int divisor;
            if(elemAfterDot >= 5)
            {
                divisor = Convert.ToInt32(Math.Pow(10, elemAfterDot));
                newTolVal = Convert.ToInt32(tolVal * divisor);    
            }
            else
            {
                divisor = Convert.ToInt32(Math.Pow(10, 4));
                newTolVal = Convert.ToInt32(tolVal * divisor);
            }
            /*Console.WriteLine(newTolVal);
            Console.WriteLine(divisor);
            Console.WriteLine(newTolVal / Convert.ToDouble(divisor));
            */
            
            // size of array input
            Console.WriteLine("Enter the positive integer to define the size of an array: ");
            var inputNumberElem = Console.ReadLine();
            int numberElem;

            while (!int.TryParse(inputNumberElem, out numberElem) || numberElem <= 0)
            {
                Console.WriteLine("Error occured!");
                Console.WriteLine("Please, check the value you have entered: \n 1) Integer \n 2) Positive ( > 0) \n 3) No special symbols (except \"+\")");
                inputNumberElem = Console.ReadLine();
            }

            // check the info

            // creating an array
            double[] array = new double[numberElem];

            // creating an array of different signs of noise-values
            int[] sigma = new int[numberElem];
            for (int i = 0; i < numberElem; i++)
            {
                Random rnd = new Random();
                sigma[i] = Convert.ToInt32(rnd.NextDouble());
                //Console.WriteLine(sigma[i]);
            }

            // creating an array of noise
            double[] noise = new double[numberElem];
            for (int i = 0; i < numberElem; i++)
            {
                Random rnd = new Random();
                if (sigma[i] == 1)
                {
                    noise[i] = rnd.Next(newTolVal) / Convert.ToDouble(divisor);
                }
                else
                {
                    noise[i] = -1*rnd.Next(newTolVal) / Convert.ToDouble(divisor);
                }
                array[i] = basVal + noise[i];
                //Console.WriteLine(noise[i]);
                //Console.WriteLine(array[i]);
            }

            // 
            var minArray = array[0];
            var maxArray = array[0];
            for (int i = 0; i < numberElem; i++)
            {
                if (array[i] < minArray) minArray = array[i];
                if (array[i] > maxArray) maxArray = array[i];
            }
            var tolVal2 = (maxArray - minArray)/10;
            //Console.WriteLine(tolVal2);
            // the limits of a interval are (-tolVal2; tolVal2)

            // filtration
            int counter = 0;
            double sumArray = 0;
            double[] filtVal = new double[numberElem];

            for (int i = 0; i < numberElem; i++)
            {
                if (array[i] >= (basVal + (-1 * tolVal2)) && array[i] <= (basVal + tolVal2))
                {
                    counter++;
                    filtVal[i] = array[i];
                }
                else filtVal[i] = 0;
                sumArray += array[i];
            }

            Console.WriteLine("The number of filtered values = " + counter);
            double percentage = Convert.ToDouble(counter) / numberElem * 100;
            Console.WriteLine("This is the " + percentage + "% of total number of values in the array.");

            // mathematical expected value
            var mathExpVal = sumArray / numberElem;
            Console.WriteLine("The mathematical expected value is " + mathExpVal);

            // shifted (n <= 30) and non-shifted (n > 30) dispersion
            double disp = 0;
            for (int i = 0; i < numberElem; i++)
            {
                if (numberElem <= 30) disp += Math.Pow(numberElem, -1) * Math.Pow(array[i] - mathExpVal, 2);
                else disp += (1.0 / (numberElem - 1)) * Math.Pow(array[i] - mathExpVal, 2);
            }
            Console.WriteLine("The dispersion = " + disp);

            // standart deviation
            var stanDev = Math.Sqrt(disp);
            Console.WriteLine("The standart deviation = " + stanDev);

            // calculating frequency 
            Dictionary<double, int> freqValues= new Dictionary<double, int>();
            for (int i = 0; i < numberElem; i++)
            {
                if (filtVal[i] != 0)
                {
                    var counter3 = 0;
                    var temp = filtVal[i];
                    for (int j = 0; j < numberElem; j++)
                    {
                        if (temp == filtVal[j]) counter3++;
                    }
                    if (freqValues.ContainsKey(temp) == false) freqValues.Add(temp, counter3);
                }
            }
            
            // calculating probability 
            // I use written by myself the Laplace function
            var prob1 = Laplace((basVal + tolVal2 - mathExpVal) / stanDev);
            var prob2 = Laplace((basVal - tolVal2 - mathExpVal) / stanDev);
            var prob = prob1 - prob2;
            Console.WriteLine("The probability of hitting into the interval = " + prob + "\n");

            // Creating a report
            string path = @"C:\Practicaltask\Report.txt";
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine("Report");
                    sw.WriteLine("The basic value = " + basVal);
                    sw.WriteLine("The tolerance value = " + tolVal);
                    sw.WriteLine("The number of elements in the array = " + numberElem);
                    sw.WriteLine("The generated array:");
                    for (int i = 0; i < numberElem; i++)
                    {
                        sw.WriteLine(array[i]);
                    }
                    sw.WriteLine(" ");
                    sw.WriteLine("The filtered array:");
                    foreach (var value in filtVal)
                    {
                        if (value != 0) sw.WriteLine(value);
                    }

                    sw.WriteLine("The lower bound of a range = ");
                    sw.WriteLine(basVal + (-1 * tolVal2));
                    sw.WriteLine("The upper bound of a range = ");
                    sw.WriteLine(basVal + tolVal2);
                    sw.WriteLine(" ");
                    sw.WriteLine("The mathematical expected value = " + mathExpVal);
                    sw.WriteLine("The statistical dispersion = " + disp);
                    sw.WriteLine("The standart deviation = " + stanDev);
                    sw.WriteLine("The probability of hitting within range = " + prob); 
                    sw.WriteLine("The frequence of hitting within range = "); 
                    foreach (var item in freqValues) sw.WriteLine("Value = {0}, Frequency = {1}", item.Key, item.Value);
                    sw.WriteLine("Deia Karunyk, @karunyk_deia");
                    sw.WriteLine("Write for any questions. Have a nice day!");
                }
            }

            // Additional Task*

            // values of the Laplace function for given confidence values
            string[] confPer = new string[] { "50%", "75%", "80%", "95%" };
            double[] lapVal = new double[] { 0.68, 1.15, 1.29, 1.96 }; // 50%, 75%, 80%, 95% confidence values

            // calculating confidence intervals
            double[] lowBoundCon = new double[4];
            double[] upperBoundCon = new double[4];

            for (int i = 0; i < 4; i++)
            {
                var counter2 = 0;
                lowBoundCon[i] = mathExpVal - (lapVal[i] * Math.Sqrt(disp) / Math.Sqrt(numberElem));
                upperBoundCon[i] = mathExpVal + (lapVal[i] * Math.Sqrt(disp) / Math.Sqrt(numberElem));
                for (int j = 0; j < numberElem; j++)
                {
                    if (array[j] > lowBoundCon[i] && array[j] < upperBoundCon[i]) counter2++;
                }

                Console.WriteLine("The interval for " + confPer[i] + " confidence level is = (" + lowBoundCon[i] + "; " + upperBoundCon[i] + ")");
                Console.WriteLine("The Hit rate for interval = " + lapVal[i]);
                Console.WriteLine("The number of elements that fit into interval = " + counter2 + " or " + Convert.ToDouble(counter2) / numberElem * 100 + "%.\n");
            }

            // Additional task**
            //Console.WriteLine(Laplace(3.6));

            // The Laplace Direct Integral function
            static double Laplace(double x)
            {
                double result = 0.5 * (erf(x / Math.Sqrt(2)));
                return result;
            }

            // The Laplace Invert Integral function
            static double LaplaceInv(double p)
            {
                double result = Math.Sqrt(2) * erfInv(2 * p);
                return result;
            }

            // The ERF function
            static double erf(double x)
            {
                double t1 = 0.254829592;
                double t2 = -0.284496736;
                double t3 = 1.421413741;
                double t4 = -1.453152027;
                double t5 = 1.061405429;
                double t6 = 0.3275911;
                int sigma = 1;
                if (x < 0) sigma = -1;
                x = Math.Abs(x);
                double t = 1.0 / (1.0 + t6 * x);
                double y = 1.0 - (((((t5 * t + t4) * t) + t3) * t + t2) * t + t1) * t * Math.Exp(-x * x);

                return sigma * y;
            }

            // The inverse ERF function
            static double erfInv(double x)
            {
                double tt1, tt2, lnx, sgn;
                sgn = (x < 0) ? -1.0f : 1.0f;

                x = (1 - x) * (1 + x);        // x = 1 - x*x;
                lnx = Math.Log(x);

                tt1 = 2 / (Math.PI * 0.147) + 0.5f * lnx;
                tt2 = 1 / (0.147) * lnx;

                return (sgn * Math.Sqrt(-tt1 + Math.Sqrt(tt1 * tt1 - tt2)));
            }

            // The factorial function
            /*static int fact(int x)
            {
                int factorial = 1;
                for (int i = x; i > 0; i--) factorial *= i;
                return factorial;
            }
            */

            // And...let me use my function, to make Additional task*
            double[] lapValOWN = new double[] { LaplaceInv(0.25), LaplaceInv(0.375), LaplaceInv(0.4), LaplaceInv(0.475) }; // 50%, 75%, 80%, 95% confidence values
            double[] lowBoundCon2 = new double[4];
            double[] upperBoundCon2 = new double[4];
            for (int i = 0; i < 4; i++)
            {
                var counter4 = 0;
                lowBoundCon2[i] = mathExpVal - (lapValOWN[i] * Math.Sqrt(disp) / Math.Sqrt(numberElem));
                upperBoundCon2[i] = mathExpVal + (lapValOWN[i] * Math.Sqrt(disp) / Math.Sqrt(numberElem));
                for (int j = 0; j < numberElem; j++)
                {
                    if (array[j] > lowBoundCon2[i] && array[j] < upperBoundCon2[i]) counter4++;
                }

                Console.WriteLine("The interval for " + confPer[i] + " confidence level is = (" + lowBoundCon2[i] + "; " + upperBoundCon2[i] + ")");
                Console.WriteLine("The Hit rate for interval = " + lapValOWN[i]);
                Console.WriteLine("The number of elements that fit into interval = " + counter4 + " or " + Convert.ToDouble(counter4) / numberElem * 100 + "%.\n");
            }
        }
    }
}
