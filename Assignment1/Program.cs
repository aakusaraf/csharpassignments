using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1
{
    public class Program : Igeneric<String>
    {
        public String name;
        public string item { get => name; set => this.item=name; }

        static void Main(string[] args)
        {

            Program p1 = new Program();
            Console.WriteLine("This is an example of interface implementation");

            enter:
            Console.Write("Enter your name: ");
            p1.name=Console.ReadLine();

            if (p1.name.Length==0)
            {
                Console.WriteLine("Please write a name!");
                goto enter;
            }
            else
            {
                Console.WriteLine("Welcome, " + p1.printData() + "!");
            }
            

            Console.ReadKey();
        }

        public string printData()
        {
            return item;
        }
    }
}
