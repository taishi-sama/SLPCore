using System;
using System.IO;
using System.Collections.Generic;
using SimpleScanner;
using SimpleParser;
using SLPCore.Inits;
using SLPCore.Types;
using SLPCore.Exceptions;
using SLPCore.Runtime;
using BenchmarkDotNet.Running;
using SLPCore;

namespace SimpleCompiler
{
    public class SimpleCompilerMain
    {
        public static void Main()
        {

            //foreach (var i in Directory.EnumerateFiles(@"..\..\Tests", "*.slp", SearchOption.AllDirectories))
            //{
            //    Run(i);
            //};
            var summary = BenchmarkRunner.Run<BenchmarkLang>();
            //Run(@"..\..\..\bench.txt");
            Console.ReadLine();
        }
        public static void Run(string FileName) 
        {
            try
            {
                string Text = File.ReadAllText(FileName);

                Scanner scanner = new Scanner();
                scanner.SetSource(Text, 0);

                Parser parser = new Parser(scanner);
                Console.WriteLine($"Анализ {FileName}");
                var b = parser.Parse();
                if (!b)
                    Console.WriteLine("Ошибка");
                else
                {
                    Console.WriteLine("Синтаксическое дерево построено");
                    //foreach (var st in parser.root.StList)
                    //    Console.WriteLine(st);
                    (var t, var c, var f, var i) = InitLang.InitTables();
                    var e = new TypeOfExpression();
                    var typechecker = new TypecheckVisitor(t, c, e);
                    parser.root.Accept(typechecker);
                    Console.WriteLine("Проверка типов завершена");
                    var runtime = new RuntimeVisitor(e, t, c, i);
                    parser.root.Accept(runtime);

                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Файл {0} не найден", FileName);
            }
            catch (LexException e)
            {
                Console.WriteLine($"Лексическая ошибка. " + e.Message);
            }
            catch (SyntaxException e)
            {
                Console.WriteLine($"Синтаксическая ошибка. " + e.Message);
            }
            catch (TypecheckException e)
            {
                Console.WriteLine($"Ошибка проверки типов на строке {e.Location.StartLine} : {e.Location.StartColumn}. " + e.Message);
            }
        }

    }
}
